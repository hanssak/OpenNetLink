// dllmain.h : ��� Ŭ������ �����Դϴ�.

class CAddFileRMModule : public ATL::CAtlDllModuleT< CAddFileRMModule >
{
public :
	DECLARE_LIBID(LIBID_AddFileRMLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_ADDFILERM, "{3E6847EC-D384-4C83-B1EC-79B0CE25A696}")
};

extern class CAddFileRMModule _AtlModule;
