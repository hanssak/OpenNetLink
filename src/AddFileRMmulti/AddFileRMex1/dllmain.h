// dllmain.h : 모듈 클래스의 선언입니다.

class CAddFileRMModule : public ATL::CAtlDllModuleT< CAddFileRMModule >
{
public :
	DECLARE_LIBID(LIBID_AddFileRMLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_ADDFILERM, "{7533109A-F68E-4DA1-B5B0-AFB6080E6FCE}")
};

extern class CAddFileRMModule _AtlModule;
