
// SGCleanDlg.cpp : ���� ����
//

#include "stdafx.h"
#include "SGClean.h"
#include "SGCleanDlg.h"
#include "afxdialogex.h"
#include <TlHelp32.h>


#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// ���� ���α׷� ������ ���Ǵ� CAboutDlg ��ȭ �����Դϴ�.

class CAboutDlg : public CDialogEx
{
public:
	CAboutDlg();

// ��ȭ ���� �������Դϴ�.
	enum { IDD = IDD_ABOUTBOX };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV �����Դϴ�.

// �����Դϴ�.
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


// CSGCleanDlg ��ȭ ����


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


// CSGCleanDlg �޽��� ó����

BOOL CSGCleanDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// �ý��� �޴��� "����..." �޴� �׸��� �߰��մϴ�.
	ModifyStyleEx(WS_EX_APPWINDOW, WS_EX_TOOLWINDOW);
	// IDM_ABOUTBOX�� �ý��� ��� ������ �־�� �մϴ�.
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

	// �� ��ȭ ������ �������� �����մϴ�. ���� ���α׷��� �� â�� ��ȭ ���ڰ� �ƴ� ��쿡��
	//  �����ӿ�ũ�� �� �۾��� �ڵ����� �����մϴ�.
	SetIcon(m_hIcon, TRUE);			// ū �������� �����մϴ�.
	SetIcon(m_hIcon, FALSE);		// ���� �������� �����մϴ�.

	// TODO: ���⿡ �߰� �ʱ�ȭ �۾��� �߰��մϴ�.
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
	return TRUE;  // ��Ŀ���� ��Ʈ�ѿ� �������� ������ TRUE�� ��ȯ�մϴ�.
}

BOOL CSGCleanDlg::GetProcessModule(DWORD dwPID, CString sProcessName)
{
	HANDLE hModuleSnap = NULL;
	MODULEENTRY32 me32 = {0};
	hModuleSnap = CreateToolhelp32Snapshot(TH32CS_SNAPMODULE, dwPID);
	if(hModuleSnap == (HANDLE)-1)
		return (FALSE);
	me32.dwSize = sizeof(MODULEENTRY32);

	//�ش� ���μ����� ��⸮��Ʈ�� ������ ������ ���μ����̸��� �����ϸ�
	//true�� �����Ѵ�.
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

	//���μ����� �޸𸮻� ������ ù��° ���μ����� ��´�
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
						unsigned long nCode; //���μ��� ���� ����
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

// ��ȭ ���ڿ� �ּ�ȭ ���߸� �߰��� ��� �������� �׸�����
//  �Ʒ� �ڵ尡 �ʿ��մϴ�. ����/�� ���� ����ϴ� MFC ���� ���α׷��� ��쿡��
//  �����ӿ�ũ���� �� �۾��� �ڵ����� �����մϴ�.

void CSGCleanDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // �׸��⸦ ���� ����̽� ���ؽ�Ʈ�Դϴ�.

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// Ŭ���̾�Ʈ �簢������ �������� ����� ����ϴ�.
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// �������� �׸��ϴ�.
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialogEx::OnPaint();
	}
}

// ����ڰ� �ּ�ȭ�� â�� ���� ���ȿ� Ŀ���� ǥ�õǵ��� �ý��ۿ���
//  �� �Լ��� ȣ���մϴ�.
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