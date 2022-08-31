// AddFileRM.cpp : DLL 내보내기의 구현입니다.


#include "stdafx.h"
#include "resource.h"
#include "AddFileRM_i.h"
#include "dllmain.h"
#include "Shlobj.h"
#include "sitedata.h"

// DLL이 OLE에 의해 언로드될 수 있는지 결정하는 데 사용됩니다.
STDAPI DllCanUnloadNow(void)
{
	return _AtlModule.DllCanUnloadNow();
}

// 클래스 팩터리를 반환하여 요청된 형식의 개체를 만듭니다.
STDAPI DllGetClassObject(REFCLSID rclsid, REFIID riid, LPVOID* ppv)
{
	return _AtlModule.DllGetClassObject(rclsid, riid, ppv);
}

// DllRegisterServer - 시스템 레지스트리에 항목을 추가합니다.
STDAPI DllRegisterServer(void)
{
	// 개체, 형식 라이브러리 및 형식 라이브러리에 들어 있는 모든 인터페이스를 등록합니다.
	HRESULT hr = _AtlModule.DllRegisterServer();
	::SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, NULL, NULL);
	return hr;
}

// DllUnregisterServer - 시스템 레지스트리에서 항목을 제거합니다.
STDAPI DllUnregisterServer(void)
{
	HRESULT hr = _AtlModule.DllUnregisterServer();
	::SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, NULL, NULL);
	return hr;
}

// DllInstall - 항목을 사용자별 및 컴퓨터별로 시스템 레지스트리에 추가하거나 시스템 레지스트리에서 제거합니다.
STDAPI DllInstall(BOOL bInstall, LPCWSTR pszCmdLine)
{
	HRESULT hr = E_FAIL;
	static const wchar_t szUserSwitch[] = L"user";

	if (pszCmdLine != NULL)
	{
		if (_wcsnicmp(pszCmdLine, szUserSwitch, _countof(szUserSwitch)) == 0)
		{
			ATL::AtlSetPerUserRegistration(true);
		}
	}

	if (bInstall)
	{	
		hr = DllRegisterServer();
		if (FAILED(hr))
		{
			DllUnregisterServer();
		}
	}
	else
	{
		hr = DllUnregisterServer();
	}

	return hr;
}


// AddFileRM.cpp : CAddFileRM의 구현입니다.

#include "stdafx.h"
#include "AddFileRM.h"
#include <atlconv.h>
#include <atltime.h>

// CAddFileRM
HRESULT CAddFileRM::Initialize ( 
	LPCITEMIDLIST pidlFolder,
	LPDATAOBJECT pDataObj,
	HKEY hProgID )
{
	FORMATETC fmt = { CF_HDROP, NULL, DVASPECT_CONTENT, -1, TYMED_HGLOBAL };
	STGMEDIUM stg = { TYMED_HGLOBAL };
	HDROP     hDrop;

	if(NULL == pDataObj)
		return E_INVALIDARG;

	// Look for CF_HDROP data in the data object.
	if ( FAILED( pDataObj->GetData ( &fmt, &stg )))
	{
		// Nope! Return an "invalid argument" error back to Explorer.
		return E_INVALIDARG;
	}

	// Get a pointer to the actual data.
	hDrop = (HDROP) GlobalLock ( stg.hGlobal );

	m_bData=false;
	// Make sure it worked.
	if ( NULL == hDrop )
	{
		m_bData=false;
		return E_INVALIDARG;
	}
	m_bData=true;

	m_uNumFiles=0;
	m_uNumFiles = DragQueryFile ( hDrop, 0xFFFFFFFF, NULL, 0 );

	if ( 0 == m_uNumFiles )
	{
		GlobalUnlock ( stg.hGlobal );
		ReleaseStgMedium ( &stg );
		return E_INVALIDARG;
	}

	HRESULT hr = S_OK;

	m_v.reserve((int)m_uNumFiles);
	for(UINT i=0; i<m_uNumFiles; ++i)
	{
		p_szFile = new TCHAR[MAX_PATH+32];
		if ( 0 == DragQueryFile ( hDrop, i, p_szFile, MAX_PATH ))
		{
			hr = E_INVALIDARG;
		}
		else
		{
			if (i == 0)
			{
				m_v.push_back(_T("0"));
			}
			m_v.push_back(p_szFile);
		}
	}

	GlobalUnlock ( stg.hGlobal );
	ReleaseStgMedium ( &stg );

	return hr;
}

