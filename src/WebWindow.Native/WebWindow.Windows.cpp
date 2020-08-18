#include "WebWindow.h"
#include <stdio.h>
#include <map>
#include <mutex>
#include <condition_variable>
#include <comdef.h>
#include <atomic>
#include <Shlwapi.h>

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
	// Create the window
	_webMessageReceivedCallback = webMessageReceivedCallback;
	_parent = parent;
	_hWnd = CreateWindowEx(
		0,                              // Optional window styles.
		CLASS_NAME,                     // Window class
		title,							// Window text
		WS_OVERLAPPEDWINDOW,            // Window style

		// Size and position
		CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT,

		parent ? parent->_hWnd : NULL,       // Parent window
		NULL,       // Menu
		_hInstance, // Instance handle
		this        // Additional application data
	);
	hwndToWebWindow[_hWnd] = this;
}

// Needn't to release the handles.
WebWindow::~WebWindow() {}


HWND WebWindow::getHwnd()
{
	return _hWnd;
}

LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
	switch (uMsg)
	{
	case WM_HOTKEY:
	{
		WebWindow* webWindow = hwndToWebWindow[hwnd];
		if (webWindow)
		{
			webWindow->OnHotKey(wParam);
		}
		return 0;
	}
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
			webWindow->RefitContent();
			int width, height;
			webWindow->GetSize(&width, &height);
			webWindow->InvokeResized(width, height);
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
	MSG msg = { };
	while (GetMessage(&msg, NULL, 0, 0))
	{
		TranslateMessage(&msg);
		DispatchMessage(&msg);
	}
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

void WebWindow::AttachWebView()
{
	std::atomic_flag flag = ATOMIC_FLAG_INIT;
	flag.test_and_set();

	HRESULT envResult = CreateWebView2EnvironmentWithDetails(nullptr, nullptr, nullptr,
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
void WebWindow::ShowUserNotification(AutoString image, AutoString title, AutoString message)
{
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
#define REGHOTKEY_ID		11000
void WebWindow::RegisterClipboardHotKey(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode)
{

	int nHotKeyID = REGHOTKEY_ID + groupID;

	UnregisterHotKey(getHwnd(), nHotKeyID);

	UINT fsModifiers = 0;

	if(bAlt)
		fsModifiers |= MOD_ALT;             // Alt 키 조합 (0x0001)
	if (bControl)
		fsModifiers |= MOD_CONTROL;			// Control 키 조합 (0x0002)
	if (bShift)
		fsModifiers |= MOD_SHIFT;			// Shift 키 조합 (0x0004)
	if (bWin)
		fsModifiers |= MOD_WIN;			// Window 키 조합 (0x0008)

	//bool bRegRet = ::RegisterHotKey(_hWnd, nHotKeyID, fsModifiers, chVKCode);
	bool bRegRet = ::RegisterHotKey(_hWnd, nHotKeyID, 0, 0x56);
	DWORD dwError = GetLastError();
	if (bRegRet != TRUE)
	{
		MessageBox(_hWnd, L"Clipboard HotKey Register Fail!", L"ClipBoard HotKey", MB_OK);
		LPVOID lpMsgBuf = NULL;
		FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS, NULL, dwError, 0, (LPTSTR)&lpMsgBuf, 0, NULL);
		wchar_t chMessage[MAX_PATH];
		memset(chMessage, 0x00, sizeof(chMessage));
		wcscpy_s(chMessage, (TCHAR*)lpMsgBuf);
		//MessageBox(_hWnd, chMessage, L"Clipboard HotKey GetLastError Message", MB_OK);
	}
	else
		MessageBox(_hWnd, L"Clipboard HotKey Register Success!", L"ClipBoard HotKey", MB_OK);
}

void WebWindow::UnRegisterClipboardHotKey(int groupID)
{
	int nHotKeyID = REGHOTKEY_ID + groupID;
	UnregisterHotKey(getHwnd(), nHotKeyID);
}

void WebWindow::OnHotKey(WPARAM wParam)
{
	int groupID = 0;
	for (int i = 0; i < 10; i++)
	{
		if (wParam == (REGHOTKEY_ID + i))
		{
			groupID = i;
			int ret = SendClipBoard(groupID);
			MessageBox(_hWnd, L"Clipboard hotKey Excute!!!", L"ClipBoard HotKey", MB_OK);
		}
	}
}
char* WidecodeToUtf8(wchar_t* strUnicde, char* chDest)
{

	if (strUnicde == NULL)
		return NULL;

	size_t len = WideCharToMultiByte(CP_UTF8, 0, strUnicde, wcslen(strUnicde), NULL, 0, NULL, NULL);
	WideCharToMultiByte(CP_UTF8, 0, strUnicde, wcslen(strUnicde), chDest, len, NULL, NULL);

	return chDest;
}
int WebWindow::SendClipBoard(int groupID)
{
	HBITMAP hbm;
	HWND hwnd = GetDesktopWindow();
	HGLOBAL hglb;

	int nRetClipboard = 0;

	if (OpenClipboard(hwnd))
	{
		if (IsClipboardFormatAvailable(CF_BITMAP) || IsClipboardFormatAvailable(CF_DIB))
		{
			hbm = (HBITMAP)GetClipboardData(CF_BITMAP);
			GlobalLock(hbm);
			//nRetClipboard = SendClipboardBitmap(groupID, hbm);
			GlobalUnlock(hbm);
		} //
		else if (IsClipboardFormatAvailable(CF_TEXT) || IsClipboardFormatAvailable(CF_OEMTEXT) || IsClipboardFormatAvailable(CF_UNICODETEXT))
		{
			if ((hglb = GetClipboardData(CF_UNICODETEXT)))
			{
				wchar_t* wclpstr = (wchar_t*)GlobalLock(hglb);
				size_t len = (wcslen(wclpstr) + 2) * sizeof(wchar_t);
				len *= 2;

				char* chData = new char[len];
				memset(chData, 0x00, len);
				WidecodeToUtf8(wclpstr, chData);
				//nRetClipboard = SendClipboardText(groupID, chData);
				GlobalUnlock(hglb);
				delete[] chData;
			}
			else
				nRetClipboard = 1;

		}
		else
			nRetClipboard = 1;
	}
	else
		nRetClipboard = 1;

	CloseClipboard();

	return nRetClipboard;
}
