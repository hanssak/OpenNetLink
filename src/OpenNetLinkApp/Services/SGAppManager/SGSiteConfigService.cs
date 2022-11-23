using System;
using OpenNetLinkApp.Models.SGConfig;
using System.IO;
using System.Text.Json;
using System.Text;
using System.Collections.Generic;
using HsNetWorkSG;
using OpenNetLinkApp.Models.SGNetwork;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGSiteConfigService
    {
        public bool m_bUseClipAlarmType { get; set; }                                       // 클립보드 알림 형식 사용 유무
        public PAGE_TYPE m_enMainPage { get; set; }                                         // 메인페이지
        public bool m_bUseMainPageType { get; set; }                                        // 메인화면 사용 여부
        public bool m_bUseClipCopyAndSend { get; set; }                                     // 클립보드 복사 후 전송 사용 유무
        public bool m_bUseURLRedirectionAlarm { get; set; }                                 // URL 자동전환 알림 사용 유무
        public bool m_bUseURLRedirectionAlarmType { get; set; }                             // URL 자동전환 알림 타입 선택 사용 유무
        public bool m_bRFileAutoSend { get; set; }                                          // 마우스 우클릭 후 자동전송 사용 유무
        public bool bShowAfterApprAutoCheck { get; set; }                                     // 사후결재 기본 체크 사용 유무
        public bool m_bRecvFolderOpen { get; set; }                                         // 파일 수신 후 폴더 열기 사용 유무
        public bool m_bManualDownFolderChange { get; set; }                                 // 수동다운로드로 다운 시 폴더 선택 사용 유무
        public bool m_bFileRecvAlarmRetain { get; set; }                                    // 파일 수신 후 알림 유지 사용 유무
        public bool m_bApprCountAlarmRetain { get; set; }                                   // 승인대기 알림 유지 사용 유무
        public bool m_bApprCompleteAlarmRetain { get; set; }                                // 승인완료 알림 유지 사용 유무
        public bool m_bApprRejectAlarmRetain { get; set; }                                  // 반려 알림 유지 사용 유무
        public bool m_bUseApprCountAlaram { get; set; }                                     // 승인대기 알림 사용 유무.
        public bool m_bUseCloseTrayMove { get; set; }                                       // 종료 시 트레이 사용 유무.
        public bool m_bUseStartTrayMove { get; set; }                                       // 프로그램 시작 시 트레이 이동 사용 유무.
        public bool m_bUseStartProgramReg { get; set; }                                     // 시작 프로그램 등록 사용 유무.
        public bool m_bUseLanguageSet { get; set; }                                         // 언어설정 사용 유무.
        public bool m_bUseDashBoard { get; set; }                                           // 대쉬보드 창 사용 유무.
        public bool m_bViewFileFilter { get; set; }                                         // (환경설정) 확장자 제한 화면 표시 유무.
        public bool m_bUseForceUpdate { get; set; }                                         // 넘기는 기능 없이 무조건 업데이트 사용 유무
        public bool m_bUseSFMRight { get; set; }
        public List<ISGSiteConfig> SiteConfigInfo { get; }

        public bool GetUseLoginIDSave(int groupID);
        public bool GetUseAutoLogin(int groupID);
        public bool GetUseAutoLoginCheck(int groupID);
        public bool GetUseApprLineLocalSave(int groupID);
        public int GetZipPWBlock(int groupID);
        public bool GetUseApprLineChkBlock(int groupID);
        public bool GetUseApprDeptSearch(int groupID);
        public bool GetUseApprTreeSearch(int groupID);
        public bool GetUseUserPWChange(int groupID);
        public string GetPWChangeProhibitLimit(int groupID);
        public int GetPWChangeApplyCnt(int groupID);
        public string GetInitPasswordInfo(int groupID);

        public bool GetUseScreenLock();

        /// <summary>
        /// pageService 꺼 사용하세요 (siteConfig는 agent의 강제설정으로 만 사용)
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="bUseClipBoard"></param>
        public void SetUseClipBoard(int groupID, bool bUseClipBoard);
        /// <summary>
        /// pageService 꺼 사용하세요 (siteConfig는 agent의 강제설정으로 만 사용)
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool GetUseClipBoard(int groupID);
        public void SetUseURLRedirection(int groupID, bool bUseURLRedirection);
        public bool GetUseURLRection(int groupID);
        public void SetUseFileSend(int groupID, bool bUseFileSend);
        public bool GetUseFileSend(int groupID);

        /// <summary>
        /// 수신 폴더 사용자 변경 여부 가져오기
        /// </summary>
        /// <param name="groupID">그룹ID</param>
        /// <returns>true : 사용자 변경 가능  false : 사용자 변경 불가능</returns>
        public bool GetUseRecvFolderChange(int groupID);

        /// <summary>
        /// 로그인 유저별 다운로드 경로 사용 여부 가져오기
        /// </summary>
        /// <param name="groupID">그룹ID</param>
        /// <returns>true : 로그인 유저별 수신경로 사용, false : 로그인 유저별 수신경로 미사용</returns>
        public bool GetUseUserRecvDownPath(int groupID);
        public bool GetUseEmailManageApprove(int groupID);
        public bool GetUsePCURL(int groupID);
        public bool GetUseClipApprove(int groupID);
        public bool GetUsePublicBoard(int groupID);
        public bool GetUseCertSend(int groupID);
        public bool GetUseClipAlarmTypeChange();
        public bool GetUseMainPageTypeChange();
        public bool GetUseClipCopyAndSend();
        public bool GetUseURLRedirectionAlarm();
        public bool GetUseURLRedirectionAlarmType();
        public bool GetRFileAutoSend();
        public bool GetAfterApprAutoCheck();
        public bool GetRecvFolderOpen();
        public bool GetManualDownFolderChange();
        public bool GetFileRecvAlarmRetain();
        public bool GetApprCountAlarmRetain();
        public bool GetApprCompleteAlarmRetain();
        public bool GetApprRejectAlarmRetain();
        public bool GetUseApprCountAlaram();
        public bool GetUseCloseTrayMove();
        public bool GetUseStartTrayMove();
        public bool GetUseStartProgramReg();
        public bool GetUseLanguageSet();
        public bool GetUseDashBoard();
        public bool GetViewFileFilter();
        public bool GetUseForceUpdate();

        public bool GetViewSGSideBarUIBadge();

        public bool GetViewSGHeaderUIAlarmNoriAllDel();

        public bool GetUseDropErrorUI();

        public bool GetViewDlpApproverMyDept();

        /// <summary>
        /// 클립보드 파일전송형태 전송때, 무조건 결재없이  전송되게 함.
        /// </summary>
        /// <returns></returns>
        public bool GetUseClipBoardNoApproveButFileTrans();

        /// <summary>
        /// 처음 개발된 filefullPath 길이를 90 으로해서 송.수신에서 다 차단하는 방식사용 사용 유무
        /// </summary>
        /// <returns>false : 90길이로 송수신시차단, true : OS가 지원하는 최대한 길이 사용</returns>
        public bool GetUseOSMaxFilePath();

        /// <summary>
        /// 클립보드 파일전송형태 전송때, 0:CheckBox 및 결재 설정대로(개발중...), 1:사전, 2:사후 로 전송되게 적용
        /// </summary>
        /// <returns></returns>
        public int GetClipUseAfterApprove();

        /// <summary>
        /// 처음 접속 Server(Network) 를 사용자가 선택할 수 있도록 할건지 유무(Network.json 파일에 2개이상있어야 가능)
        /// </summary>
        /// <returns></returns>
        public bool GetUseSelectFirstConnectNetServer();

        /// <summary>
        /// 3망전송기능에서 한번에 모든 망에 자료를 송신하는 기능 사용 유무
        /// </summary>
        /// <returns></returns>
        //public bool GetUseNetOverAllsend();

        /// <summary>
        /// virus / apt 등에 대한 대외처리 신청하는 기능 사용 유무
        /// </summary>
        /// <returns></returns>
        public bool GetUseFileCheckException();

        /// <summary>
        /// 파일포워드 기능 사용할 것인지 유무
        /// </summary>
        /// <returns></returns>
        public bool GetUseFileForward();

        /// <summary>
        /// 파일수신하기전에 포워드로 받는사람이 먼저 수신가능 유무
        /// </summary>
        /// <returns></returns>
        public bool GetUseFileForwardDownBeforeRecv();

        /// <summary>
        /// PassWord 걸린 ZIP 파일에 대해 파일추가 거부하게 할지 유무(true:거부)
        /// </summary>
        /// <returns></returns>
        public bool GetUseDenyPasswordZip(int groupID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool GetUseFileClipManageUI(int groupID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="bUse"></param>
        //public void SetUseFileClipManageUI(int groupID, bool bUse);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool GetUseFileClipApproveUI(int groupID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="bUse"></param>
        //protected void SetUseFileClipApproveUI(int groupID, bool bUse);

        /// <summary>
        /// ClipBoard를 파일전송 Type으로 전송
        /// </summary>
        /// <returns></returns>
        public bool GetUseClipBoardFileTrans(int groupID);


        public bool GetNoApproveManageUI(int groupID);

        /// <summary>
        /// 파일포워딩 기능 사용유무
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool GetUseFileForward(int groupID);

        /// <summary>
        /// 클립보드 image / Text중에 사용할 것 선택전송하는 기능 사용유무
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool GetUseClipTypeSelectSend(int groupID);


        public bool GetUseClipTypeTextFirstSend(int groupID);


        public bool GetUseAgentBlockValueChange();

        public bool GetApproveAfterLimit(int groupID);

        public bool GetClipBoardApproveAfterLimit(int groupID);


        public bool GetUseOneToMultiLogin();


        public bool GetUseAppLoginType(int groupId);


        public int GetAppLoginType(int groupId);


        public bool GetUseOneAClockChangeAgentTimer();


        /// <summary>
        /// 결재자 추가 시 부서 표시 방식을 Step/Tree 타입 중 Step타입 표시 여부 (Tree 옵션일 경우, bApprDeptSearch 옵션 무효화)
        /// </summary>
        /// <returns></returns>
        public string GetApproverSearchType(int groupId);

        /// <summary>
        /// 결재자 관련 팝업 시 직접 입력하여 결재자를 검색할 수 있는 기능 사용 유무 (Input 컨트롤 표시 유무)
        /// </summary>
        /// <returns></returns>
        public bool GetUseInputSearchInApproverTree(int groupId);

        /// <summary>
        /// 수신자 추가 시 부서 표시 방식을 Step/Tree 타입 중 Step타입 표시 여부 (Tree 옵션일 경우, bApprDeptSearch 옵션 무효화)
        /// </summary>
        /// <returns></returns>
        public string GetReceiverSearchType(int groupId);

        /// <summary>
        /// 수신자 관련 팝업 시 직접 입력하여 결재자를 검색알 수 있는 기능 사용 (Input 컨트롤 표시 유무)
        /// </summary>
        /// <returns></returns>
        public bool GetUseInputSearchInReceiverTree(int groupId);

        /// <summary>
        /// 대결재등록 시 부서 표시 방식을 SEARCH/TREE 타입 중 설정 (TREE 옵션일 경우, bApprDeptSearch 옵션 무효화)
        /// </summary>
        /// <returns></returns>
        public string GetProxySearchType(int groupId);

        /// <summary>
        /// 대결재등록 시 직접 입력하여 결재자를 검색알 수 있는 기능 사용 (Input 컨트롤 표시 유무)
        /// </summary>
        /// <returns></returns>
        public bool GetUseInputSearchInProxyTree(int groupId);

        /// <summary>
        /// 보안결재자 등록 시 부서 표시 방식을 SEARCH/TREE 타입 중 설정 (TREE 옵션일 경우, bApprDeptSearch 옵션 무효화)
        /// </summary>
        /// <returns></returns>
        public string GetSecurityApproverSearchType(int groupId);
        /// <summary>
        /// 보안결재자 등록 시 직접 입력하여 결재자를 검색알 수 있는 기능 사용 (Input 컨트롤 표시 유무)
        /// </summary>
        /// <returns></returns>
        public bool GetUseInputSearchInSecurityApproverTree(int groupId);

        public string GetApproveExtSelectType(int groupId);

        public bool GetUseApproveExt(int groupId);

        public bool GetUseInputSearchApproveExtTree(int groupId);

    }

    internal class SGSiteConfigService : ISGSiteConfigService
    {
        public bool m_bUseClipAlarmType { get; set; } = true;                               // 클립보드 알림 형식 사용 유무
        public PAGE_TYPE m_enMainPage { get; set; } = PAGE_TYPE.NONE;                       // 메인페이지 설정
        public bool m_bUseMainPageType { get; set; } = true;                                // 메인화면 변경 사용 여부
        public bool m_bUseClipCopyAndSend { get; set; } = false;                            // 클립보드 복사 후 전송 사용 유무
        public bool m_bUseURLRedirectionAlarm { get; set; } = false;                        // URL 자동전환 알림 사용 유무
        public bool m_bUseURLRedirectionAlarmType { get; set; } = false;                    // URL 자동전환 알림 타입 선택 사용 유무
        public bool m_bRFileAutoSend { get; set; } = false;                                 // 마우스 우클릭 후 자동전송 사용 유무
        public bool bShowAfterApprAutoCheck { get; set; } = true;                             // 사후결재 기본 체크 사용 유무
        public bool m_bRecvFolderOpen { get; set; } = true;                                 // 파일 수신 후 폴더 열기 사용 유무
        public bool m_bManualDownFolderChange { get; set; } = false;                        // 수동다운로드로 다운 시 폴더 선택 사용 유무
        public bool m_bFileRecvAlarmRetain { get; set; } = true;                            // 파일 수신 후 알림 유지 사용 유무
        public bool m_bApprCountAlarmRetain { get; set; } = true;                           // 승인대기 알림 유지 사용 유무
        public bool m_bApprCompleteAlarmRetain { get; set; } = true;                        // 승인완료 알림 유지 사용 유무
        public bool m_bApprRejectAlarmRetain { get; set; } = true;                          // 반려 알림 유지 사용 유무
        public bool m_bUseApprCountAlaram { get; set; } = true;                             // 승인대기 알림 사용 유무.
        public bool m_bUseCloseTrayMove { get; set; } = true;                               // 종료 시 트레이 사용 유무.
        public bool m_bUseStartTrayMove { get; set; } = false;                              // 프로그램 시작 시 트레이 이동 사용 유무.
        public bool m_bUseStartProgramReg { get; set; } = false;                            // 시작 프로그램 등록 사용 유무.
        public bool m_bUseLanguageSet { get; set; } = false;                                // 언어설정 사용 유무.
        public bool m_bUseDashBoard { get; set; } = true;                                   // 대쉬보드 창 사용 유무.
        public bool m_bViewFileFilter { get; set; } = true;                                 // (환경설정) 확장자 제한 화면 표시 유무.
        public bool m_bUseForceUpdate { get; set; } = true;                                 // 넘기는 기능 없이 무조건 업데이트 사용 유무
        public bool m_bViewSGSideBarUIBadge { get; set; } = false;                          // 왼쪽 메뉴들에서 Badge 나오게할지 유무 설정값
        public bool m_bViewSGHeaderUIAlarmNoriAllDel { get; set; } = true;                  // 상단 HeaderUI에서 Alarm, Noti 상에 Badge 전체 삭제 메뉴 나오게할지 유무
        public bool m_bViewDropFileAddError { get; set; } = false;                          // 파일추가때, 5GB 이상 파일 추가되면 최대추가 파일크기가 5GB라고 UI가 나오는거 사용안함(false)
        public bool m_bViewDlpApproverSelectMyDept { get; set; } = false;                   // 정보보안 결재자 선택 화면 뜰때, 자기부서에 있는 사람들만 검색되어 나오도록 할 것이니 유무(true:자기부서만,false:전체)
        public bool m_bClipBoardNoApproveButFileTrans { get; set; } = false;                // 정보보안 결재자 선택 화면 뜰때, 자기부서에 있는 사람들만 검색되어 나오도록 할 것이니 유무(true:자기부서만,false:전체)
        public bool m_bUseUserRecvDownPath { get; set; } = false;                           // 로그인 유저별 다운로드 경로 사용 여부 (true : 사용, false : 미사용)
        public bool m_bUseOSMaxFilePath { get; set; } = true;                               // OS제공 최대 길이 사용 여부 (true : OS가 지원하는 최대한 길이 사용 false : filefullPath : 90, 파일/폴더이름길이 : 80) 
        public int m_nClipAfterApproveUseType { get; set; } = 0;                                    // 클립보드 파일전송형태 전송때, 0:CheckBox 및 결재 설정대로, 1:사전, 2:사후 로 전송되게 적용

        public bool m_bUseUserSelectFirstServer { get; set; } = false;                       // 사용자가 처음접속하는 Server(Network) 를 선택할 수 있을지 유무

        //public bool m_bUseNetOverAllsend { get; set; } = false;                              // 3망 연결된 망에 한번에 전부 전송하는 기능 사용유무

        public bool m_bUseFileCheckException { get; set; } = true;                              // virus / Apt에대한 예외처리 요청 기능 사용유무

        public bool m_bUseFileForward { get; set; } = false;                                    // 파일포워드 기능 사용유무(site.넷마블)

        public bool m_bUseFileForwardDownNotRecv { get; set; } = true;                         // 파일 수신되기전에 파일포워드로 다운로드 가능유무

        public bool m_bUseEmailManageApprove { get; set; } = false;                         // Email 관리 및 결재 기능 사용유무

        public bool m_bUseAgentBlockValueChange { get; set; } = true;                       // tbl_agent_block 에 들어가는 Type 값을 WebManager에서 data를 보여줄 수 있는 형태로 변경(WebManager/NetLink와 맞춤)

        public bool m_bUseSFMRight { get; set; } = true;                                    // (파일 전송할 때) 자신이 대결재자로 등록되어 있으면 대결재자의 권한을 따라가는지 여부 true면 따라가고 false면 따라가지 않는다.

        public bool m_bUseSelfSSOlogin { get; set; } = true;                                    // (다중망 로그인할 때) 한곳에 로그인하면, 나머지 망은 전부 로그인 처리하는 동작 사용유무

        public bool bUseAgentTime1aClock { get; set; } = false;         // 사후결재 정책, 자정에  검색화면 검색날짜 UI / 일일 송순가능수 UI 변경되는거 Server 시간이 아니라 agent 시간기준으로 동작(XX:00:00에 동작)

        public List<ISGSiteConfig> SiteConfigInfo { get; set; } = null;
        public SGSiteConfigService()
        {
            SiteConfigInfo = new List<ISGSiteConfig>();
            string strNetworkFileName = "wwwroot/conf/NetWork.json";
            string jsonString = File.ReadAllText(strNetworkFileName);
            List<ISGNetwork> listNetworks = new List<ISGNetwork>();
            using (JsonDocument document = JsonDocument.Parse(jsonString))
            {
                JsonElement root = document.RootElement;
                JsonElement NetWorkElement = root.GetProperty("NETWORKS");
                //JsonElement Element;
                foreach (JsonElement netElement in NetWorkElement.EnumerateArray())
                {
                    SGNetwork sgNet = new SGNetwork();
                    string strJsonElement = netElement.ToString();
                    var options = new JsonSerializerOptions
                    {
                        ReadCommentHandling = JsonCommentHandling.Skip,
                        AllowTrailingCommas = true,
                        PropertyNameCaseInsensitive = true,
                    };
                    sgNet = JsonSerializer.Deserialize<SGNetwork>(strJsonElement, options);
                    listNetworks.Add(sgNet);
                }
            }
            int count = listNetworks.Count;
            for (int i = 0; i < count; i++)
            {
                SGSiteConfig sgSiteConfig = new SGSiteConfig();
                sgSiteConfig.m_bUserIDSave = true;                      // 로그인한 ID 저장 여부
                sgSiteConfig.m_bAutoLogin = false;                      // 자동로그인 사용 여부.
                sgSiteConfig.m_bAutoLoginCheck = false;                 // 자동로그인 체크박스 체크여부.
                sgSiteConfig.m_bApprLineLocalSave = false;              // 결재라인 로컬 저장 여부.
                sgSiteConfig.m_nZipPWBlock = 0;                         // zip 파일 패스워드 검사 여부 ( 0 : 사용 안함, 1 : 비번 걸려 있을 경우 차단,  2 : 비번이 안걸려 있을 경우 차단 )
                sgSiteConfig.m_bTitleDescSameChk = false;               // 파일 전송 시 제목과 설명의 연속된 동일 문자 체크 여부.
                sgSiteConfig.m_bApprLineChkBlock = true;               // 고정 결재라인 차단 시 결재라인이 존재하지 않는 사용자에 대해 파일 전송 차단 여부 ( true : 전송 차단, false : 전송 허용 )
                sgSiteConfig.m_bApprDeptSearch = true;                  // 결재자 검색 창의 타부서 수정 가능 여부.
                sgSiteConfig.m_bUserPWChange = false;                   // 사용자 패스워드 변경 사용 여부.
                sgSiteConfig.m_strPWChangeProhibitLimit = "";           // 패스워드 사용금지 문자열 지정.
                sgSiteConfig.m_nPWChangeApplyCnt = 9;                   // 패스워드 변경 시 허용되는 자리수 지정.
                sgSiteConfig.m_strInitPasswd = "1K27SdexltsW0ubSCJgsZw=="; // hsck@2301

                sgSiteConfig.m_bUseScreenLock = true;                   // 화면잠금 사용유무 값설정
                sgSiteConfig.m_bRecvFolderChange = true;                // 수신폴더 변경 사용 여부

                //sgSiteConfig.m_bUseEmail = false;                // 이메일 결재 사용 유무
                //sgSiteConfig.m_bUsePCURL = false;                       // PCURL 사용 유무.
                //sgSiteConfig.m_bUseClipApprove = true;                 // 클립보드 결재 사용 유무.
                //sgSiteConfig.m_bUsePublicBoard = false;                 // 공지사항 사용 유무.

                //sgSiteConfig.m_bUseFileClipManageUI = false;            // 클립보드 파일형태 전송시 관리 UI
                //sgSiteConfig.m_bUseFileClipApproveUI = false;           // 클립보드 파일형태 전송시 결재 UI

                sgSiteConfig.m_bUseApproveAfterLimit = true;            // 사후결재 결재 Count 제한 사용유무
                sgSiteConfig.m_bUseClipBoardApproveAfterLimit = true;

                sgSiteConfig.m_bUserIDSave = true;                     //유저 아이디 저장
                sgSiteConfig.m_bUseScreenLock = true;                  //Screen Lock 설정여부

                sgSiteConfig.m_bRecvFolderChange = true;               // 수신폴더 변경 여부
                sgSiteConfig.m_bUseUserRecvDownPath = true;            // 로그인 유저별 다운로드 경로 사용 여부

                sgSiteConfig.m_bUseDenyPasswordZip = false;             // zip 같은 압축파일들 패스워드 걸려 있을때, 파일추가 안되게 할지 유무

                sgSiteConfig.m_bUseEmail = false;                        // 이메일 결재 사용 유무
                sgSiteConfig.m_bUsePCURL = false;                        // PCURL 사용여부
                sgSiteConfig.m_bUseClipApprove = false;                  //클립보드 결재 사용 유무
                sgSiteConfig.m_bUsePublicBoard = true;                  // 공지사항 사용 유무.
                
                sgSiteConfig.m_bUseCertSend = true;                     // 공인인증서 전송 사용 유무.
                sgSiteConfig.m_bUseClipBoardFileTrans = true;           // 클립보드 파일형태 전송 사용유무

                sgSiteConfig.m_bUseFileClipManageUI = true;             // 클립보드 파일형태 전송에 따른 관리UI 보여줄지 여부
                sgSiteConfig.m_bUseFileClipApproveUI = false;            // 클립보드 파일형태 전송에 따른 결재UI 보여줄지 여부

                sgSiteConfig.m_bUseClipTypeSelectSend = false;           // 클립보드 Mixed 일때, 사용자가 클립보드 선택해서 전송하는 기능 사용유무
                sgSiteConfig.m_bUseClipTypeTextFirstSend = false;        // 클립보드 Mixed 일때, Text 우선 사용(false:IMAGE 우선사용) - 사용자가 클립보드 선택해서 전송하는 기능 사용일때 이 설정은 동작X

                //각 화면별 검색 방법 설정
                sgSiteConfig.strApproverSearchType = "TREE";
                sgSiteConfig.bUseInputSearchInApproverTree = true;
                sgSiteConfig.strReceiverSearchType = "SEARCH";
                sgSiteConfig.bUseInputSearchInReceiverTree = true;
                sgSiteConfig.strProxySearchType = "TREE";   //대결재자 TREE 사용
                sgSiteConfig.bUseInputSearchInProxyTree = true;
                sgSiteConfig.strSecurityApproverSearchType = "SEARCH";
                sgSiteConfig.bUseInputSearchInSecurityApproverTree = true;
                sgSiteConfig.strApproveExtApproverSearchType = "SEARCH";        // 결재필수 확장자 검색됐을때, 결재자 검색방식
                sgSiteConfig.bUseApproveExt = true;                             // 결재필수 확장자 결재하는 기능 사용유무
                sgSiteConfig.bUseInputSearchApproveExtTree = false;           // 결재필수 확장자, 직접 입력하여 결재자를 검색알 수 있는 기능 사용 (Input 컨트롤 표시 유무)


                SiteConfigInfo.Add(sgSiteConfig);

            }

            SetUseClipAlarmTypeChange(true);                            // 클립보드 알림타입 변경 사용 유무           
            SetUseClipCopyAndSend(false);                               // 클립보드 복사 후 자동 전송 사용 유무

            // 현재 : 아래 두 값으로 설정 UI가 나오고 안나오고함 => 서버설정으로 나오도록 수정해야함
            // Footer 쪽도 URLRedirection 사용유무 설정에 따라서 UI 설정되도록 수정
            SetUseURLRedirectionAlarm(true);                            // URL 리다이렉션 알림 타입 사용 여부.(사용자가 설정가능유무)
            SetUseURLRedirectionAlarmType(true);                        // URL 리다이렉션 알림 타입 선택 사용 여부.

            SetRFileAutoSend(true);                                    // 오른쪽 마우스 클릭 후 자동 전송 사용 여부.
            SetAfterApprAutoCheck(true);                                // 사후결재 기본 체크

            SetRecvFolderOpen(true);                                    // 파일 수신 후 폴더 열기 사용 여부.
            SetManualDownFolderChange(false);                           // 수동다운로드 시 폴더 선택 사용 여부.

            SetFileRecvAlarmRetain(false);                              // 파일 수신 알림 유지 사용 여부.
            SetApprCountAlarmRetain(false);                             // 승인 대기 알림 유지 사용 여부.
            SetApprCompleteAlarmRetain(false);                          // 승인 완료 알림 유지 사용 여부.
            SetApprRejectAlarmRetain(false);                            // 승인 반려 알림 유지 사용 여부.
            SetUseApprCountAlaram(true);                                // 승인 대기 알림 사용 여부.

            SetUseCloseTrayMove(true);                                  // 종료 시 트레이 사용 여부.
            SetUseStartTrayMove(false);                                 // 프로그램 시작시 트레이 이동 여부.

            SetUseStartProgramReg(false);                               // 시작 프로그램 등록 사용 여부.

            SetUseLanguageSet(true);                                   // 언어 설정 사용 여부.

            SetUseDashBoard(true);                                      // 대쉬보드 창 사용 유무.
            SetUseForceUpdate(true);                                    // 넘기는 기능 없이 무조건 업데이트 사용유무
            SetMainPage(PAGE_TYPE.NONE);                                // 메인화면 설정 => DashBoard 사용 안하면 DASHBOARD로 선택했더라도 DASHBOARD는 나타나지 않음
            SetUseMainPageTypeChange(true);                            // 메인화면 변경 타입 사용 유무
            SetViewFileFilter(true);                                    // (환경설정) 확장자 제한 화면 표시 유무.
            SetUseOSMaxFilePath(true);                                  // OS제공 최대 길이 사용 여부 (true : OS가 지원하는 최대한 길이 사용 false : filefullPath : 90, 파일/폴더이름길이 : 80) 

            /*SetUseEmailManageApprove(0,false);                                     // 이메일 결재 사용 유무.
            SetUsePCURL(0, false);                                      // PCURL 사용 유무.
            SetUseClipApprove(0, false);                                // 클립보드 결재 사용 유무.
            SetUsePublicBoard(0, false);                                // 공지사항 사용 유무.
            SetUseCertSend(0, false);                                   // 공인인증서 전송 사용 유무.*/

        }
        public bool GetUseLoginIDSave(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bUserIDSave;
            return false;
        }
        private void SetUseLoginIDSave(int groupID, bool bUserIDSave)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bUserIDSave = bUserIDSave;
        }
        public bool GetUseAutoLogin(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bAutoLogin;
            return false;
        }
        private void SetUseAutoLogin(int groupID, bool bAutoLogin)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bAutoLogin = bAutoLogin;
        }

        public bool GetUseAutoLoginCheck(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bAutoLoginCheck;
            return false;
        }
        private void SetUseAutoLoginCheck(int groupID, bool bAutoLoginCheck)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bAutoLoginCheck = bAutoLoginCheck;
        }

        public bool GetUseApprLineLocalSave(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bApprLineLocalSave;
            return false;
        }
        private void SetUseApprLineLocalSave(int groupID, bool bApprLineLocalSave)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bApprLineLocalSave = bApprLineLocalSave;
        }
        public int GetZipPWBlock(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_nZipPWBlock;
            return 0;
        }
        private void SetZipPWBlock(int groupID, int nZipPWBlock)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_nZipPWBlock = nZipPWBlock;
        }
        public bool GetUseApprLineChkBlock(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bApprLineChkBlock;
            return false;
        }
        private void SetUseApprLineChkBlock(int groupID, bool bApprLineChkBlock)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bApprLineChkBlock = bApprLineChkBlock;
        }
        public bool GetUseApprDeptSearch(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bApprDeptSearch;
            return false;
        }
        private void SetUseApprDeptSearch(int groupID, bool bApprDeptSearch)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bApprDeptSearch = bApprDeptSearch;
        }
        public bool GetApproveAfterLimit(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bUseApproveAfterLimit;
            return false;
        }
        private void SetApproveAfterLimit(int groupID, bool bApproveAfterLimit)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bUseApproveAfterLimit = bApproveAfterLimit;
        }
        public bool GetClipBoardApproveAfterLimit(int groupId)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupId < listSiteConfig.Count)
                return listSiteConfig[groupId].m_bUseClipBoardApproveAfterLimit;
            return false;
        }
        private void SetClipBoardApproveAfterLimit(int groupID, bool bClipBoardApproveAfterLimit)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bUseClipBoardApproveAfterLimit = bClipBoardApproveAfterLimit;
        }


        public bool GetUseAppLoginType(int groupId)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupId < listSiteConfig.Count)
                return listSiteConfig[groupId].m_bUseAppLoginType;
            return false;
        }

        public int GetAppLoginType(int groupId)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupId < listSiteConfig.Count)
                return listSiteConfig[groupId].m_nLoginType;
            return 0;
        }


        public bool GetUseApprTreeSearch(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bApprTreeSearch;
            return false;
        }
        private void SetUseApprTreeSearch(int groupID, bool bApprTreeSearch)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bApprTreeSearch = bApprTreeSearch;
        }
        public bool GetUseUserPWChange(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bUserPWChange;
            return false;
        }
        private void SetUseUserPWChange(int groupID, bool bUserPWChange)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bUserPWChange = bUserPWChange;
        }
        public string GetPWChangeProhibitLimit(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_strPWChangeProhibitLimit;
            return "";
        }
        private void SetPWChangeProhibitLimit(int groupID, string strPWChangeProhibitLimit)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_strPWChangeProhibitLimit = strPWChangeProhibitLimit;
        }

        public int GetPWChangeApplyCnt(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_nPWChangeApplyCnt;
            return 9;
        }
        private void SetPWChangeApplyCnt(int groupID, int nPWChangeApplyCnt)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_nPWChangeApplyCnt = nPWChangeApplyCnt;
        }
        public string GetInitPasswordInfo(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_strInitPasswd;
            return "";
        }
        private void SetInitPasswordInfo(int groupID, string strInitPasswd)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_strInitPasswd = strInitPasswd;
        }

        private void SetUseScreenLock(int groupID, bool bUseScreenLock)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bUseScreenLock = bUseScreenLock;
        }
        public bool GetUseScreenLock()
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            bool bUse = false;

            int count = listSiteConfig.Count;
            for (int i = 0; i < count; i++)
            {
                bUse |= listSiteConfig[i].m_bUseScreenLock;
            }
            return bUse;
        }

        public void SetUseClipBoard(int groupID, bool bUseClipBoard)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bUseClipBoard = bUseClipBoard;
        }
        public bool GetUseClipBoard(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bUseClipBoard;
            return false;
        }

        public void SetUseURLRedirection(int groupID, bool bUseURLRedirection)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bUseURLRedirection = bUseURLRedirection;
        }
        public bool GetUseURLRection(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bUseURLRedirection;
            return false;
        }

        public void SetUseFileSend(int groupID, bool bUseFileSend)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bUseFileSend = bUseFileSend;
        }
        public bool GetUseFileSend(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bUseFileSend;
            return false;
        }
        public bool GetUseOSMaxFilePath()
        {
            return m_bUseOSMaxFilePath;
        }
        private void SetUseOSMaxFilePath(bool bUseOSMaxPath)
        {
            m_bUseOSMaxFilePath = bUseOSMaxPath;
        }

        public bool GetUseRecvFolderChange(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bRecvFolderChange;
            return false;
        }
        public bool GetUseUserRecvDownPath(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bUseUserRecvDownPath;
            return false;
        }
        private void SetUseUserRecvDownPath(int groupID, bool bUseUserRecvDownPath)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bUseUserRecvDownPath = bUseUserRecvDownPath;
        }
        public bool GetUseEmailManageApprove(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bUseEmail;
            return false;
        }
        private void SetUseEmailManageApprove(int groupID, bool bUseEmail)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bUseEmail = bUseEmail;
        }

        public bool GetUsePCURL(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bUsePCURL;
            return false;
        }
        private void SetUsePCURL(int groupID, bool bUsePCURL)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bUsePCURL = bUsePCURL;
        }

        public bool GetUseClipApprove(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bUseClipApprove;
            return false;
        }
        private void SetUseClipApprove(int groupID, bool bUseClipApprove)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bUseClipApprove = bUseClipApprove;
        }

        public bool GetUsePublicBoard(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bUsePublicBoard;
            return false;
        }
        private void SetUsePublicBoard(int groupID, bool bUsePublicBoard)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bUsePublicBoard = bUsePublicBoard;
        }

        public bool GetUseCertSend(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bUseCertSend;
            return false;
        }
        private void SetUseCertSend(int groupID, bool bUseCertSend)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bUseCertSend = bUseCertSend;
        }

        private void SetUseClipAlarmTypeChange(bool bUseClipAlarmType)
        {
            m_bUseClipAlarmType = bUseClipAlarmType;
        }

        public bool GetUseClipAlarmTypeChange()
        {
            return m_bUseClipAlarmType;
        }

        private void SetUseClipCopyAndSend(bool bUseClipCopyAndSend)
        {
            m_bUseClipCopyAndSend = bUseClipCopyAndSend;
        }
        public bool GetUseClipCopyAndSend()
        {
            return m_bUseClipCopyAndSend;
        }
        private void SetUseURLRedirectionAlarm(bool bUseURLRedirectionAlarm)
        {
            m_bUseURLRedirectionAlarm = bUseURLRedirectionAlarm;
        }
        public bool GetUseURLRedirectionAlarm()
        {
            return m_bUseURLRedirectionAlarm;
        }
        private void SetUseURLRedirectionAlarmType(bool bUseURLRedirectionAlarmType)
        {
            m_bUseURLRedirectionAlarmType = bUseURLRedirectionAlarmType;
        }
        public bool GetUseURLRedirectionAlarmType()
        {
            return m_bUseURLRedirectionAlarmType;
        }
        private void SetRFileAutoSend(bool bRFileAutoSend)
        {
            m_bRFileAutoSend = bRFileAutoSend;
        }
        public bool GetRFileAutoSend()
        {
            return m_bRFileAutoSend;
        }
        private void SetAfterApprAutoCheck(bool bAfterApprAutoCheck)
        {
            bShowAfterApprAutoCheck = bAfterApprAutoCheck;
        }
        public bool GetAfterApprAutoCheck()
        {
            return bShowAfterApprAutoCheck;
        }
        private void SetRecvFolderOpen(bool bRecvFolderOpen)
        {
            m_bRecvFolderOpen = bRecvFolderOpen;
        }
        public bool GetRecvFolderOpen()
        {
            return m_bRecvFolderOpen;
        }
        private void SetManualDownFolderChange(bool bManualDownFolderChange)
        {
            m_bManualDownFolderChange = bManualDownFolderChange;
        }
        public bool GetManualDownFolderChange()
        {
            return m_bManualDownFolderChange;
        }
        private void SetFileRecvAlarmRetain(bool bFileRecvAlarmRetain)
        {
            m_bFileRecvAlarmRetain = bFileRecvAlarmRetain;
        }
        public bool GetFileRecvAlarmRetain()
        {
            return m_bFileRecvAlarmRetain;
        }
        private void SetApprCountAlarmRetain(bool bApprCountAlarmRetain)
        {
            m_bApprCountAlarmRetain = bApprCountAlarmRetain;
        }
        public bool GetApprCountAlarmRetain()
        {
            return m_bApprCountAlarmRetain;
        }
        private void SetApprCompleteAlarmRetain(bool bApprCompleteAlarmRetain)
        {
            m_bApprCompleteAlarmRetain = bApprCompleteAlarmRetain;
        }
        public bool GetApprCompleteAlarmRetain()
        {
            return m_bApprCompleteAlarmRetain;
        }
        private void SetApprRejectAlarmRetain(bool bApprRejectAlarmRetain)
        {
            m_bApprRejectAlarmRetain = bApprRejectAlarmRetain;
        }
        public bool GetApprRejectAlarmRetain()
        {
            return m_bApprRejectAlarmRetain;
        }
        private void SetUseApprCountAlaram(bool bUseApprCountAlaram)
        {
            m_bUseApprCountAlaram = bUseApprCountAlaram;
        }
        public bool GetUseApprCountAlaram()
        {
            return m_bUseApprCountAlaram;
        }
        private void SetUseCloseTrayMove(bool bUseCloseTrayMove)
        {
            m_bUseCloseTrayMove = bUseCloseTrayMove;
        }
        public bool GetUseCloseTrayMove()
        {
            return m_bUseCloseTrayMove;
        }
        private void SetUseStartTrayMove(bool bUseStartTrayMove)
        {
            m_bUseStartTrayMove = bUseStartTrayMove;
        }
        public bool GetUseStartTrayMove()
        {
            return m_bUseStartTrayMove;
        }
        private void SetUseStartProgramReg(bool bUseStartProgramReg)
        {
            m_bUseStartProgramReg = bUseStartProgramReg;
        }
        public bool GetUseStartProgramReg()
        {
            return m_bUseStartProgramReg;
        }
        private void SetUseLanguageSet(bool bUseLanguageSet)
        {
            m_bUseLanguageSet = bUseLanguageSet;
        }
        public bool GetUseLanguageSet()
        {
            return m_bUseLanguageSet;
        }
        private void SetUseDashBoard(bool bUseDashBoard)
        {
            m_bUseDashBoard = bUseDashBoard;
        }
        public bool GetUseDashBoard()
        {
            return m_bUseDashBoard;
        }

        private void SetMainPage(PAGE_TYPE mainPage)
        {
            m_enMainPage = mainPage;
        }
        private PAGE_TYPE getMainPage()
        {
            return m_enMainPage;
        }
        private void SetUseMainPageTypeChange(bool bUseMainPage)
        {
            m_bUseMainPageType = bUseMainPage;
        }
        public bool GetUseMainPageTypeChange()
        {
            return m_bUseMainPageType;
        }

        private void SetViewFileFilter(bool bViewFileFilter)
        {
            m_bViewFileFilter = bViewFileFilter;
        }
        public bool GetViewFileFilter()
        {
            return m_bViewFileFilter;
        }

        private void SetViewSGSideBarUIBadge(bool bView)
        {
            m_bViewSGSideBarUIBadge = bView;
        }
        public bool GetViewSGSideBarUIBadge()
        {
            return m_bViewSGSideBarUIBadge;
        }

        private void SetViewSGHeaderUIAlarmNoriAllDel(bool bView)
        {
            m_bViewSGHeaderUIAlarmNoriAllDel = bView;
        }
        public bool GetViewSGHeaderUIAlarmNoriAllDel()
        {
            return m_bViewSGHeaderUIAlarmNoriAllDel;
        }
        private void SetUseForceUpdate(bool bUseForceUpdate)
        {
            m_bUseForceUpdate = bUseForceUpdate;
        }
        public bool GetUseForceUpdate()
        {
            return m_bUseForceUpdate;
        }

        private void SetUseDropErrorUI(bool bUseDropFileErrorUI)
        {
            m_bViewDropFileAddError = bUseDropFileErrorUI;
        }
        public bool GetUseDropErrorUI()
        {
            return m_bViewDropFileAddError;
        }
        public bool GetViewDlpApproverMyDept()
        {
            return m_bViewDlpApproverSelectMyDept;
        }
        public bool GetUseClipBoardNoApproveButFileTrans()
        {
            return m_bClipBoardNoApproveButFileTrans;
        }
        public int GetClipUseAfterApprove()
        {
            return m_nClipAfterApproveUseType;
        }
        public void SetClipUseAfterApprove(int useAterApproveUseType)
        {
            m_nClipAfterApproveUseType = useAterApproveUseType;
        }

        public bool GetUseSelectFirstConnectNetServer()
        {
            return m_bUseUserSelectFirstServer;
        }

        /*public bool GetUseNetOverAllsend()
        {
            return m_bUseNetOverAllsend;
        }*/

        public bool GetUseFileCheckException()
        {
            return m_bUseFileCheckException;
        }

        public bool GetUseFileForward()
        {
            return m_bUseFileForward;
        }

        public bool GetUseFileForwardDownBeforeRecv()
        {
            return m_bUseFileForwardDownNotRecv;
        }

        public bool GetUseDenyPasswordZip(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bUseDenyPasswordZip;
            return false;
        }

        public bool GetUseFileClipManageUI(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bUseFileClipManageUI;
            return false;
        }
        private void SetUseFileClipManageUI(int groupID, bool bUse)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bUseFileClipManageUI = bUse;
        }

        public bool GetUseFileClipApproveUI(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bUseFileClipApproveUI;
            return false;
        }
        private void SetUseFileClipApproveUI(int groupID, bool bUse)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bUseFileClipApproveUI = bUse;
        }

        public bool GetUseClipBoardFileTrans(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bUseClipBoardFileTrans;
            return false;
        }

        private void SetUseClipBoardFileTrans(int groupID, bool bUse)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bUseClipBoardFileTrans = bUse;
        }

        public bool GetNoApproveManageUI(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bNoApproveManageUI;
            return false;
        }


        public bool GetUseFileForward(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bFileForward;
            return false;
        }

        public bool GetUseClipTypeSelectSend(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bUseClipTypeSelectSend;
            return false;
        }

        private void SetUseClipTypeSelectSend(int groupID, bool bUse)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bUseClipTypeSelectSend = bUse;
        }

        public bool GetUseClipTypeTextFirstSend(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bUseClipTypeTextFirstSend;
            return false;
        }

        public void SetUseClipTypeTextFirstSend(int groupID, bool bUse)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bUseClipTypeTextFirstSend = bUse;
        }

        public bool GetUseAgentBlockValueChange()
        {
            return m_bUseAgentBlockValueChange;
        }

        public bool GetUseOneToMultiLogin()
        {
            return m_bUseSelfSSOlogin;
        }

        public bool GetUseOneAClockChangeAgentTimer()
        {
            return bUseAgentTime1aClock;
        }


        public string GetApproveExtSelectType(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].strApproveExtApproverSearchType;
            return String.Empty;
        }

        public bool GetUseApproveExt(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].bUseApproveExt;
            return false;
        }

        public bool GetUseInputSearchApproveExtTree(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].bUseInputSearchApproveExtTree;
            return false;
        }

        public string GetApproverSearchType(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].strApproverSearchType;
            return String.Empty;
        }

        public bool GetUseInputSearchInApproverTree(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].bUseInputSearchInApproverTree;
            return false;
        }

        public string GetReceiverSearchType(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].strReceiverSearchType;
            return String.Empty;
        }

        public bool GetUseInputSearchInReceiverTree(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].bUseInputSearchInReceiverTree;
            return false;
        }

        public string GetProxySearchType(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].strProxySearchType;
            return String.Empty;
        }

        public bool GetUseInputSearchInProxyTree(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].bUseInputSearchInProxyTree;
            return false;
        }

        public string GetSecurityApproverSearchType(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].strSecurityApproverSearchType;
            return String.Empty;
        }

        public bool GetUseInputSearchInSecurityApproverTree(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].bUseInputSearchInSecurityApproverTree;
            return false;
        }

    }
}