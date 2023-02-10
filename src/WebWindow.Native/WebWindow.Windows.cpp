#include "WebWindow.h"
#include <stdio.h>
#include <map>
#include <mutex>
#include <condition_variable>
#include <comdef.h>
#include <atomic>
#include <Shlwapi.h>
#include <string>
#include <atlimage.h>
#include <iostream>
#include <filesystem>
#include <fileapi.h>

using namespace std;

#include "wintoastlib.h"
using namespace WinToastLib;

#define WM_USER_SHOWMESSAGE (WM_USER + 0x0001)
#define WM_USER_INVOKE (WM_USER + 0x0002)
#define WM_USER_SHOW_APP (WM_USER + 0x0003)

using namespace Microsoft::WRL;

#include <tchar.h>
#include <atlconv.h>

#define STR_LOG		"<LOG "
#define STR_LOG_SRC "<SRC "
#define STR_TAIL	" />"
#define STR_LVL		" <LVL %d /> "

#define LIMIT_LOG_SIZE	(1024*1024*1024)

#ifndef WM_COPYGLOBALDATA
#define WM_COPYGLOBALDATA 0x0049
#endif
#ifndef CHANGEWINDOWMESSAGEFILTER
typedef BOOL(WINAPI* CHANGEWINDOWMESSAGEFILTER)(UINT message, DWORD dwFlag);
#endif

LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);
LPCWSTR CLASS_NAME = L"WebWindow";
std::mutex invokeLockMutex;
HINSTANCE WebWindow::_hInstance;
HWND messageLoopRootWindowHandle;
std::map<HWND, WebWindow*> hwndToWebWindow;

void* SelfThis = nullptr;

BYTE* g_ptrByte = NULL;		// result
BYTE* g_ptrExByte = NULL;		// result
bool g_bDoingSendClipBoard = false;


bool g_bDoExit2TrayUse = false;
bool g_bStartTray = true;
bool g_bClipCopyNsend = false;

std::map<int, wstring> mapHotKey;

enum Results {
	ToastClicked,					// user clicked on the toast
	ToastDismissed,					// user dismissed the toast
	ToastTimeOut,					// toast timed out
	ToastHided,						// application hid the toast
	ToastNotActivated,				// toast was not activated
	ToastFailed,					// toast failed
	SystemNotSupported,				// system does not support toasts
	UnhandledOption,				// unhandled option
	MultipleTextNotSupported,		// multiple texts were provided
	InitializationFailure,			// toast notification manager initialization failure
	ToastNotLaunched				// toast could not be launched
};

#define COMMAND_ACTION		L"--action"
#define COMMAND_AUMI		L"--aumi"
#define COMMAND_APPNAME		L"--appname"
#define COMMAND_APPID		L"--appid"
#define COMMAND_EXPIREMS	L"--expirems"
#define COMMAND_TEXT		L"--text"
#define COMMAND_HELP		L"--help"
#define COMMAND_IMAGE		L"--image"
#define COMMAND_SHORTCUT	L"--only-create-shortcut"
#define COMMAND_AUDIOSTATE  L"--audio-state"
#define COMMAND_ATTRIBUTE   L"--attribute"



class CustomHandler : public IWinToastHandler {
public:
	std::wstring strNavi;
	WebWindow* m_window;
	CustomHandler()
	{
		strNavi = L"";
	}
	CustomHandler(WebWindow* window)
	{
		m_window = window;
	}
	~CustomHandler()
	{
		m_window = NULL;
	}
	void toastActivated() const override {
		std::wcerr << L"Toast Clicked :" << strNavi.c_str() << std::endl;
		if (m_window)
		{
			if (strNavi.length() > 0)
			{
				((WebWindow*)m_window)->InvokeRequestedNavigateURL(strNavi.c_str());
			}
		}
		//MessageBox(NULL, L"Found", L"Found", MB_OK);
		//delete this;
	}

	void toastActivated(int actionIndex) const override {
		std::wcerr << L"The user clicked on action #" << actionIndex << std::endl;
		//std::wcout << L"strNaviURI : " << strNavi.c_str() << std::endl;
		if (m_window)
		{
			if (strNavi.length() > 0)
			{
				//((WebWindow*)m_window)->InvokeRequestedNavigateURL((AutoString)strNavi.c_str());
			}
		}
		//exit(16 + actionIndex);
		//MessageBox(NULL, L"Found", L"Found", MB_OK);
		//delete this;
	}

	void toastDismissed(WinToastDismissalReason state) const override {
		switch (state) {
		case UserCanceled:
			std::wcerr << L"The user dismissed this toast" << std::endl;
			//exit(1);
			break;
		case TimedOut:
			std::wcerr << L"The toast has timed out" << std::endl;
			//exit(2);
			break;
		case ApplicationHidden:
			std::wcerr << L"The application hid the toast using ToastNotifier.hide()" << std::endl;
			//exit(3);
			break;
		default:
			std::wcerr << L"Toast not activated" << std::endl;
			//exit(4);
			break;
		}
		//MessageBox(NULL, L"Found", L"Found", MB_OK);
		//delete this;
	}

	void toastFailed() const override {
		std::wcout << L"Error showing current toast" << std::endl;
		//exit(5);
	}

	void SetNaviURI(std::wstring str)
	{
		strNavi = str;
	}
};
CustomHandler* g_CustomHandler = NULL;

#include "NativeLog.h"
#include "TrayFunc.h"

struct InvokeWaitInfo
{
	std::condition_variable completionNotifier;
	bool isCompleted;
};

struct ShowMessageParams
{
	std::wstring title;
	std::wstring body;
	UINT type;
};

#include <tlhelp32.h>
#define TH32CS_SNAPPROCESS  0x00000002
#define  PATT_SIZE      1024
typedef HANDLE(WINAPI* CreateToolhelp32Snapshot_t)(DWORD dwFlags, DWORD th32ProcessID);	// for CreateToolhelp32Snapshot
typedef BOOL(WINAPI* ProcessWalk_t)(HANDLE hSnapshot, LPPROCESSENTRY32 lppe);				// for Process snap function
typedef BOOL(WINAPI* ThreadWalk_t)(HANDLE hSnapshot, LPTHREADENTRY32 lppe);				// for Thread snap function

static int KillProcess(DWORD dwProcessId, HWND hWnd = NULL, int bForce = 0)
{
	HANDLE hProc = OpenProcess(PROCESS_ALL_ACCESS, FALSE, dwProcessId);
	if (hProc) {
		if (TerminateProcess(hProc, 0)) {
			unsigned long nCode;
			GetExitCodeProcess(hProc, &nCode);
		}
		CloseHandle(hProc);
	}
	return 0;
}

static wstring Utf8ToWidecode(string strUtf8)
{
	if (strUtf8.empty() == true)
		return L"";
	int utf8Len = (int)strUtf8.length();
	int nLen = MultiByteToWideChar(CP_UTF8, 0, strUtf8.data(), utf8Len, NULL, NULL);

	WCHAR* pUnicode = new WCHAR[nLen + 1];
	memset(pUnicode, 0x00, nLen + 1);

	nLen = MultiByteToWideChar(CP_UTF8, 0, strUtf8.data(), utf8Len, pUnicode, nLen);
	pUnicode[nLen] = NULL;
	wstring strUnicdoe = pUnicode;

	delete[] pUnicode;

	return strUnicdoe;
}

static wchar_t* Utf8ToWidecode(char* strUtf8, wchar_t* chWide, int nLen)
{
	wstring strWide = Utf8ToWidecode(strUtf8);
	wcscpy_s(chWide, nLen, strWide.data());

	return chWide;
}



void WebWindow::Register(HINSTANCE hInstance)
{
	_hInstance = hInstance;

	// Register the window class	
	WNDCLASSW wc = { };
	wc.lpfnWndProc = WindowProc;
	wc.hInstance = hInstance;
	wc.lpszClassName = CLASS_NAME;
	RegisterClass(&wc);

	SetThreadDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE);
}

WebWindow::WebWindow(AutoString title, WebWindow* parent, WebMessageReceivedCallback webMessageReceivedCallback)
{
	SelfThis = this;
	// Create the window
	_webMessageReceivedCallback = webMessageReceivedCallback;
	g_bDoExit2TrayUse = false;

	_parent = parent;
	_hWnd = CreateWindowEx(
		0,                              // Optional window styles.
		CLASS_NAME,                     // Window class
		title,							// Window text
		WS_OVERLAPPEDWINDOW,            // Window style

		// Size and position
		//CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT,
		CW_USEDEFAULT, CW_USEDEFAULT, 1220, 720,

		parent ? parent->_hWnd : NULL,       // Parent window
		NULL,       // Menu
		_hInstance, // Instance handle
		this        // Additional application data
	);
	hwndToWebWindow[_hWnd] = this;

	
	tray.icon = (char*)TRAY_ICON1;
	tray.menu = (struct tray_menu *)malloc(sizeof(struct tray_menu)*8);

    /*tray.menu[0] = {(char*)"About",0,0,0,hello_cb,NULL,NULL};
    tray.menu[1] = {(char*)"-",0,0,0,NULL,NULL,NULL};
    tray.menu[2] = {(char*)"Hide",0,0,0,toggle_show,NULL,NULL};
    tray.menu[3] = {(char*)"-",0,0,0,NULL,NULL,NULL};
    tray.menu[4] = {(char*)TEXT_EXIT,0,0,0,quit_cb,NULL,NULL};
    tray.menu[5] = {NULL,0,0,0,NULL,NULL,NULL};*/

	//tray.menu[0] = { (char*)"About",0,0,0,hello_cb,NULL,NULL };
	//tray.menu[1] = { (char*)"-",0,0,0,NULL,NULL,NULL };
	tray.menu[0] = { (char*)TEXT_HIDE,0,0,0,toggle_show,NULL,NULL };
	tray.menu[1] = { (char*)"-",0,0,0,NULL,NULL,NULL };
	tray.menu[2] = { (char*)TEXT_EXIT,0,0,0,quit_cb,NULL,NULL };
	tray.menu[3] = { NULL,0,0,0,NULL,NULL,NULL };

	/*
            {.text = "About", .disabled = 0, .checked = 0, .usedCheck = 0, .cb = hello_cb},
            {.text = "-", .disabled = 0, .checked = 0, .usedCheck = 0, .cb = NULL, .context = NULL},
            {.text = "Hide", .disabled = 0, .checked = 0, .usedCheck = 0, .cb = toggle_show},
            {.text = "-", .disabled = 0, .checked = 0, .usedCheck = 0, .cb = NULL, .context = NULL},
            {.text = "Quit", .disabled = 0, .checked = 0, .usedCheck = 0, .cb = quit_cb},
            {.text = NULL, .disabled = 0, .checked = 0, .usedCheck = 0, .cb = NULL, .context = NULL}}
	*/

	m_nAppNotiID = 0;
	g_CustomHandler = new CustomHandler(this);
}

