#include "WebWindow.h"

#ifdef _WIN32
# define EXPORTED __declspec(dllexport)
#else
# define EXPORTED
#endif

extern "C"
{
#ifdef _WIN32
	EXPORTED void WebWindow_register_win32(HINSTANCE hInstance)
	{
		WebWindow::Register(hInstance);
	}

	EXPORTED HWND WebWindow_getHwnd_win32(WebWindow* instance)
	{
		return instance->getHwnd();
	}
#elif OS_MAC
	EXPORTED void WebWindow_register_mac()
	{
		WebWindow::Register();
	}

	EXPORTED void WebWindow_GenerateHotKey(WebWindow* instance, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode)
	{
		instance->GenerateHotKey(bAlt, bControl,  bShift, bWin, chVKCode);
	}
#endif

	EXPORTED WebWindow* WebWindow_ctor(AutoString title, WebWindow* parent, WebMessageReceivedCallback webMessageReceivedCallback)
	{
		return new WebWindow(title, parent, webMessageReceivedCallback);
	}

	EXPORTED void WebWindow_dtor(WebWindow* instance)
	{
		delete instance;
	}

	EXPORTED void WebWindow_SetTitle(WebWindow* instance, AutoString title)
	{
		instance->SetTitle(title);
	}

	EXPORTED void WebWindow_Show(WebWindow* instance)
	{
		instance->Show();
	}

	EXPORTED void WebWindow_WaitForExit(WebWindow* instance)
	{
		instance->WaitForExit();
	}

	EXPORTED void WebWindow_ShowMessage(WebWindow* instance, AutoString title, AutoString body, unsigned int type)
	{
		instance->ShowMessage(title, body, type);
	}

	EXPORTED void WebWindow_Invoke(WebWindow* instance, ACTION callback)
	{
		instance->Invoke(callback);
	}

	EXPORTED void WebWindow_NavigateToString(WebWindow* instance, AutoString content)
	{
		instance->NavigateToString(content);
	}

	EXPORTED void WebWindow_NavigateToUrl(WebWindow* instance, AutoString url)
	{
		instance->NavigateToUrl(url);
	}

	EXPORTED void WebWindow_SendMessage(WebWindow* instance, AutoString message)
	{
		instance->SendMessage(message);
	}

	EXPORTED void WebWindow_ShowUserNotification(WebWindow* instance, AutoString image, AutoString title, AutoString message, AutoString navURI, AutoString appName)
	{
		instance->ShowUserNotification(image, title, message, navURI, appName);
	}

	EXPORTED void WebWindow_AddCustomScheme(WebWindow* instance, AutoString scheme, WebResourceRequestedCallback requestHandler)
	{
		instance->AddCustomScheme(scheme, requestHandler);
	}

	EXPORTED void WebWindow_SetResizable(WebWindow* instance, int resizable)
	{
		instance->SetResizable(resizable);
	}

	EXPORTED void WebWindow_GetSize(WebWindow* instance, int* width, int* height)
	{
		instance->GetSize(width, height);
	}

	EXPORTED void WebWindow_SetSize(WebWindow* instance, int width, int height)
	{
		instance->SetSize(width, height);
	}

	EXPORTED void WebWindow_SetResizedCallback(WebWindow* instance, ResizedCallback callback)
	{
		instance->SetResizedCallback(callback);
	}

	EXPORTED void WebWindow_GetAllMonitors(WebWindow* instance, GetAllMonitorsCallback callback)
	{
		instance->GetAllMonitors(callback);
	}

	EXPORTED unsigned int WebWindow_GetScreenDpi(WebWindow* instance)
	{
		return instance->GetScreenDpi();
	}

	EXPORTED void WebWindow_GetPosition(WebWindow* instance, int* x, int* y)
	{
		instance->GetPosition(x, y);
	}

	EXPORTED void WebWindow_SetPosition(WebWindow* instance, int x, int y)
	{
		instance->SetPosition(x, y);
	}

	EXPORTED void WebWindow_SetMovedCallback(WebWindow* instance, MovedCallback callback)
	{
		instance->SetMovedCallback(callback);
	}

	EXPORTED void WebWindow_SetTopmost(WebWindow* instance, int topmost)
	{
		instance->SetTopmost(topmost);
	}

	EXPORTED void WebWindow_SetIconFile(WebWindow* instance, AutoString filename)
	{
		instance->SetIconFile(filename);
	}

	EXPORTED void WebWindow_SetNTLogCallback(WebWindow* instance, NTLogCallback callback)
	{
		instance->SetNTLogCallback(callback);
	}

	EXPORTED void WebWindow_SetClipBoardCallback(WebWindow* instance, ClipBoardCallback callback)
	{
		instance->SetClipBoardCallback(callback);
	}
	EXPORTED void WebWindow_SetRecvClipBoardCallback(WebWindow* instance, RecvClipBoardCallback callback)
	{
		instance->SetRecvClipBoardCallback(callback);
	}
	EXPORTED void WebWindow_SetRequestedNavigateURLCallback(WebWindow* instance, RequestedNavigateURLCallback callback)
	{
		instance->SetRequestedNavigateURLCallback(callback);
	}
	EXPORTED void WebWindow_SetURLChangedCallback(WebWindow* instance, URLChangedCallback callback)
	{
		instance->SetURLChangedCallback(callback);
	}
	EXPORTED void WebWindow_SetDragNDropCallback(WebWindow* instance, DragNDropCallback callback)
	{
		instance->SetDragNDropCallback(callback);
	}
	
	EXPORTED void WebWindow_RegClipboardHotKey(WebWindow* instance, int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode)
	{
		instance->RegisterClipboardHotKey(groupID, bAlt, bControl,  bShift, bWin, chVKCode);
	}

	EXPORTED void WebWindow_UnRegClipboardHotKey(WebWindow* instance, int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode)
	{
		instance->UnRegisterClipboardHotKey(groupID, bAlt, bControl,  bShift, bWin, chVKCode);
	}

	EXPORTED void WebWindow_RegClipboardHotKeyNetOver(WebWindow* instance, int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode, int nIdx)
	{
		instance->RegisterClipboardHotKeyNetOver(groupID, bAlt, bControl, bShift, bWin, chVKCode, nIdx);
	}

	EXPORTED void WebWindow_UnRegClipboardHotKeyNetOver(WebWindow* instance, int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode, int nIdx)
	{
		instance->UnRegisterClipboardHotKeyNetOver(groupID, bAlt, bControl, bShift, bWin, chVKCode, nIdx);
	}

	EXPORTED void WebWindow_FolderOpen(WebWindow* instance, AutoString strFileDownPath)
	{
		instance->FolderOpen(strFileDownPath);
	}
	EXPORTED void WebWindow_SetTrayText(WebWindow* instance, AutoString tooltip, AutoString show, AutoString hide, AutoString exit, AutoString hyphen)
	{
		instance->SetTrayText(tooltip, show, hide, exit, hyphen);
	}
	EXPORTED void WebWindow_OnHotKey(WebWindow* instance, int groupID)
	{
		instance->OnHotKey(groupID);
	}
	EXPORTED void WebWindow_SetClipBoardData(WebWindow* instance, int groupID, int nType, int nClipSize, void* data)
	{
		instance->SetClipBoard(groupID, nType, nClipSize, data);
	}
	EXPORTED void WebWindow_ProgramExit(WebWindow* instance)
	{
		instance->ProgramExit();
	}
	EXPORTED void WebWindow_SetTrayUse(WebWindow* instance, bool useTray)
	{
		instance->SetTrayUse(useTray);
	}
	EXPORTED void WebWindow_SetTrayStartUse(WebWindow* instance, bool useTrayStart)
	{
		instance->SetTrayStartUse(useTrayStart);
	}
	EXPORTED void WebWindow_SetUseClipCopyNsend(WebWindow* instance, bool bUse)
	{
		instance->SetUseClipCopyNsend(bUse);
	}
	EXPORTED void WebWindow_SetUseClipBoardPasteHotKey(WebWindow* instance, int groupID, bool bUse)
	{
		instance->SetUseClipBoardPasteHotKey(groupID, bUse);
	}

	EXPORTED void WebWindow_SetUseHttpUrl(WebWindow* instance, bool bUse)
	{
		instance->SetUseHttpUrl(bUse);
	}
	EXPORTED void WebWindow_SetForeground(WebWindow* instance)
	{
		instance->SetForeground();
	}
	EXPORTED void WebWindow_SetNativeClipboardHotKey(WebWindow* instance, int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode, int nIdx)
	{
		instance->SetNativeClipboardHotKey(groupID, bAlt, bControl, bShift, bWin, chVKCode, nIdx);
	}
	EXPORTED bool WebWindow_GetTrayUse(WebWindow* instance)
	{
		return instance->GetTrayUse();
	}
	EXPORTED void WebWindow_MoveWebWindowToTray(WebWindow* instance)
	{
		instance->MoveWebWindowToTray();
	}
	EXPORTED void WebWindow_MoveTrayToWebWindow(WebWindow* instance)
	{
		instance->MoveTrayToWebWindow();
	}
	EXPORTED void WebWindow_RegStartProgram(WebWindow* instance)
	{
		instance->RegisterStartProgram();
	}
	EXPORTED void WebWindow_UnRegStartProgram(WebWindow* instance)
	{
		instance->UnRegisterStartProgram();
	}
	EXPORTED void WebWindow_UseClipSelect(WebWindow* instance, int groupID)
	{
		instance->ClipTypeSelect(groupID);
	}
	EXPORTED void WebWindow_ClipMemFree(WebWindow* instance, int groupID)
	{
		instance->ClipMemFree(groupID);
	}
	EXPORTED void WebWindow_UseClipFirstSendType(WebWindow* instance, int groupID)
	{
		instance->ClipFirstSendTypeText(groupID);
	}
	EXPORTED void WebWindow_SetClipBoardSendFlag(WebWindow* instance, int groupID)
	{
		instance->SetClipBoardSendFlag(groupID);
	}
	EXPORTED void WebWindow_SetDragNDropFilePath(WebWindow* instance)
	{
		instance->SetDragNDropFilePath();
	}
}

extern "C" void _NTLog_(const void *Self, int nLevel, const char *pszFuncName, const char *pszFileName, const int nLineNo, const char *pszFormat, ...)
{
#define MAX_LOG_LENGTH 4096
	int nLoc = 0;
    va_list valist;
	char szNativeLog[MAX_LOG_LENGTH];

	if (pszFileName != NULL)
	{
		char* plpPos = NULL;
		plpPos = (char*)strrchr(pszFileName, '\\');
		if (plpPos != NULL)
			pszFileName = plpPos+1;
	}

    va_start(valist, pszFormat);
    nLoc += snprintf(szNativeLog+nLoc, MAX_LOG_LENGTH-256, "[NATIVE] ");
    nLoc += vsnprintf(szNativeLog+nLoc, MAX_LOG_LENGTH-256, pszFormat, valist);
    nLoc += snprintf(szNativeLog+nLoc, MAX_LOG_LENGTH-256, " in method %s at %s:%d", pszFuncName, pszFileName, nLineNo);
    va_end(valist);

   ((WebWindow *)Self)->NTLog(nLevel, (AutoString)szNativeLog);
}	