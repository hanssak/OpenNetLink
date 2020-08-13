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
#include <sys/types.h>
#include <sys/stat.h>
#include <string>

#define UI_DRAG_TARGETS_COUNT 3

enum
{
    DT_TEXT,
    DT_URI,
    DT_URI_LIST
};

typedef struct
{
    const gchar *command;
	gchar **files;
} DragNDropData;
#endif
typedef char* AutoString;
#endif

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

struct FileInfoDND
{
    mode_t      st_mode; //S_IFDIR(0), S_IFREG(1)
    off_t       st_size;
    time_t      tCreate;
    time_t      tLast;
    std::string strFullName;
};

typedef void (*ACTION)();
typedef void (*WebMessageReceivedCallback)(AutoString message);
typedef void* (*WebResourceRequestedCallback)(AutoString url, int* outNumBytes, AutoString* outContentType);
typedef int (*GetAllMonitorsCallback)(const Monitor* monitor);
typedef void (*ResizedCallback)(int width, int height);
typedef void (*MovedCallback)(int x, int y);
typedef int (*GetDragDropListCallback)(const FileInfoDND* dragList);
typedef void (*NTLogCallback)(int nLevel, AutoString pcMessage);

class WebWindow
{
private:
	WebMessageReceivedCallback _webMessageReceivedCallback;
	MovedCallback _movedCallback;
	ResizedCallback _resizedCallback;
	NTLogCallback _ntlogCallback;
#ifdef _WIN32
	static HINSTANCE _hInstance;
	HWND _hWnd;
	WebWindow* _parent;
	wil::com_ptr<IWebView2Environment3> _webviewEnvironment;
	wil::com_ptr<IWebView2WebView5> _webviewWindow;
	std::map<std::wstring, WebResourceRequestedCallback> _schemeToRequestHandler;
	void AttachWebView();
#elif OS_LINUX
	GtkApplication* _app;
	GtkWidget* _window;
	GtkWidget* _webview;
	GtkWidget* _dialog;
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
	void GetDragDropList(GetDragDropListCallback callback);
	void SetNTLogCallback(NTLogCallback callback) { _ntlogCallback = callback; }
	void NTLog(int nLevel, AutoString pcMessage) { if (_ntlogCallback) _ntlogCallback(nLevel, pcMessage); }

#if OS_LINUX
	static gpointer DragNDropWorker(gpointer data);
	static size_t chopN(char *str, size_t n);
	static std::string UrlEncode(std::string strUri);
	static std::string UrlDecoded(std::string strUri);
	static FileInfoDND GetFileInfoDND(std::string strFile);
#else
	void MouseDropFilesAccept();
#endif
};

#endif // !WEBWINDOW_H
