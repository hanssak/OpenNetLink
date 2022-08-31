
// SGClean.h : PROJECT_NAME 응용 프로그램에 대한 주 헤더 파일입니다.
//

#pragma once

#ifndef __AFXWIN_H__
	#error "PCH에 대해 이 파일을 포함하기 전에 'stdafx.h'를 포함합니다."
#endif

#include "resource.h"		// 주 기호입니다.


//typedef HANDLE(WINAPI* CreateToolhelp32Snapshot_t)(DWORD dwFlags, DWORD th32ProcessID);	// for CreateToolhelp32Snapshot
//typedef BOOL(WINAPI* ProcessWalk_t)(HANDLE hSnapshot, LPPROCESSENTRY32 lppe);				// for Process snap function
//typedef BOOL(WINAPI* ThreadWalk_t)(HANDLE hSnapshot, LPTHREADENTRY32 lppe);				// for Thread snap function


// CSGCleanApp:
// 이 클래스의 구현에 대해서는 SGClean.cpp을 참조하십시오.
//

class CSGCleanApp : public CWinApp
{
public:
	CSGCleanApp();

// 재정의입니다.
public:
	virtual BOOL InitInstance();

// 구현입니다.

	DECLARE_MESSAGE_MAP()
	
	/**
	*@breif 파라메타로 기능을 설정한다.
	*@return 0:SecureGate.exe종료 1:PCtoURL Proxy off
	*/
	int ParamCheck();

	BOOL OffProxyRegistry();
	//Proxy Status ini 설정 (0:PCtoURL 미사용 1:PCtoURL 사용)
	void SetIniProxyStatus(TCHAR *chStatus);

	void Refresh();

	string GetModulePath();

	BOOL GetProcessModule(DWORD dwPID, CString sProcessName);

	BOOL ProcessKill(CString strProcessName);
};

extern CSGCleanApp theApp;