// Needn't to release the handles.
WebWindow::~WebWindow() 
{ 
	if (g_CustomHandler)
		free(g_CustomHandler);
	if(tray.menu) free(tray.menu);
}


HWND WebWindow::getHwnd()
{
	return _hWnd;
}

LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
	LPMINMAXINFO mmi;
	switch (uMsg)
	{
	case WM_SYSCOMMAND:
	{
		if (hwnd == messageLoopRootWindowHandle)
		{
			if (wParam == SC_MINIMIZE || wParam == SC_RESTORE)
			{
				struct tray_menu* item = tray.menu;
				do
				{
					if (strcmp(item->text, TEXT_HIDE) == 0 ||
						strcmp(item->text, TEXT_SHOW) == 0) {
						toggle_minimize(item);
						break;
					}
				} while ((++item)->text != NULL);
			}
			else if (wParam == SC_SCREENSAVE)
			{
				NTLog(SelfThis, Info, "WindowProc, WM_SYSCOMMAND, SC_SCREENSAVE (##########) !!!");
			}
			else if (wParam == SC_TASKLIST)
			{
				NTLog(SelfThis, Info, "WindowProc, WM_SYSCOMMAND, SC_TASKLIST (##########) !!!");
			}
			else if (wParam == SC_MONITORPOWER)
			{
				NTLog(SelfThis, Info, "WindowProc, WM_SYSCOMMAND, SC_MONITORPOWER (##########) !!!");
			}


		}
	}
	break;
	case WM_GETMINMAXINFO:
		mmi = (LPMINMAXINFO)lParam;
		mmi->ptMinTrackSize.x = WINDOW_MIN_WIDTH;
		mmi->ptMinTrackSize.y = WINDOW_MIN_HEIGHT;
		return 0;
	//case WM_QUIT:
	//{
	//	NTLog(SelfThis, Info, "Called : OpenNetLink - WM_QUIT !!!");
	//	hwndToWebWindow.erase(hwnd);
	//	WinToast::instance()->clear();
	//	DWORD pid = GetCurrentProcessId();
	//	KillProcess(pid);
	//	if (!pid)
	//		KillProcess(pid);
	//	break;
	//}
	case WM_CLOSE:
		NTLog(SelfThis, Info, "Called : OpenNetLink - WM_CLOSE - TrayUse : %s", g_bDoExit2TrayUse ?"YES":"NO");
		if (g_bDoExit2TrayUse)
		{
			if (hwnd == messageLoopRootWindowHandle)
			{
				struct tray_menu* item = tray.menu;
				do
				{
					if (strcmp(item->text, TEXT_HIDE) == 0 ||
						strcmp(item->text, TEXT_SHOW) == 0) 
					{
						item->checked = false;
						toggle_show(item);
						break;
					}
				} while ((++item)->text != NULL);
			}
		}
		else
		{
			char chBuf[512] = { 0, };
			GetTempPathA(sizeof(chBuf), chBuf);
			std::string tempPath = chBuf;
			std::string filePath = tempPath + "testd.sock";

			if (std::remove(filePath.data()) == 0) // delete file
				NTLog(SelfThis, Info, "Called : WindowProc, Success: Remove File [%s]", filePath.data());
			else
				NTLog(SelfThis, Err, "Called : WindowProc, Fail: Remove File [%s] Err[%s]", filePath.data(), strerror(errno));
			
			tray_exit();
			printf("Exit!!\n");
			hwndToWebWindow.erase(hwnd);
			WinToast::instance()->clear();
			if (hwnd == messageLoopRootWindowHandle)
			{
				PostQuitMessage(0);
				printf("PostQuitMessage - %s(%d)\n", __FILE__, __LINE__);
			}
			DWORD pid = GetCurrentProcessId();
			KillProcess(pid);
			if (!pid)
				KillProcess(pid);
		}
		return 0;

	case WM_WINDOWPOSCHANGING:
	{
		//NTLog(SelfThis, Info, "Called : WindowProc, WM_WINDOWPOSCHANGING");
		WINDOWPOS* lpwndpos = (WINDOWPOS *)lParam;
		if (lpwndpos != NULL && g_bStartTray)
			lpwndpos->flags &= ~SWP_SHOWWINDOW;

		return 0;
	}
	case WM_DESTROY:
	{
		// Only terminate the message loop if the window being closed is the one that
		// started the message loop

		char chBuf[512] = { 0, };
		GetTempPathA(sizeof(chBuf), chBuf);
		std::string tempPath = chBuf;
		std::string filePath = tempPath + "testd.sock";

		if (std::remove(filePath.data()) == 0) // delete file
			NTLog(SelfThis, Info, "Called : WindowProc, Success: Remove File [%s]", filePath.data());
		else
			NTLog(SelfThis, Err, "Called : WindowProc, Fail: Remove File [%s] Err[%s]", filePath.data(), strerror(errno));

		hwndToWebWindow.erase(hwnd);
		if (hwnd == messageLoopRootWindowHandle)
		{
			PostQuitMessage(0);
		}
		return 0;
	}
	case WM_USER_SHOWMESSAGE:
	{
		ShowMessageParams* params = (ShowMessageParams*)wParam;
		MessageBox(hwnd, params->body.c_str(), params->title.c_str(), params->type);
		delete params;
		return 0;
	}

	case WM_USER_INVOKE:
	{
		ACTION callback = (ACTION)wParam;
		callback();
		InvokeWaitInfo* waitInfo = (InvokeWaitInfo*)lParam;
		{
			std::lock_guard<std::mutex> guard(invokeLockMutex);
			waitInfo->isCompleted = true;
		}
		waitInfo->completionNotifier.notify_one();
		return 0;
	}
	case WM_USER_SHOW_APP:
	{
		struct tray_menu* item = tray.menu;
		if (item != NULL)
		{
			do
			{
				if (strcmp(item->text, TEXT_HIDE) == 0 ||
					strcmp(item->text, TEXT_SHOW) == 0) {
					toggle_show_force(item, true);
					// toggle_minimize(item);
					break;
				}
			} while ((++item)->text != NULL);
		}
		break;
	}
	case WM_DPICHANGED:
	{
		RECT* const prcNewWindow = (RECT*)lParam;
		SetWindowPos(hwnd,
			NULL,
			prcNewWindow->left,
			prcNewWindow->top,
			prcNewWindow->right - prcNewWindow->left,
			prcNewWindow->bottom - prcNewWindow->top,
			SWP_NOZORDER | SWP_NOACTIVATE);
		
		break;
	}
	case WM_SIZE:
	{
		WebWindow* webWindow = hwndToWebWindow[hwnd];
		if (webWindow)
		{
			int width, height;
			webWindow->GetSize(&width, &height);
			/*
			if (width <= WINDOW_MIN_WIDTH && height <= WINDOW_MIN_HEIGHT)
			{
				webWindow->RefitContent();
				webWindow->SetSize(WINDOW_MIN_WIDTH, WINDOW_MIN_HEIGHT);
				return 0;
			}
			*/
			webWindow->RefitContent();
			webWindow->InvokeResized(width, height);

			printf("webWindow Width = %d, Height = %d\n",width,height);
		}
		return 0;
	}
	case WM_MOVE:
	{
		WebWindow* webWindow = hwndToWebWindow[hwnd];
		if (webWindow)
		{
			int x, y;
			webWindow->GetPosition(&x, &y);
			webWindow->InvokeMoved(x, y);
		}
		return 0;
	}
	break;
	}

	return DefWindowProc(hwnd, uMsg, wParam, lParam);
}

void WebWindow::RefitContent()
{
	//if (_webviewWindow)
	if (_webviewWindowController)
	{
		RECT bounds;
		GetClientRect(_hWnd, &bounds);
		//_webviewWindow->put_Bounds(bounds);
		_webviewWindowController->put_Bounds(bounds);
		_webviewWindowController->put_IsVisible(TRUE);
	}
}

void WebWindow::SetTitle(AutoString title)
{
	SetWindowText(_hWnd, title);
}

void WebWindow::Show()
{
	
	ShowWindow(_hWnd, SW_SHOWDEFAULT);

	//MouseDropFilesAccept();

	// Strangely, it only works to create the webview2 *after* the window has been shown,
	// so defer it until here. This unfortunately means you can't call the Navigate methods
	// until the window is shown.
	if (!_webviewWindow)
	{
		AttachWebView();
	}
}

void WebWindow::WaitForExit()
{
	messageLoopRootWindowHandle = _hWnd;

	// Run the message loop
#if 0
	MSG msg = { };
	while (GetMessage(&msg, NULL, 0, 0))
	{
		TranslateMessage(&msg);
		DispatchMessage(&msg);
	}
#else
	if (tray_init(&tray) < 0)
	{
		// printf("failed to create tray\n");
		NTLog(this, Fatal, "Failed to Create Tray\n");
		return ;
	}
	while (tray_loop(1) == 0)
	{
		// printf("iteration\n");
	}
#endif
}

void WebWindow::ShowMessage(AutoString title, AutoString body, UINT type)
{
	ShowMessageParams* params = new ShowMessageParams;
	params->title = title;
	params->body = body;
	params->type = type;
	PostMessage(_hWnd, WM_USER_SHOWMESSAGE, (WPARAM)params, 0);
}

