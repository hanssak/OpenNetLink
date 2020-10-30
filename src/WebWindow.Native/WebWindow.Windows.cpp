#include "WebWindow.h"
#include <stdio.h>
#include <map>
#include <mutex>
#include <condition_variable>
#include <comdef.h>
#include <atomic>
#include <Shlwapi.h>
#include <string>
using namespace std;

#include "wintoastlib.h"
using namespace WinToastLib;

#define WM_USER_SHOWMESSAGE (WM_USER + 0x0001)
#define WM_USER_INVOKE (WM_USER + 0x0002)

using namespace Microsoft::WRL;

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

BYTE* result = NULL;
bool _bTrayUse = false;


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
	void toastActivated() const{
		std::wcout << L"The user clicked in this toast" << std::endl;
		std::wcout << L"strNaviURI : " << strNavi.c_str() << std::endl;
		if (m_window)
		{
			if (strNavi.length() > 0)
			{
				((WebWindow*)m_window)->InvokeRequestedNavigateURL(strNavi.c_str());
			}
		}
		//exit(0);
	}

	void toastActivated(int actionIndex) const{
		std::wcout << L"The user clicked on action #" << actionIndex << std::endl;
		std::wcout << L"strNaviURI : " << strNavi.c_str() << std::endl;
		if (m_window)
		{
			if (strNavi.length() > 0)
			{
				//((WebWindow*)m_window)->InvokeRequestedNavigateURL((AutoString)strNavi.c_str());
			}
		}
		//exit(16 + actionIndex);
	}

	void toastDismissed(WinToastDismissalReason state) const{
		switch (state) {
		case UserCanceled:
			std::wcout << L"The user dismissed this toast" << std::endl;
			//exit(1);
			break;
		case TimedOut:
			std::wcout << L"The toast has timed out" << std::endl;
			//exit(2);
			break;
		case ApplicationHidden:
			std::wcout << L"The application hid the toast using ToastNotifier.hide()" << std::endl;
			//exit(3);
			break;
		default:
			std::wcout << L"Toast not activated" << std::endl;
			//exit(4);
			break;
		}
	}

	void toastFailed() const{
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

static char* ConvertWCtoC(wchar_t* str)
{
	//반환할 char* 변수 선언
	char* pStr;

	//입력받은 wchar_t 변수의 길이를 구함
	int strSize = WideCharToMultiByte(CP_ACP, 0, str, -1, NULL, 0, NULL, NULL);
	//char* 메모리 할당
	pStr = new char[strSize];

	//형 변환 
	WideCharToMultiByte(CP_ACP, 0, str, -1, pStr, strSize, 0, 0);
	return pStr;
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
	_bTrayUse = false;

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
    tray.menu[0] = {(char*)"About",0,0,0,hello_cb,NULL,NULL};
    tray.menu[1] = {(char*)"-",0,0,0,NULL,NULL,NULL};
    tray.menu[2] = {(char*)"Hide",0,0,0,toggle_show,NULL,NULL};
    tray.menu[3] = {(char*)"-",0,0,0,NULL,NULL,NULL};
    tray.menu[4] = {(char*)"Quit",0,0,0,quit_cb,NULL,NULL};
    tray.menu[5] = {NULL,0,0,0,NULL,NULL,NULL};
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
	case WM_GETMINMAXINFO:
		mmi = (LPMINMAXINFO)lParam;
		mmi->ptMinTrackSize.x = WINDOW_MIN_WIDTH;
		mmi->ptMinTrackSize.y = WINDOW_MIN_HEIGHT;
		return 0;
	case WM_CLOSE:
		if (_bTrayUse)
		{
			printf("Tray Move!!");
			if (hwnd == messageLoopRootWindowHandle)
			{
				struct tray_menu* item = tray.menu;
				do
				{
					if (strcmp(item->text, "Hide") == 0) {
						toggle_show(item);
						break;
					}
				} while ((++item)->text != NULL);
			}
		}
		else
		{
			tray_exit();
			printf("Exit!!\n");
			hwndToWebWindow.erase(hwnd);
			if (hwnd == messageLoopRootWindowHandle)
			{
				PostQuitMessage(0);
				printf("PostQuitMessage\n");
			}
			DWORD pid = GetCurrentProcessId();
			KillProcess(pid);
			if (!pid)
				KillProcess(pid);
		}
		return 0;
	case WM_DESTROY:
	{
		// Only terminate the message loop if the window being closed is the one that
		// started the message loop

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
	if (_webviewWindow)
	{
		RECT bounds;
		GetClientRect(_hWnd, &bounds);
		_webviewWindow->put_Bounds(bounds);
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
		printf("failed to create tray\n");
		return ;
	}
	while (tray_loop(1) == 0)
	{
		//printf("iteration\n");
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
	std::atomic_flag flag = ATOMIC_FLAG_INIT;
	flag.test_and_set();

	std::wstring edgeFolderPath = GetInstallPath();
	HRESULT envResult = CreateWebView2EnvironmentWithDetails(edgeFolderPath.c_str(), nullptr, nullptr,
	//HRESULT envResult = CreateWebView2EnvironmentWithDetails(nullptr, nullptr, nullptr,
		Callback<IWebView2CreateWebView2EnvironmentCompletedHandler>(
			[&, this](HRESULT result, IWebView2Environment* env) -> HRESULT {
				HRESULT envResult = env->QueryInterface(&_webviewEnvironment);
				if (envResult != S_OK)
				{
					return envResult;
				}

				// Create a WebView, whose parent is the main window hWnd
				env->CreateWebView(_hWnd, Callback<IWebView2CreateWebViewCompletedHandler>(
					[&, this](HRESULT result, IWebView2WebView* webview) -> HRESULT {
						if (result != S_OK) { return result; }
						result = webview->QueryInterface(&_webviewWindow);
						if (result != S_OK) { return result; }

						// Add a few settings for the webview
						// this is a redundant demo step as they are the default settings values
						IWebView2Settings* Settings;
						_webviewWindow->get_Settings(&Settings);
						Settings->put_IsScriptEnabled(TRUE);
						Settings->put_AreDefaultScriptDialogsEnabled(TRUE);
						Settings->put_IsWebMessageEnabled(TRUE);

						// Register interop APIs
						EventRegistrationToken webMessageToken;
						_webviewWindow->AddScriptToExecuteOnDocumentCreated(L"window.external = { sendMessage: function(message) { window.chrome.webview.postMessage(message); }, receiveMessage: function(callback) { window.chrome.webview.addEventListener(\'message\', function(e) { callback(e.data); }); } };", nullptr);
						_webviewWindow->add_WebMessageReceived(Callback<IWebView2WebMessageReceivedEventHandler>(
							[this](IWebView2WebView* webview, IWebView2WebMessageReceivedEventArgs* args) -> HRESULT {
								wil::unique_cotaskmem_string message;
								args->get_WebMessageAsString(&message);
								_webMessageReceivedCallback(message.get());
								return S_OK;
							}).Get(), &webMessageToken);

						EventRegistrationToken webResourceRequestedToken;
						_webviewWindow->AddWebResourceRequestedFilter(L"*", WEBVIEW2_WEB_RESOURCE_CONTEXT_ALL);
						_webviewWindow->add_WebResourceRequested(Callback<IWebView2WebResourceRequestedEventHandler>(
							[this](IWebView2WebView* sender, IWebView2WebResourceRequestedEventArgs* args)
							{
								IWebView2WebResourceRequest* req;
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
											wil::com_ptr<IWebView2WebResourceResponse> response;
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

	wchar_t ModelID[MAX_PATH];
	memset(ModelID, 0x00, sizeof(ModelID));
	wsprintf(ModelID, L"Noti%d", m_nAppNotiID++);
	appUserModelID = (LPWSTR)ModelID;
	
	onlyCreateShortcut = false;

	WinToast::instance()->setAppName(appName);
	WinToast::instance()->setAppUserModelId(appUserModelID);

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
	if (!text)
		text = (LPWSTR)L"Hello, world!";

	if (!WinToast::instance()->initialize()) {
		std::wcerr << L"Error, your system in not compatible!" << std::endl;
		return;
	}
	bool withImage = (imagePath != NULL);
	WinToastTemplate templ(withImage ? WinToastTemplate::ImageAndText02 : WinToastTemplate::Text02);
	templ.setTextField(text, WinToastTemplate::FirstLine);
	templ.setAudioOption(audioOption);
	templ.setAttributionText(attribute);

	for (auto const& action : actions)
		templ.addAction(action);
	if (expiration)
		templ.setExpiration(expiration);
	if (withImage)
		templ.setImagePath(imagePath);

	if ((g_CustomHandler != NULL) && (navURI != NULL))
	{
		g_CustomHandler->SetNaviURI(navURI);
		std::wcerr << "URI : " << navURI << endl;
	}
	if (WinToast::instance()->showToast(templ, g_CustomHandler) < 0) {
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

	int Ret = SendClipBoard(groupID);
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

int WebWindow::SendClipBoard(int groupID)
{
	HBITMAP hbm;
	HWND hwndDesktop = GetDesktopWindow();
	HGLOBAL hglb;

	int nClipbard = 0;

	int nType = 0;
	
	size_t nTotalLen = 0;

	FILE* fd;
	errno_t err;

	int rSize = 1024 * 64;
	struct stat st;
	size_t nRead = 0;

	int rCount = 0; 
	int len = 0;

	if (OpenClipboard(hwndDesktop))
	{
		if (IsClipboardFormatAvailable(CF_BITMAP) || IsClipboardFormatAvailable(CF_DIB))
		{
			hbm = (HBITMAP)GetClipboardData(CF_BITMAP);
			GlobalLock(hbm);
			char filePath[512];
			if (GetClipboardBitmap(hbm, filePath) == false)
			{
				GlobalUnlock(hbm);
				CloseClipboard();
				return -1;
			}
			nTotalLen = GetLoadBitmapSize(filePath);
			printf("GetLoadBitmapSize after filepath = %s\n", filePath);
			//nTotalLen = LoadClipboardBitmap(filePath, result);
			if (nTotalLen < 0)
			{
				GlobalUnlock(hbm);
				CloseClipboard();
				return -1;
			}

			if ((err = fopen_s(&fd, filePath, "rb")) != 0)
			{
				MessageBox(_hWnd, L"Clipboard image Load Fail!", L"Error Clipboard Img", MB_OK);
				GlobalUnlock(hbm);
				CloseClipboard();
				return -1;
			}

			result = new BYTE[nTotalLen];
			printf("result nTotalLen = %zd\n",nTotalLen);
			memset(result, 0x00, nTotalLen);
		
			stat(filePath, &st);
			rCount = (int)(st.st_size / (1024 * 64)) + ((st.st_size % (1024 * 64)) ? 1 : 0);
			size_t nReadTotalLen = 0;
			for (int i = 0, len = st.st_size; i < rCount; i++)
			{
				if (len > (1024 * 64))
					rSize = (1024 * 64);
				else
					rSize = len;
				if ((nRead = fread(result + nReadTotalLen, 1, rSize, fd)) <= 0)
				{
					fclose(fd);
					GlobalUnlock(hbm);
					CloseClipboard();
					ClipDataBufferClear();
					return -1;
				}
				len -= rSize;
				nReadTotalLen += nRead;

				//printf("nReadTotalLen : %zd , nRead : %zd\n\n", nReadTotalLen, nRead);
			}
			fclose(fd);

			GlobalUnlock(hbm);
			nType = 2;
		}
		else if (IsClipboardFormatAvailable(CF_TEXT) || IsClipboardFormatAvailable(CF_OEMTEXT) || IsClipboardFormatAvailable(CF_UNICODETEXT))
		{
			if ((hglb = GetClipboardData(CF_UNICODETEXT)))
			{
				wchar_t* wclpstr = (wchar_t*)GlobalLock(hglb);
				size_t len = (wcslen(wclpstr) + 2) * sizeof(wchar_t);
				len *= 2;

				result = new BYTE[len];
				memset(result, 0x00, len);
				WidecodeToUtf8(wclpstr, (char*)result);
				nTotalLen=strlen((char*)result);
				GlobalUnlock(hglb);
				nType = 1;
			}
		}
		else
			return -1;

	}

	CloseClipboard();

	if(_clipboardCallback!=NULL)
		_clipboardCallback(groupID, nType, (int)nTotalLen, result);

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
size_t WebWindow::LoadClipboardBitmap(char* filePath, BYTE* result)
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
	result = new BYTE[(int)st.st_size];

	int rCount = (int)(st.st_size / (1024 * 64)) + ((st.st_size % (1024 * 64)) ? 1 : 0);
	int len = 0;
	for (int i = 0, len=st.st_size; i < rCount; i++)
	{
		if (len > (1024 * 64))
			rSize = (1024 * 64);
		else
			rSize = len;
		if ((nRead = fread(result+nTotalLen, 1, rSize, fd)) <= 0)
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
void WebWindow::ClipDataBufferClear()
{
	if (result != NULL)
	{
		delete[] result;
		result = NULL;
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
		}
		else if (nType == 2)
		{
			sprintf_s(workdirpath, ".\\work");
			CreateDirectoryA(workdirpath, NULL);
			sprintf_s(filepath, "%swork\\recvClip.dat", GetModulePath());
			DeleteFileA(filepath);
			printf("FilePath = %s\n", filepath);
			printf("nClipSize = %ld\n", nClipSize);
			SaveImage(filepath , (void*)data, nClipSize);
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
	hwndToWebWindow.erase(hwnd);
	if (hwnd == messageLoopRootWindowHandle)
	{
		PostQuitMessage(0);
		printf("PostQuitMessage\n");
	}
	DWORD pid = GetCurrentProcessId();
	KillProcess(pid);
	if (!pid)
		KillProcess(pid);
}

void WebWindow::MoveWebWindowToTray()
{
	NTLog(this, Info, "Called : OpenNetLink Move To Tray");
	struct tray_menu *item = tray.menu;
	do
	{
		if (strcmp(item->text, "Hide") == 0) {
			toggle_show(item);
			break;
		}
	} while ((++item)->text != NULL);
}

void WebWindow::RegisterStartProgram()
{
	// TODO: Register logic for start program
}

void WebWindow::UnRegisterStartProgram()
{
	// TODO: UnRegister logic for start program
}