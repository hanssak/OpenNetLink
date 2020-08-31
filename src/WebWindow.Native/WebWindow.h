#ifndef WEBWINDOW_H
#define WEBWINDOW_H

#ifdef _WIN32
#include <Windows.h>
#include <wrl/event.h>
#include <map>
#include <string>
#include <wil/com.h>
#include <WebView2.h>
typedef const wchar_t* AutoString;
typedef unsigned short mode_t;
#else
#ifdef OS_LINUX
#include <functional>
#include <gtk/gtk.h>
#include <keybinder.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <string>

#define MAX_CLIPBOARD_PARM 8
typedef struct stClipBoardParam
{
	int nGroupId;
	char szExt[8];
	void *self;
} ClipBoardParam;
#endif
typedef char* AutoString;
#endif

typedef enum enDefineClipType
{
	D_CLIP_TEXT 	= 1,
	D_CLIP_IMAGE 	= 2,
	D_CLIP_OBJECT 	= 3
} D_CLIP_TYPE;

//
// Summary:
//     Specifies the meaning and relative importance of a log event.
typedef enum _enLogEventLevel
{
	//
	// Summary:
	//     Anything and everything you might want to know about a running block of code.
	Verbose = 0,
	//
	// Summary:
	//     Internal system events that aren't necessarily observable from the outside.
	Debug = 1,
	//
	// Summary:
	//     The lifeblood of operational intelligence - things happen.
	Info = 2,
	//
	// Summary:
	//     Service is degraded or endangered.
	Warning = 3,
	//
	// Summary:
	//     Functionality is unavailable, invariants are broken or data is lost.
	Error = 4,
	//
	// Summary:
	//     If you have a pager, it goes off when one of these occurs.
	Fatal = 5
} EVT_LOGLEVEL;

struct Monitor
{
	struct MonitorRect
	{
		int x, y;
		int width, height;
	} monitor, work;
};

typedef void (*ACTION)();
typedef void (*WebMessageReceivedCallback)(AutoString message);
typedef void* (*WebResourceRequestedCallback)(AutoString url, int* outNumBytes, AutoString* outContentType);
typedef int (*GetAllMonitorsCallback)(const Monitor* monitor);
typedef void (*ResizedCallback)(int width, int height);
typedef void (*MovedCallback)(int x, int y);
typedef void (*NTLogCallback)(int nLevel, AutoString pcMessage);
typedef void (*ClipBoardCallback)(const int nGroupId, const int nType, const int nLength, const void *pMem);
typedef void (*RecvClipBoardCallback)(const int nGroupId);

class WebWindow
{
private:
	WebMessageReceivedCallback _webMessageReceivedCallback;
	MovedCallback _movedCallback;
	ResizedCallback _resizedCallback;
	NTLogCallback _ntlogCallback;
	ClipBoardCallback _clipboardCallback;
	RecvClipBoardCallback _recvclipboardCallback;
#ifdef _WIN32
	static HINSTANCE _hInstance;
	HWND _hWnd;
	WebWindow* _parent;
	wil::com_ptr<IWebView2Environment3> _webviewEnvironment;
	wil::com_ptr<IWebView2WebView5> _webviewWindow;
	std::map<std::wstring, WebResourceRequestedCallback> _schemeToRequestHandler;
	void AttachWebView();
	char m_chModulePath[MAX_PATH];
#elif OS_LINUX
	GtkApplication* _app;
	GtkWidget* _window;
	GtkWidget* _webview;
	ClipBoardParam _clipboard[MAX_CLIPBOARD_PARM];
#elif OS_MAC
	void* _window;
	void* _webview;
	void* _webviewConfiguration;
	void AttachWebView();
#endif

public:
#ifdef _WIN32
	static void Register(HINSTANCE hInstance);
	HWND getHwnd();
	void RefitContent();
#elif OS_MAC
	static void Register();
#endif

	WebWindow(AutoString title, WebWindow* parent, WebMessageReceivedCallback webMessageReceivedCallback);
	~WebWindow();
	void SetTitle(AutoString title);
	void Show();
	void WaitForExit();
	void ShowMessage(AutoString title, AutoString body, unsigned int type);
	void Invoke(ACTION callback);
	void NavigateToUrl(AutoString url);
	void NavigateToString(AutoString content);
	void SendMessage(AutoString message);
	void ShowUserNotification(AutoString image, AutoString title, AutoString message);
	void AddCustomScheme(AutoString scheme, WebResourceRequestedCallback requestHandler);
	void SetResizable(bool resizable);
	void GetSize(int* width, int* height);
	void SetSize(int width, int height);
	void SetResizedCallback(ResizedCallback callback) { _resizedCallback = callback; }
	void InvokeResized(int width, int height) { if (_resizedCallback) _resizedCallback(width, height); }
	void GetAllMonitors(GetAllMonitorsCallback callback);
	unsigned int GetScreenDpi();
	void GetPosition(int* x, int* y);
	void SetPosition(int x, int y);
	void SetMovedCallback(MovedCallback callback) { _movedCallback = callback; }
	void InvokeMoved(int x, int y) { if (_movedCallback) _movedCallback(x, y); }
	void SetTopmost(bool topmost);
	void SetIconFile(AutoString filename);
	void SetNTLogCallback(NTLogCallback callback) { _ntlogCallback = callback; }
	void NTLog(int nLevel, AutoString pcMessage) { if (_ntlogCallback) _ntlogCallback(nLevel, pcMessage); }
	void SetClipBoardCallback(ClipBoardCallback callback) { _clipboardCallback = callback; }
	void SetRecvClipBoardCallback(RecvClipBoardCallback callback) { _recvclipboardCallback = callback; }
	void InvokeClipBoard(const int nGroupId, const int nType, const int nLength, const void *pMem) { if (_clipboardCallback) _clipboardCallback(nGroupId, nType, nLength, pMem); }

#if OS_LINUX
	void RegisterClipboardHotKey(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode);
	void UnRegisterClipboardHotKey(int groupID);
	void OnHotKey(int groupID) {}
#else
	void MouseDropFilesAccept();
	void RegisterClipboardHotKey(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode) {}
	void UnRegisterClipboardHotKey(int groupID) {}
	void OnHotKey(int groupID);
	int SendClipBoard(int groupID);
	bool SaveBitmapFile(HBITMAP hBitmap, LPCTSTR lpFileName);
	bool GetClipboardBitmap(HBITMAP hbm, char* bmpPath);
	size_t GetLoadBitmapSize(char* filePath);
	size_t LoadClipboardBitmap(char* filePath, BYTE* result);
	void ClipDataBufferClear();
	char* GetModulePath();
	bool SaveImage(char* PathName, void* lpBits, int size);
#endif
	void SetClipBoard(int groupID, int nType, int nClipSize, void* data);

	void FolderOpen(AutoString strDownPath);


};

#endif // !WEBWINDOW_H
