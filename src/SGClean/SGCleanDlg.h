
// SGCleanDlg.h : ��� ����
//

#pragma once


// CSGCleanDlg ��ȭ ����
class CSGCleanDlg : public CDialogEx
{
// �����Դϴ�.
public:
	CSGCleanDlg(CWnd* pParent = NULL);	// ǥ�� �������Դϴ�.

// ��ȭ ���� �������Դϴ�.
	enum { IDD = IDD_SGCLEAN_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV �����Դϴ�.


// �����Դϴ�.
protected:
	HICON m_hIcon;
	HANDLE m_hThread;
	// ������ �޽��� �� �Լ�
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()
	static UINT ExitThread(LPVOID lpvoid);
	void KillAgent();
	BOOL ProcessKill(CString strProcessName);
	BOOL GetProcessModule(DWORD dwPID, CString sProcessName);
	bool m_bReRun;
#ifdef _UPDATE_POLICY_
	bool m_bAutoLogin;
#endif
public:
	void SetReRun(bool bReRun);
	bool GetReRun();
#ifdef _UPDATE_POLICY_
	void SetAutoLogin(bool bAuto);
	bool GetAutoLogin();
#endif
};
