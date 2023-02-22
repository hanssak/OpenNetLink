using System;
using System.Net;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;
using HsNetWorkSG;
using HsNetWorkSGData;
using OpenNetLinkApp.Models.SGNetwork;
using OpenNetLinkApp.Services.SGAppManager;
using Serilog.Events;
using OpenNetLinkApp.Data;
using System.IO;
using System.Text.Json;
using OpenNetLinkApp.PageEvent;
using OpenNetLinkApp.Data.SGDicData;
using OpenNetLinkApp.Data.SGDicData.SGUnitData;
using System.Linq;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;
using OpenNetLinkApp.Data.SGQuery;
using OpenNetLinkApp.Models.SGConfig;
using System.Runtime.Serialization.Json;
using Microsoft.EntityFrameworkCore.Storage;

using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;


namespace OpenNetLinkApp.Services
{
    public class HSCmdCenter
    {

        private ConcurrentDictionary<int, HsNetWork> m_DicNetWork = new ConcurrentDictionary<int, HsNetWork>();
        public SGDicRecvData sgDicRecvData = new SGDicRecvData();
        public SGSendData sgSendData = new SGSendData();
        public SGPageEvent sgPageEvent = new SGPageEvent();
        public ConcurrentDictionary<int, bool> m_DicFileRecving = new ConcurrentDictionary<int, bool>();
        public ConcurrentDictionary<int, bool> m_DicFileSending = new ConcurrentDictionary<int, bool>();
        public string m_strCliVersion = "";
        public int m_nNetWorkCount = 0;
        private bool m_bRecvFileDelThreadDo = false;
        object nlock = new object();

        object objDataRecv = new object();
        object objSvrRecv = new object();


        public HSCmdCenter()
        {

        }

        ~HSCmdCenter()
        {

        }

        public void Init()
        {
            HsNetWork hsNetwork = null;

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

            List<string> RecvDownList = new List<string>();
            var serializer = new DataContractJsonSerializer(typeof(SGAppConfig));
            string AppConfig = Environment.CurrentDirectory + "/wwwroot/conf/AppEnvSetting.json";
            if (File.Exists(AppConfig))
            {
                using (FileStream fs = File.OpenRead(AppConfig))
                {
                    SGAppConfig appConfig = (SGAppConfig)serializer.ReadObject(fs);
                    RecvDownList = appConfig.RecvDownPath;
                }
            }

            serializer = new DataContractJsonSerializer(typeof(SGVersionConfig));
            string VersionConfig = Environment.CurrentDirectory + "/wwwroot/conf/AppVersion.json";
            if (File.Exists(VersionConfig))
            {
                using (FileStream fs = File.OpenRead(VersionConfig))
                {
                    SGVersionConfig versionConfig = (SGVersionConfig)serializer.ReadObject(fs);
                }
            }

            bool isCheckHardSpace = true;
            foreach(SGNetwork sgNetwork in listNetworks)
            {
                if (sgNetwork.NetPos == "IN")
                    isCheckHardSpace = true;
                else
                    isCheckHardSpace = false;
            }
            //serializer = new DataContractJsonSerializer(typeof(SGopConfig));
            //string OpConfig = Environment.CurrentDirectory + "/wwwroot/conf/AppOPsetting.json";
            //if (File.Exists(OpConfig))
            //{
            //    using (FileStream fs = File.OpenRead(OpConfig))
            //    {
            //        SGopConfig opConfig = (SGopConfig)serializer.ReadObject(fs);
            //        isCheckHardSpace = opConfig.bUseChkHardSpace;
            //    }
            //}



            int count = listNetworks.Count;
            SetNetWorkCount(count);
            string strModulePath = "";
            for (int i = 0; i < count; i++)
            {
                IPAddress address;
                string strIP = listNetworks[i].IPAddress;
                if (IPAddress.TryParse(strIP, out address) == false)
                {
                    IPAddress[] addresses = Dns.GetHostAddresses(strIP);
                    strIP = addresses[0].ToString();
                }
                int port = listNetworks[i].Port;
                int groupID = listNetworks[i].GroupID;
                int ConnectType = listNetworks[i].ConnectType;

                HsConnectType hsContype = HsConnectType.Direct;
                if (ConnectType == 1)
                    hsContype = HsConnectType.FindServer;

                hsNetwork = new HsNetWork();
                string strTlsVer = listNetworks[i].TlsVersion;
                string strDownPath = "";
                if (RecvDownList != null && RecvDownList.Count > i)
                    strDownPath = RecvDownList[i];

                if (strDownPath.Equals(""))
                {
                    strModulePath = System.IO.Directory.GetCurrentDirectory();
                    strDownPath = System.IO.Path.Combine(strModulePath, "Download");
                }
                else
                {
                    strDownPath = ConvertRecvDownPath(strDownPath);
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    strModulePath = strModulePath.Replace("/", "\\");
                    strDownPath = strDownPath.Replace("/", "\\");
                }
                else
                {
                    strModulePath = strModulePath.Replace("\\", "/");
                    strDownPath = strDownPath.Replace("\\", "/");
                }

                if (strTlsVer.Equals("1.2"))
                    hsNetwork.Init(hsContype, strIP, port, false, SslProtocols.Tls12, strModulePath, strDownPath, groupID.ToString());    // basedir 정해진 후 설정 필요
                else if (strTlsVer.Equals("1.0"))
                    hsNetwork.Init(hsContype, strIP, port, false, SslProtocols.Tls, strModulePath, strDownPath, groupID.ToString());    // basedir 정해진 후 설정 필요
                else
                    hsNetwork.Init(hsContype, strIP, port, false, SslProtocols.Tls12, strModulePath, strDownPath, groupID.ToString());    // basedir 정해진 후 설정 필요

                hsNetwork.SGSvr_EventReg(SGSvrRecv);
                hsNetwork.SGData_EventReg(SGDataRecv);
                hsNetwork.SGException_EventReg(SGExceptionRecv);
                hsNetwork.SetGroupID(groupID);
                hsNetwork.SetFileRecvPossible(false);
                hsNetwork.SetIsCheckHardSpace(isCheckHardSpace);

                //PageStatusData.RefreshInfoEvent()

                //hsNetwork.SetHszMultiThread(false);

                //hsNetwork.SetSessionDuplicateEventReg(OnSessionDuplicate);
                m_DicNetWork[groupID] = hsNetwork;
            }
        }

        /*public void OnSessionDuplicate(int groupId, SgEventType sgEventType)
        {
            System.Diagnostics.Debug.WriteLine("SessionDuplicate...");
        }*/

        /// <summary>
        /// 사용자ID폴더 사용여부에 따른 수신 경로 변경 (Mac만 적용)
        /// </summary>
        /// <param name="DownPath">수신경로</param>
        /// <param name="userID">사용자ID</param>
        /// <returns>수신경로</returns>
        public string ConvertRecvDownPath(string DownPath)
        {
            string strDownPath = "";
            if (DownPath.Contains("%USERPATH%"))
            {
                string strFullHomePath = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
                strDownPath = DownPath.Replace("%USERPATH%", strFullHomePath);
            }
            else if (DownPath.Contains("%MODULEPATH%"))
            {
                string strModulePath = System.IO.Directory.GetCurrentDirectory();
                strDownPath = DownPath.Replace("%MODULEPATH%", strModulePath);
            }
            else
            {
                strDownPath = DownPath;
            }
            return strDownPath;
        }

        public SGData GetSGSvrData(int groupid)
        {
            SGData data = null;
            data = sgDicRecvData.GetSvrData(groupid);
            return data;
        }
        public SGData GetLoginData(int groupid)
        {
            SGData data = null;
            data = sgDicRecvData.GetLoginData(groupid);
            return data;
        }
        public int GetLoginDataCount()
        {
            return sgDicRecvData.GetLoginDataCount();
        }

        public SGData GetUserData(int groupid)
        {
            SGData data = null;
            data = sgDicRecvData.GetUserData(groupid);
            return data;
        }

        public SGData GetTransManageData(int groupid)
        {
            SGData data = null;
            data = sgDicRecvData.GetTransManageData(groupid);
            return data;
        }
        public SGData GetApprManageData(int groupid)
        {
            SGData data = null;
            data = sgDicRecvData.GetApprManageData(groupid);
            return data;
        }
        public SGData GetDetailData(int groupid)
        {
            SGData data = null;
            data = sgDicRecvData.GetDetailData(groupid);
            return data;
        }
        /// <summary>
        /// 기본 결재라인 데이터 (APPROVEDEFAULT)
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public SGData GetApprLineData(int groupid)
        {
            SGData data = null;
            data = sgDicRecvData.GetApprLineData(groupid);
            return data;
        }

        public void SetApprLineList(int groupid, LinkedList<ApproverInfo> LinkedApprInfo)
        {
            sgDicRecvData.SetApprLineList(groupid, LinkedApprInfo);
        }
        public SGData GetDeptApprLineSearchData(int groupid)
        {
            SGData data = null;
            data = sgDicRecvData.GetDeptApprLineSearchData(groupid);
            return data;
        }
        public SGData GetBoardNoti(int groupid)
        {
            SGData data = null;
            data = sgDicRecvData.GetBoardNoti(groupid);
            return data;
        }
        public SGData GetSGGpkiData(int groupid)
        {
            SGData data = null;
            data = sgDicRecvData.GetGpkiData(groupid);
            return data;
        }

        public SGData GetDeptInfoData(int groupid)
        {
            SGData data = sgDicRecvData.GetDeptInfoData(groupid);
            return data;
        }


        private void SGExceptionRecv(int groupId, SgEventType sgEventType)
        {
            SgEventType sgEType = sgEventType;

            switch (sgEType)
            {
                case SgEventType.SG_SOCKET_TAG_EXCEPTION:                       // 오프라인
                    OffLineAfterSend(groupId);
                    break;
                case SgEventType.SG_SESSIONDUPLICATE:
                    SessionDuplicateEvent seEvent = sgPageEvent.GetSessionDuplicateEvent(groupId);
                    if (seEvent != null)
                    {
                        PageEventArgs e = new PageEventArgs();
                        e.result = 0;
                        e.strMsg = "SESSIONDUPLICATE";
                        seEvent(groupId, e);
                    }
                    System.Diagnostics.Debug.WriteLine("HsNetWork Session Duplicate Exception Received..");
                    break;
                default:
                    break;
            }
            return;
        }
        public void OffLineAfterSend(int groupId)
        {
            OffLineNotiEvent offlineEvent = null;
            offlineEvent = sgPageEvent.GetOffLineNotiEvent();
            if (offlineEvent != null)
            {
                offlineEvent(groupId);
            }
        }
        private void SGDataRecv(int groupId, eCmdList cmd, SGData sgData)
        {
            //lock (objDataRecv)

            HsNetWork hs = null;
            int nRet = 0;

            nRet = sgData.GetResult();
            switch (cmd)
            {
                case eCmdList.eLINKCHK:
                    SetHoliday(groupId, sgData);
                    break;
                case eCmdList.eSEEDKEY:                                                  // SEEDKEY_ACK : seed key 요청 응답
                    break;

                case eCmdList.eBIND:                                                  // BIND_ACK : user bind(connect) 인증 응답
                    BindAfterSend(nRet, groupId, sgData);
                    break;

                case eCmdList.eCHANGEPASSWD:                                                  // 비밀번호 변경 요청 응답.
                    ChgPassWDAfterSend(nRet, groupId);
                    break;

                case eCmdList.eDEPTINFO:                                                  // 부서정보 조회 요청 응답.
                    DeptInfoAfterSend(nRet, groupId, sgData);
                    {
                        int result = sgData.GetResult();
                        sgData.GetRecordData("DeptCount");
                    }
                    break;

                case eCmdList.eURLLIST:                                                  // URL 자동전환 리스트 요청 응답.
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        sgDicRecvData.SetUrlListData(hs, groupId, sgData);
                        URLListAfterSend(nRet, groupId, sgData);
                    }
                    break;

                case eCmdList.eUSERINFOEX:                                                  // USERINFOEX : 사용자 정보 응답.
                    UserInfoAfterSend(nRet, groupId, sgData);
                    break;

                case eCmdList.eAPPRINSTCUR:                                                  // 현재 등록된 대결재자 정보 요청 응답.
                    ApprInstAfterSend(nRet, groupId, sgData);
                    break;
                case eCmdList.eAPPRINSTCLEAR:                                       //대결자삭제
                case eCmdList.eAPPRINSTREG:                                         //대결자등록
                    CommonResultAfterSend(nRet, groupId, sgData);
                    break;
                case eCmdList.eFILETRANSLIST:                                                  // 전송관리 조회 리스트 데이터 요청 응답.
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        sgDicRecvData.SetTransManageData(hs, groupId, sgData);
                        TransSearchAfterSend(nRet, groupId);
                    }
                    break;

