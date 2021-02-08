// dllmain.h : 모듈 클래스의 선언입니다.

class CAddFileRMModule : public ATL::CAtlDllModuleT< CAddFileRMModule >
{
public :
	DECLARE_LIBID(LIBID_AddFileRMLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_ADDFILERM, "{65ACD30A-3581-4AD5-8A98-068B8E848EE5}")
};

extern class CAddFileRMModule _AtlModule;