void WebWindow::Invoke(ACTION callback)
{
	InvokeWaitInfo waitInfo = {};
	PostMessage(_hWnd, WM_USER_INVOKE, (WPARAM)callback, (LPARAM)&waitInfo);

	// Block until the callback is actually executed and completed
	// TODO: Add return values, exception handling, etc.
	std::unique_lock<std::mutex> uLock(invokeLockMutex);
	waitInfo.completionNotifier.wait(uLock, [&] { return waitInfo.isCompleted; });
}
std::wstring WebWindow::GetInstallPathFromRegistry()
{
	std::wstring path = L"";

	HKEY handle = nullptr;
	auto result = RegOpenKeyEx(HKEY_LOCAL_MACHINE,
		LR"(SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Microsoft Edge)",
		0,
		KEY_READ,
		&handle);

	if (result != ERROR_SUCCESS)
		result = RegOpenKeyEx(HKEY_LOCAL_MACHINE,
			LR"(SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Microsoft Edge)",
			0,
			KEY_READ,
			&handle);

	if (result == ERROR_SUCCESS)
	{
		TCHAR buffer[MAX_PATH + 1]{ 0 };
		DWORD type = REG_SZ;
		DWORD size = MAX_PATH;
		result = RegQueryValueEx(handle, L"InstallLocation", 0, &type, reinterpret_cast<LPBYTE>(buffer), &size);
		if (result == ERROR_SUCCESS)
			path += buffer;

		TCHAR version[100]{ 0 };
		size = 100;
		result = RegQueryValueEx(handle, L"Version", 0, &type, reinterpret_cast<LPBYTE>(version), &size);
		if (result == ERROR_SUCCESS)
		{
			std::wstring tmp = path.substr(path.length() - 1, path.length());
			if (wcscmp(tmp.c_str(), L"\\") != 0)
				path += L"\\";

			path += std::wstring{ version };
		}
		else
			path = L"";

		RegCloseKey(handle);
	}

	return path;
}