                case eCmdList.eFILEAPPROVE:                                                  // 결재관리 조회 리스트 데이터 요청 응답.
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        sgDicRecvData.SetApprManageData(hs, groupId, sgData);
                        ApprSearchAfterSend(nRet, groupId);
                    }
                    break;

                case eCmdList.eSYSTEMRUNENV:                                                       // 시스템 환경정보 요청에 대한 응답.
                    SystemRunAfterSend(nRet, groupId, sgData);

                    break;

                case eCmdList.eSESSIONCOUNT:                                                  // 사용자가 현재 다른 PC 에 로그인되어 있는지 여부 확인 요청에 대한 응답.
                    System.Diagnostics.Debug.WriteLine("SESSIONCOUNT ON HSCmdCenter:" + groupId);
                    if (nRet != 0)
                        BindAfterSend(nRet, groupId, sgData);
                    break;

                case eCmdList.eAPPROVEDEFAULT:                                                  // 사용자기본결재정보조회 요청 응답.
                    ApprLineAfterSend(nRet, groupId, sgData);
                    break;

                case eCmdList.eFILETRANSLISTQUERYCOUNT:                                                  // 전송관리 조회 리스트 데이터 Count 요청 응답.
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        string strCount = sgData.GetSvrRecordTagData("RECORD");
                        int count = 0;
                        if (!strCount.Equals(""))
                        {
                            strCount = strCount.Replace("\u0001", "");
                            count = Convert.ToInt32(strCount);
                        }
                        TransSearchCountAfterSend(nRet, groupId, count);
                    }
                    break;

                case eCmdList.eFILETRANSLISTQUERY:                                                  // 전송관리 조회 리스트 데이터 요청 응답.
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        sgDicRecvData.SetTransManageData(hs, groupId, sgData);
                        TransSearchAfterSend(nRet, groupId);
                    }
                    //RMouseFileAddNotiAfterSend(nRet, groupId);
                    break;

                case eCmdList.eFILEAPPRLISTQUERYCOUNT:                                           // 결재관리 조회 리스트 데이터 Count 요청 응답. (쿼리 방식) 
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        string sCount = sgData.GetSvrRecordTagData("RECORD");
                        int cnt = 0;
                        if (!sCount.Equals(""))
                        {
                            sCount = sCount.Replace("\u0001", "");
                            cnt = Convert.ToInt32(sCount);
                        }
                        ApprSearchCountAfterSend(nRet, groupId, cnt);
                    }
                    break;

                case eCmdList.eFILEAPPRLISTQUERY:                                           // 결재관리 조회 리스트 요청 응답. (쿼리 방식) 
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        sgDicRecvData.SetApprManageData(hs, groupId, sgData);
                        ApprSearchAfterSend(nRet, groupId);
                    }
                    break;

                case eCmdList.eFILETRANSDETAILQUERY:                                         // 전송 상세보기 조회 리스트 요청 응답. (쿼리 방식) 
                    break;

                case eCmdList.eFILEAPPRDETAILQUERY:                                         // 결재 상세보기 조회 리스트 요청 응답. (쿼리 방식) 
                    break;

                case eCmdList.eSENDCANCEL:                                                  // 전송취소 요청 응답 
                    TransCancelAfterSend(nRet, groupId);
                    break;

                case eCmdList.eAPPROVEBATCH:                                                // 일괄결재 응답.
                    ApproveBatchAfterSend(nRet, groupId, sgData);
                    break;

                case eCmdList.eTRANSDETAIL:
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        sgDicRecvData.SetDetailData(hs, groupId, sgData);
                        string strTransSeq = sgData.GetBasicTagData("TRANSSEQ");
                        DetailSearchAfterSend(nRet, groupId, strTransSeq);
                    }
                    break;

                case eCmdList.eDEPTAPPRLINESEARCHQUERY:                                     // 같은 부서 결재자 정보 리스트    
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        sgDicRecvData.SetDeptApprLineSearchData(hs, groupId, sgData);
                        DeptApprLineSearchAfterSend(nRet, groupId);
                    }
                    break;
                case eCmdList.eDEPTAPPRLINEREFLASHQUERY:                                    // 타 부서 결재자 정보 리스트
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        sgDicRecvData.SetDeptApprLineSearchData(hs, groupId, sgData);
                        DeptApprLineReflashAfterSend(nRet, groupId);
                    }
                    break;

                case eCmdList.eFILESENDPROGRESSNOTI:
                    FileSendProgressNotiAfterSend(nRet, groupId, sgData);
                    break;
                case eCmdList.eFILERECVPROGRESSNOTI:
                    FileRecvProgressNotiAfterSend(nRet, groupId, sgData);
                    break;
                case eCmdList.eFILEPREVPROGRESSNOTI:                                        // 파일 미리보기 수신 진행률 노티.
                    FilePrevProgressNotiAfterSend(nRet, groupId, sgData);
                    break;

                case eCmdList.eCLIPBOARDTXT:                                                    // 클립보드 데이터 Recv
                    ClipRecvNotiAfterSend(nRet, groupId, sgData);
                    break;

                case eCmdList.eSUBDATAEXCHANGE:                                                    // 클립보드 데이터 Recv(서버에서)
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        UrlServerRecvNotiAfterSend(nRet, groupId, sgData);
                    }
                    break;

                case eCmdList.eSUBDATANOTIFY:                                                    // 클립보드 데이터 Recv(서버에서)
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        UrlBrowserRecvNotiAfterSend(nRet, groupId, sgData);
                    }
                    break;

                case eCmdList.eRMOUSEFILEADD:                                                   // 마우스 우클릭 이벤트 노티
                    RMouseFileAddNotiAfterSend(nRet, groupId);
                    break;

                case eCmdList.eAPPROVECOUNT:                                                // 승인대기 노티.
                    ApproveCountNotiAfterSend(nRet, eCmdList.eAPPROVECOUNT, groupId, sgData);
                    break;
                case eCmdList.eVIRUSSCAN:                                                   // 바이러스 검출 노티.
                    VirusScanNotiAfterSend(nRet, eCmdList.eVIRUSSCAN, groupId, sgData);
                    break;
                case eCmdList.eAPTSCAN:                                                     // APT 노티.
                    VirusScanNotiAfterSend(nRet, eCmdList.eAPTSCAN, groupId, sgData);
                    break;
                case eCmdList.eEMAILAPPROVENOTIFY:                                          // 메일 승인대기 노티.
                    EmailApproveNotiAfterSend(nRet, eCmdList.eAPPROVECOUNT, groupId, sgData);
                    break;
                case eCmdList.eBOARDNOTIFY:                                                 // 공지사항 노티.
                    BoardNotiAfterSend(nRet, eCmdList.eBOARDNOTIFY, groupId, sgData);
                    break;
                case eCmdList.eAPPROVEACTIONNOTIFY:                                         // 사용자 결재 완료(승인/반려)노티.
                    ApproveActionNotiAfterSend(nRet, eCmdList.eAPPROVEACTIONNOTIFY, groupId, sgData);
                    break;

                case eCmdList.eLOGOUT:                                                      // 로그아웃 노티.
                    LogOutNotiAfterSend(nRet, groupId, sgData);
                    break;

                case eCmdList.eUSEDAYFILETRANS:                                             // 사용된 일일 파일 전송 사용량 및 횟수 데이터.
                    UseDayFileInfoNotiAfterSend(nRet, groupId, sgData);
                    break;

                case eCmdList.eUSEDAYCLIPTRANS:                                             // 사용된 일일 클립보드 전송 사용량 및 횟수 데이터.
                    UseDayClipInfoNotiAfterSend(nRet, groupId, sgData);
                    break;

                case eCmdList.eCLIENTUNLOCK:                                                      // 화면잠금 해제
                    ScreenLockClearAfterSend(nRet, groupId, sgData);
                    break;

                case eCmdList.eZIPDEPTHINFO:                                                    // zip 파일 내부검사 설정 정보 조회.
                    ZipDepthInfoSetting(nRet, groupId, sgData);
                    break;

                case eCmdList.eCLIENTVERSION:                                                       // 업데이트 노티.
                    UpgradeNotiAfterSend(nRet, sgData);
                    break;

                case eCmdList.eDASHBOARDCOUNT:                                  // 대쉬보드 조회 쿼리 데이터.
                    DashBoardCountAfterSend(nRet, groupId, sgData);
                    break;

                case eCmdList.eDASHBOARDTRANSREQCOUNT:                              // 대쉬보드 전송요청 Count 쿼리
                    DashBoardTransReqCountAfterSend(nRet, groupId, sgData);
                    break;

                case eCmdList.eDASHBOARDAPPRWAITCOUNT:                              // 대쉬보드 승인대기 Count 쿼리
                    DashBoardApprWaitCountAfterSend(nRet, groupId, sgData);
                    break;

                case eCmdList.eDASHBOARDAPPRCONFIRMCOUNT:                              // 대쉬보드 승인 Count 쿼리
                    DashBoardApprConfirmCountAfterSend(nRet, groupId, sgData);
                    break;

                case eCmdList.eDASHBOARDAPPRREJECTCOUNT:                              // 대쉬보드 반려 Count 쿼리
                    DashBoardApprRejectCountAfterSend(nRet, groupId, sgData);
                    break;

                case eCmdList.ePASSWDCHGDAY:                                        // 패스워드 변경날짜 조회.
                    PasswdChgDayAfterSend(nRet, groupId, sgData);
                    break;

                case eCmdList.eBOARDNOTIFYSEARCH:                                   // 공지사항 조회 결과 
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        sgDicRecvData.SetBoardNoti(hs, groupId, sgData);
                        BoardNotiSearchAfterSend(nRet, groupId);
                    }
                    break;

                case eCmdList.eGPKIRANDOM:                                   // Gpki_Random 결과 
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        sgDicRecvData.SetGpkiData(hs, groupId, sgData);
                        RecvSvrGPKIRandomAfterSend(groupId);
                    }
                    break;

                case eCmdList.eGPKICERT:                                   // Gpki_Cert 결과 
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        sgDicRecvData.SetGpkiData(hs, groupId, sgData);
                        RecvSvrGPKICertAfterSend(groupId);
                    }
                    break;

                case eCmdList.eCHANGEGPKICN:                                   // CHANGEGPKI_CN 결과 
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        sgDicRecvData.SetGpkiData(hs, groupId, sgData);
                        RecvSvrGPKIRegAfterSend(groupId);
                    }
                    break;
                case eCmdList.ePRIVACYNOTIFY:                                     //개인정보 Noti
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        PrivacyNotiEvent privacyNotiEvent = sgPageEvent.GetPrivacyNotiEvent(groupId);
                        if (privacyNotiEvent != null) privacyNotiEvent(groupId, sgData);
                    }
                    break;
                case eCmdList.eEMAILAPPROVEBATCH:
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        ResponseEvent resEvent = sgPageEvent.GetEmailApprBatchEvent(groupId);
                        if (resEvent != null) resEvent(groupId, sgData);
                    }
                    break;
                case eCmdList.eSECURITYAPPROVERQUERY:
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        SecurityApproverSearchEvent securityApproverSearchEvent = sgPageEvent.GetSecurityApproverSearchEvent(groupId);
                        if (securityApproverSearchEvent != null) securityApproverSearchEvent(groupId, sgData);
                    }
                    break;
                case eCmdList.eCOUNTQUERY:
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        QueryCountEvent queryCountEvent = sgPageEvent.GetQueryCountEvent(groupId);
                        if (queryCountEvent != null) queryCountEvent(groupId, sgData);
                    }
                    break;
                case eCmdList.eLISTQUERY:
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        QueryListEvent queryListEvent = sgPageEvent.GetQueryListEvent(groupId);
                        if (queryListEvent != null) queryListEvent(groupId, sgData);
                    }
                    break;
                case eCmdList.eDETAILQUERY:
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        QueryDetailEvent queryDetailEvent = sgPageEvent.GetQueryDetailEvent(groupId);
                        if (queryDetailEvent != null) queryDetailEvent(groupId, sgData);
                    }
                    break;
                case eCmdList.eRECORDEXISTCHECKQUERY:
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        QueryRecordExistCheckEvent queryRecordExistCheckEvent = sgPageEvent.GetQueryRecordExistCheckEvent(groupId);
                        if (queryRecordExistCheckEvent != null) queryRecordExistCheckEvent(groupId, sgData);
                    }
                    break;
                case eCmdList.eEMAILSENDCANCEL:
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        EmailSendCancelEvent emailSendCancelEvent = sgPageEvent.GetEmailSendCancelEvent(groupId);
                        if (emailSendCancelEvent != null) emailSendCancelEvent(groupId, sgData);
                    }
                    break;
                case eCmdList.eDOWNLOADCOUNT:
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        DownloadCountEvent downloadCountEvent = sgPageEvent.GetDownloadCountEvent(groupId);
                        if (downloadCountEvent != null) downloadCountEvent(groupId, sgData);
                    }
                    break;

                case eCmdList.eFILEMAXLENGTH:
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        //Serilog.Log.Logger.Error("HsCmdCenter - eCmdList.eFILEMAXLENGTH - ########## - ");
                        FileRecvErrInfoEvent filerecvErrEvent = sgPageEvent.GetAddFIleRecvErrEvent(groupId);
                        if (filerecvErrEvent != null) filerecvErrEvent(groupId, sgData);
                    }
                    break;
                case eCmdList.eFORWARDFILEINFO:
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        FileForwardEvent fileforwardEvent = sgPageEvent.GetFileForwardNotifyEventAdd(groupId);
                        if (fileforwardEvent != null) fileforwardEvent(groupId, sgData);
                    }
                    break;
                case eCmdList.eHSCKQUERY:
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {

                    }
                    break;
                case eCmdList.eGENERICNOTI:
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        GenericNotiType2Event eventData = sgPageEvent.GetGenericNotiType2EventAdd(groupId);
                        if (eventData != null) eventData(groupId, sgData);
                    }
                    break;
                case eCmdList.eSKIPFILENOTI:
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        SkipFileNotiEvent eventData = sgPageEvent.GetSkipFileNotiEventAdd();
                        if (eventData != null) eventData(groupId, sgData);
                    }
                    break;
                case eCmdList.eUPDATEPOLICY:
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        NotiUpdatePolicyEvent updatePolicyEvent = sgPageEvent.GetUpdatePolicyEvent();
                        if (updatePolicyEvent != null) updatePolicyEvent(groupId);
                    }
                    break;
                default:
                    hs = GetConnectNetWork(groupId);
                    if (hs != null)
                    {
                        CommonQueryReciveEvent queryListEvent = sgPageEvent.GetQueryReciveEvent(groupId, cmd);
                        object[] obj = new object[] { sgData };
                        if (queryListEvent != null)
                        {
                            queryListEvent(groupId, obj);
                        }

                    }
                    break;

            }

        }


        private void SGSvrRecv(int groupId, int cmd, SGData sgData)
        {
            //lock(objSvrRecv)

            SGData tmpData = GetSGSvrData(groupId);
            if (tmpData == null)
                tmpData = new SGData();

            switch (cmd)
            {
                case 2005:                                                              // usertype, logintype, systemid, tlsversion
                    tmpData.m_DicTagData["USERTYPE"] = sgData.m_DicTagData["USERTYPE"].Base64EncodingStr();
                    tmpData.m_DicTagData["LOGINTYPE"] = sgData.m_DicTagData["LOGINTYPE"].Base64EncodingStr();
                    tmpData.m_DicTagData["SYSTEMID"] = sgData.m_DicTagData["SYSTEMID"].Base64EncodingStr();
                    tmpData.m_DicTagData["TLSVERSION"] = sgData.m_DicTagData["TLSVERSION"].Base64EncodingStr();

                    RecvSvrAfterSend(groupId, sgData.m_DicTagData["LOGINTYPE"]);
                    //SGSvrData sgTmp = (SGSvrData)sgDicRecvData.GetSvrData(0);
                    //eLoginType e = sgTmp.GetLoginType();
                    break;
                case 2102:                                                              // gpki_cn
                    tmpData.m_DicTagData["GPKI_CN"] = sgData.m_DicTagData["GPKI_CN"].Base64EncodingStr();
                    RecvSvrGPKIAfterSend(groupId);
                    break;
                case 2103:                                                              // filemime.conf
                    FileMimeRecvEvent fileMimeRecvEvent = sgPageEvent.GetFileMimeRecvEvent();
                    if(fileMimeRecvEvent != null)
                    {
                        fileMimeRecvEvent(groupId);
                    }
                    break;

            }

            sgDicRecvData.SetSvrData(groupId, tmpData);

        }
        public void RecvSvrAfterSend(int groupId, string loginType)
        {
            SvrEvent svEvent = sgPageEvent.GetSvrEvent(groupId);
            if (svEvent != null)
            {
                svEvent(groupId, loginType);
            }
        }

        public void BindAfterSend(int nRet, int groupId, SGData sgData)
        {
            nRet = sgData.GetResult();
            string strMsg = "";

            HsNetWork hs = null;
            if (m_DicNetWork.TryGetValue(groupId, out hs) == false && nRet == 0)
            {
                HsLog.info($"BindAfterSend - BIND Success But - m_DicNetWork.TryGetValue return false");
                return;
            }


            if (nRet == 0)
            {

                //hs = m_DicNetWork[groupId];
                sgDicRecvData.SetLoginData(hs, groupId, sgData);
                SGLoginData sgLoginBind = (SGLoginData)sgDicRecvData.GetLoginData(groupId);
                Int64 nFilePartSize = sgLoginBind.GetFilePartSize();
                Int64 nFileBandWidth = sgLoginBind.GetFileBandWidth();
                int nLinkCheckTime = sgLoginBind.GetLinkCheckTime();
                nLinkCheckTime = (nLinkCheckTime * 2) / 3;
                bool bDummy = sgLoginBind.GetUseDummyPacket();
                hs.SetNetworkInfo(nFilePartSize, nFileBandWidth, bDummy, nLinkCheckTime);
                SendUserInfoEx(groupId, sgLoginBind.GetUserID());

            }
            else
            {
                LoginEvent LoginResult_Event = null;
                LoginResult_Event = sgPageEvent.GetLoginEvent(groupId);
                if (LoginResult_Event != null)
                {
                    strMsg = SGLoginData.LoginFailMessage(nRet);
                    PageEventArgs e = new PageEventArgs();
                    e.result = nRet;
                    e.strMsg = strMsg;
                    LoginResult_Event(groupId, e);
                }

            }
        }
        public void ChgPassWDAfterSend(int nRet, int groupId)
        {
            ChangePassWDNotiEvent chgPassWDEvent = null;
            chgPassWDEvent = sgPageEvent.GetChgPassWDNotiEvent();
            if (chgPassWDEvent != null)
            {
                PageEventArgs args = new PageEventArgs();
                args.result = nRet;
                chgPassWDEvent(groupId, args);
            }
        }
        public void UserInfoAfterSend(int nRet, int groupId, SGData sgData)
        {
            SGLoginData sgLoginUserInfo = null;
            if (nRet == 0)
            {
                HsNetWork hs = null;
                if (m_DicNetWork.TryGetValue(groupId, out hs) == true)
                {
                    hs = m_DicNetWork[groupId];
                    sgDicRecvData.SetUserData(hs, groupId, sgData);
                }
            }
            sgLoginUserInfo = (SGLoginData)sgDicRecvData.GetLoginData(groupId);
            if (sgLoginUserInfo != null)
            {
                SendApproveLine(groupId, sgLoginUserInfo.GetUserID());
                SendUserSFMInfo(groupId, sgLoginUserInfo.GetUserID());
            }
        }

        public void URLListAfterSend(int nRet, int groupId, SGData sgData)
        {
            UrlListEvent urllistEvent = null;
            urllistEvent = sgPageEvent.GetServerURLlistEvent(groupId);
            if (urllistEvent != null)
            {
                PageEventArgs args = new PageEventArgs();
                args.result = nRet;
                // UI 단의 api callback호출
                urllistEvent(groupId, args);
            }
        }
        public void ApprInstAfterSend(int nRet, int groupId, SGData sgData)
        {
            ProxySearchEvent PSevent = null;
            sgPageEvent.DicProxySearch.TryGetValue(groupId, out PSevent);
            if (PSevent != null)
                PSevent(groupId, sgData);
        }
        public void CommonResultAfterSend(int nRet, int groupId, SGData sgData)
        {
            CommonResultEvent CommonEvent = null;
            sgPageEvent.DicCommonResult.TryGetValue(groupId, out CommonEvent);
            if (CommonEvent != null)
                CommonEvent(groupId, sgData);
        }

        /// <summary>
        /// 수신폴더에 있는 파일들을 정해놓은 시간이 지나면 삭제하는 함수(strDeletePath : 수신폴더경로, nDeleteTimeHour : 지난시간)
        /// </summary>
        /// <param name="strDeletePath"></param>
        /// <param name="nDeleteTimeHour"></param>
        private void DeleteTimeOverFiles(string strDeletePath, int nDeleteTimeHour)
        {

            if (string.IsNullOrEmpty(strDeletePath))
                return;

            HsLog.info($"DeleteTimeOverFiles : {strDeletePath}, delete OVer Time : {nDeleteTimeHour}");
            if (nDeleteTimeHour < 1)
            {
                HsLog.info($"DeleteTimeOverFiles - invalid input : {nDeleteTimeHour}");
                return;
            }

            String FolderName = strDeletePath;

            // 지정한 Folder 아래있는 파일들만 가져옴
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(FolderName);

            // 내부 파일들 삭제
            foreach (System.IO.FileInfo File in di.GetFiles())
            {

                DateTime tmCreate = File.CreationTime;
                DateTime tmDelete = tmCreate.AddHours(nDeleteTimeHour);
                DateTime tmCurrent = DateTime.Now;

                HsLog.info($"DeleteTimeOverFiles - File : {File.FullName}, " +
                    $"CreateTime : {tmCreate.ToString("yyyy/MM/dd HH:mm:ss")}, " +
                    $"DeleteTime : {tmDelete.ToString("yyyy/MM/dd HH:mm:ss")}, " +
                    $"CueentTime : {tmCurrent.ToString("yyyy/MM/dd HH:mm:ss")}");

                if ((tmCurrent - tmDelete).TotalSeconds > 0)
                {
                    System.IO.File.Delete(File.FullName);
                    HsLog.info($"DeleteTimeOverFiles - FileDelte! : {File.FullName}");
                }

            } // foreach (System.IO.FileInfo File in di.GetFiles())


            // 내부 Directory 삭제
            foreach (System.IO.DirectoryInfo Dir in di.GetDirectories())
            {
                // Recursive Search
                DeleteTimeOverFiles(Dir.FullName, nDeleteTimeHour);

                if (Directory.EnumerateFileSystemEntries(Dir.FullName).Any() == false)
                {
                    Directory.Delete(Dir.FullName);
                    HsLog.info($"DeleteTimeOverFiles - Delete Empty Folder : {Dir.FullName}");
                }
            }


        }

        private void RecvFileDeleteCycleThread(object obj)
        {

            lock (nlock)
            {
                if (m_bRecvFileDelThreadDo)
                    return;

                m_bRecvFileDelThreadDo = true;
            }

            Stopwatch sw = new Stopwatch();
            HsLog.info($"Recv File Delete Cycle - Thread - Do");
            HSCmdCenter hSCmdCenter = (HSCmdCenter)obj;
            int nIdx = 0;
            int[] nArryDeleteTime = new int[hSCmdCenter.m_nNetWorkCount];
            string strDownPath = "";

            while (true)
            {
                // 30초마다 한번씩 삭제 동작 : NetLink 기준
                Thread.Sleep(30 * 1000);

                // 로그인 상태 / 삭제주기 정책값 확인
                for (nIdx = 0; nIdx < hSCmdCenter.m_nNetWorkCount; nIdx++)
                {
                    nArryDeleteTime[nIdx] = 0;
                    SGLoginData sgLoginData = null;
                    bool bIsLogin = false;
                    sgLoginData = (SGLoginData)hSCmdCenter.GetLoginData(nIdx);
                    if (sgLoginData != null)
                    {
                        PageStatusData tmpData = null;
                        if (PageStatusService.m_DicPageStatusData.TryGetValue(nIdx, out tmpData))
                        {
                            if (PageStatusService.m_DicPageStatusData[nIdx].GetLogoutStatus() == false &&
                                PageStatusService.m_DicPageStatusData[nIdx].GetConnectStatus() == true)
                                bIsLogin = true;
                        }
                    }

                    if (bIsLogin && sgLoginData != null)
                    {
                        nArryDeleteTime[nIdx] = sgLoginData.GetFileRemoveCycle();
                        HsLog.info($"Recv File Delete Cycle - Thread - groupid : {nIdx} , DELETECYCLE : {nArryDeleteTime[nIdx]}");
                    }
                    else
                    {
                        HsLog.info($"Recv File Delete Cycle - Thread - groupid : {nIdx} , Logout Status!");
                    }


                }


                for (nIdx = 0; nIdx < hSCmdCenter.m_nNetWorkCount; nIdx++)
                {
                    if (nArryDeleteTime[nIdx] > 0)
                    {
                        // 삭제주기 설정된 값마다 삭제
                        strDownPath = GetDownLoadPath(nIdx);
                        HsLog.info($"Recv File Delete Cycle - Thread - groupid : {nIdx} , DeletePath : {strDownPath}");


                        DeleteTimeOverFiles(strDownPath, nArryDeleteTime[nIdx]);
                    }
                }

            } // while (true)

        }

        public void SystemRunAfterSend(int nRet, int groupId, SGData sgData)
        {
            if (nRet == 0)
            {
                SGLoginData sgLoginDataSystemRun = (SGLoginData)sgDicRecvData.GetLoginData(groupId);
                sgLoginDataSystemRun.AddRunSystemEnvData(sgData);
                HsNetWork hs = null;
                if (m_DicNetWork.TryGetValue(groupId, out hs) == true)
                {
                    sgDicRecvData.SetLoginData(hs, groupId, sgLoginDataSystemRun);
                    /*
                    sgLoginDataSystemRun = (SGLoginData)sgDicRecvData.GetLoginData(groupId);
                    string strHszDefaultOption = sgLoginDataSystemRun.GetHszDefaultOption();
                    int nHszOption = sgLoginDataSystemRun.GetHszDefaultDec();
                    int nApproveTypeSFM = sgLoginDataSystemRun.GetApproveTypeSFM();
                    string strInterLockEmail = sgLoginDataSystemRun.GetInterLockEmail();
                    */
                    hs = m_DicNetWork[groupId];
                    int hszOpt = sgLoginDataSystemRun.GetHszDefaultDec();
                    hs.SetHszDefault(hszOpt);
                    hs.SetManualDownLoad(sgLoginDataSystemRun.GetManualDownload());
                }
                RequestUrlList(groupId, sgLoginDataSystemRun.GetUserID());


                // 자동삭제 기능동작
                Thread tr = null;
                tr = new Thread(new ParameterizedThreadStart(RecvFileDeleteCycleThread));
                tr.Start(this);

                LoginEvent LoginResult_Event = null;
                LoginResult_Event = sgPageEvent.GetLoginEvent(groupId);
                if (LoginResult_Event != null)
                {
                    PageEventArgs e = new PageEventArgs();
                    e.result = 0;
                    e.strMsg = "";
                    LoginResult_Event(groupId, e);
                }
            }
        }

        /// <summary>
        /// 사용자의 결재정보라인 받은 정보를 SgDicRecvData 에 저장
        /// </summary>
        /// <param name="nRet"></param>
        /// <param name="groupId"></param>
        /// <param name="sgData"></param>

        public void ApprLineAfterSend(int nRet, int groupId, SGData sgData)
        {
            if (nRet == 0)
            {
                HsNetWork hs = null;
                if (m_DicNetWork.TryGetValue(groupId, out hs) == true)
                {
                    hs = m_DicNetWork[groupId];
                    sgDicRecvData.SetApprLineData(hs, groupId, sgData);
                }
            }
            /*
            SGApprLineData sgApprLineData = (SGApprLineData)sgDicRecvData.GetApprLineData(groupId);
            List<string> strListName = sgApprLineData.GetApprAndLineName();
            List<string> strListSeq = sgApprLineData.GetApprAndLineSeq();
            */
            SGUserData sgUserData = (SGUserData)sgDicRecvData.GetUserData(groupId);
            SGLoginData sgLoginDataApproveDefault = (SGLoginData)sgDicRecvData.GetLoginData(groupId);
            string strTeamCode = sgUserData.GetTeamCode();
            string strUserID = sgLoginDataApproveDefault.GetUserID();
            //SendInstApprove(groupId, strUserID, strTeamCode);
            SendSystemRunEnv(groupId, strUserID);
        }

        public void TransSearchAfterSend(int nRet, int groupId)
        {
            TransSearchEvent TransSearchResult_Event = sgPageEvent.GetTransSearchEvent(groupId);
            if (TransSearchResult_Event != null)
            {
                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                string strMsg = "";
                if (nRet != 0)
                    strMsg = SGTransManageData.ReturnMessage(eTransManageMsg.eSearchError);
                e.strMsg = strMsg;
                TransSearchResult_Event(groupId, e);
            }
        }
        //LinkCheck에서 Holiday LoginData에 Holiday 셋팅
        public void SetHoliday(int groupId, SGData sgData)
        {
            SGLoginData sgLoginData = (SGLoginData)sgDicRecvData.GetLoginData(groupId);
            if (sgLoginData != null)
                sgLoginData.SetTagData("HOLIDAY", sgData.GetEncTagData("HOLIDAY"));
        }

        public void TransSearchCountAfterSend(int nRet, int groupId, int count)
        {
            TransSearchCountEvent TransSearchCountResult_Event = sgPageEvent.GetTransSearchCountEvent(groupId);
            if (TransSearchCountResult_Event != null)
            {
                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                string strMsg = "";
                if (nRet != 0)
                    strMsg = SGTransManageData.ReturnMessage(eTransManageMsg.eSearchError);
                else
                    strMsg = SGTransManageData.ReturnMessage(eTransManageMsg.eNotData);
                e.strMsg = strMsg;
                e.count = count;
                TransSearchCountResult_Event(groupId, e);
            }
        }

        public void ApprSearchAfterSend(int nRet, int groupId)
        {
            ApprSearchEvent ApprSearchResult_Event = sgPageEvent.GetApprSearchEvent(groupId);
            if (ApprSearchResult_Event != null)
            {
                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                string strMsg = "";
                if (nRet != 0)
                    strMsg = SGApprManageData.ReturnMessage(eApprManageMsg.eSearchError);
                e.strMsg = strMsg;
                ApprSearchResult_Event(groupId, e);
            }
        }

        public void ApprSearchCountAfterSend(int nRet, int groupId, int count)
        {
            ApprSearchCountEvent ApprSearchCountResult_Event = sgPageEvent.GetApprSearchCountEvent(groupId);
            if (ApprSearchCountResult_Event != null)
            {
                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                string strMsg = "";
                if (nRet != 0)
                    strMsg = SGApprManageData.ReturnMessage(eApprManageMsg.eSearchError);
                else
                    strMsg = SGApprManageData.ReturnMessage(eApprManageMsg.eNotData);
                e.strMsg = strMsg;
                e.count = count;
                ApprSearchCountResult_Event(groupId, e);
            }
        }

        public void TransCancelAfterSend(int nRet, int groupId)
        {
            TransCancelEvent TransCancelResult_Event = sgPageEvent.GetTransCancelEvent(groupId);
            if (TransCancelResult_Event != null)
            {
                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                string strMsg = "";
                if (nRet != 0)
                    strMsg = SGTransManageData.ReturnMessage(eTransManageMsg.eTransCancelError);
                else
                    strMsg = SGTransManageData.ReturnMessage(eTransManageMsg.eTransCancelSuccess);

                e.strMsg = strMsg;
                TransCancelResult_Event(groupId, e);
            }
        }

        public void ApproveBatchAfterSend(int nRet, int groupId, SGData data)
        {
            if (data == null)
                return;
            string strProcID = data.GetBasicTagData("PROCID");
            ApprBatchEvent ApprBatchResult_Event = sgPageEvent.GetApprBatchEvent(groupId);
            if (ApprBatchResult_Event != null)
            {
                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                string strMsg = "";
                if (nRet != 0)
                    strMsg = SGApprManageData.ReturnMessage(eApprManageMsg.eApprBatchError);
                else
                {
                    if (strProcID.Equals("A"))                                                       // 승인 
                        strMsg = SGApprManageData.ReturnMessage(eApprManageMsg.eApprBatchActionSuccess);
                    else if (strProcID.Equals("R"))                                                       // 반려 
                        strMsg = SGApprManageData.ReturnMessage(eApprManageMsg.eApprBatchRejectSuccess);
                    else
                        strMsg = SGApprManageData.ReturnMessage(eApprManageMsg.eApprBatchActionSuccess);
                }

                e.strDummy = strProcID;
                e.strMsg = strMsg;
                ApprBatchResult_Event(groupId, e);
            }
        }

        public void DetailSearchAfterSend(int nRet, int groupId, string strTransSeq)
        {
            DetailSearchEvent DetailSearchResult_Event = sgPageEvent.GetDetailSearchEvent(groupId);
            if (DetailSearchResult_Event != null)
            {
                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                string strMsg = "";
                if (nRet != 0)
                    strMsg = SGDetailData.ReturnMessage(eDetailManageMsg.eSearchError);
                else
                    strMsg = strTransSeq;
                e.strMsg = strMsg;
                DetailSearchResult_Event(groupId, e);
            }
        }

        public void DeptApprLineSearchAfterSend(int nRet, int groupId)
        {
            DeptApprLineSearchEvent DeptApprLineSearchResult_Event = sgPageEvent.GetDeptApprLineSearchEvent(groupId);
            if (DeptApprLineSearchResult_Event != null)
            {
                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                string strMsg = "";
                if (nRet != 0)
                    strMsg = SGDeptApprLineSearchData.ReturnMessage(eDeptApprLineSearchManageMsg.eSearchError);

                e.strMsg = strMsg;

                DeptApprLineSearchResult_Event(groupId, e);
            }
        }

        public void DeptApprLineReflashAfterSend(int nRet, int groupId)
        {
            DeptApprLineReflashEvent DeptApprLineReflashResult_Event = sgPageEvent.GetDeptApprLineReflashEvent(groupId);
            if (DeptApprLineReflashResult_Event != null)
            {
                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                string strMsg = "";
                if (nRet != 0)
                    strMsg = SGDeptApprLineSearchData.ReturnMessage(eDeptApprLineSearchManageMsg.eSearchError);

                e.strMsg = strMsg;

                DeptApprLineReflashResult_Event(groupId, e);
            }
        }

        public void FileSendProgressNotiAfterSend(int nRet, int groupId, SGData data)
        {
            FileSendProgressEvent FileSendProgress_Event = sgPageEvent.GetFileSendProgressEvent(groupId);
            if (FileSendProgress_Event != null)
            {
                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                string strMsg = "";
                e.strMsg = strMsg;

                int count = 0;
                string strProgress = data.GetBasicTagData("PROGRESS");
                if (strProgress != "-1" && !strProgress.Equals(""))
                    count = Convert.ToInt32(strProgress);
                e.count = count;

                FileSendProgress_Event(groupId, e);
            }
        }
        public void FileRecvProgressNotiAfterSend(int nRet, int groupId, SGData data)
        {
            //FileRecvProgressEvent FileRecvProgress_Event = sgPageEvent.GetFileRecvProgressEvent(groupId);
            FileRecvProgressEvent FileRecvProgress_Event = sgPageEvent.GetFileRecvProgressEvent();
            if (FileRecvProgress_Event != null)
            {
                RecvDataEventArgs e = new RecvDataEventArgs();
                e.result = nRet;
                e.strMsg = data.GetBasicTagData("TRANSSEQ");
                e.strFilePath = data.GetBasicTagData("FILENAME");
                e.strDataType = data.GetBasicTagData("DATATYPE");

                int count = 0;
                string strProgress = data.GetBasicTagData("PROGRESS");
                if (!strProgress.Equals(""))
                    count = Convert.ToInt32(strProgress);
                e.count = count;

                FileRecvProgress_Event(groupId, e);
            }
        }
        public void FilePrevProgressNotiAfterSend(int nRet, int groupId, SGData data)
        {
            FilePrevProgressEvent FilePrevProgress_Event = sgPageEvent.GetFilePrevProgressEvent(groupId);
            if (FilePrevProgress_Event != null)
            {
                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                e.strMsg = data.GetBasicTagData("FILENAME");

                int count = 0;
                string strProgress = data.GetBasicTagData("PROGRESS");
                if (!strProgress.Equals(""))
                    count = Convert.ToInt32(strProgress);
                e.count = count;

                FilePrevProgress_Event(groupId, e);
            }
        }
        public void ClipRecvNotiAfterSend(int nRet, int groupId, SGData data)
        {
            RecvClipEvent recvClip_Event = sgPageEvent.GetRecvClipEvent(groupId);
            if (recvClip_Event != null)
            {
                RecvClipEventArgs e = new RecvClipEventArgs();
                string strDataType = data.GetBasicTagData("DATATYPE");
                if (!strDataType.Equals(""))
                    e.nDataType = Convert.ToInt32(strDataType);

                e.ClipDataSize = data.byteData.Length;
                if (data.byteData != null)
                {
                    e.ClipData = data.byteData.ToArray();
                }
                recvClip_Event(groupId, e);
            }
        }

        public void UrlServerRecvNotiAfterSend(int nRet, int groupId, SGData data)
        {
            RecvUrlEvent recvUrl_Event = sgPageEvent.GetServerRecvUrlEvent(groupId);
            if (recvUrl_Event != null)
            {
                RecvUrlEventArgs e = new RecvUrlEventArgs();
                string strData = data.GetEncTagData("SUBDATA");
                if (strData.Length > 0)
                    e.strUrlData = strData;

                recvUrl_Event(groupId, e);
            }
        }

        public void UrlBrowserRecvNotiAfterSend(int nRet, int groupId, SGData data)
        {
            RecvUrlEvent recvUrl_Event = sgPageEvent.GetBrowserRecvUrlEvent(groupId);
            if (recvUrl_Event != null)
            {
                RecvUrlEventArgs e = new RecvUrlEventArgs();
                string strData = data.GetEncTagData("SUBDATA");
                if (strData.Length > 0)
                    e.strUrlData = strData;

                strData = data.GetEncTagData("GROUPID");
                if (strData.Length > 0)
                    groupId = Convert.ToInt32(strData);

                recvUrl_Event(groupId, e);
            }
        }


        public void RMouseFileAddNotiAfterSend(int nRet, int groupId)
        {
            AddFileRMHeaderEvent addFileRMHeader_Event = sgPageEvent.GetAddRMHeaderEventAdd();
            if (addFileRMHeader_Event != null)
            {
                string strRMouseFilePath = PageStatusData.GetRMFIlePath();

                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                //e.strMsg = strRMouseFilePath;
                e.strMsg = "";

                FileAddManage fileAddManage = new FileAddManage(groupId);
                groupId = fileAddManage.LoadRMFileGroupID(strRMouseFilePath);
                addFileRMHeader_Event(groupId, e);
            }
            /*
            AddFileRMEvent addFileRM_Event = sgPageEvent.GetAddFileRMEvent(groupId);
            if (addFileRM_Event != null)
            {
                string strRMouseFilePath = PageStatusData.GetRMFIlePath();

                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                //e.strMsg = strRMouseFilePath;
                e.strMsg = "";

                FileAddManage fileAddManage = new FileAddManage(groupId);
                groupId = fileAddManage.LoadRMFileGroupID(strRMouseFilePath);
                addFileRM_Event(groupId, e);
            }
            */
        }
        public void ApproveCountNotiAfterSend(int nRet, eCmdList cmd, int groupId, SGData data)
        {
            ServerNotiEvent sNotiEvent = sgPageEvent.GetServerNotiEvent();
            if (sNotiEvent != null)
            {
                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                e.strMsg = "";
                e.count = Convert.ToInt32(data.GetBasicTagData("APPROVECOUNT"));
                e.strDummy = data.GetBasicTagData("APPROVEUSERKIND");

                sNotiEvent(groupId, cmd, e);
            }
        }
        public void VirusScanNotiAfterSend(int nRet, eCmdList cmd, int groupId, SGData sgData)
        {
            //APTAndVirusNotiEvent AptAndVirusEvent = sgPageEvent.GetAPTAndVirusNotiEvent(groupId);
            APTAndVirusNotiEvent AptAndVirusEvent = sgPageEvent.GetAPTAndVirusNotiEvent();
            if (AptAndVirusEvent != null)
            {
                AptAndVirusEventArgs e = new AptAndVirusEventArgs();
                e.result = nRet;
                e.strTransSeq = sgData.GetBasicTagData("TRANSSEQ");
                e.strTitle = sgData.GetBasicTagData("TITLE");
                e.strMsg = sgData.GetBasicTagData("VIRUS_MSG");
                AptAndVirusEvent(groupId, cmd, e);
            }
        }
        public void EmailApproveNotiAfterSend(int nRet, eCmdList cmd, int groupId, SGData data)
        {
            ServerNotiEvent sNotiEvent = sgPageEvent.GetServerNotiEvent();
            if (sNotiEvent != null)
            {
                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                e.count = 0;
                string strCount = data.GetBasicTagData("EMAILAPPROVECOUNT");
                if (!strCount.Equals(""))
                    e.count = Convert.ToInt32(strCount);
                e.strMsg = "";
                sNotiEvent(groupId, cmd, e);
            }
        }

        public void BoardNotiAfterSend(int nRet, eCmdList cmd, int groupId, SGData data)
        {
            ServerNotiEvent sNotiEvent = sgPageEvent.GetServerNotiEvent();
            if (sNotiEvent != null)
            {
                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                e.count = 0;
                e.strMsg = data.GetBasicTagData("BOARDHASH");
                sNotiEvent(groupId, cmd, e);
            }
        }
        public void ApproveActionNotiAfterSend(int nRet, eCmdList cmd, int groupId, SGData sgData)
        {
            ApproveActionNotiEvent ApprActionEvent = sgPageEvent.GetApproveActionNotiEvent();
            if (ApprActionEvent != null)
            {
                ApproveActionEventArgs e = new ApproveActionEventArgs();
                e.result = nRet;
                e.strTransSeq = sgData.GetBasicTagData("TRANSSEQ");
                e.strTitle = sgData.GetBasicTagData("TITLE");

                string strAction = sgData.GetBasicTagData("ACTION");
                if (!strAction.Equals(""))
                    e.Action = Convert.ToInt32(strAction);

                string strApprKind = sgData.GetBasicTagData("APPROVEKIND");
                if (!strApprKind.Equals(""))
                    e.ApproveKind = Convert.ToInt32(strApprKind);

                string strApprUserKind = sgData.GetBasicTagData("APPROVEUSERKIND");
                if (!strApprUserKind.Equals(""))
                    e.ApproveUserKind = Convert.ToInt32(strApprUserKind);

                ApprActionEvent(groupId, cmd, e);
            }
        }

        public void UseDayFileInfoNotiAfterSend(int nRet, int groupId, SGData sgData)
        {
            UseDayFileNotiEvent useDayFileEvent = sgPageEvent.GetUseDayFileNotiEvent(groupId);
            if (useDayFileEvent != null)
            {
                FileAndClipDayArgs args = new FileAndClipDayArgs();
                if (nRet == 0)
                {
                    string strData = sgData.GetBasicTagData("RECORD");
                    string[] strArray = strData.Split('\u0001');
                    args.result = nRet;
                    string strSize = strArray[1];
                    string strCount = strArray[2];
                    if (!strSize.Equals(""))
                        args.Size = Convert.ToInt64(strSize);
                    if (!strCount.Equals(""))
                        args.Count = Convert.ToInt32(strCount);
                }
                else
                {
                    args.result = nRet;
                    args.Size = 0;
                    args.Count = 0;
                }
                useDayFileEvent(groupId, args);
            }
        }
        public void UseDayClipInfoNotiAfterSend(int nRet, int groupId, SGData sgData)
        {
            UseDayClipNotiEvent useDayClipEvent = sgPageEvent.GetUseDayClipNotiEvent(groupId);
            if (useDayClipEvent != null)
            {
                FileAndClipDayArgs args = new FileAndClipDayArgs();
                if (nRet == 0)
                {
                    string strData = sgData.GetBasicTagData("RECORD");
                    string[] strArray = strData.Split('\u0001');
                    args.result = nRet;
                    string strSize = strArray[1];
                    string strCount = strArray[2];
                    if (!strSize.Equals(""))
                        args.Size = Convert.ToInt64(strSize);
                    if (!strCount.Equals(""))
                        args.Count = Convert.ToInt32(strCount);
                }
                else
                {
                    args.result = nRet;
                    args.Size = 0;
                    args.Count = 0;
                }
                useDayClipEvent(groupId, args);
            }
        }

        public void LogOutNotiAfterSend(int nRet, int groupId, SGData data)
        {
            LogoutNotiEvent LogOut_Event = sgPageEvent.GetLogoutNotiEvent();
            if (LogOut_Event != null)
            {
                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                e.strMsg = data.GetBasicTagData("REASON");
                LogOut_Event(groupId, e);
            }
        }

        public void ScreenLockClearAfterSend(int nRet, int groupId, SGData data)
        {
            ScreenLockClearNotiEvent SCClear_Event = sgPageEvent.GetScreenLockClearNotiEvent();
            if (SCClear_Event != null)
            {
                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                e.strMsg = data.GetBasicTagData("REASON");
                SCClear_Event(groupId, e);
            }
        }
        public void ZipDepthInfoSetting(int nRet, int groupId, SGData sgData)
        {
            SGLoginData sgLoginData = (SGLoginData)sgDicRecvData.GetLoginData(groupId);
            if (nRet == 0)
            {
                sgLoginData.AddZipDepthInfo(sgData);
            }
            else
            {
                sgLoginData.AddData("I_CLIENT_ZIP_DEPTH", "0/0");
                sgLoginData.AddData("E_CLIENT_ZIP_DEPTH", "0/0");
            }
        }

        public void UpgradeNotiAfterSend(int nRet, SGData data)
        {
            ClientUpgradeEvent clipUpdate = sgPageEvent.GetClientUpgradeNotiEvent();
            if (clipUpdate != null)
            {
                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                e.strMsg = data.GetBasicTagData("CLIVERSION");
                clipUpdate(e);
            }
        }

        public void DashBoardCountAfterSend(int nRet, int groupID, SGData data)
        {
            DashBoardCountEvent dashBoardCount = null;
            dashBoardCount = sgPageEvent.GetDashBoardCountEvent(groupID);
            if (dashBoardCount != null)
            {
                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                e.strMsg = data.GetBasicTagData("RECORD");
                dashBoardCount(groupID, e);
            }
        }
        public void DashBoardTransReqCountAfterSend(int nRet, int groupID, SGData data)
        {
            DashBoardTransReqCountEvent dashBoardTransReqCount = null;
            dashBoardTransReqCount = sgPageEvent.GetDashBoardTransReqCountEvent(groupID);
            if (dashBoardTransReqCount != null)
            {
                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                e.strMsg = data.GetBasicTagData("RECORD");
                e.strMsg = e.strMsg.Replace("\u0001", "");
                dashBoardTransReqCount(groupID, e);
            }
        }
        public void DashBoardApprWaitCountAfterSend(int nRet, int groupID, SGData data)
        {
            DashBoardApprWaitCountEvent dashBoardApprWaitCount = null;
            dashBoardApprWaitCount = sgPageEvent.GetDashBoardApprWaitCountEvent(groupID);
            if (dashBoardApprWaitCount != null)
            {
                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                e.strMsg = data.GetBasicTagData("RECORD");
                e.strMsg = e.strMsg.Replace("\u0001", "");
                dashBoardApprWaitCount(groupID, e);
            }
        }
        public void DashBoardApprConfirmCountAfterSend(int nRet, int groupID, SGData data)
        {
            DashBoardApprConfirmCountEvent dashBoardApprConfirmCount = null;
            dashBoardApprConfirmCount = sgPageEvent.GetDashBoardApprConfirmCountEvent(groupID);
            if (dashBoardApprConfirmCount != null)
            {
                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                e.strMsg = data.GetBasicTagData("RECORD");
                e.strMsg = e.strMsg.Replace("\u0001", "");
                dashBoardApprConfirmCount(groupID, e);
            }
        }

        public void DashBoardApprRejectCountAfterSend(int nRet, int groupID, SGData data)
        {
            DashBoardApprRejectCountEvent dashBoardApprRejectCount = null;
            dashBoardApprRejectCount = sgPageEvent.GetDashBoardApprRejectCountEvent(groupID);
            if (dashBoardApprRejectCount != null)
            {
                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                e.strMsg = data.GetBasicTagData("RECORD");
                e.strMsg = e.strMsg.Replace("\u0001", "");
                dashBoardApprRejectCount(groupID, e);
            }
        }
        public void PasswdChgDayAfterSend(int nRet, int groupID, SGData data)
        {
            PasswdChgDayEvent passwdChgDay = null;
            passwdChgDay = sgPageEvent.GetPasswdChgDayEvent(groupID);
            if (passwdChgDay != null)
            {
                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                e.strMsg = data.GetBasicTagData("RECORD");
                passwdChgDay(groupID, e);
            }
        }

        public void BoardNotiSearchAfterSend(int nRet, int groupID)
        {
            BoardNotiSearchEvent boardNotiSearch = null;
            boardNotiSearch = sgPageEvent.GetBoardNotiSearchEvent();
            if (boardNotiSearch != null)
            {
                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                boardNotiSearch(groupID, e);
            }
        }


        public void DeptInfoAfterSend(int nRet, int groupId, SGData sgData)
        {
            if (nRet == 0)
            {
                HsNetWork hs = null;
                if (m_DicNetWork.TryGetValue(groupId, out hs) == true)
                {
                    hs = m_DicNetWork[groupId];
                    sgDicRecvData.SetDeptInfoData(hs, groupId, sgData);
                }
            }

            //결재 조회 화면의 이벤트
            DeptInfoNotiEvent deptInfoReceiveEvent = sgPageEvent.GetDeptInfoEvent(groupId);
            if (deptInfoReceiveEvent != null)
                deptInfoReceiveEvent(groupId);
        }
        public void SetDetailDataChange(int groupid, SGDetailData sgData)
        {
            HsNetWork hs = null;
            hs = GetConnectNetWork(groupid);
            if (hs != null)
            {
                sgDicRecvData.SetDetailDataChange(hs, groupid, sgData);
            }
        }

        public bool GetFileRecving(int groupid)
        {
            bool bRecving = false;
            if (m_DicFileRecving.TryGetValue(groupid, out bRecving) != true)
                return bRecving;
            return m_DicFileRecving[groupid];
        }

        public void SetFileRecving(int groupid, bool bRecving)
        {
            bool bTemp = false;
            if (m_DicFileRecving.TryGetValue(groupid, out bTemp) == true)
            {
                if (m_DicFileRecving.TryRemove(groupid, out bTemp) == false)
                {
                    m_DicFileRecving.TryUpdate(groupid, bRecving, !bRecving);
                    return;
                }
                //m_DicFileRecving.Remove(groupid);
            }

            m_DicFileRecving.TryAdd(groupid, bRecving);
            //m_DicFileRecving[groupid] = bRecving;
        }

        public bool GetFileSending(int groupid)
        {
            bool bSending = false;
            if (m_DicFileSending.TryGetValue(groupid, out bSending) != true)
                return bSending;
            return m_DicFileSending[groupid];
        }

        public void SetFileSending(int groupid, bool bSending)
        {
            bool bTemp = false;
            if (m_DicFileSending.TryGetValue(groupid, out bTemp) == true)
            {
                if (m_DicFileSending.TryRemove(groupid, out bTemp) == false)
                {
                    m_DicFileSending.TryUpdate(groupid, bSending, !bSending);
                    return;
                }
                //m_DicFileSending.Remove(groupid);
                return;
            }

            m_DicFileSending.TryAdd(groupid, bSending);
            //m_DicFileSending[groupid] = bSending;
        }

        /// <summary>
        /// 다운로드 경로 설정하기
        /// </summary>
        /// <param name="groupid">그룹ID</param>
        /// <param name="strDownPath">다운로드 경로</param>
        /// <returns>0 : 설정성공   -1 : 설정실패</returns>
        public int SetDownLoadPath(int groupid, string strDownPath)
        {
            HsNetWork hsNetWork = GetConnectNetWork(groupid);
            int ret = 0;
            if (hsNetWork != null)
                ret = hsNetWork.SetDownLoadPath(strDownPath);
            return ret;
        }

        /// <summary>
        /// 다운로드 경로 가져오기
        /// </summary>
        /// <param name="groupid">그룹ID</param>
        /// <returns>다운로드 경로</returns>
        public string GetDownLoadPath(int groupid)
        {
            string strDownPath = "";
            HsNetWork hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                strDownPath = hsNetWork.GetDownLoadPath();
            return strDownPath;
        }

        /// <summary>
        /// 기본 다운로드 경로 설정 (OpenNetLink 환경설정에 저장된 경로) => 팝업창으로 임시 수신경로 지정하여 경로 변경할 경우 기본 다운경로는 변경되지 않음
        /// </summary>
        /// <param name="groupid">그룹ID</param>
        /// <param name="strDownPath">기본 다운로드 경로</param>
        /// <returns>0 : 설정 성공   -1 : 설정 실패</returns>
        public int SetBaseDownLoadPath(int groupid, string strDownPath)
        {
            HsNetWork hsNetWork = GetConnectNetWork(groupid);
            int ret = 0;
            if (hsNetWork != null)
                ret = hsNetWork.SetBaseDownLoadPath(strDownPath);
            return ret;
        }

        /// <summary>
        /// 기본 다운로드 경로 가져오기 (OpenNetLink 환경설정에 저장된 경로)
        /// </summary>
        /// <param name="groupid">그룹ID</param>
        /// <returns>기본 다운로드 경로</returns>
        public string GetBaseDownLoadPath(int groupid)
        {
            string strDownPath = "";
            HsNetWork hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                strDownPath = hsNetWork.GetBaseDownLoadPath();
            return strDownPath;
        }

        /// <summary>
        /// OS제공 최대 파일경로 길이 사용 유무 설정 (true : OS제공 최대 파일경로길이 사용   false : Hanssak 설정 길이 사용)
        /// </summary>
        /// <param name="groupid">그룹ID</param>
        /// <param name="useOSMaxPath">OS제공 최대 파일경로 사용여부</param>
        /// <returns>0 : 설정성공   -1 : 설정실패</returns>
        public int SetUseOSMaxPath(int groupid, bool useOSMaxPath)
        {
            HsNetWork hsNetWork = GetConnectNetWork(groupid);
            int ret = 0;
            if (hsNetWork != null)
                ret = hsNetWork.SetUseOSMaxPath(useOSMaxPath);
            return ret;
        }

        /// <summary>
        /// 송신 파일 전체경로 최대 길이 가져오기
        /// </summary>
        /// <param name="groupid">그룹ID</param>
        /// <returns>송신 파일 전체경로 최대 길이</returns>
        public int GetSendFilePathLengthMax(int groupid)
        {
            HsNetWork hsNetWork = GetConnectNetWork(groupid);
            int ret = 0;
            if (hsNetWork != null)
                ret = hsNetWork.GetSendFilePathLengthMax();
            return ret;
        }

        /// <summary>
        /// 송신 파일명 최대길이 가져오기
        /// </summary>
        /// <param name="groupid">그룹ID</param>
        /// <returns>송신 파일명 최대길이</returns>
        public int GetSendFileNameLengthMax(int groupid)
        {
            HsNetWork hsNetWork = GetConnectNetWork(groupid);
            int ret = 0;
            if (hsNetWork != null)
                ret = hsNetWork.GetSendFileNameLengthMax();
            return ret;
        }

        /// <summary>
        /// 수신 파일 전체경로 최대 길이 가져오기
        /// </summary>
        /// <param name="groupid">그룹ID</param>
        /// <returns>수신 파일 전체경로 최대 길이</returns>
        public int GetReceiveFilePathLengthMax(int groupid)
        {
            HsNetWork hsNetWork = GetConnectNetWork(groupid);
            int ret = 0;
            if (hsNetWork != null)
                ret = hsNetWork.GetReceiveFilePathLengthMax();
            return ret;
        }

        /// <summary>
        /// 수신 파일명 최대길이 가져오기
        /// </summary>
        /// <param name="groupid">그룹ID</param>
        /// <returns>수신 파일명 최대길이</returns>
        public int GetReceiveFileNameLengthMax(int groupid)
        {
            HsNetWork hsNetWork = GetConnectNetWork(groupid);
            int ret = 0;
            if (hsNetWork != null)
                ret = hsNetWork.GetReceiveFileNameLengthMax();
            return ret;
        }

        public HsNetWork GetConnectNetWork(int groupid)
        {
            HsNetWork hs = null;
            if (!m_DicNetWork.TryGetValue(groupid, out hs))
                return null;
            hs = m_DicNetWork[groupid];
            return hs;
        }

        public void SetAllowSessionDuplicate(int groupid)
        {
            HsNetWork hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                hsNetWork.bIgnoreSessionDuplicate = true;
        }

        public int Login(int groupid, string strID, string strPW, string strCurCliVersion, string otp, int loginType = 0)
        {
            HsNetWork hsNetWork = GetConnectNetWork(groupid);
            int ret = 0;
            if (hsNetWork != null)
                ret = hsNetWork.Login(strID, strPW, otp, strCurCliVersion, 0, loginType);
            return 0;
        }

        public int LoginAD(int groupid, string strID, string strPW, string strCurCliVersion, string otp, int loginType = 0)
        {
            HsNetWork hsNetWork = GetConnectNetWork(groupid);
            int ret = 0;
            if (hsNetWork != null)
                ret = hsNetWork.Login(strID, strPW, otp, strCurCliVersion, 9, loginType);
            return 0;
        }

        public int LoginGpki(int groupid, string strID, string strCurCliVersion)
        {
            HsNetWork hsNetWork = GetConnectNetWork(groupid);
            int ret = 0;
            if (hsNetWork != null)
                ret = hsNetWork.Login(strID, strID, "", strCurCliVersion, 9);

            return 0;
        }

        public int SendUserInfoEx(int groupid, string strUserID)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestUserInfoEx(hsNetWork, groupid, strUserID);
            return -1;
        }

        public int SendUserInfoCheck(int groupid, string strUserID)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestUserInfoCheck(hsNetWork, groupid, strUserID);
            return -1;
        }

        public int SendApproveLine(int groupid, string strUserID)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestApproveLine(hsNetWork, groupid, strUserID);
            return -1;
        }
        public int SendInstApprove(int groupid, string strUserID, string strTeamCode)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestInstApprove(hsNetWork, groupid, strUserID, strTeamCode);
            return -1;
        }

        public int SendSystemEnv(int groupid, string strUserID)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSystemEnv(hsNetWork, groupid, strUserID);
            return -1;
        }
        public int SendSystemRunEnv(int groupid, string strUserID)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSystemRunEnv(hsNetWork, groupid, strUserID);
            return -1;
        }

        public int RequestUrlList(int groupid, string strUserID)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestUrlList(hsNetWork, groupid, strUserID);
            return -1;
        }

        public int SendChangePasswd(int groupid, string strUserID, string strOldPasswd, string strNewPasswd)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestChangePasswd(hsNetWork, groupid, strUserID, strOldPasswd, strNewPasswd);
            return -1;
        }
        public int SendOTPNumber(int groupid, string strUserID, string otpNumber)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestOTPRegist(hsNetWork, groupid, strUserID, otpNumber);
            return -1;
        }

        public int SendDeptInfo(int groupid, string strUserID)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestDeptInfo(hsNetWork, groupid, strUserID);
            return -1;
        }

        public int SendFileTransInfo(int groupid, string strUserID, string strFromDate, string strToDate, string strTransKind, string strTransStatus, string strApprStatus, string strDlp, string strTitle, string strDataType)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestFileTransList(hsNetWork, groupid, strUserID, strFromDate, strToDate, strTransKind, strTransStatus, strApprStatus, strDlp, strTitle, strDataType);
            return -1;
        }
        public int SendFileApprInfo(int groupid, string strUserID, string strFromDate, string strToDate, string strApprKind, string strTransKind, string strApprStatus, string strReqUserName, string strDlp, string strTitle, string strDlpApprove, string strApprover, string strDataType)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestFileApprList(hsNetWork, groupid, strUserID, strFromDate, strToDate, strApprKind, strTransKind, strApprStatus, strReqUserName, strDlp, strTitle, strDlpApprove, strApprover, strDataType);
            return -1;
        }

        public int SendTransDetail(int groupid, string strUserID, string strTransSeq)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestTransDetail(hsNetWork, groupid, strUserID, strTransSeq);
            return -1;
        }
        public int SendDownloadCount(int groupid, string strUserID, string strTransSeq)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestDownloadCount(hsNetWork, groupid, strUserID, strTransSeq);
            return -1;
        }

        public int SendTransDaySize(int groupid, string strUserID)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestTransDaySize(hsNetWork, groupid, strUserID);
            return -1;
        }

        public int SendApproveAlway(int groupid, string strUserID)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestApproveAlway(hsNetWork, groupid, strUserID);
            return -1;
        }

        public int SendApproveBatch(int groupid, string strUserID, string strProcID, string strReason, string strApproveSeqs, string strApprover, string strApproveUserKind)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestApproveBatch(hsNetWork, groupid, strUserID, strProcID, strReason, strApproveSeqs, strApprover, strApproveUserKind);
            return -1;
        }
        public int SendEmailApproveBatch(int groupid, string strUserID, string strProcID, string strReason, string strApproveSeqs)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestEmailApproveBatch(hsNetWork, groupid, strUserID, strProcID, strReason, strApproveSeqs);
            return -1;
        }
        public int SendTransCancel(int groupid, string strUserID, string strTransSeq, string strAction, string strReason)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendCancel(hsNetWork, groupid, strUserID, strTransSeq, strAction, strReason);
            return -1;
        }
        public int SendForwardCancel(int groupid, string strUserID, string strTransSeq)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestForwardCancel(hsNetWork, groupid, strUserID, strTransSeq);
            return -1;
        }
        public int RequestAutoDownload(int groupid, string strUserID, string strTransSeq)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestAutoDownload(hsNetWork, groupid, strUserID, strTransSeq);
            return -1;
        }
        public int RequestManualDownload(int groupid, string strUserID, string strTransSeq)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestManualDownload(hsNetWork, groupid, strUserID, strTransSeq);
            return -1;
        }

        public int SendTransListCountQuery(int groupid, string strUserID, string strQuery)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendTransListCountQuery(hsNetWork, groupid, strUserID, strQuery);
            return -1;
        }
        public int SendCountQuery(int groupid, string strUserID, string strQuery)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendCountQuery(hsNetWork, groupid, strUserID, strQuery);
            return -1;
        }

        /// <summary>
        /// 쿼리문(List) 전송
        /// </summary>
        /// <param name="groupid">그룹ID</param>
        /// <param name="strUserID">쿼리문 요청 사용자 ID</param>
        /// <param name="strQuery">쿼리문</param>
        /// <returns></returns>
        public int SendListQuery(int groupid, string strUserID, string strQuery)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendListQuery(hsNetWork, groupid, strUserID, strQuery);
            return -1;
        }

        /// <summary>
        /// 쿼리문(Detail) 전송
        /// </summary>
        /// <param name="groupid">그룹ID</param>
        /// <param name="strUserID">쿼리문 요청 사용자 ID</param>
        /// <param name="strQuery">쿼리문</param>
        /// <returns></returns>
        public int SendDetailQuery(int groupid, string strUserID, string strQuery)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendDetailQuery(hsNetWork, groupid, strUserID, strQuery);
            return -1;
        }
        public int SendRecordExistCheckQuery(int groupid, string strUserID, string strQuery)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestRecordExistCheckQuery(hsNetWork, groupid, strUserID, strQuery);
            return -1;
        }

        public int SendEmailTransferCancel(int groupid, string strUserID, string emailSeq)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendEmailCancel(hsNetWork, groupid, strUserID, emailSeq);
            return -1;
        }

        public int SendTransListQuery(int groupid, string strUserID, string strQuery)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendTransListQuery(hsNetWork, groupid, strUserID, strQuery);
            return -1;
        }
        public int SendApprListCountQuery(int groupid, string strUserID, string strQuery)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendApprListCountQuery(hsNetWork, groupid, strUserID, strQuery);
            return -1;
        }
        public int SendApprListQuery(int groupid, string strUserID, string strQuery)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendApprListQuery(hsNetWork, groupid, strUserID, strQuery);
            return -1;
        }
        public int SendTransDetailQuery(int groupid, string strUserID, string strQuery)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendTransDetailQuery(hsNetWork, groupid, strUserID, strQuery);
            return -1;
        }

        public int SendApprDetailQuery(int groupid, string strUserID, string strQuery)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendApprDetailQuery(hsNetWork, groupid, strUserID, strQuery);
            return -1;
        }

        public int SendDeptApprLineSearchQuery(int groupid, string strUserID, string strQuery)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendDeptApprLineSearchQuery(hsNetWork, groupid, strUserID, strQuery);
            return -1;
        }

        public int SendSecurityApproverQuery(int groupid, string strUserID, string strQuery)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSecurityApproverQuery(hsNetWork, groupid, strUserID, strQuery);
            return -1;
        }

        public int SendFileTrans(int groupid, string strUserID, string strMid, string strPolicyFlag, string strTitle, string strContents, bool bApprSendMail, bool bAfterApprove, int nDlp, string strRecvPos, string strZipPasswd, bool bPrivachApprove, string strSecureString, string strDataType, int nApprStep, List<string> ApprLineSeq, List<HsStream> FileList, string strNetOver3info, string receiver)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            int nRet = -1;
            if (hsNetWork != null)
                nRet = sgSendData.RequestSendFileTrans(hsNetWork, groupid, strUserID, strMid, strPolicyFlag, strTitle, strContents, bApprSendMail, bAfterApprove, nDlp, strRecvPos, strZipPasswd, bPrivachApprove, strSecureString, strDataType, nApprStep, ApprLineSeq, FileList, strNetOver3info, receiver);

            if (nRet == -2)
                SendFileTransCancel();
            return nRet;
        }

        public int SendFileTrans(int groupid, string strUserID, string strMid, string strPolicyFlag, string strTitle, string strContents, bool bApprSendMail, bool bAfterApprove, int nDlp, string strRecvPos, string strZipPasswd, bool bPrivachApprove, string strSecureString, string strDataType, int nApprStep, string ApprLineSeq, List<HsStream> FileList, string strNetOver3info, string receiver)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            int nRet = -1;
            if (hsNetWork != null)
                nRet = sgSendData.RequestSendFileTrans(hsNetWork, groupid, strUserID, strMid, strPolicyFlag, strTitle, strContents, bApprSendMail, bAfterApprove, nDlp, strRecvPos, strZipPasswd, bPrivachApprove, strSecureString, strDataType, nApprStep, ApprLineSeq, FileList, strNetOver3info, receiver);

            if (nRet == -2)
                SendFileTransCancel();
            return nRet;
        }

        public int ContinueSendFileTrans(int groupid, Dictionary<string,string> values, string strNetOver3info, string hszFileName, int currentFileSize)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            int nRet = -1;
            if (hsNetWork != null)
                nRet = sgSendData.RequestContinueSendFileTrans(hsNetWork, groupid, values, strNetOver3info, hszFileName, currentFileSize);

            if (nRet == -2)
                SendFileTransCancel();
            return nRet;
        }


        public void SendFileTransCancel()
        {
            sgSendData.RequestSendFileTransCancel();
        }

        public int SendFilePrev(int groupid, string strUserID, string strTransSeq, string strFileName, string strFileKey, string strFileSeq, string strOrgData)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendFilePrev(hsNetWork, groupid, strUserID, strTransSeq, strFileName, strFileKey, strFileSeq, strOrgData);
            return -1;
        }
        //파일 이어전송 파일정보 요청
        public int SendFileUploadInfo(int groupid, string strUserID, string mid, string totalPart, string totalSize)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendFileUploadInfo(hsNetWork, strUserID, mid, totalPart, totalSize);
            return -1;
        }
        public int SendEmailDownload(int groupid, string strUserID, string stEmailSeq, string sFileName, string filekey, string fileseq)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendEmailDownload(hsNetWork, groupid, strUserID, stEmailSeq, sFileName, filekey, fileseq);
            return -1;
        }

        public void SendFilePrevCancel()
        {
        }
        public int SendClipboard(int groupid, string strUserID, int TotalCount, int CurCount, int DataType, int ClipboardSize, byte[] ClipData)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendClipBoard(hsNetWork, strUserID, TotalCount, CurCount, DataType, ClipboardSize, ClipData);
            return -1;
        }

        public int SendClipboard(int groupid, string str3NetDestSysID, string strUserID, int TotalCount, int CurCount, int DataType, int ClipboardSize, byte[] ClipData)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendClipBoard(hsNetWork, str3NetDestSysID, strUserID, TotalCount, CurCount, DataType, ClipboardSize, ClipData);
            return -1;
        }

        public int SendUrlRedirectionData(int groupid, string strUserID, int nTotalCount, int CurrentCount, int nSubDataType, string strUrlData)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.SendUrlRedirectionData(hsNetWork, groupid, strUserID, nTotalCount, CurrentCount, nSubDataType, strUrlData);
            return -1;
        }

        public int SendAPTAndVirusConfirm(int groupid, string strUserID, string strTransSeq, bool bVirus)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
            {
                if (bVirus)
                    return sgSendData.RequestSendVirusConfirm(hsNetWork, strUserID, strTransSeq);
                else
                    return sgSendData.RequestSendAptConfirm(hsNetWork, strUserID, strTransSeq);
            }
            return -1;
        }

        public int SendFileAddErr(int groupid, string strUserID, string strQuery)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendFileAddErr(hsNetWork, strUserID, strQuery);
            return -1;
        }

        public int SendUseDayFileTransInfo(int groupid, string strUserID, string strQuery)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendUseDayFileTransInfo(hsNetWork, strUserID, strQuery);
            return -1;
        }

        public int SendUseDayClipboardInfo(int groupid, string strUserID, string strQuery)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendUseDayClipboardInfo(hsNetWork, strUserID, strQuery);
            return -1;
        }

        public void SendLogOut(int groupid, string strUserID)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                hsNetWork.logOut();
        }
        public int SendScreenLockClear(int groupid, string strUserID, string strPasswd, string strLoginType)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendScreenLockClear(hsNetWork, strUserID, strPasswd, strLoginType);
            return -1;
        }

        public int SendZipDepthInfo(int groupid, string strUserID, string strQuery)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendZipDepthInfo(hsNetWork, strUserID, strQuery);
            return -1;
        }
        public int SendDashBoardCount(int groupid, string strUserID, string strQuery)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendDashBoardCountQuery(hsNetWork, strUserID, strQuery);
            return -1;
        }

        public int SendDashBoardTransReqCount(int groupid, string strUserID, string strQuery)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendDashBoardTransReqCountQuery(hsNetWork, strUserID, strQuery);
            return -1;
        }
        public int SendDashBoardApprWaitCount(int groupid, string strUserID, string strQuery)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendDashBoardApprWaitCountQuery(hsNetWork, strUserID, strQuery);
            return -1;
        }
        public int SendDashBoardApprConfirmCount(int groupid, string strUserID, string strQuery)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendDashBoardApprConfirmCountQuery(hsNetWork, strUserID, strQuery);
            return -1;
        }
        public int SendDashBoardApprRejectCount(int groupid, string strUserID, string strQuery)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendDashBoardApprRejectCountQuery(hsNetWork, strUserID, strQuery);
            return -1;
        }

        public int SendPasswdChgDay(int groupid, string strUserID, string strQuery)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendPasswdChgDayQuery(hsNetWork, strUserID, strQuery);
            return -1;
        }
        public int SendBoardNotiSearch(int groupid, string strUserID, string strQuery)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendBoardNotiSearch(hsNetWork, strUserID, strQuery);
            return -1;
        }
        public int SendBoardNotiConfirm(int groupid, string strUserID, string strQuery)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendBoardNotiConfirm(hsNetWork, strUserID, strQuery);
            return -1;
        }

        public void SendSVRGPKIRegInfo(int groupid, string strGPKIList)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                sgSendData.RequestSendSVRGPKIRegInfo(hsNetWork, strGPKIList);
        }

        public void SendSVRGPKIRandomKey(int groupid, string strGPKIuID)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                sgSendData.RequestSendSVRGPKIRandom(hsNetWork, strGPKIuID);
        }

        public void SendSVRGPKICert(int groupid, string strUserID, string sessionKey, byte[] byteSignedDataHex)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                sgSendData.RequestSendSVRGPKICert(hsNetWork, strUserID, sessionKey, byteSignedDataHex);
        }

        public void SendSVRGPKIRegChange(int groupid, string strUserID, string strGpkiCn)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                sgSendData.RequestSendSVRGPKIRegChange(hsNetWork, strUserID, strGpkiCn);
        }

        public void Send_PRIVACY_CONTINUE(int groupid, string strUserID, string transSeq, string dlpApprove, string privacyConfirmSeq, string NetType)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                sgSendData.RequestSend_PRIVACY_CONTINUE(hsNetWork, strUserID, transSeq, dlpApprove, privacyConfirmSeq, NetType);
        }

        public void SetPassWord(int groupid, string strNewPassWD)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
            {
                SGData sgData = new SGData();
                sgData.SetSessionKey(hsNetWork.GetSeedKey());
                sgData.SetTagData("NEWPASSWORD", strNewPassWD);
                string strEncNewPassWD = sgData.GetEncTagData("NEWPASSWORD");
                hsNetWork.SetPassWord(strNewPassWD);
            }
            return;
        }

        public void SetCliVersion(string strCliVersion)
        {
            m_strCliVersion = strCliVersion;
        }
        public string GetCliVersion()
        {
            return m_strCliVersion;
        }

        public void SetNetWorkCount(int nNetWorkCount)
        {
            m_nNetWorkCount = nNetWorkCount;
        }
        public int GetNetWorkCount()
        {
            return m_nNetWorkCount;
        }

        public void SetFileRecvPossible(int groupid, bool bFileRecvPossible)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
            {
                hsNetWork.SetFileRecvPossible(bFileRecvPossible);
            }
            return;
        }
        public bool GetFileRecvPossible(int groupid)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
            {
                return hsNetWork.GetFileRecvPossible();
            }
            return false;
        }

        public void SetUseUserRecvDownPath(int groupid, bool bUseUserRecvDownPath)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
            {
                hsNetWork.SetUseUserRecvDownPath(bUseUserRecvDownPath);
            }
            return;
        }
        public bool GetUseUserRecvDownPath(int groupid)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
            {
                return hsNetWork.GetUseUserRecvDownPath();
            }
            return false;
        }

        public void SetAllFileRecvPossible(bool bFileRecvPossible)
        {
            int count = GetNetWorkCount();
            for (int i = 0; i < count; i++)
            {
                HsNetWork hsNetWork = null;
                hsNetWork = GetConnectNetWork(i);
                if (hsNetWork != null)
                    hsNetWork.SetFileRecvPossible(bFileRecvPossible);
            }
        }

        public void RecvSvrGPKIAfterSend(int groupId)
        {
            SvrGPKIEvent svGpkiEvent = sgPageEvent.GetSvrGPKIEvent(groupId);
            if (svGpkiEvent != null)
            {
                svGpkiEvent(groupId);
            }
        }


        public void RecvSvrGPKIRandomAfterSend(int groupId)
        {
            // Random
            SvrGPKIRandomKeyEvent svGpkiRandomEvent = sgPageEvent.GetSvrGPKIRandomEvent(groupId);
            if (svGpkiRandomEvent != null)
            {
                svGpkiRandomEvent(groupId);
            }
        }


        public void RecvSvrGPKICertAfterSend(int groupId)
        {
            // Cert
            SvrGPKICertEvent svGpkiCertEvent = sgPageEvent.GetSvrGPKICertEvent(groupId);
            if (svGpkiCertEvent != null)
            {
                svGpkiCertEvent(groupId);
            }
        }


        public void RecvSvrGPKIRegAfterSend(int groupId)
        {
            // Reg
            SvrGPKIRegEvent svGpkiRegEvent = sgPageEvent.GetSvrGPKIRegEvent(groupId);
            if (svGpkiRegEvent != null)
            {
                svGpkiRegEvent(groupId);
            }
        }

        public SGData GetURLlistData(int groupid)
        {
            SGData data = null;
            data = sgDicRecvData.GetUrlListData(groupid);
            return data;
        }

        public int SendUrlData(int groupid, string strUserid, string strUrlData)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendUrl(hsNetWork, groupid, strUserid, 1, 1, 1, strUrlData);

            return -1;
        }

        public SGData GetSFMListData(int groupId)
        {
            SGData data = sgDicRecvData.GetSFMListData(groupId);
            return data;
        }
        public int SendUserSFMInfo(int groupId, string userId)
        {
            SGUserData userData = (SGUserData)sgDicRecvData.GetUserData(groupId);
            string strQuery = ApproveProxy.GetSFMApporverRight(userData.GetUserSequence());
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupId);
            if (hsNetWork != null)
            {
                sgPageEvent.SetQueryReciveEvent(groupId, eCmdList.eSFMIINFOQUERY, SFMInfoAfterSend);
                return sgSendData.RequestCommonSendQuery(hsNetWork, eCmdList.eSFMIINFOQUERY, userId, strQuery);
            }

            return -1;
        }

        public void SFMInfoAfterSend(int groupId, object[] e)
        {
            sgDicRecvData.SetSFMListData(groupId, e[0] as SGData);
        }

        public int CommonSendQuery(eCmdList eCmd, int groupid, string strUserID, string strQuery)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestCommonSendQuery(hsNetWork, eCmd, strUserID, strQuery);
            return -1;
        }
        public int SendGenericNotiType2(int groupid, string strUserID, string strUserName, string strDeptName, string strFileName, string strPreworkType)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.SendGenericNotiType2(hsNetWork, strUserID, strUserName, strDeptName, strFileName, strPreworkType);
            return -1;
        }

    }
}
