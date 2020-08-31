// AddFileRM.h : CAddFileRM�� �����Դϴ�.

#pragma once
#include "resource.h"       // �� ��ȣ�Դϴ�.



#include "AddFileRM_i.h"
#include <shlobj.h>
#include <comdef.h>
#include <vector>

#if defined(_WIN32_WCE) && !defined(_CE_DCOM) && !defined(_CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA)
#error "���� ������ COM ��ü�� ��ü DCOM ������ �������� �ʴ� Windows Mobile �÷����� ���� Windows CE �÷������� ����� �������� �ʽ��ϴ�. ATL�� ���� ������ COM ��ü�� ������ �����ϰ� ���� ������ COM ��ü ������ ����� �� �ֵ��� _CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA�� �����Ͻʽÿ�. rgs ������ ������ ���� DCOM Windows CE�� �ƴ� �÷������� �����Ǵ� ������ ������ ���̹Ƿ� 'Free'�� �����Ǿ� �ֽ��ϴ�."
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
	bool m_bData; //�����Ͱ� �ִ����� ���� bool


};

OBJECT_ENTRY_AUTO(__uuidof(AddFileRM), CAddFileRM)