std::wstring WebWindow::GetInstallPathFromDisk()
{
	//std::wstring path = LR"(c:\Program Files (x86)\Microsoft\Edge\Application\)";
	//std::wstring path = LR"(c:\Program Files (x86)\Microsoft\Edge Dev\Application\)";
	std::wstring path = LR"(c:\Program Files (x86)\Microsoft\Edge\Application\)";
	std::wstring pattern = path + L"*";

	WIN32_FIND_DATA ffd{ 0 };
	HANDLE hFind = FindFirstFile(pattern.c_str(), &ffd);
	if (hFind == INVALID_HANDLE_VALUE)
	{
		[[maybe_unused]] DWORD error = ::GetLastError();
		return {};
	}

	do
	{
		if (ffd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
		{
			std::wstring name{ ffd.cFileName };
			int a, b, c, d;
			if (4 == swscanf_s(ffd.cFileName, L"%d.%d.%d.%d", &a, &b, &c, &d))
			{
				FindClose(hFind);
				return path + name;
			}
		}
	} while (FindNextFile(hFind, &ffd) != 0);

	FindClose(hFind);

	return {};
}
std::wstring WebWindow::GetInstallPath()
{
	std::wstring installPath = L"";
	installPath = GetInstallPathFromRegistry();
	if (installPath.empty() == true)
		installPath = GetInstallPathFromDisk();

	return installPath;
}
void WebWindow::AttachWebView()
{
	//Edge Dev 경로 지정 코드 2021/02/09 YKH
	char strEdgePath[_MAX_PATH] = { 0, };
	char strBuffer[_MAX_PATH] = { 0, };
	char* pstrBuffer = NULL;
	pstrBuffer = _getcwd(strBuffer, _MAX_PATH);
	strcpy_s(strEdgePath, pstrBuffer);
	strcat_s(strEdgePath, "\\wwwroot\\edge");
	//strcat_s(strEdgePath, GetInstallPath().c_str());
	wchar_t wEdgePath[_MAX_PATH];
	size_t cn;
	mbstowcs_s(&cn, wEdgePath, strEdgePath, strlen(strEdgePath) + 1);//Plus null
	LPWSTR lpEdgeptr = wEdgePath;
	
	std::atomic_flag flag = ATOMIC_FLAG_INIT;
	flag.test_and_set();

	std::wstring Edgepath = L"D:\\file\\edge";
	std::wstring tempEdgePath = L"D:\\file\\86.0.622.68";

	std::wstring edgeFolderPath = GetInstallPath();

	wprintf(L"PintF - %s\n", edgeFolderPath.c_str());

	//HRESULT envResult = CreateCoreWebView2EnvironmentWithOptions(tempEdgePath.c_str(), nullptr, nullptr,
	//HRESULT envResult = CreateCoreWebView2EnvironmentWithOptions(nullptr, nullptr, nullptr,
	HRESULT envResult = CreateCoreWebView2EnvironmentWithOptions(lpEdgeptr, nullptr, nullptr,
		Callback<ICoreWebView2CreateCoreWebView2EnvironmentCompletedHandler>(
			[&, this](HRESULT result, ICoreWebView2Environment* env) -> HRESULT {
				HRESULT envResult = env->QueryInterface(&_webviewEnvironment);
				if (envResult != S_OK)
				{
					return envResult;
				}

				// Create a WebView, whose parent is the main window hWnd
				env->CreateCoreWebView2Controller(_hWnd, Callback<ICoreWebView2CreateCoreWebView2ControllerCompletedHandler>(
					[&, this](HRESULT result, ICoreWebView2Controller* webview) -> HRESULT {
						if (result != S_OK) { return result; }
						result = webview->QueryInterface(&_webviewWindowController);
						if (result != S_OK) { return result; }

						result = _webviewWindowController->get_CoreWebView2(&_webviewWindow);
						if (result != S_OK) { return result; }

						// Add a few settings for the webview
						// this is a redundant demo step as they are the default settings values
						ICoreWebView2Settings* Settings;
						_webviewWindow->get_Settings(&Settings);
						Settings->put_IsScriptEnabled(TRUE);
						Settings->put_AreDefaultScriptDialogsEnabled(TRUE);
						Settings->put_IsWebMessageEnabled(TRUE);

						// Register interop APIs
						EventRegistrationToken webMessageToken;
						_webviewWindow->AddScriptToExecuteOnDocumentCreated(L"window.external = { sendMessage: function(message) { window.chrome.webview.postMessage(message); }, receiveMessage: function(callback) { window.chrome.webview.addEventListener(\'message\', function(e) { callback(e.data); }); } };", nullptr);
						_webviewWindow->add_WebMessageReceived(Callback<ICoreWebView2WebMessageReceivedEventHandler>(
							[this](ICoreWebView2* webview, ICoreWebView2WebMessageReceivedEventArgs* args) -> HRESULT {
								wil::unique_cotaskmem_string message;
								//args->get_WebMessageAsString(&message);
								args->get_WebMessageAsJson(&message);
								_webMessageReceivedCallback(message.get());
								return S_OK;
							}).Get(), &webMessageToken);

						EventRegistrationToken webResourceRequestedToken;
						_webviewWindow->AddWebResourceRequestedFilter(L"*", COREWEBVIEW2_WEB_RESOURCE_CONTEXT_ALL);
						_webviewWindow->add_WebResourceRequested(Callback<ICoreWebView2WebResourceRequestedEventHandler>(
							[this](ICoreWebView2* sender, ICoreWebView2WebResourceRequestedEventArgs* args)
							{
								ICoreWebView2WebResourceRequest* req;
								args->get_Request(&req);

								wil::unique_cotaskmem_string uri;
								req->get_Uri(&uri);
								std::wstring uriString = uri.get();
								size_t colonPos = uriString.find(L':', 0);
								if (colonPos > 0)
								{
									std::wstring scheme = uriString.substr(0, colonPos);
									WebResourceRequestedCallback handler = _schemeToRequestHandler[scheme];
									if (handler != NULL)
									{
										int numBytes;
										AutoString contentType;
										wil::unique_cotaskmem dotNetResponse(handler(uriString.c_str(), &numBytes, &contentType));

										if (dotNetResponse != nullptr && contentType != nullptr)
										{
											std::wstring contentTypeWS = contentType;

											IStream* dataStream = SHCreateMemStream((BYTE*)dotNetResponse.get(), numBytes);
											wil::com_ptr<ICoreWebView2WebResourceResponse> response;
											_webviewEnvironment->CreateWebResourceResponse(
												dataStream, 200, L"OK", (L"Content-Type: " + contentTypeWS).c_str(),
												&response);
											args->put_Response(response.get());
										}
									}
								}

								return S_OK;
							}
						).Get(), &webResourceRequestedToken);

						RefitContent();

						flag.clear();
						return S_OK;
					}).Get());
				return S_OK;
			}).Get());

	if (envResult != S_OK)
	{
		_com_error err(envResult);
		LPCTSTR errMsg = err.ErrorMessage();
		MessageBox(_hWnd, errMsg, L"Error instantiating webview", MB_OK);
	}
	else
	{
		// Block until it's ready. This simplifies things for the caller, so they
		// don't need to regard this process as async.
		MSG msg = { };
		while (flag.test_and_set() && GetMessage(&msg, NULL, 0, 0))
		{
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
	}
}

void WebWindow::NavigateToUrl(AutoString url)
{
	_webviewWindow->Navigate(url);
}

void WebWindow::NavigateToString(AutoString content)
{
	_webviewWindow->NavigateToString(content);
}

void WebWindow::SendMessage(AutoString message)
{
	_webviewWindow->PostWebMessageAsString(message);
}

// TODO: Call UserNotification on Windows API
void WebWindow::ShowUserNotification(AutoString image, AutoString title, AutoString message, AutoString navURI)
{
	//return;
	if (!WinToast::isCompatible()) {
		std::wcerr << L"Error, your system in not supported!" << std::endl;
		//return Results::SystemNotSupported;
		return;
	}

	LPWSTR appName = (LPWSTR)L"Console WinToast Example",
		appUserModelID = (LPWSTR)L"WinToast Console Example",
		text = NULL,
		imagePath = NULL,
		attribute = (LPWSTR)L"";
	std::vector<std::wstring> actions;
	INT64 expiration = 0;


	bool onlyCreateShortcut = false;
	WinToastTemplate::AudioOption audioOption = WinToastTemplate::AudioOption::Default;

	imagePath=(LPWSTR)image;
	//actions.push_back(L"OK");
	expiration = 0;
	appName = (LPWSTR)L"OpenNetLink";

	wchar_t ModelID[MAX_PATH] = { 0, };
	wsprintf(ModelID, L"Noti%d", m_nAppNotiID++);
	//appUserModelID = (LPWSTR)ModelID;
	
	const auto aumi = WinToast::configureAUMI(L"HANSSAK", L"SecureGate", L"OpenNetLink", ModelID);

	onlyCreateShortcut = false;

	WinToast::instance()->setAppName(appName);
	//WinToast::instance()->setAppUserModelId(appUserModelID);

	// WinToast::instance()->setAppUserModelId(aumi);
	WinToast::instance()->setAppUserModelId(appName);

	/*
	if (onlyCreateShortcut) {
		if (imagePath || text || actions.size() > 0 || expiration) {
			std::wcerr << L"--only-create-shortcut does not accept images/text/actions/expiration" << std::endl;
			return;
		}
		enum WinToast::ShortcutResult result = WinToast::instance()->createShortcut();
		return;
	}
	*/
	wchar_t strMessage[MAX_PATH];
	memset(strMessage, 0x00, sizeof(strMessage));
	wsprintf(strMessage, L"%s\r\n\r\n%s", title, message);
	text = (LPWSTR)strMessage;

	if (!WinToast::instance()->initialize()) {
		std::wcerr << L"Error, your system in not compatible!" << std::endl;
		return;
	}
	bool withImage = (imagePath != NULL);
	WinToastTemplate templ(withImage ? WinToastTemplate::ImageAndText02 : WinToastTemplate::Text02);
	templ.setTextField(text, WinToastTemplate::FirstLine);
	templ.setAudioOption(audioOption);
	templ.setAttributionText(attribute);
	// 5초
	//templ.setDuration(WinToastTemplate::Duration::Short);	//	약7초
	templ.setDuration(WinToastTemplate::Duration::System);	//	약5초
	//templ.setDuration(WinToastTemplate::Duration::Long);	//	약23초
	
	for (auto const& action : actions)
		templ.addAction(action);
	if (expiration)
		templ.setExpiration(expiration);
	if (withImage)
		templ.setImagePath(imagePath);

	//templ.addAction(L"None");
	//templ.setScenario(WinToastTemplate::Scenario::Reminder);

	//if (g_CustomHandler != NULL)
	//{
	//	g_CustomHandler->SetNaviURI(navURI != NULL ? navURI : L"");
	//	std::wcerr << "URI : " << navURI << endl;
	//}
	CustomHandler* handler = new CustomHandler(this);
	
	if (handler != NULL)
	{
		handler->SetNaviURI(navURI != NULL ? navURI : L"");
		USES_CONVERSION;
		NTLog(SelfThis, Info, "SetNaviURI : %s", navURI != NULL ? T2A(navURI) : "(NONE)");
		std::wcerr << "URI : " << navURI << endl;
	}

	if (WinToast::instance()->showToast(templ, handler) < 0) {
		std::wcerr << L"Could not launch your toast notification!";
		return;
	}
	
}

void WebWindow::AddCustomScheme(AutoString scheme, WebResourceRequestedCallback requestHandler)
{
	_schemeToRequestHandler[scheme] = requestHandler;
}

void WebWindow::SetResizable(bool resizable)
{
	LONG_PTR style = GetWindowLongPtr(_hWnd, GWL_STYLE);
	if (resizable) style |= WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX;
	else style &= (~WS_THICKFRAME) & (~WS_MINIMIZEBOX) & (~WS_MAXIMIZEBOX);
	SetWindowLongPtr(_hWnd, GWL_STYLE, style);
}

void WebWindow::GetSize(int* width, int* height)
{
	RECT rect = {};
	GetWindowRect(_hWnd, &rect);
	if (width) *width = rect.right - rect.left;
	if (height) *height = rect.bottom - rect.top;
}

void WebWindow::SetSize(int width, int height)
{
	SetWindowPos(_hWnd, HWND_TOP, 0, 0, width, height, SWP_NOMOVE | SWP_NOZORDER);
}

BOOL MonitorEnum(HMONITOR monitor, HDC, LPRECT, LPARAM arg)
{
	auto callback = (GetAllMonitorsCallback)arg;
	MONITORINFO info = {};
	info.cbSize = sizeof(MONITORINFO);
	GetMonitorInfo(monitor, &info);
	Monitor props = {};
	props.monitor.x = info.rcMonitor.left;
	props.monitor.y = info.rcMonitor.top;
	props.monitor.width = info.rcMonitor.right - info.rcMonitor.left;
	props.monitor.height = info.rcMonitor.bottom - info.rcMonitor.top;
	props.work.x = info.rcWork.left;
	props.work.y = info.rcWork.top;
	props.work.width = info.rcWork.right - info.rcWork.left;
	props.work.height = info.rcWork.bottom - info.rcWork.top;
	return callback(&props) ? TRUE : FALSE;
}

void WebWindow::GetAllMonitors(GetAllMonitorsCallback callback)
{
	if (callback)
	{
		EnumDisplayMonitors(NULL, NULL, MonitorEnum, (LPARAM)callback);
	}
}

unsigned int WebWindow::GetScreenDpi()
{
	return GetDpiForWindow(_hWnd);
}

void WebWindow::GetPosition(int* x, int* y)
{
	RECT rect = {};
	GetWindowRect(_hWnd, &rect);
	if (x) *x = rect.left;
	if (y) *y = rect.top;
}

void WebWindow::SetPosition(int x, int y)
{
	SetWindowPos(_hWnd, HWND_TOP, x, y, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
}

void WebWindow::SetTopmost(bool topmost)
{
	SetWindowPos(_hWnd, topmost ? HWND_TOPMOST : HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
}

void WebWindow::SetIconFile(AutoString filename)
{
	HICON icon = (HICON)LoadImage(NULL, filename, IMAGE_ICON, 0, 0, LR_LOADFROMFILE);
	if (icon)
	{
		::SendMessage(_hWnd, WM_SETICON, ICON_SMALL, (LPARAM)icon);
	}
}
void WebWindow::FolderOpen(AutoString strDownPath)
{
	::ShellExecute(NULL, L"open", NULL, NULL, strDownPath, SW_SHOWNORMAL);
}
void WebWindow::MouseDropFilesAccept()
{
	DragAcceptFiles(getHwnd(), TRUE);

	CHANGEWINDOWMESSAGEFILTER ChangeWindowMessageFilter = NULL;
	HINSTANCE hDll;
	hDll = LoadLibrary(L"USER32.DLL");
	if (hDll) {
		ChangeWindowMessageFilter = (CHANGEWINDOWMESSAGEFILTER)GetProcAddress(hDll, "ChangeWindowMessageFilter");
		if (ChangeWindowMessageFilter)
		{
			ChangeWindowMessageFilter(WM_COPYDATA, MSGFLT_ADD);
			ChangeWindowMessageFilter(WM_DROPFILES, MSGFLT_ADD);
			ChangeWindowMessageFilter(WM_COPYGLOBALDATA, MSGFLT_ADD);
		}
		FreeLibrary(hDll);
	}
}

void WebWindow::OnHotKey(int groupID)
{
	//MessageBox(_hWnd, L"HotKey Recv", L"HotKey Event", MB_OK);

	NTLog(SelfThis, Info, "Called : WebWindow::OnHotKey - groupID : %d", groupID);
	int Ret = SendClipBoard(groupID);
}

void WebWindow::ClipTypeSelect(int groupID)
{
	m_mapBoolUseClipSelect[groupID] = true;
}

void WebWindow::ClipFirstSendTypeText(int groupID)
{
	m_mapBoolClipSendTextFirst[groupID] = true;
}

void WebWindow::ClipMemFree(int groupID)
{
	// WriteLog(0, (TCHAR*)_T(__FILE__), __LINE__, (TCHAR*)_T("WebWindow - ClipMemFree - groupID : %d"), groupID);
	NTLog(SelfThis, Info, "WebWindow::ClipMemFree - groupID : %d", groupID);
	ClipDataBufferClear();
}

void WebWindow::SetClipBoardSendFlag(int groupID)
{
	g_bDoingSendClipBoard = false;
}


char* WidecodeToUtf8(wchar_t* strUnicde, char* chDest)
{
	if (strUnicde == NULL)
		return NULL;
	int unicodeLen = (int)wcslen(strUnicde);
	int len = WideCharToMultiByte(CP_UTF8, 0, strUnicde, unicodeLen, NULL, 0, NULL, NULL);
	WideCharToMultiByte(CP_UTF8, 0, strUnicde, unicodeLen, chDest, len, NULL, NULL);
	return chDest;
}

/**
*@breif 로그 생성시간을 반환한다.
*@prarm st 현재시간
*@return 로그시간
*/
tstring LogTime(SYSTEMTIME& st)
{
	TCHAR chTime[MAX_PATH * 2] = { 0, };
	swprintf(chTime, MAX_PATH * 2, _T("%04d-%02d-%02d %02d:%02d:%02d.%03d"),
		st.wYear, st.wMonth, st.wDay, st.wHour, st.wMinute, st.wSecond, st.wMilliseconds);

	return tstring(chTime);
}

/**
*@breif 로그파일명을 반환하다.
*@prarm st 현재시간
*@return 로그파일명
*/
tstring LogFile(SYSTEMTIME& st)
{
	TCHAR chLogFile[MAX_PATH] = { 0, };
	swprintf(chLogFile, MAX_PATH, _T("SecureGate-WinDLL-%04d%02d%02d.log"), st.wYear, st.wMonth, st.wDay);
	return tstring(chLogFile);
}


/**
*@breif 로그파일을 생성할 경로를 반환한다.
*@return 로그파일 생성경로
*/
tstring GetLogPath()
{
	TCHAR szPath[1024], szDrive[64], szDir[512];
	::GetModuleFileName(NULL, szPath, sizeof(szPath));

#ifdef _UNICODE
	_wsplitpath_s(szPath, szDrive, _countof(szDrive), szDir, _countof(szDir), NULL,0, NULL,0);
	//_tsplitpath(szPath, szDrive, szDir, NULL, NULL);
#else
	_splitpath(szPath, szDrive, szDir, NULL, NULL);
#endif

	tstring strPath = szDrive;
	strPath.append(szDir);
	strPath.append(_T("wwwroot\\Log\\"));

	return strPath;
}

/**
*@breif 로그를 생성한다.
*@prarm lvl 로그 레벨
*@prarm strFile 로그파일
*@prarm strTime 로그시간
*@prarm strLog 로그
*@prarm strSrc 로그를 남기는 소스 정보
*/
void LogWrite(int lvl, tstring strFile, tstring strTime, tstring strLog, tstring strSrc)
{
	tstring strLogFile = GetLogPath();
	SHCreateDirectory(NULL, strLogFile.data());
	strLogFile.append(strFile);

	FILE* f = NULL;
	errno_t err = _wfopen_s(&f, strLogFile.data(), _T("a+"));
	if (f == NULL || err == EINVAL)
	{
		return;
	}

	fseek(f, 0L, SEEK_END);
	// 시간
#ifdef _UNICODE
	USES_CONVERSION;
	fwrite(W2A(strTime.data()), 1, strTime.length(), f);
#else
	fwrite(strTime.data(), 1, strTime.length(), f);
#endif

	// 로그레벨
	char chText[MAX_PATH] = { 0, };
	sprintf_s(chText, sizeof(chText), STR_LVL, lvl);
	fwrite(chText, 1, strlen(chText), f);

	// 로그내용
	fwrite(STR_LOG, 1, strlen(STR_LOG), f);
#ifdef _UNICODE

	//fwrite("Data Conver Error", 1, strlen("Data Conver Error"), f);

	if (strLog.length() > 0)
		fwrite(W2A(strLog.data()), 1, strLog.length(), f);
	else
		fwrite(" ", 1, strlen(" "), f);

#else
	fwrite(strLog.data(), 1, strLog.length(), f);
#endif

	fwrite(STR_TAIL, 1, strlen(STR_TAIL), f);

	// 소스
	if (strSrc.empty() != true)
	{

		size_t pos = 0;
		pos = strSrc.rfind(_T('\\'));
		if (tstring::npos != pos)
			strSrc = strSrc.substr(pos+1);

		fwrite(" ", 1, strlen(" "), f);
		fwrite(STR_LOG_SRC, 1, strlen(STR_LOG_SRC), f);
#ifdef _UNICODE
		fwrite(W2A(strSrc.data()), 1, strSrc.length(), f);
#else
		fwrite(strSrc.data(), 1, strSrc.length(), f);
#endif
		fwrite(STR_TAIL, 1, strlen(STR_TAIL), f);
	}

	fwrite("\n", 1, strlen("\n"), f);
	fclose(f);
}

/**
*@breif 현재 문제 있음. (@@@@@.사용하면 App 종료됨.)
*@prarm lvl 로그 레벨
*@prarm chFile 소스파일이름
*@prarm line 소스파일위치 Line정보
*@prarm chfmt Log에 출력할 comment
*/
void WebWindow::WriteLog(int lvl, TCHAR* chFile, int line, TCHAR* chfmt, ...)
{

	return;

	va_list args;
	va_start(args, chfmt);

	int len = tvsnprintf(NULL, 0, chfmt, args);	// 길이 반환
	TCHAR* msg = new TCHAR[len + 1];
	memset(msg, 0x00, len + 1);
	tvsnprintf(msg, len + 1, chfmt, args);	// 길이 반환
	va_end(args);


	SYSTEMTIME st;
	::GetLocalTime(&st);
	tstring strTime = LogTime(st);
	tstring strLogFile = LogFile(st);

	// 소스 정보
	TCHAR chSrc[MAX_PATH*2] = { 0, };
#ifdef _DEBUG
	TCHAR* chFile2 = _tcsrchr(chFile, '\\');
	swprintf(chSrc, MAX_PATH * 2, _T("%s(%d)"), chFile2 + 1, line);
#else
	swprintf(chSrc, MAX_PATH * 2, _T("%s(%d)"), chFile, line);
#endif

	LogWrite(lvl, strLogFile, strTime, msg, chSrc);	// msg

	delete[] msg;

}

/// <summary>
/// 클립보드 Text memory로 복사(utf8) (이전 Mem 초기화함) <br/>
/// bClearPreMem : 이전 Mem(g_ptrByte) clear 유무 <br/>
/// bUseExtraMem : 이전 ExMem(g_ptrByte) clear / 사용 유무 <br/>
/// return : 읽어드린크기
/// </summary>
/// <returns></returns>
size_t WebWindow::SaveTxtDataMem(bool bClearPreMem, bool bClearExPreMem, bool bUseExtraMem)
{

	// 이전 memory 정리후 동작
	ClipDataBufferClear(bClearPreMem, bClearExPreMem);

	size_t nTotalLen = 0;
	HGLOBAL hglb;

	if ((hglb = GetClipboardData(CF_UNICODETEXT)))
	{
		wchar_t* wclpstr = (wchar_t*)GlobalLock(hglb);
		size_t len = wcslen(wclpstr);
		if (len < 1)
		{
			//WriteLog(0, (TCHAR*)_T(__FILE__), __LINE__, (TCHAR*)_T("WebWindow - ClipBoard - CF_UNICODETEXT : Empty!"));
			NTLog(SelfThis, Info, "WebWindow::SaveTxtDataMem - ClipBoard - CF_UNICODETEXT : Empty!");
			return -1;
		}

		len = (len + 2) * sizeof(wchar_t);
		len *= 2;

		BYTE** ppData = NULL;
		ppData = bUseExtraMem ? &g_ptrExByte : &g_ptrByte;
		*ppData = new BYTE[len];

		memset(*ppData, 0x00, len);
		WidecodeToUtf8(wclpstr, (char*)*ppData);
		nTotalLen = strlen((char*)*ppData);

		GlobalUnlock(hglb);
		
		return nTotalLen;
	}

	return -1;
}

/// <summary>
/// 클립보드 이미지로 저장하는 함수(이전 Mem 초기화함) (return : 읽어드린크기)
/// </summary>
/// <returns></returns>
size_t WebWindow::SaveImageFile(bool bClearPreMem, bool bClearExPreMem, bool bUseExtraMem)
{

	// 이전 memory 정리
	ClipDataBufferClear(bClearPreMem, bClearExPreMem);

	size_t nRead = 0;
	int rCount = 0;
	size_t nTotalLen = 0;
	int rSize = 1024 * 64;

	FILE* fd = NULL;
	errno_t err;
	struct stat st;
	HBITMAP hbm;

	hbm = (HBITMAP)GetClipboardData(CF_BITMAP);
	GlobalLock(hbm);
	char filePath[512] = { 0, };
	if (GetClipboardBitmap(hbm, filePath) == false)
	{
		GlobalUnlock(hbm);
		CloseClipboard();
		return -1;
	}
	nTotalLen = GetLoadBitmapSize(filePath);
	//printf("GetLoadBitmapSize after filepath = %s\n", filePath);
	NTLog(SelfThis, Info, "WebWindow::SaveImageFile - GetLoadBitmapSize after filepath : %s", filePath);

	//nTotalLen = LoadClipboardBitmap(filePath, g_ptrByte);
	if (nTotalLen < 1)
	{
		NTLog(SelfThis, Info, "WebWindow::SaveImageFile - ClipBoard - CF_BITMAP : Empty!");
		//WriteLog(0, (TCHAR*)_T(__FILE__), __LINE__, (TCHAR*)_T("WebWindow - ClipBoard - CF_BITMAP : Empty!"));
		GlobalUnlock(hbm);
		CloseClipboard();
		return -1;
	}

	if ((err = fopen_s(&fd, filePath, "rb")) != 0)
	{
		NTLog(SelfThis, Info, "WebWindow::SaveImageFile - Clipboard image Load Fail!");
		MessageBox(_hWnd, L"Clipboard image Load Fail!", L"Error Clipboard Img", MB_OK);
		GlobalUnlock(hbm);
		CloseClipboard();
		return -1;
	}

	/*g_ptrByte = new BYTE[nTotalLen];
	printf("result nTotalLen = %zd\n", nTotalLen);
	memset(g_ptrByte, 0x00, nTotalLen);*/

	BYTE** ppData = NULL;
	ppData = bUseExtraMem ? &g_ptrExByte : &g_ptrByte;
	*ppData = new BYTE[nTotalLen];
	memset(*ppData, 0x00, nTotalLen);


	stat(filePath, &st);
	rCount = (int)(st.st_size / (1024 * 64)) + ((st.st_size % (1024 * 64)) ? 1 : 0);
	size_t nReadTotalLen = 0;
	for (int i = 0, len = st.st_size; i < rCount; i++)
	{
		if (len > (1024 * 64))
			rSize = (1024 * 64);
		else
			rSize = len;
		if ((nRead = fread(*ppData + nReadTotalLen, 1, rSize, fd)) <= 0)
		{
			fclose(fd);
			GlobalUnlock(hbm);
			CloseClipboard();
			ClipDataBufferClear();
			NTLog(SelfThis, Info, "WebWindow::SaveImageFile - fread - error : %d", nRead);
			return -1;
		}
		len -= rSize;
		nReadTotalLen += nRead;

		//printf("nReadTotalLen : %zd , nRead : %zd\n\n", nReadTotalLen, nRead);
	}
	fclose(fd);

	GlobalUnlock(hbm);
	
	return nTotalLen;
}


void FreeClipHotKey(int nGroupID)
{

	if (mapHotKey.find(nGroupID) == mapHotKey.end())
	{
		NTLog(SelfThis, Info, "FreeClipHotKey - nGroupID : %d, ClipHotKey Data Empty!", nGroupID);
		return;
	}

	wstring strClipHotKey = mapHotKey[nGroupID];
	if (strClipHotKey.size() == 5)
	{
		wstring strWinKey = strClipHotKey.substr(0,1);
		wstring strCtrlKey = strClipHotKey.substr(1, 1);
		wstring strAltKey = strClipHotKey.substr(2, 1);
		wstring strShiftKey = strClipHotKey.substr(3, 1);
		wstring strKeyName = strClipHotKey.substr(4, 1);

		Sleep(100);
		if (strKeyName.empty() != true)
			keybd_event((BYTE)strKeyName.data(), 0x98, KEYEVENTF_KEYUP, 0);
		if (strShiftKey.compare(_T("1")) == 0)
			keybd_event(VK_SHIFT, 0x9d, KEYEVENTF_KEYUP, 0);
		if (strCtrlKey.compare(_T("1")) == 0)
			keybd_event(VK_CONTROL, 0x9d, KEYEVENTF_KEYUP, 0);
		if (strAltKey.compare(_T("1")) == 0)
			keybd_event(VK_MENU, 0x9d, KEYEVENTF_KEYUP, 0);
		if (strWinKey.compare(_T("1")) == 0)
		{
			keybd_event(VK_LWIN, 0x9d, KEYEVENTF_KEYUP, 0);
			keybd_event(VK_RWIN, 0x9d, KEYEVENTF_KEYUP, 0);
		}
	}
	else
	{
		NTLog(SelfThis, Info, "FreeClipHotKey - nGroupID : %d, ClipHotKey Data is Wrong(Size Must be 5!) (Data : %s)", nGroupID, (AutoString)strClipHotKey.data());
	}

}

void AutoCopyClipBoard(int groupID)
{
	HWND h_active_wnd = ::GetForegroundWindow();
	if (h_active_wnd != NULL)
	{
		::ShowWindow(h_active_wnd, SW_SHOW);
		::SetForegroundWindow(h_active_wnd);
		::SetFocus(h_active_wnd);

		// 필요한지 확인
		FreeClipHotKey(groupID);

		keybd_event(VK_CONTROL, 0x9d, 0, 0);
		keybd_event('C', 0x98, 0, 0);
		keybd_event('C', 0x98, KEYEVENTF_KEYUP, 0);
		keybd_event(VK_CONTROL, 0x9d, KEYEVENTF_KEYUP, 0);
		Sleep(300);
	}
}

int WebWindow::SendClipBoard(int groupID)
{

	if (g_bDoingSendClipBoard)
	{
		NTLog(SelfThis, Info, "WebWindow::SendClipBoard - SendClipBoard(GroupID : %d) - Send is Doing, so Return-", groupID);
		//WriteLog(0, (TCHAR*)_T(__FILE__), __LINE__, (TCHAR*)_T("WebWindow - SendClipBoard(GroupID : %d) - Send is Doing, so Return- "), groupID);
		return -1;
	}

	g_bDoingSendClipBoard = true;

	bool bUseClipSelectSend = false;

	if (m_mapBoolUseClipSelect.find(groupID) != m_mapBoolUseClipSelect.end())
		bUseClipSelectSend = m_mapBoolUseClipSelect[groupID];

	NTLog(SelfThis, Info, "WebWindow::SendClipBoard - groupID : %d, ClipSelectSend - Use : %s", groupID, bUseClipSelectSend ? "True" : "False");
	//WriteLog(0, (TCHAR*)_T(__FILE__), __LINE__, (TCHAR*)_T("WebWindow - SendClipBoard - groupID : %d, ClipSelectSend - Use : %s"), groupID, bUseClipSelectSend?_T("True"): _T("False"));

	if (g_bClipCopyNsend)
	{
		AutoCopyClipBoard(groupID);
	}

	HWND hwndDesktop = GetDesktopWindow();

	int nType = 0;	
	size_t nTotalLen = 0;
	size_t nTotalExLen = 0;

	if (OpenClipboard(hwndDesktop))
	{
		//bUseClipSelectSend = false;	// Img-Text 선택 전송 기능 강제 Off

		if (bUseClipSelectSend && (IsClipboardFormatAvailable(CF_TEXT) || IsClipboardFormatAvailable(CF_OEMTEXT) || IsClipboardFormatAvailable(CF_UNICODETEXT)))
		{
			if (IsClipboardFormatAvailable(CF_BITMAP) || IsClipboardFormatAvailable(CF_DIB))
			{

				NTLog(SelfThis, Info, "WebWindow::SendClipBoard - groupID : %d, ClipSelectSend - Status(#######)", groupID);
				//WriteLog(0, (TCHAR*)_T(__FILE__), __LINE__, (TCHAR*)_T("WebWindow - SendClipBoard - groupID : %d, ClipSelectSend - Status(#######)"), groupID);

				if ((nTotalLen = SaveImageFile()) > 0)
				{
					nTotalExLen = SaveTxtDataMem(false, true, true);
					if (nTotalExLen > 0)
					{
						NTLog(SelfThis, Info, "WebWindow::SendClipBoard - ClipBoard - Bitmap - Size : %d, Text - Size : %d", nTotalLen, nTotalExLen);
						//WriteLog(0, (TCHAR*)_T(__FILE__), __LINE__, (TCHAR*)_T("WebWindow - ClipBoard - Bitmap - Size : %d, Text - Size : %d"), nTotalLen, nTotalExLen);
						nType = 3;
					}
				}				
			}
		}

		if (nType != 3)	// Img-Text 선택 전송이 아닐때
		{

			bool bUseClipTextFirstSend = false;
			if (m_mapBoolClipSendTextFirst.find(groupID) != m_mapBoolClipSendTextFirst.end())
				bUseClipTextFirstSend = m_mapBoolClipSendTextFirst[groupID];


			if (bUseClipSelectSend)
				NTLog(SelfThis, Info, "WebWindow::SendClipBoard - groupID : %d, ClipSelectSend - NOT Status(#######)", groupID);
				//WriteLog(0, (TCHAR*)_T(__FILE__), __LINE__, (TCHAR*)_T("WebWindow - SendClipBoard - groupID : %d, ClipSelectSend - NOT Status(#######)"), groupID);

			if (bUseClipTextFirstSend)
			{
				NTLog(SelfThis, Info, "WebWindow::SendClipBoard - groupID : %d, ClipType - Text First", groupID);
				//WriteLog(0, (TCHAR*)_T(__FILE__), __LINE__, (TCHAR*)_T("WebWindow - SendClipBoard - groupID : %d, ClipType - Text First"), groupID);

				// Text 먼저전송 설정
				if (IsClipboardFormatAvailable(CF_TEXT) || IsClipboardFormatAvailable(CF_OEMTEXT) || IsClipboardFormatAvailable(CF_UNICODETEXT))
				{
					if ((nTotalLen = SaveTxtDataMem()) > 0)
					{
						nType = 1;
						// WriteLog(0, (TCHAR*)_T(__FILE__), __LINE__, (TCHAR*)_T("WebWindow - ClipBoard - Text - Size : %d"), nTotalLen);
						NTLog(SelfThis, Info, "WebWindow::SendClipBoard - ClipBoard - Text - Size : %d", nTotalLen);
					}
				}
				else if (IsClipboardFormatAvailable(CF_BITMAP) || IsClipboardFormatAvailable(CF_DIB))
				{
					if ((nTotalLen = SaveImageFile()) > 0)
					{
						nType = 2;
						//WriteLog(0, (TCHAR*)_T(__FILE__), __LINE__, (TCHAR*)_T("WebWindow - ClipBoard - Image - Size : %d"), nTotalLen);
						NTLog(SelfThis, Info, "WebWindow::SendClipBoard - ClipBoard - Image - Size : %d", nTotalLen);
					}
				}
			}
			else
			{

				NTLog(SelfThis, Info, "WebWindow::SendClipBoard - groupID : %d, ClipType - Image First", groupID);
				//WriteLog(0, (TCHAR*)_T(__FILE__), __LINE__, (TCHAR*)_T("WebWindow - SendClipBoard - groupID : %d, ClipType - Image First"), groupID);

				// Image 먼저전송 설정 - 기본!
				if (IsClipboardFormatAvailable(CF_BITMAP) || IsClipboardFormatAvailable(CF_DIB))
				{
					if ((nTotalLen = SaveImageFile()) > 0)
					{
						nType = 2;
						NTLog(SelfThis, Info, "WebWindow::SendClipBoard - ClipBoard - Image - Size : %d", nTotalLen);
						//WriteLog(0, (TCHAR*)_T(__FILE__), __LINE__, (TCHAR*)_T("WebWindow - ClipBoard - Image - Size : %d"), nTotalLen);
					}
				}
				else if (IsClipboardFormatAvailable(CF_TEXT) || IsClipboardFormatAvailable(CF_OEMTEXT) || IsClipboardFormatAvailable(CF_UNICODETEXT))
				{
					if ((nTotalLen = SaveTxtDataMem()) > 0)
					{
						nType = 1;
						NTLog(SelfThis, Info, "WebWindow::SendClipBoard - ClipBoard - Text - Size : %d", nTotalLen);
						// WriteLog(0, (TCHAR*)_T(__FILE__), __LINE__, (TCHAR*)_T("WebWindow - ClipBoard - Text - Size : %d"), nTotalLen);
					}
				}

			}

			if (nType == 0)
			{
				CloseClipboard();
				g_bDoingSendClipBoard = false;
				ClipDataBufferClear();
				NTLog(SelfThis, Info, "WebWindow::SendClipBoard - UnKnown Type - Error : %d", groupID);
				//WriteLog(0, (TCHAR*)_T(__FILE__), __LINE__, (TCHAR*)_T("WebWindow - ClipBoard - UnKnown Type - Error : %d"), groupID);				
				return -1;
			}

		} // if (nTotalLen < 1)

	}
	else
	{
		//WriteLog(0, (TCHAR*)_T(__FILE__), __LINE__, (TCHAR*)_T("WebWindow - OpenClipboard - Error : %d"), groupID);
		NTLog(SelfThis, Info, "WebWindow::SendClipBoard - OpenClipboard - Error : %d", groupID);
	}

	CloseClipboard();

	//WriteLog(0, (TCHAR*)_T(__FILE__), __LINE__, (TCHAR*)_T("WebWindow - SendClipBoard - _clipboardCallback(UI:ClipBoardHandler) : 0x%016X"), _clipboardCallback);
	NTLog(SelfThis, Info, "WebWindow::SendClipBoard - _clipboardCallback(UI:ClipBoardHandler) : 0x%016X", _clipboardCallback);

	if (_clipboardCallback != NULL)
	{

		if (nType == 1 || nType == 2)
			_clipboardCallback(groupID, nType, (int)nTotalLen, g_ptrByte, (int)0, NULL);
		else if (nType == 3)
			_clipboardCallback(groupID, nType, (int)nTotalLen, g_ptrByte, (int)nTotalExLen, g_ptrExByte);
		else
			_clipboardCallback(groupID, nType, (int)nTotalLen, g_ptrByte, (int)0, NULL);

	}		

	if (nType != 3)
		g_bDoingSendClipBoard = false;

	return 0;
}

bool WebWindow::SaveBitmapFile(HBITMAP hBitmap, LPCTSTR lpFileName)
{
	// 파일 생성
	DeleteFile(lpFileName);
	HANDLE hFile = CreateFile(lpFileName, GENERIC_WRITE, 0, NULL,
		CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
	if (hFile == INVALID_HANDLE_VALUE) return FALSE;

	// 비트맵(DDB) 정보 얻기
	BITMAP bmp;
	GetObject(hBitmap, sizeof(BITMAP), &bmp);

	// 비트맵(DIB) 정보 설정
	BITMAPINFOHEADER bmih;
	ZeroMemory(&bmih, sizeof(BITMAPINFOHEADER));
	bmih.biSize = sizeof(BITMAPINFOHEADER);
	bmih.biWidth = bmp.bmWidth;     // 가로
	bmih.biHeight = bmp.bmHeight;   // 세로
	bmih.biPlanes = 1;
	bmih.biBitCount = 24;           // 픽셀당 비트수(BPP)
	bmih.biCompression = BI_RGB;

	// 비트맵(DIB) 데이터 추출
	// 데이터의 크기를 알아낸다
	HDC hDC = GetDC(NULL);
	GetDIBits(hDC, hBitmap, 0, bmp.bmHeight, NULL,
		(LPBITMAPINFO)&bmih, DIB_RGB_COLORS);

	// 데이터 저장 공간 확보
	LPVOID lpDIBits = new BYTE[bmih.biSizeImage];
	GetDIBits(hDC, hBitmap, 0, bmp.bmHeight, lpDIBits,
		(LPBITMAPINFO)&bmih, DIB_RGB_COLORS);
	ReleaseDC(NULL, hDC);

	// 비트맵 파일 정보 설정
	BITMAPFILEHEADER bmfh;
	bmfh.bfType = 'MB';
	bmfh.bfOffBits = sizeof(BITMAPFILEHEADER) + sizeof(BITMAPINFOHEADER);
	bmfh.bfSize = bmfh.bfOffBits + bmih.biSizeImage;
	bmfh.bfReserved1 = bmfh.bfReserved2 = 0;

	// 파일 데이터 기록
	DWORD dwWritten;
	WriteFile(hFile, &bmfh, sizeof(BITMAPFILEHEADER), &dwWritten, NULL);
	WriteFile(hFile, &bmih, sizeof(BITMAPINFOHEADER), &dwWritten, NULL);
	WriteFile(hFile, lpDIBits, bmih.biSizeImage, &dwWritten, NULL);
	CloseHandle(hFile);

	// 메모리 해제
	delete[] lpDIBits;
	return TRUE;
}

char* WebWindow::GetModulePath()
{
	char szpath[1024], szdrive[64], szdir[512];
	::GetModuleFileNameA(NULL, szpath, sizeof(szpath));

	_splitpath_s(szpath, szdrive, 64, szdir, 512, NULL,0, NULL,0);

	memset(m_chModulePath, 0x00, sizeof(m_chModulePath));
	sprintf_s(m_chModulePath, "%s%s", szdrive, szdir);
	return m_chModulePath;
}

bool WebWindow::GetClipboardBitmap(HBITMAP hbm, char* bmpPath)
{
	char  filepath[512], workdirpath[512];
	WCHAR wszBuff[512];

	// 1. 파일 저장
	sprintf_s(workdirpath, ".\\work");
	//CreateAppDir(workdirpath, 512,1);
	CreateDirectoryA(workdirpath,NULL);
	sprintf_s(filepath, "%swork\\cur_clip.dat",GetModulePath());
	printf("filepath = %s", filepath);
	MultiByteToWideChar(CP_ACP, 0, filepath, -1, wszBuff, sizeof(filepath));
	if (SaveBitmapFile(hbm, wszBuff))
	{
		strcpy_s(bmpPath, 512,filepath);
		return true;
	}
	MessageBox(_hWnd, L"Clipboard image Save Fail!", L"Error Clipboard Img", MB_OK);
	return false;
}

size_t WebWindow::GetLoadBitmapSize(char* filePath)
{
	struct stat st;
	stat(filePath, &st);
	return st.st_size;
}
size_t WebWindow::LoadClipboardBitmap(char* filePath, BYTE* pByte)
{
	FILE* fd;
	struct stat st;
	int rSize = 1024 * 64;
	size_t nTotalLen = 0, nRead = 0;
	errno_t err;
	if ((err = fopen_s(&fd, filePath, "rb")) != 0)
	{
		MessageBox(_hWnd, L"Clipboard image Load Fail!", L"Error Clipboard Img", MB_OK);
		return false;
	}
	stat(filePath, &st);
	pByte = new BYTE[(int)st.st_size];

	int rCount = (int)(st.st_size / (1024 * 64)) + ((st.st_size % (1024 * 64)) ? 1 : 0);
	int len = 0;
	for (int i = 0, len=st.st_size; i < rCount; i++)
	{
		if (len > (1024 * 64))
			rSize = (1024 * 64);
		else
			rSize = len;
		if ((nRead = fread(pByte +nTotalLen, 1, rSize, fd)) <= 0)
		{
			fclose(fd);
			return -1;
		}
		len -= rSize;
		nTotalLen += nRead;

		printf("nTotalLen : %zd , nRead : %zd\n\n", nTotalLen, nRead);
		//wchar_t chSize[64];
		//memset(chSize, 0x00, sizeof(chSize));
		//wsprintf(chSize,L"nRead = %d, nTotalLen = %d",nRead,nTotalLen);
		//MessageBox(_hWnd, chSize, L"Clipboard Img Load", MB_OK);
	}
	fclose(fd);
	return nTotalLen;
}

void WebWindow::ClipDataBufferClear(bool bClearPreMem, bool bClearPreExMem)
{

	if (bClearPreMem)
	{
		if (g_ptrByte != NULL)
		{
			delete[] g_ptrByte;
			g_ptrByte = NULL;
		}
	}

	if (bClearPreExMem)
	{
		if (g_ptrExByte != NULL)
		{
			delete[] g_ptrExByte;
			g_ptrExByte = NULL;
		}
	}

}

void LoadBitmapTest(char* filepath,void* buffer)
{
	FILE* pFile = NULL;
	errno_t err;
	if ((err = fopen_s(&pFile, filepath, "rb")) != 0)
	{
		return;
	}
	struct stat st;
	stat(filepath, &st);

	if (buffer != NULL)
	{
		delete[] buffer;
		buffer = NULL;
	}

	buffer = new char[st.st_size];
	fread(buffer, sizeof(char), st.st_size, pFile);
	fclose(pFile);
}

void WebWindow::SetClipBoard(int groupID,int nType, int nClipSize, void* data)
{
	char  filepath[512], workdirpath[512];
	void* buffer=NULL;
	if (OpenClipboard(0))
	{
		EmptyClipboard();

		if (nType == 1)
		{
			HGLOBAL hText = NULL;
			wchar_t* chData = new wchar_t[nClipSize + (size_t)4];
			memset(chData, 0x00, sizeof(chData));
			Utf8ToWidecode((char*)data, chData, nClipSize + 4);
			hText = GlobalAlloc(GMEM_MOVEABLE | GMEM_DDESHARE, (wcslen(chData) + 4) * sizeof(wchar_t));
			wchar_t* ptr = NULL;
			if(hText!=NULL)
				ptr = (wchar_t*)GlobalLock(hText);
			wcscpy_s(ptr, wcslen(chData) + 4, chData);
			SetClipboardData(CF_UNICODETEXT, hText);
			GlobalUnlock(hText);
			delete[] chData;
			CloseClipboard();
			if (hText != NULL)
				GlobalFree(hText);

			//WriteLog(0, (TCHAR*)_T(__FILE__), __LINE__, (TCHAR*)_T("Recv ClipBoard - groupID : %d - Text-Size : %d"), groupID, nClipSize);
			NTLog(SelfThis, Info, "Recv ClipBoard - WebWindow::SetClipBoard - groupID : %d - Text-Size : %d", groupID, nClipSize);

		}
		else if (nType == 2)
		{

			//WriteLog(0, (TCHAR*)_T(__FILE__), __LINE__, (TCHAR*)_T("Recv ClipBoard - groupID : %d - Image-Size : %d"), groupID, nClipSize);
			NTLog(SelfThis, Info, "Recv ClipBoard - WebWindow::SetClipBoard - groupID : %d - Image-Size : %d", groupID, nClipSize);

			sprintf_s(workdirpath, ".\\work");
			CreateDirectoryA(workdirpath, NULL);
			sprintf_s(filepath, "%swork\\cur_clip.bmp", GetModulePath());
			DeleteFileA(filepath);
			printf("FilePath = %s\n", filepath);
			printf("nClipSize = %ld\n", nClipSize);
			SaveImage(filepath , (void*)data, nClipSize);

		#if 1 // Mac 호환 Code 적용

			USES_CONVERSION;			
			CImage img;
			img.Load(A2W(filepath));
			CDC memDC;
			memDC.CreateCompatibleDC(NULL);
			CBitmap bitmap;
			bitmap.CreateCompatibleBitmap(CDC::FromHandle(::GetDC(getHwnd())), img.GetWidth(), img.GetHeight());
			memDC.SelectObject(&bitmap);
			img.BitBlt(memDC.GetSafeHdc(), 0, 0, img.GetWidth(), img.GetHeight(), 0, 0, SRCCOPY);

			EmptyClipboard();
			//put the data on the clipboard 
			GlobalLock(bitmap.GetSafeHandle());
			SetClipboardData(CF_BITMAP, bitmap.GetSafeHandle());
			GlobalUnlock(bitmap.GetSafeHandle());			

			CloseClipboard();

			memDC.DeleteDC();
			bitmap.Detach();
			
			//WriteLog(0, (TCHAR*)_T(__FILE__), __LINE__, (TCHAR*)_T("Recv ClipBoard - Done !"));
			NTLog(SelfThis, Info, "Recv ClipBoard - Done !");

			DeleteFileA(filepath);

		#else

			HBITMAP hBitmap = NULL;
			hBitmap = (HBITMAP)LoadImageA(NULL, filepath, IMAGE_BITMAP,
				0, 0, LR_LOADFROMFILE | LR_CREATEDIBSECTION);
			if (hBitmap == NULL)
			{
				MessageBox(_hWnd, L"hBitmap NULL", L"Clipboard", MB_OK);
				CloseClipboard();
				return;
			}
			DIBSECTION ds;
			::GetObject(hBitmap, sizeof(DIBSECTION), &ds);
			ds.dsBmih.biCompression = BI_RGB;
			HDC hdc = ::GetDC(NULL);
			HBITMAP hbitmap_ddb = ::CreateDIBitmap(
				hdc, &ds.dsBmih, CBM_INIT, ds.dsBm.bmBits, (BITMAPINFO*)&ds.dsBmih, DIB_RGB_COLORS);
			::ReleaseDC(NULL, hdc);
			SetClipboardData(CF_BITMAP, hbitmap_ddb);
			CloseClipboard();
			DeleteFileA(filepath);

		#endif

		}
		else 
			CloseClipboard();
	}
	CloseClipboard();

	if (_recvclipboardCallback != NULL)
		_recvclipboardCallback(groupID);

	return;
}


bool WebWindow::SaveImage(char* PathName, void* lpBits, int size) 
{
	FILE* pFile = NULL;
	errno_t err;
	if ((err = fopen_s(&pFile, PathName, "wb")) != 0)
	{
		MessageBox(_hWnd, L"Recv BMP image Save Fail!", L"Error Clipboard Img", MB_OK);
		return false;
	}
	fwrite(lpBits, 1, size, pFile);
	if (pFile != NULL)
		fclose(pFile);
	/*
	tagBITMAPFILEHEADER bfh = *(tagBITMAPFILEHEADER*)lpBits;
	tagBITMAPINFOHEADER bih = *(tagBITMAPINFOHEADER*)((unsigned char*)lpBits + sizeof(tagBITMAPFILEHEADER));
	
	RGBQUAD             rgb = *(RGBQUAD*)((unsigned char*)lpBits + sizeof(tagBITMAPFILEHEADER) + sizeof(tagBITMAPINFOHEADER));
	
	printf("BITMAP Width = %d, HEIGHT = %d\n", bih.biWidth, bih.biHeight);

	// Create a new file for writing
	FILE* pFile = NULL;
	errno_t err;
	if ((err = fopen_s(&pFile, PathName, "wb")) != 0)
	{
		MessageBox(_hWnd, L"Recv BMP image Save Fail!", L"Error Clipboard Img", MB_OK);
		return false;
	}

	size_t nWrittenFileHeaderSize = fwrite(&bfh, 1, sizeof(BITMAPFILEHEADER), pFile);
	size_t nWrittenInfoHeaderSize = fwrite(&bih, 1, sizeof(BITMAPINFOHEADER), pFile);
	size_t nWrittenDIBDataSize = fwrite((unsigned char*)lpBits + sizeof(tagBITMAPFILEHEADER) + sizeof(tagBITMAPINFOHEADER), 1, size - sizeof(BITMAPFILEHEADER) - sizeof(BITMAPINFOHEADER), pFile);
	printf("nWrittenFileHeaderSize = %zu, nWrittenInfoHeaderSize = %zu, nWrittenDIBDataSize = %zu\n", nWrittenFileHeaderSize, nWrittenInfoHeaderSize, nWrittenDIBDataSize);
	if(pFile!=NULL)
		fclose(pFile);
	*/
	return true;
}

void WebWindow::ProgramExit()
{
	NTLog(this, Info, "Called : OpenNetLink Exit");
	hwndToWebWindow.erase(hwnd);

	WinToast::instance()->clear();

	if (hwnd == messageLoopRootWindowHandle)
	{
		PostQuitMessage(0);
		printf("PostQuitMessage - %s(%d)\n", __FILE__, __LINE__);
	}
	DWORD pid = GetCurrentProcessId();
	KillProcess(pid);
	if (!pid)
		KillProcess(pid);
}

bool WebWindow::GetTrayUse()
{
	NTLog(this, Info, "Called : OpenNetLink Tray Status");
	return g_bDoExit2TrayUse;
}

void WebWindow::MoveWebWindowToTray()
{
	NTLog(this, Info, "Called : OpenNetLink Move To Tray");
	struct tray_menu *item = tray.menu;
	do
	{
		if (strcmp(item->text, TEXT_HIDE) == 0) {
			toggle_show(item);
			break;
		}
	} while ((++item)->text != NULL);
}

void WebWindow::MoveTrayToWebWindow()
{
	NTLog(this, Info, "Called : OpenNetLink Move To WebWindow");
	struct tray_menu* item = tray.menu;
	do
	{
		if (strcmp(item->text, TEXT_SHOW) == 0) {
			toggle_show(item);
			break;
		}
	} while ((++item)->text != NULL);
}

void WebWindow::MinimizeWebWindow()
{
	NTLog(this, Info, "Called : OpenNetLink Minimize WebWindow");
	struct tray_menu *item = tray.menu;
	do
	{
		if (strcmp(item->text, TEXT_HIDE) == 0) {
			toggle_minimize(item);
			break;
		}
	} while ((++item)->text != NULL);
}

void WebWindow::RegisterStartProgram()
{
	// TODO: Register logic for start program
	// C# UI Code로 동작함
}

void WebWindow::UnRegisterStartProgram()
{
	// TODO: UnRegister logic for start program
	// C# Code로 동작함
}

/// <summary>
/// 
/// </summary>
/// <param name="bUseStartTray"></param>
void WebWindow::SetTrayStartUse(bool bUseStartTray)
{
	g_bStartTray = bUseStartTray;

	if (_hWnd != NULL &&
		::IsWindow(_hWnd))
	{
		if (g_bStartTray == false)
		{
			::SetForegroundWindow(_hWnd);
			::ShowWindow(_hWnd, SW_RESTORE);
			::ShowWindow(_hWnd, SW_SHOW);
		}

		// struct tray_menu item;
		// toggle_show(&item);

		struct tray_menu* item = tray.menu;
		do
		{
			if (strcmp(item->text, TEXT_HIDE) == 0 ||
				strcmp(item->text, TEXT_SHOW) == 0)
			{
				item->text = (char*)(g_bStartTray? TEXT_SHOW : TEXT_HIDE);
				item->checked = g_bStartTray;
				tray_update(&tray);
				break;
			}

		} while ((++item)->text != NULL);

	}

}

/// <summary>
/// static 함수, tray의 Show / Hide text 및 check 상태 설정
/// </summary>
/// <param name="bSetTextShowNchecked">TRUE:"Show", checked 적용</param>
void WebWindow::SetTrayStatus(bool bSetTextShowNchecked)
{
	struct tray_menu* item = tray.menu;
	do
	{
		if (strcmp(item->text, TEXT_HIDE) == 0 ||
			strcmp(item->text, TEXT_SHOW) == 0)
		{
			item->text = (char*)(bSetTextShowNchecked ? TEXT_SHOW : TEXT_HIDE);
			item->checked = bSetTextShowNchecked;
			tray_update(&tray);
			break;
		}

	} while ((++item)->text != NULL);

}

void WebWindow::SetTrayUse(bool useTray)
{
	g_bDoExit2TrayUse = useTray;
	//NTLog(this, Info, "Called : SetTrayUse(@@@@@@@@@@) : %s", (AutoString)(g_bDoExit2TrayUse ? "Yes" : "No"));
}

void WebWindow::SetUseClipCopyNsend(bool bUseCopyNsend)
{
	g_bClipCopyNsend = bUseCopyNsend;
	//NTLog(this, Info, "Called : SetUseClipCopyNsend(################) : %s", (AutoString)(bUseCopyNsend ? "Yes" : "No"));
}

void WebWindow::SetNativeClipboardHotKey(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode, int nIdx)
{
	wstring strTempHotKey = _T("");

	strTempHotKey += (bWin ? _T("1") : _T("0"));
	strTempHotKey += (bControl ? _T("1") : _T("0"));
	strTempHotKey += (bAlt ? _T("1") : _T("0"));
	strTempHotKey += (bShift ? _T("1") : _T("0"));
	strTempHotKey += chVKCode;

	NTLog(this, Info, "Called - SetNativeClipboardHotKey(###) - GroupID : %d, bAlt : %s, bControl : %s, bShift : %s, bWin : %s, VKCode : %c, nIdx : %d, Sum-Data : %s", 
		groupID,
		(AutoString)(bAlt ? L"Y" : L"N"),
		(AutoString)(bControl ? L"Y" : L"N"),
		(AutoString)(bShift ? L"Y" : L"N"),
		(AutoString)(bWin ? L"Y" : L"N"),
		(wchar_t)chVKCode, nIdx,
		(AutoString)strTempHotKey.data());

	mapHotKey[groupID] = strTempHotKey;
}

// End Of File