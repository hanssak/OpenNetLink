
// SGCleanDlg.h : 헤더 파일
//

#pragma once


// CSGCleanDlg 대화 상자
class CSGCleanDlg : public CDialogEx
{
// 생성입니다.
public:
	CSGCleanDlg(CWnd* pParent = NULL);	// 표준 생성자입니다.

// 대화 상자 데이터입니다.
	enum { IDD = IDD_SGCLEAN_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV 지원입니다.


// 구현입니다.
protected:
	HICON m_hIcon;
	HANDLE m_hThread;
	// 생성된 메시지 맵 함수
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