HRESULT CAddFileRM::QueryContextMenu (
	HMENU hmenu,
	UINT  uMenuIndex, 
	UINT  uidFirstCmd,
	UINT  uidLastCmd,
	UINT  uFlags )
{
	// If the flags include CMF_DEFAULTONLY then we shouldn't do anything.
	if ( uFlags & CMF_DEFAULTONLY )
	{
		return MAKE_HRESULT ( SEVERITY_SUCCESS, FACILITY_NULL, 0 );
	}

	//메뉴 표시
	UINT uID = uidFirstCmd;
	if(m_bData==true)
	{
		InsertMenu ( hmenu, uMenuIndex, MF_BYPOSITION, uID++, _T(R_CONTEXT_NAME_1));
		//bitmap
		HBITMAP hBMP = (HBITMAP) LoadImage((HMODULE)_AtlBaseModule.m_hInst, MAKEINTRESOURCE(IDB_BITMAP1), IMAGE_BITMAP, 16, 16, 0);	
		SetMenuItemBitmaps(hmenu, uMenuIndex++, MF_BYPOSITION, hBMP, hBMP);

		InsertMenu(hmenu, uMenuIndex, MF_BYPOSITION, uID++, _T(R_CONTEXT_NAME_0));
		//bitmap
		HBITMAP hBMP2 = (HBITMAP)LoadImage((HMODULE)_AtlBaseModule.m_hInst, MAKEINTRESOURCE(IDB_BITMAP3), IMAGE_BITMAP, 16, 16, 0);
		SetMenuItemBitmaps(hmenu, uMenuIndex, MF_BYPOSITION, hBMP2, hBMP2);
	}

	return MAKE_HRESULT ( SEVERITY_SUCCESS, FACILITY_NULL, 1 );
}

HRESULT CAddFileRM::GetCommandString (
	UINT_PTR  idCmd,
	UINT  uFlags,
	UINT* pwReserved,
	LPSTR pszName,
	UINT  cchMax )
{
	USES_CONVERSION;

	// Check idCmd, it must be 0 since we have only one menu item.
	if ( 0 != idCmd )
		return E_INVALIDARG;

	// If Explorer is asking for a help string, copy our string into the
	// supplied buffer.
	if ( uFlags & GCS_HELPTEXT )
	{
		LPCTSTR szText = _T(R_CONTEXT_NAME);

		if ( uFlags & GCS_UNICODE )
		{
			// We need to cast pszName to a Unicode string, and then use the
			// Unicode string copy API.
			lstrcpynW ( (LPWSTR) pszName, T2CW(szText), cchMax );
		}
		else
		{
			// Use the ANSI string copy API to return the help string.
			lstrcpynA ( pszName, T2CA(szText), cchMax );
		}

		return S_OK;
	}

	return E_INVALIDARG;
}

