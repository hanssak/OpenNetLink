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

namespace OpenNetLinkApp.Services
{
    public class HSCmdCenter
    {

        private Dictionary<int, HsNetWork> m_DicNetWork = new Dictionary<int, HsNetWork>();
        public SGDicRecvData sgDicRecvData = new SGDicRecvData();
        public SGSendData sgSendData = new SGSendData();
        public SGPageEvent sgPageEvent = new SGPageEvent();
        public Dictionary<int, bool> m_DicFileRecving = new Dictionary<int, bool>();

        //public event LoginEvent LoginResult_Event;
        public HSCmdCenter()
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

            int count = listNetworks.Count;
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

                strModulePath = System.IO.Directory.GetCurrentDirectory();
                if (strTlsVer.Equals("1.2"))
                    hsNetwork.Init(hsContype, strIP, port, false, SslProtocols.Tls12, strModulePath, groupID.ToString());    // basedir 정해진 후 설정 필요
                else if(strTlsVer.Equals("1.0"))
                    hsNetwork.Init(hsContype, strIP, port, false, SslProtocols.Tls, strModulePath, groupID.ToString());    // basedir 정해진 후 설정 필요
                else
                    hsNetwork.Init(hsContype, strIP, port, false, SslProtocols.Tls12, strModulePath, groupID.ToString());    // basedir 정해진 후 설정 필요

