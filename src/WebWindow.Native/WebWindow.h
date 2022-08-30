#ifndef WEBWINDOW_H
#define WEBWINDOW_H

#ifdef _WIN32

#define _AFXDLL

#include <afx.h>
#include <afxwin.h>         // MFC �ٽ� �� ǥ�� ���� ����Դϴ�.

//#include <Windows.h>
#include <wrl/event.h>
#include <map>
#include <string>
#include <wil/com.h>
#include <WebView2.h>
#include <direct.h>

typedef const wchar_t* AutoString;
typedef unsigned short mode_t;

#include <WinBase.h>
#include <atlimage.h>

#ifdef _UNICODE
	#define tstring wstring
	#define tsprintf  swprintf_s
	#define tvsnprintf _vsnwprintf
	#define tstat64 _wstat64_s
#else
	//typedef string tstring;
	#define tstring string
	#define tsprintf sprintf_s
	#define tvsnprintf vsnprintf_s
	#define tstat64 _stat64_s
#endif

#else
#ifdef OS_LINUX
#include <functional>
#include <gtk/gtk.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <string>
#include <fstream>
#include <iostream>
#include <pwd.h>

#define MAX_CLIPBOARD_PARM 8
typedef struct stClipBoardParam
{
	int nGroupId;
	char szExt[8];
	void *self;
} ClipBoardParam;
#elif OS_MAC
#include <cstddef>
#include <cstdio>
#include <string>
#include <fstream>
#include <iostream>
#include <pwd.h>
#include <sys/stat.h>
#include <sys/types.h>
#endif

typedef char* AutoString;
extern void *SelfThis;
#endif

#define WINDOW_MIN_WIDTH 750
#define WINDOW_MIN_HEIGHT 750

extern bool _bTrayUse;

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
	Err = 4,
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
typedef void (*ClipBoardCallback)(const int nGroupId, const int nType, const int nLength, const void *pMem, const int nExLength, const void* pExMem);
typedef void (*RecvClipBoardCallback)(const int nGroupId);
typedef void (*RequestedNavigateURLCallback)(AutoString navURI);

class WebWindow
{
private:
	WebMessageReceivedCallback _webMessageReceivedCallback;
	MovedCallback _movedCallback;
	ResizedCallback _resizedCallback;
	NTLogCallback _ntlogCallback;
	ClipBoardCallback _clipboardCallback;
	RecvClipBoardCallback _recvclipboardCallback;
	RequestedNavigateURLCallback _requestedNavigateURLCallback;

public:
#ifdef _WIN32
	static HINSTANCE _hInstance;
	HWND _hWnd;
	WebWindow* _parent;
	//wil::com_ptr<IWebView2Environment3> _webviewEnvironment;
	//wil::com_ptr<IWebView2WebView5> _webviewWindow;
	wil::com_ptr<ICoreWebView2Environment> _webviewEnvironment;
	wil::com_ptr<ICoreWebView2Controller> _webviewWindowController;
	wil::com_ptr<ICoreWebView2> _webviewWindow;
	std::map<std::wstring, WebResourceRequestedCallback> _schemeToRequestHandler;
	void AttachWebView();
	std::wstring GetInstallPath();
	std::wstring GetInstallPathFromRegistry();
	std::wstring GetInstallPathFromDisk();
	char m_chModulePath[MAX_PATH];

	std::map<int, bool> m_mapBoolUseClipSelect;
	std::map<int, bool> m_mapBoolClipSendTextFirst;

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
	int m_nAppNotiID;
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
	void ShowUserNotification(AutoString image, AutoString title, AutoString message, AutoString navURI = nullptr);
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
	void SetRequestedNavigateURLCallback(RequestedNavigateURLCallback callback) { _requestedNavigateURLCallback = callback; }
	void InvokeClipBoard(const int nGroupId, const int nType, const int nLength, const void *pMem, const int nExLength, const void* pExMem) { if (_clipboardCallback) _clipboardCallback(nGroupId, nType, nLength, pMem, nExLength, pExMem); }
	void InvokeRequestedNavigateURL(AutoString navURI) { if (_requestedNavigateURLCallback) _requestedNavigateURLCallback(navURI); }
	void SetTrayUse(bool useTray) { _bTrayUse = useTray; }

#if OS_LINUX
	void RegisterClipboardHotKey(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode);
	void UnRegisterClipboardHotKey(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode);
	void RegisterClipboardHotKeyNetOver(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode, int nIdx);
	void UnRegisterClipboardHotKeyNetOver(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode, int nIdx);
#elif _WIN32
	void MouseDropFilesAccept();
	void RegisterClipboardHotKey(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode) {}
	void UnRegisterClipboardHotKey(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode) {}
	void RegisterClipboardHotKeyNetOver(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode, int nIdx) {}
	void UnRegisterClipboardHotKeyNetOver(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode, int nIdx) {}

	int SendClipBoard(int groupID);
	size_t SaveImageFile(bool bClearPreMem = true, bool bClearExPreMem = true, bool bUseExtraMem = false);
	size_t SaveTxtDataMem(bool bClearPreMem = true, bool bClearExPreMem = true, bool bUseExtraMem = false);

	bool SaveBitmapFile(HBITMAP hBitmap, LPCTSTR lpFileName);
	bool GetClipboardBitmap(HBITMAP hbm, char* bmpPath);
	size_t GetLoadBitmapSize(char* filePath);
	size_t LoadClipboardBitmap(char* filePath, BYTE* result);
	void ClipDataBufferClear(bool bClearPreMem = true, bool bClearPreExMem = true);
	char* GetModulePath();
	bool SaveImage(char* PathName, void* lpBits, int size);

	void WriteLog(int lvl, TCHAR* chFile /* __FILE__*/, int line /* __LINE__ */, TCHAR* chfmt, ...);


#elif OS_MAC
	void GenerateHotKey(bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode);
	void RegisterQuitHotKey();
	void RegisterClipboardHotKey(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode);
	void UnRegisterClipboardHotKey(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode);
	void RegisterClipboardHotKeyNetOver(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode, int nIdx);
	void UnRegisterClipboardHotKeyNetOver(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode, int nIdx);
	AutoString ReadFileAndSaveForContextualTransfer(AutoString strPath, AutoString pCmdBuf, int nSize);
	int ContextualTransferClient(AutoString pCmdGuId, int nSize);
#endif

	void OnHotKey(int groupID);
	void ClipTypeSelect(int groupID);
	void ClipFirstSendTypeText(int groupID);

	void ClipMemFree(int groupID);
	void SetClipBoardSendFlag(int groupID);

	void SetClipBoard(int groupID, int nType, int nClipSize, void* data);

	void FolderOpen(AutoString strDownPath);
	void ProgramExit();
	void MoveWebWindowToTray();
	void MoveTrayToWebWindow();
	void MinimizeWebWindow();
	void RegisterStartProgram();
	void UnRegisterStartProgram();
	bool GetTrayUse();

};

#endif // !WEBWINDOW_H