HRESULT CAddFileRM::InvokeCommand ( LPCMINVOKECOMMANDINFO pCmdInfo )
{
	// If lpVerb really points to a string, ignore this function call and bail out.
	if (0 != HIWORD(pCmdInfo->lpVerb))
		return E_INVALIDARG;

	// Get the command index - the only valid one is 0.
	if(LOWORD(pCmdInfo->lpVerb)==0 && m_bData==true)
	{	
		HKEY hKey = NULL;
		DWORD bufflen = 0, dwType = 0;
		TCHAR sztemp[260];
		TCHAR szModulePath[260];
		TCHAR szRListPath[260];
		TCHAR szListFilePath[260];
		memset(sztemp, 0x00, sizeof(sztemp));
		memset(szModulePath, 0x00, sizeof(szModulePath));
		memset(szRListPath, 0x00, sizeof(szRListPath));
		memset(szListFilePath, 0x00, sizeof(szListFilePath));

		if(RegOpenKeyExW(HKEY_CLASSES_ROOT, _T("CLSID\\{55752D5F-87A0-4685-A886-6FD56FEC1C01}\\InprocServer32"), 0, KEY_READ, &hKey) == ERROR_SUCCESS) 
		{
			bufflen = sizeof(sztemp);
			RegQueryValueExW(hKey, _T(""), 0, &dwType, (LPBYTE)sztemp, &bufflen);
			RegCloseKey(hKey);
		}
		else
		{
			return E_INVALIDARG;
		}

		for (int i = wcslen(sztemp); i >= 0; i--)
		{
			if(sztemp[i] == '\\')
			{
				wcsncpy(szModulePath, sztemp, i);
				break;
			}
		}

		//디렉토리 생성
		TCHAR chPath[512]={0,};
		if(SHGetSpecialFolderPath(NULL, chPath, CSIDL_PROFILE, FALSE)==TRUE)
		{
			wsprintf(szRListPath, _T("%s\\AppData\\LocalLow\\HANSSAK\\RList"),chPath);
		}
		else
		{
			return E_INVALIDARG;
		}

		SHCreateDirectoryEx(NULL, szRListPath, NULL);
		SetFileAttributes(szRListPath, FILE_ATTRIBUTE_HIDDEN);

		//현재시간
		//memset(sztemp, 0x00, sizeof(sztemp));
		//CTime time = CTime::GetCurrentTime();
		//SYSTEMTIME st;
		//time.GetAsSystemTime(st);
		//wsprintf (sztemp, _T("%04d%02d%02d%02d%02d%02d"),st.wYear, st.wMonth, st.wDay, st.wHour, st.wMinute, st.wSecond);
		wsprintf (szListFilePath, _T("%s\\RList.txt"),szRListPath);
		DeleteFile(szListFilePath);

		//파일을 만든다.
		FILE *fWrite;
		fWrite=_wfopen(szListFilePath, L"wb");
		WORD mark = 0xFEFF;
		fwrite(&mark, sizeof(WORD), 1, fWrite);	

		TCHAR tchBuffer[MAX_PATH + 32] = { 0, };

		for(vector<wchar_t*>::size_type i=0; i<m_v.size(); ++i)
		{
			if(i>0)
			{
				fwprintf(fWrite, _T("\r\n"));
				// fwprintf(fWrite, m_v[i]);

				memcpy(tchBuffer, m_v[i], (MAX_PATH + 32) * sizeof(TCHAR));
				fwrite(tchBuffer, sizeof(TCHAR) * _tcslen(tchBuffer), 1, fWrite);

				if (m_v[i] != NULL)
					delete [] m_v[i];
			}
			else
			{
				fwprintf(fWrite, m_v[i]);
			}

			//if (m_v[i] != NULL)
			//	delete [] m_v[i];

		}

		if(fWrite)
			fclose(fWrite);
		m_v.clear();
		
		SetFileAttributes(szListFilePath, FILE_ATTRIBUTE_HIDDEN);

		TCHAR szParam [(MAX_PATH + 32)];
		memset(szParam, 0x00, sizeof(szParam));
		//wsprintf(szParam, _T("%d"), 0);

		TCHAR szRunPath[MAX_PATH];
		memset(szRunPath, 0x00, sizeof(szRunPath));
		wsprintf(szRunPath, _T("%s\\ContextTransferClient.exe"), szModulePath);

		::ShellExecute(NULL, _T("open"), szRunPath, _T("0"), NULL, SW_SHOWNORMAL);

		return S_OK;
	}
	else
		return E_INVALIDARG;
}
