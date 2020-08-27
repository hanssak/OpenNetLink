// AddFileRM.h : CAddFileRM의 선언입니다.

#pragma once
#include "resource.h"       // 주 기호입니다.



#include "AddFileRM_i.h"
#include <shlobj.h>
#include <comdef.h>
#include <vector>

#if defined(_WIN32_WCE) && !defined(_CE_DCOM) && !defined(_CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA)
#error "단일 스레드 COM 개체는 전체 DCOM 지원을 포함하지 않는 Windows Mobile 플랫폼과 같은 Windows CE 플랫폼에서 제대로 지원되지 않습니다. ATL이 단일 스레드 COM 개체의 생성을 지원하고 단일 스레드 COM 개체 구현을 사용할 수 있도록 _CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA를 정의하십시오. rgs 파일의 스레딩 모델은 DCOM Windows CE가 아닌 플랫폼에서 지원되는 유일한 스레딩 모델이므로 'Free'로 설정되어 있습니다."
#endif

using namespace ATL;
using namespace std;

// CAddFileRM

class ATL_NO_VTABLE CAddFileRM :
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<CAddFileRM, &CLSID_AddFileRM>,
	public IDispatchImpl<IAddFileRM, &IID_IAddFileRM, &LIBID_AddFileRMLib, /*wMajor =*/ 1, /*wMinor =*/ 0>,
	public IShellExtInit,
	public IContextMenu
{
public:
	CAddFileRM()
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_ADDFILERM)


BEGIN_COM_MAP(CAddFileRM)
	COM_INTERFACE_ENTRY(IAddFileRM)
	COM_INTERFACE_ENTRY(IDispatch)
	COM_INTERFACE_ENTRY(IShellExtInit)
	COM_INTERFACE_ENTRY(IContextMenu)
END_COM_MAP()



	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
		return S_OK;
	}

	void FinalRelease()
	{
	}

public:
	// IShellExtInit
	STDMETHOD(Initialize)(LPCITEMIDLIST, LPDATAOBJECT, HKEY);

public:
	// IContextMenu
	STDMETHOD(GetCommandString)(UINT_PTR, UINT, UINT*, LPSTR, UINT);
	STDMETHOD(InvokeCommand)(LPCMINVOKECOMMANDINFO);
	STDMETHOD(QueryContextMenu)(HMENU, UINT, UINT, UINT, UINT);

public:
	TCHAR *p_szFile;
	UINT m_uNumFiles;
	vector<wchar_t*> m_v;
	bool m_bData; //데이터가 있는지에 대한 bool


};

OBJECT_ENTRY_AUTO(__uuidof(AddFileRM), CAddFileRM)
