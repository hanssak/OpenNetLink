// dllmain.h : 모듈 클래스의 선언입니다.

class CAddFileRMModule : public ATL::CAtlDllModuleT< CAddFileRMModule >
{
public :
	DECLARE_LIBID(LIBID_AddFileRMLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_ADDFILERM, "{8964E4E1-C545-41C7-A3BD-150D5E1AE97A}")
};

extern class CAddFileRMModule _AtlModule;
