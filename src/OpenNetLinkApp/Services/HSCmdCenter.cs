using System;
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

namespace OpenNetLinkApp.Services
{
    public class HSCmdCenter
    {

        private Dictionary<int, HsNetWork> m_DicNetWork = new Dictionary<int, HsNetWork>();
        public SGDicRecvData sgDicRecvData = new SGDicRecvData();
        public SGSendData sgSendData = new SGSendData();
        public SGPageEvent sgPageEvent = new SGPageEvent();
        public Dictionary<int, bool> m_DicFileRecving = new Dictionary<int, bool>();

        public string m_strCliVersion = "";
        public int m_nNetWorkCount = 0;

        //public event LoginEvent LoginResult_Event;
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

            int count = listNetworks.Count;
            SetNetWorkCount(count);
            string strModulePath = "";
            for (int i = 0; i < count; i++)
            {
                string strIP = listNetworks[i].IPAddress;
                int port = listNetworks[i].Port;
                int groupID = listNetworks[i].GroupID;
                int ConnectType = listNetworks[i].ConnectType;

                HsConnectType hsContype = HsConnectType.Direct;
                if (ConnectType == 1)
                    hsContype = HsConnectType.FindServer;

                hsNetwork = new HsNetWork();
                string strTlsVer = listNetworks[i].TlsVersion;

                string strDownPath = RecvDownList[i];
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
                m_DicNetWork[groupID] = hsNetwork;
            }
        }
        public string ConvertRecvDownPath(string DownPath)
        {
            string strDownPath = "";
            if (DownPath.Contains("%USERPATH%"))
            {
                /*
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    string strFullHomePath = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
                    strDownPath = DownPath.Replace("%USERPATH%", strFullHomePath);
                }
                else
                {
                    string strHomeDrive = Environment.GetEnvironmentVariable("HOMEDRIVE");
                    string strHomePath = Environment.GetEnvironmentVariable("HOMEPATH");
                    string strFullHomePath = Path.Combine(strHomeDrive, strHomePath);
                    strDownPath = DownPath.Replace("%USERPATH%", strFullHomePath);
                }
                */
                string strHomeDrive = Environment.GetEnvironmentVariable("HOMEDRIVE");
                string strHomePath = Environment.GetEnvironmentVariable("HOMEPATH");
                string strFullHomePath = strHomeDrive + strHomePath;
                strDownPath = DownPath.Replace("%USERPATH%", strFullHomePath);
            }
            else if (DownPath.Contains("%MODULEPATH%"))
            {
                string strModulePath = System.IO.Directory.GetCurrentDirectory();
                strDownPath = DownPath.Replace("%MODULEPATH%", strModulePath);
            }
            else
                strDownPath = DownPath;
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
        private void SGExceptionRecv(int groupId, SgEventType sgEventType)
        {
            SgEventType sgEType = sgEventType;

            switch (sgEType)
            {
                case SgEventType.SG_SOCKET_TAG_EXCEPTION:                       // 오프라인
                    OffLineAfterSend(groupId);
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
            HsNetWork hs = null;
            int nRet = 0;
            nRet = sgData.GetResult();
            switch (cmd)
            {
                case eCmdList.eSEEDKEY:                                                  // SEEDKEY_ACK : seed key 요청 응답
                    break;

                case eCmdList.eBIND:                                                  // BIND_ACK : user bind(connect) 인증 응답
                    BindAfterSend(nRet, groupId, sgData);
                    break;

                case eCmdList.eCHANGEPASSWD:                                                  // 비밀번호 변경 요청 응답.
                    ChgPassWDAfterSend(nRet, groupId);
                    break;

                case eCmdList.eDEPTINFO:                                                  // 부서정보 조회 요청 응답.
                    break;

                case eCmdList.eURLLIST:                                                  // URL 자동전환 리스트 요청 응답.
                    // FileMime.conf 요청하는 함수 구현 필요. 추후 개발 
                    URLListAfterSend(nRet, groupId, sgData);
                    break;

                case eCmdList.eUSERINFOEX:                                                  // USERINFOEX : 사용자 정보 응답.
                    UserInfoAfterSend(nRet, groupId, sgData);
                    break;

                case eCmdList.eAPPRINSTCUR:                                                  // 현재 등록된 대결재자 정보 요청 응답.
                    ApprInstAfterSend(nRet, groupId, sgData);
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
                    DashBoardCountAfterSend(nRet,groupId,sgData);
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

                default:
                    break;

            }

            return;
        }

        private void SGSvrRecv(int groupId, int cmd, SGData sgData)
        {
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

                    RecvSvrAfterSend(groupId);
                    //SGSvrData sgTmp = (SGSvrData)sgDicRecvData.GetSvrData(0);
                    //eLoginType e = sgTmp.GetLoginType();
                    break;
                case 2102:                                                              // gpki_cn
                    tmpData.m_DicTagData["GPKI_CN"] = sgData.m_DicTagData["GPKI_CN"];
                    break;
                case 2103:                                                              // filemime.conf
                    break;

            }

            sgDicRecvData.SetSvrData(groupId, tmpData);
        }
        public void RecvSvrAfterSend(int groupId)
        {
            SvrEvent svEvent = sgPageEvent.GetSvrEvent(groupId);
            if (svEvent != null)
            {
                svEvent(groupId);
            }
        }
        public void BindAfterSend(int nRet, int groupId, SGData sgData)
        {
            nRet = sgData.GetResult();
            string strMsg = "";
            if (nRet == 0)
            {
                HsNetWork hs = null;
                if (m_DicNetWork.TryGetValue(groupId, out hs) == true)
                {
                    hs = m_DicNetWork[groupId];
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
            if(chgPassWDEvent!=null)
            {
                PageEventArgs args = new PageEventArgs();
                args.result = nRet;
                chgPassWDEvent(groupId, args);
            }
        }
        public void UserInfoAfterSend(int nRet, int groupId, SGData sgData)
        {
            if (nRet == 0)
            {
                HsNetWork hs = null;
                if (m_DicNetWork.TryGetValue(groupId, out hs) == true)
                {
                    hs = m_DicNetWork[groupId];
                    sgDicRecvData.SetUserData(hs, groupId, sgData);
                }
            }
            SGLoginData sgLoginUserInfo = (SGLoginData)sgDicRecvData.GetLoginData(groupId);
            SendApproveLine(groupId, sgLoginUserInfo.GetUserID());
        }

        public void URLListAfterSend(int nRet, int groupId, SGData sgData)
        {
            SGLoginData sgLoginData = null;
            sgLoginData = (SGLoginData)GetLoginData(groupId);
            if (sgLoginData == null)
                return;

            string strUserID = sgLoginData.GetUserID();
            SGQueryExtend sgQuery = new SGQueryExtend();
            string strQuery = sgQuery.GetUnzipCheckDepth();
            SendZipDepthInfo(groupId, strUserID, strQuery);
        }
        public void ApprInstAfterSend(int nRet, int groupId, SGData sgData)
        {
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
                }
                SendUrlList(groupId, sgLoginDataSystemRun.GetUserID());

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
            SendInstApprove(groupId, strUserID, strTeamCode);
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
                if (!strProgress.Equals(""))
                    count = Convert.ToInt32(strProgress);
                e.count = count;

                FileSendProgress_Event(groupId, e);
            }
        }
        public void FileRecvProgressNotiAfterSend(int nRet, int groupId, SGData data)
        {
            FileRecvProgressEvent FileRecvProgress_Event = sgPageEvent.GetFileRecvProgressEvent(groupId);
            if (FileRecvProgress_Event != null)
            {
                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                e.strMsg = data.GetBasicTagData("TRANSSEQ");

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

        public void DashBoardCountAfterSend(int nRet,int groupID, SGData data)
        {
            DashBoardCountEvent dashBoardCount = null;
            dashBoardCount = sgPageEvent.GetDashBoardCountEvent(groupID);
            if(dashBoardCount!=null)
            {
                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                e.strMsg = data.GetBasicTagData("RECORD");
                dashBoardCount(groupID,e);
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
            if(passwdChgDay!=null)
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
            if(boardNotiSearch!=null)
            {
                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                boardNotiSearch(groupID, e);
            }
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
                m_DicFileRecving.Remove(groupid);
            }
            m_DicFileRecving[groupid] = bRecving;
        }

        public int SetDownLoadPath(int groupid,string strDownPath)
        {
            HsNetWork hsNetWork = GetConnectNetWork(groupid);
            int ret = 0;
            if (hsNetWork != null)
                ret = hsNetWork.SetDownLoadPath(strDownPath);
            return ret;
        }
        public string GetDownLoadPath(int groupid)
        {
            string strDownPath = "";
            HsNetWork hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                strDownPath = hsNetWork.GetDownLoadPath();
            return strDownPath;
        }
        public HsNetWork GetConnectNetWork(int groupid)
        {
            HsNetWork hs = null;
            if (!m_DicNetWork.TryGetValue(groupid, out hs))
                return null;
            hs = m_DicNetWork[groupid];
            return hs;
        }

        public int Login(int groupid, string strID, string strPW, string strCurCliVersion)
        {
            HsNetWork hsNetWork = GetConnectNetWork(groupid);
            int ret = 0;
            if (hsNetWork != null)
                ret = hsNetWork.Login(strID, strPW, strCurCliVersion);
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

        public int SendUrlList(int groupid, string strUserID)
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

        public int SendTransCancel(int groupid, string strUserID, string strTransSeq, string strAction, string strReason)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendCancel(hsNetWork, groupid, strUserID, strTransSeq, strAction, strReason);
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

        public int SendFileTrans(int groupid, string strUserID, string strMid, string strPolicyFlag, string strTitle, string strContents, bool bApprSendMail, bool bAfterApprove, int nDlp, string strRecvPos, string strZipPasswd, bool bPrivachApprove, string strSecureString, string strDataType, int nApprStep, List<string> ApprLineSeq, List<HsStream> FileList)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            int nRet = -1;
            if (hsNetWork != null)
                nRet = sgSendData.RequestSendFileTrans(hsNetWork, groupid, strUserID, strMid, strPolicyFlag, strTitle, strContents, bApprSendMail, bAfterApprove, nDlp, strRecvPos, strZipPasswd, bPrivachApprove, strSecureString, strDataType, nApprStep, ApprLineSeq, FileList);

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

        public void SetPassWord(int groupid,string strNewPassWD)
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
    }
}