                hsNetwork.SGSvr_EventReg(SGSvrRecv);
                hsNetwork.SGData_EventReg(SGDataRecv);
                hsNetwork.SetGroupID(groupID);
                m_DicNetWork[groupID] = hsNetwork;
            }
        }

        ~HSCmdCenter()
        {

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
                    break;

                case eCmdList.eDEPTINFO:                                                  // 부서정보 조회 요청 응답.
                    break;

                case eCmdList.eURLLIST:                                                  // URL 자동전환 리스트 요청 응답.
                    // FileMime.conf 요청하는 함수 구현 필요. 추후 개발 
                    break;

                case eCmdList.eUSERINFOEX:                                                  // USERINFOEX : 사용자 정보 응답.
                    UserInfoAfterSend(nRet, groupId, sgData);
                    break;

                case eCmdList.eAPPRINSTCUR:                                                  // 현재 등록된 대결재자 정보 요청 응답.
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
                    if(hs !=null)
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

                case eCmdList.eCLIPBOARDTXT:                                                    // 클립보드 데이터 Recv
                    ClipRecvNotiAfterSend(nRet, groupId, sgData);
                    break;

                case eCmdList.eRMOUSEFILEADD:                                                   // 마우스 우클릭 이벤트 노티
                    RMouseFileAddNotiAfterSend(nRet, groupId);
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
                    tmpData.m_DicTagData["USERTYPE"] = sgData.m_DicTagData["USERTYPE"];
                    tmpData.m_DicTagData["LOGINTYPE"] = sgData.m_DicTagData["LOGINTYPE"];
                    tmpData.m_DicTagData["SYSTEMID"] = sgData.m_DicTagData["SYSTEMID"];
                    tmpData.m_DicTagData["TLSVERSION"] = sgData.m_DicTagData["TLSVERSION"];

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
                    sgDicRecvData.SetLoginData(hs,groupId, sgData);
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

        public void UserInfoAfterSend(int nRet,int groupId,SGData sgData)
        {
            if (nRet == 0)
            {
                HsNetWork hs = null;
                if (m_DicNetWork.TryGetValue(groupId, out hs) == true)
                {
                    hs = m_DicNetWork[groupId];
                    sgDicRecvData.SetUserData(hs,groupId, sgData);
                }
            }
            SGLoginData sgLoginUserInfo = (SGLoginData)sgDicRecvData.GetLoginData(groupId);
            SendApproveLine(groupId, sgLoginUserInfo.GetUserID());
        }

        public void SystemRunAfterSend(int nRet, int groupId,SGData sgData)
        {
            if (nRet == 0)
            {
                SGLoginData sgLoginDataSystemRun = (SGLoginData)sgDicRecvData.GetLoginData(groupId);
                sgLoginDataSystemRun.AddRunSystemEnvData(sgData);
                HsNetWork hs = null;
                if (m_DicNetWork.TryGetValue(groupId, out hs) == true)
                {
                    sgDicRecvData.SetLoginData(hs,groupId, sgLoginDataSystemRun);
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
                    sgDicRecvData.SetApprLineData(hs,groupId, sgData);
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

        public void ApprSearchCountAfterSend(int nRet, int groupId,int count)
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
                    if(strProcID.Equals("A"))                                                       // 승인 
                        strMsg = SGApprManageData.ReturnMessage(eApprManageMsg.eApprBatchActionSuccess);
                    else if (strProcID.Equals("R"))                                                       // 반려 
                        strMsg = SGApprManageData.ReturnMessage(eApprManageMsg.eApprBatchRejectSuccess);
                    else
                        strMsg = SGApprManageData.ReturnMessage(eApprManageMsg.eApprBatchActionSuccess);
                }

                e.strMsg = strMsg;
                ApprBatchResult_Event(groupId, e);
            }
        }

        public void DetailSearchAfterSend(int nRet, int groupId, string strTransSeq)
        {
            DetailSearchEvent DetailSearchResult_Event = sgPageEvent.GetDetailSearchEvent(groupId);
            if(DetailSearchResult_Event != null)
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
            if(DeptApprLineSearchResult_Event!=null)
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

        public void ClipRecvNotiAfterSend(int nRet, int groupId, SGData data)
        {
            RecvClipEvent recvClip_Event = sgPageEvent.GetRecvClipEvent(groupId);
            if(recvClip_Event!=null)
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
            AddFileRMEvent addFileRM_Event = sgPageEvent.GetAddFileRMEvent(groupId);
            if (addFileRM_Event != null)
            {
                string strRMouseFilePath = "";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    var pathWithEnv = @"%USERPROFILE%\AppData\LocalLow\HANSSAK\RList\RList.txt";
                    strRMouseFilePath = Environment.ExpandEnvironmentVariables(pathWithEnv);
                }
                else
                {
                    // 윈도우를 제외한 다른 환경에서 경로 설정 로직 필요
                    strRMouseFilePath = "/var/tmp/sgateContext.info";
                }

                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                e.strMsg = strRMouseFilePath;

                FileAddManage fileAddManage = new FileAddManage();
                groupId = fileAddManage.LoadRMFileGroupID(strRMouseFilePath);
                addFileRM_Event(groupId, e);
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

        public void SetFileRecving(int groupid,bool bRecving)
        {
            bool bTemp = false;
            if (m_DicFileRecving.TryGetValue(groupid, out bTemp) == true)
            {
                m_DicFileRecving.Remove(groupid);
            }
            m_DicFileRecving[groupid] = bRecving;
        }
        public HsNetWork GetConnectNetWork(int groupid)
        {
            HsNetWork hs = null;
            if (!m_DicNetWork.TryGetValue(groupid, out hs))
                return null;
            hs = m_DicNetWork[groupid];
            return hs;
        }

        public int Login(int groupid, string strID, string strPW)
        {
            HsNetWork hsNetWork = GetConnectNetWork(groupid);
            int ret = 0;
            if (hsNetWork != null)
                ret = hsNetWork.Login(strID, strPW);
            return 0;
        }
        public int SendUserInfoEx(int groupid, string strUserID)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork!=null)
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
                return sgSendData.RequestInstApprove(hsNetWork, groupid, strUserID,strTeamCode);
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
                return sgSendData.RequestFileTransList(hsNetWork, groupid, strUserID,strFromDate, strToDate,strTransKind, strTransStatus, strApprStatus, strDlp,strTitle, strDataType);
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

        public int SendTransDetail(int groupid, string strUserID,string strTransSeq)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestTransDetail(hsNetWork, groupid, strUserID,strTransSeq);
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
        public int SendTransListQuery(int groupid, string strUserID,string strQuery)
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

        public int SendFileTrans(int groupid, string strUserID, string strMid, string strPolicyFlag, string strTitle, string strContents, bool bApprSendMail, bool bAfterApprove, int nDlp, string strRecvPos, string strZipPasswd, bool bPrivachApprove, string strSecureString, string strDataType,int nApprStep, List<string> ApprLineSeq, List<HsStream> FileList)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendFileTrans(hsNetWork, groupid, strUserID, strMid, strPolicyFlag, strTitle, strContents, bApprSendMail, bAfterApprove, nDlp, strRecvPos, strZipPasswd, bPrivachApprove, strSecureString, strDataType, nApprStep, ApprLineSeq, FileList);
            return -1;
        }

        public int SendClipboard(int groupid, string strUserID, int TotalCount, int CurCount, int DataType,  int ClipboardSize, byte[] ClipData)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                return sgSendData.RequestSendClipBoard(hsNetWork, strUserID, TotalCount, CurCount, DataType,ClipboardSize, ClipData);
            return -1;
        }
    }
}
