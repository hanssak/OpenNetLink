
// SGCleanDlg.cpp : 구현 파일
//

#include "stdafx.h"
#include "SGClean.h"
#include "SGCleanDlg.h"
#include "afxdialogex.h"
#include <TlHelp32.h>


#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// 응용 프로그램 정보에 사용되는 CAboutDlg 대화 상자입니다.

class CAboutDlg : public CDialogEx
{
public:
	CAboutDlg();

// 대화 상자 데이터입니다.
	enum { IDD = IDD_ABOUTBOX };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 지원입니다.

// 구현입니다.
protected:
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialogEx(CAboutDlg::IDD)
{

}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialogEx)
END_MESSAGE_MAP()


// CSGCleanDlg 대화 상자


CSGCleanDlg::CSGCleanDlg(CWnd* pParent /*=NULL*/)
	: CDialogEx(CSGCleanDlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
	m_hThread=NULL;
	m_bReRun=false;
#ifdef _UPDATE_POLICY_
	m_bAutoLogin=false;
#endif
}

void CSGCleanDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CSGCleanDlg, CDialogEx)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
END_MESSAGE_MAP()


// CSGCleanDlg 메시지 처리기

BOOL CSGCleanDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// 시스템 메뉴에 "정보..." 메뉴 항목을 추가합니다.
	ModifyStyleEx(WS_EX_APPWINDOW, WS_EX_TOOLWINDOW);
	// IDM_ABOUTBOX는 시스템 명령 범위에 있어야 합니다.
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != NULL)
	{
		BOOL bNameValid;
		CString strAboutMenu;
		bNameValid = strAboutMenu.LoadString(IDS_ABOUTBOX);
		ASSERT(bNameValid);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}
	}

	// 이 대화 상자의 아이콘을 설정합니다. 응용 프로그램의 주 창이 대화 상자가 아닐 경우에는
	//  프레임워크가 이 작업을 자동으로 수행합니다.
	SetIcon(m_hIcon, TRUE);			// 큰 아이콘을 설정합니다.
	SetIcon(m_hIcon, FALSE);		// 작은 아이콘을 설정합니다.

	// TODO: 여기에 추가 초기화 작업을 추가합니다.
	MoveWindow(0, 0, 0, 0, TRUE);
//	m_hThread=CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)CSGCleanDlg::ExitThread, (LPVOID)this, 0, NULL);
	Sleep(500);

	string strPath = GetModulePath();
	strPath = strPath.substr(0, strPath.length()-1);
	//KillFullPathApplication("SecureGate.exe",(char*)strPath.data());
	KillFullPathApplication("OpenNetLinkApp.exe", (char*)strPath.data());
#ifdef _UPDATE_POLICY_
	if(GetReRun()==true && GetAutoLogin()==true)
	{
		strPath.append("\\SecureGate.exe");
		ShellExecuteA(NULL, "open", strPath.data(), "U", NULL, SW_SHOWNORMAL);
	}
	else if(GetReRun()==true)
	{
		strPath.append("\\SecureGate.exe");
		ShellExecuteA(NULL, "open", strPath.data(), NULL, NULL, SW_SHOWNORMAL);
	}
#else
	if(GetReRun()==true)
	{
		strPath.append("\\SecureGate.exe");
		ShellExecuteA(NULL, "open", strPath.data(), NULL, NULL, SW_SHOWNORMAL);
	}
#endif
	CDialog::OnCancel();
	return TRUE;  // 포커스를 컨트롤에 설정하지 않으면 TRUE를 반환합니다.
}

BOOL CSGCleanDlg::GetProcessModule(DWORD dwPID, CString sProcessName)
{
	HANDLE hModuleSnap = NULL;
	MODULEENTRY32 me32 = {0};
	hModuleSnap = CreateToolhelp32Snapshot(TH32CS_SNAPMODULE, dwPID);
	if(hModuleSnap == (HANDLE)-1)
		return (FALSE);
	me32.dwSize = sizeof(MODULEENTRY32);

	//해당 프로세스의 모듈리스트를 루프로 돌려서 프로세스이름과 동일하면
	//true를 리턴한다.
	if(Module32First(hModuleSnap, &me32)) 
	{
		do 
		{
			if(me32.szModule == sProcessName)
			{
				CloseHandle (hModuleSnap);
				return true;
			}
		}
		while(Module32Next(hModuleSnap, &me32));
	}
	CloseHandle (hModuleSnap);
	return false;
}

BOOL CSGCleanDlg::ProcessKill(CString strProcessName)
{
	HANDLE hProcessSnap = NULL;
	BOOL bRet = FALSE;
	PROCESSENTRY32 pe32 = {0};

	hProcessSnap = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
	if (hProcessSnap == (HANDLE)-1)
		return false;
	pe32.dwSize = sizeof(PROCESSENTRY32);

	//프로세스가 메모리상에 있으면 첫번째 프로세스를 얻는다
	if (Process32First(hProcessSnap, &pe32))
	{
		BOOL bCurrent = FALSE;
		MODULEENTRY32 me32 = {0};

		do 
		{
			bCurrent = GetProcessModule(pe32.th32ProcessID,strProcessName);
			if(bCurrent) 
			{
				HANDLE hProcess = OpenProcess(PROCESS_ALL_ACCESS, FALSE, pe32.th32ProcessID);
				if(hProcess) 
				{
					if(TerminateProcess(hProcess, 0)) 
					{
						unsigned long nCode; //프로세스 종료 상태
						GetExitCodeProcess(hProcess, &nCode);
					}
					CloseHandle(hProcess);
				}
			}
		}
		while (Process32Next(hProcessSnap, &pe32));
	}

	CloseHandle (hProcessSnap);
	return true;
}


void CSGCleanDlg::KillAgent()
{
	ProcessKill(_T("SecureGate.exe"));
	CDialog::OnCancel();
}

UINT CSGCleanDlg::ExitThread(LPVOID lpvoid)
{
	CSGCleanDlg * pDlg=(CSGCleanDlg *)lpvoid;
	if(pDlg!=NULL)
	{
		pDlg->KillAgent();
	}
	return 0;
}

void CSGCleanDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else
	{
		CDialogEx::OnSysCommand(nID, lParam);
	}
}

// 대화 상자에 최소화 단추를 추가할 경우 아이콘을 그리려면
//  아래 코드가 필요합니다. 문서/뷰 모델을 사용하는 MFC 응용 프로그램의 경우에는
//  프레임워크에서 이 작업을 자동으로 수행합니다.

void CSGCleanDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // 그리기를 위한 디바이스 컨텍스트입니다.

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// 클라이언트 사각형에서 아이콘을 가운데에 맞춥니다.
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// 아이콘을 그립니다.
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialogEx::OnPaint();
	}
}

// 사용자가 최소화된 창을 끄는 동안에 커서가 표시되도록 시스템에서
//  이 함수를 호출합니다.
HCURSOR CSGCleanDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}

void CSGCleanDlg::SetReRun(bool bReRun)
{
	m_bReRun=bReRun;
}

bool CSGCleanDlg::GetReRun()
{
	return m_bReRun;
}

#ifdef _UPDATE_POLICY_
void CSGCleanDlg::SetAutoLogin(bool bAuto)
{
	m_bAutoLogin=bAuto;
}

bool CSGCleanDlg::GetAutoLogin()
{
	return m_bAutoLogin;
}
#endif