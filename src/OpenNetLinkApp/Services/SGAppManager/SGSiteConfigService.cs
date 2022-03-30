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
        public bool m_bAfterApprAutoCheck { get; set; }                                     // 사후결재 기본 체크 사용 유무
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
        public List<ISGSiteConfig> SiteConfigInfo { get;}       
        
        public bool GetUseLoginIDSave(int groupID);
        public bool GetUseAutoLogin(int groupID);
        public bool GetUseAutoLoginCheck(int groupID);
        public bool GetUseApprLineLocalSave(int groupID);
        public int GetZipPWBlock(int groupID);
        public bool GetUseApprLineChkBlock(int groupID);
        public bool GetUseDlpInfoDisplay(int groupID);
        public bool GetUseApprDeptSearch(int groupID);
        public bool GetUseApprTreeSearch(int groupID);
        public int GetApprStepLimit(int groupID);
        public bool GetUseDeputyApprTerminateDel(int groupID);
        public bool GetUseUserPWChange(int groupID);
        public string GetPWChangeProhibitLimit(int groupID);
        public int GetPWChangeApplyCnt(int groupID);
        public bool GetUseURLListPolicyRecv(int groupID);
        public string GetInitPasswordInfo(int groupID);

        public bool GetUseScreenLock();
        public void SetUseClipBoard(int groupID, bool bUseClipBoard);
        public bool GetUseClipBoard(int groupID);
        public void SetUseURLRedirection(int groupID, bool bUseURLRedirection);
        public bool GetUseURLRection(int groupID);
        public void SetUseFileSend(int groupID, bool bUseFileSend);
        public bool GetUseFileSend(int groupID);
        public bool GetUseRecvFolderChange(int groupID);
        public bool GetUseEmailApprove(int groupID);
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

        public bool GetUseClipBoardNoApproveButFileTrans();
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
        public bool m_bAfterApprAutoCheck { get; set; } = true;                             // 사후결재 기본 체크 사용 유무
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
        public bool m_bViewSGSideBarUIBadge { get; set; } = false;                            // 왼쪽 메뉴들에서 Badge 나오게할지 유무 설정값
        public bool m_bViewSGHeaderUIAlarmNoriAllDel { get; set; } = true;                    // 상단 HeaderUI에서 Alarm, Noti 상에 Badge 전체 삭제 메뉴 나오게할지 유무

        public bool m_bViewDropFileAddError { get; set; } = false;                          // 파일추가때, 5GB 이상 파일 추가되면 최대추가 파일크기가 5GB라고 UI가 나오는거 사용안함(false)

        public bool m_bViewDlpApproverSelectMyDept { get; set; } = false;                          // 정보보안 결재자 선택 화면 뜰때, 자기부서에 있는 사람들만 검색되어 나오도록 할 것이니 유무(true:자기부서만,false:전체)

        public bool m_bClipBoardNoApproveButFileTrans { get; set; } = false;                          // 정보보안 결재자 선택 화면 뜰때, 자기부서에 있는 사람들만 검색되어 나오도록 할 것이니 유무(true:자기부서만,false:전체)

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
                sgSiteConfig.m_bApprLineChkBlock = false;               // 고정 결재라인 차단 시 결재라인이 존재하지 않는 사용자에 대해 파일 전송 차단 여부 ( true : 전송 차단, false : 전송 허용 )
                sgSiteConfig.m_bDlpInfoDisplay = false;                 // 전송/결재 관리 리스트에서 개인정보 검출 표시 유무 설정. ( true : 표시, false : 표시 안함 )
                sgSiteConfig.m_bApprDeptSearch = true;                  // 결재자 검색 창의 타부서 수정 가능 여부.
                sgSiteConfig.m_nApprStepLimit = 0;                      // 결재자 Step 제한 설정. ( 0 : 무제한, 그외 양수 제한 Step )
                sgSiteConfig.m_bDeputyApprTerminateDel = false;         // 설정된 대결재자가 정보를 기한이 만료되면 삭제 할지 여부 ( true : 삭제, false : 삭제 안함)
                sgSiteConfig.m_bUserPWChange = false;                   // 사용자 패스워드 변경 사용 여부.
                sgSiteConfig.m_strPWChangeProhibitLimit = "";           // 패스워드 사용금지 문자열 지정.
                sgSiteConfig.m_nPWChangeApplyCnt = 9;                   // 패스워드 변경 시 허용되는 자리수 지정.
                sgSiteConfig.m_bURLListPolicyRecv = false;              // URL 리스트 정책 받기 사용 유무
                sgSiteConfig.m_strInitPasswd = "";

                sgSiteConfig.m_bUseScreenLock = true;                   // 화면잠금 사용유무 값설정
                sgSiteConfig.m_bRecvFolderChange = true;                // 수신폴더 변경 사용 여부

                sgSiteConfig.m_bUseEmailApprove = false;                // 이메일 결재 사용 유무
                sgSiteConfig.m_bUsePCURL = false;                       // PCURL 사용 유무.
                sgSiteConfig.m_bUseClipApprove = false;                 // 클립보드 결재 사용 유무.
                sgSiteConfig.m_bUsePublicBoard = false;                 // 공지사항 사용 유무.

                SiteConfigInfo.Add(sgSiteConfig);
                
                SetPWChangeApplyCnt(i, 9);                              // 비밀번호 변경 허용 자리수
                SetInitPasswordInfo(i, "1K27SdexltsW0ubSCJgsZw==");     // hsck@2301
                SetUseAutoLogin(i, true);                               // 자동로그인 사용
                SetUseAutoLoginCheck(i, false);                         // 자동로그인 체크박스 기본 체크
                SetUseApprLineLocalSave(i, true);                       // 결재라인 로컬저장 기능 사용 
                SetUseLoginIDSave(i, false);                            // ID history 기능 사용.
                SetUseScreenLock(i, true);                              // 화면잠금 사용
                SetUseRecvFolderChange(i, true);                        // 수신 폴더 변경 사용

                SetUseEmailApprove(i, false);                           // 이메일 결재 사용 유무.
                SetUsePCURL(i, false);                                  // PCURL 사용 유무.
                SetUseClipApprove(i, false);                            // 클립보드 결재 사용 유무.
                SetUsePublicBoard(i, false);                            // 공지사항 사용 유무.
                SetUseCertSend(i, false);                               // 공인인증서 전송 사용 유무.
            }
            

            /*SetPWChangeApplyCnt(0, 9);                                // 비밀번호 변경 허용 자리수
            SetInitPasswordInfo(0, "1K27SdexltsW0ubSCJgsZw==");         // hsck@2301
            SetUseAutoLogin(0, true);                                   // 자동로그인 사용
            SetUseAutoLoginCheck(0, true);                              // 자동로그인 체크박스 기본 체크
            SetUseApprLineLocalSave(0, true);                           // 결재라인 로컬저장 기능 사용 
            SetUseLoginIDSave(0, true);                                 // ID history 기능 사용.

            SetUseScreenLock(0, true);                                  // 화면잠금 사용
            SetUseRecvFolderChange(0, true);                            // 수신 폴더 변경 사용*/

            SetUseClipAlarmTypeChange(true);                            // 클립보드 알림타입 변경 사용 유무           
            SetUseClipCopyAndSend(false);                               // 클립보드 복사 후 자동 전송 사용 유무

            // 현재 : 아래 두 값으로 설정 UI가 나오고 안나오고함 => 서버설정으로 나오도록 수정해야함
            // Footer 쪽도 URLRedirection 사용유무 설정에 따라서 UI 설정되도록 수정
            SetUseURLRedirectionAlarm(true);                            // URL 리다이렉션 알림 타입 사용 여부.(사용자가 설정가능유무)
            SetUseURLRedirectionAlarmType(true);                        // URL 리다이렉션 알림 타입 선택 사용 여부.

            SetRFileAutoSend(false);                                    // 오른쪽 마우스 클릭 후 자동 전송 사용 여부.
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

            SetUseLanguageSet(false);                                   // 언어 설정 사용 여부.

            SetUseDashBoard(true);                                      // 대쉬보드 창 사용 유무.
            SetUseForceUpdate(true);                                    // 넘기는 기능 없이 무조건 업데이트 사용유무
            SetMainPage(PAGE_TYPE.NONE);                                // 메인화면 설정 => DashBoard 사용 안하면 DASHBOARD로 선택했더라도 DASHBOARD는 나타나지 않음
            SetUseMainPageTypeChange(false);                            // 메인화면 변경 타입 사용 유무
            SetViewFileFilter(true);                                    // (환경설정) 확장자 제한 화면 표시 유무.

            /*SetUseEmailApprove(0,false);                              // 이메일 결재 사용 유무.
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
        public bool GetUseDlpInfoDisplay(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bDlpInfoDisplay;
            return false;
        }
        private void SetUseDlpInfoDisplay(int groupID, bool bDlpInfoDisplay)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bDlpInfoDisplay = bDlpInfoDisplay;
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
        public int GetApprStepLimit(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_nApprStepLimit;
            return 0;
        }
        private void SetApprStepLimit(int groupID, int nApprStepLimit)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_nApprStepLimit = nApprStepLimit;
        }
        public bool GetUseDeputyApprTerminateDel(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bDeputyApprTerminateDel;
            return false;
        }
        private void SetUseDeputyApprTerminateDel(int groupID, bool bDeputyApprTerminateDel)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bDeputyApprTerminateDel = bDeputyApprTerminateDel;
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
        public bool GetUseURLListPolicyRecv(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bURLListPolicyRecv;
            return false;
        }
        private void SetUseURLListPolicyRecv(int groupID, bool bURLListPolicyRecv)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bURLListPolicyRecv = bURLListPolicyRecv;
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

        public bool GetUseRecvFolderChange(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bRecvFolderChange;
            return false;
        }
        private void SetUseRecvFolderChange(int groupID, bool bRecvFolderChange)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bRecvFolderChange = bRecvFolderChange;
        }

        public bool GetUseEmailApprove(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bUseEmailApprove;
            return false;
        }
        private void SetUseEmailApprove(int groupID, bool bUseEmailApprove)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bUseEmailApprove = bUseEmailApprove;
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
        
        public  bool GetUseClipAlarmTypeChange()
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
            m_bAfterApprAutoCheck = bAfterApprAutoCheck;
        }
        public bool GetAfterApprAutoCheck()
        {
            return m_bAfterApprAutoCheck;
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
    }
}