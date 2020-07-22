using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;
using HsNetWorkSG;
using HsNetWorkSGData;
using OpenNetLinkApp.Models.SGNetwork;
using OpenNetLinkApp.Services.SGAppManager;
using Serilog.Events;
using OpenNetLinkApp.Models.Data;
using OpenNetLinkApp.Page.Event;
using System.IO;
using System.Text.Json;

namespace OpenNetLinkApp.Page.Event
{
    public class PageEventArgs : EventArgs
    {
        public string strMsg { get; set; }
        public int result { get; set; }
    }
    // 로그인
    public delegate void LoginEvent(int groupid, PageEventArgs e);

    // 전송관리 
    public delegate void TransSearchEvent(int groupid, PageEventArgs e);
    public delegate void TransCancleEvent(int groupid, PageEventArgs e);

    // 전송관리 상세보기
    public delegate void TransDetailCancleEvent(int groupid, PageEventArgs e);

    // 결재관리
    public delegate void ApprSearchEvent(int groupid, PageEventArgs e);
    public delegate void ApprApproveEvent(int groupid, PageEventArgs e);
    public delegate void ApprRejectEvent(int groupid, PageEventArgs e);

    // 결재관리 상세보기
    public delegate void ApprDetailApproveEvent(int groupid, PageEventArgs e);
    public delegate void ApprDetailRejectEvent(int groupid, PageEventArgs e);

}

namespace OpenNetLinkApp.Services
{
    public class HSCmdCenter
    {

        private Dictionary<int, HsNetWork> m_DicNetWork = new Dictionary<int, HsNetWork>();
        public SGDicRecvData sgDicRecvData = new SGDicRecvData();
        public SGSendData sgSendData = new SGSendData();

        public event LoginEvent LoginResult_Event;
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
            for (int i = 0; i < count; i++)
            {
                string strIP = listNetworks[i].IPAddress;
                int port = listNetworks[i].Port;
                int groupID = listNetworks[i].GroupID;
                hsNetwork = new HsNetWork();
                hsNetwork.Init(strIP, port, 0, SslProtocols.Tls);
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

        private void SGDataRecv(int groupId, eCmdList cmd, SGData sgData)
        {
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
                    break;

                case eCmdList.eFILEAPPROVE:                                                  // 결재관리 조회 리스트 데이터 요청 응답.
                    break;

                case eCmdList.eSYSTEMRUNENV:                                                       // 시스템 환경정보 요청에 대한 응답.
                    SystemRunAfterSend(nRet, groupId, sgData);

                    break;

                case eCmdList.eSESSIONCOUNT:                                                  // 사용자가 현재 다른 PC 에 로그인되어 있는지 여부 확인 요청에 대한 응답.
                    break;

                case eCmdList.eAPPROVEDEFAULT:                                                  // 사용자기본결재정보조회 요청 응답.
                    ApprLineAfterSend(nRet, groupId, sgData);
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
                sgDicRecvData.SetLoginData(groupId, sgData);
                SGLoginData sgLoginBind = (SGLoginData)sgDicRecvData.GetLoginData(groupId);
                HsNetWork hs = null;
                if (m_DicNetWork.TryGetValue(groupId, out hs) == true)
                {
                    hs = m_DicNetWork[groupId];
                    Int64 nFilePartSize = sgLoginBind.GetFilePartSize();
                    Int64 nFileBandWidth = sgLoginBind.GetFileBandWidth();
                    bool bDummy = sgLoginBind.GetUseDummyPacket();
                    hs.SetInitInfo(nFilePartSize, nFileBandWidth, bDummy);
                }

                SendUserInfoEx(groupId, sgLoginBind.GetUserID());
            }
            else
            {
                strMsg = SGLoginData.LoginFailMessage(nRet);
                PageEventArgs e = new PageEventArgs();
                e.result = nRet;
                e.strMsg = strMsg;
                LoginResult_Event(groupId, e);
            }
        }

        public void UserInfoAfterSend(int nRet,int groupId,SGData sgData)
        {
            if (nRet == 0)
                sgDicRecvData.SetUserData(groupId, sgData);
            SGLoginData sgLoginUserInfo = (SGLoginData)sgDicRecvData.GetLoginData(groupId);
            SendApproveLine(groupId, sgLoginUserInfo.GetUserID());
        }

        public void SystemRunAfterSend(int nRet, int groupId,SGData sgData)
        {
            if (nRet == 0)
            {
                SGLoginData sgLoginDataSystemRun = (SGLoginData)sgDicRecvData.GetLoginData(groupId);
                sgLoginDataSystemRun.AddRunSystemEnvData(sgData);
                sgDicRecvData.SetLoginData(groupId, sgLoginDataSystemRun);
                /*
                sgLoginDataSystemRun = (SGLoginData)sgDicRecvData.GetLoginData(groupId);
                string strHszDefaultOption = sgLoginDataSystemRun.GetHszDefaultOption();
                int nHszOption = sgLoginDataSystemRun.GetHszDefaultDec();
                int nApproveTypeSFM = sgLoginDataSystemRun.GetApproveTypeSFM();
                string strInterLockEmail = sgLoginDataSystemRun.GetInterLockEmail();
                */

                HsNetWork hs2 = null;
                if (m_DicNetWork.TryGetValue(groupId, out hs2) == true)
                {
                    hs2 = m_DicNetWork[groupId];
                    int hszOpt = sgLoginDataSystemRun.GetHszDefaultDec();
                    hs2.SetHszDefault(hszOpt);
                }
                SendUrlList(groupId, sgLoginDataSystemRun.GetUserID());

                PageEventArgs e = new PageEventArgs();
                e.result = 0;
                e.strMsg = "";
                LoginResult_Event(groupId, e);
            }
        }

        public void ApprLineAfterSend(int nRet, int groupId, SGData sgData)
        {
            if (nRet == 0)
                sgDicRecvData.SetApprLineData(groupId, sgData);
            //SGApprLineData sgApprLineData = (SGApprLineData)sgDicRecvData.GetApprLineData(groupId);
            //List<string> strListName = sgApprLineData.GetApprAndLineName();
            //List<string> strListSeq = sgApprLineData.GetApprAndLineSeq();
            SGUserData sgUserData = (SGUserData)sgDicRecvData.GetUserData(groupId);
            SGLoginData sgLoginDataApproveDefault = (SGLoginData)sgDicRecvData.GetLoginData(groupId);
            string strTeamCode = sgUserData.GetTeamCode();
            string strUserID = sgLoginDataApproveDefault.GetUserID();
            SendInstApprove(groupId, strUserID, strTeamCode);
            SendSystemRunEnv(groupId, strUserID);
        }

        public HsNetWork GetConnectNetWork(int groupid)
        {
            HsNetWork hsTmp = null;
            if (m_DicNetWork.TryGetValue(groupid, out hsTmp) == true)
            {
                return m_DicNetWork[groupid];
            }
            return null;
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
                sgSendData.RequestUserInfoEx(hsNetWork, groupid, strUserID);
            return 0;
        }

        public int SendApproveLine(int groupid, string strUserID)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                sgSendData.RequestApproveLine(hsNetWork, groupid, strUserID);
            return 0;
        }
        public int SendInstApprove(int groupid, string strUserID, string strTeamCode)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                sgSendData.RequestInstApprove(hsNetWork, groupid, strUserID,strTeamCode);
            return 0;
        }

        public int SendSystemEnv(int groupid, string strUserID)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                sgSendData.RequestSystemEnv(hsNetWork, groupid, strUserID);
            return 0;
        }
        public int SendSystemRunEnv(int groupid, string strUserID)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                sgSendData.RequestSystemRunEnv(hsNetWork, groupid, strUserID);
            return 0;
        }

        public int SendUrlList(int groupid, string strUserID)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                sgSendData.RequestUrlList(hsNetWork, groupid, strUserID);
            return 0;
        }

        public int SendChangePasswd(int groupid, string strUserID, string strOldPasswd, string strNewPasswd)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                sgSendData.RequestChangePasswd(hsNetWork, groupid, strUserID, strOldPasswd, strNewPasswd);
            return 0;
        }

        public int SendDeptInfo(int groupid, string strUserID)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                sgSendData.RequestDeptInfo(hsNetWork, groupid, strUserID);
            return 0;
        }

        public int SendFileTransInfo(int groupid, string strUserID, string strFromDate, string strToDate, string strTransKind, string strTransStatus, string strApprStatus, string strDlp, string strTitle, string strDataType)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                sgSendData.RequestFileTransList(hsNetWork, groupid, strUserID,strFromDate, strToDate,strTransKind, strTransStatus, strApprStatus, strDlp,strTitle, strDataType);
            return 0;
        }
        public int SendFileApprInfo(int groupid, string strUserID, string strFromDate, string strToDate, string strApprKind, string strTransKind, string strApprStatus, string strReqUserName, string strDlp, string strTitle, string strDlpApprove, string strApprover, string strDataType) 
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                sgSendData.RequestFileApprList(hsNetWork, groupid, strUserID, strFromDate, strToDate, strApprKind, strTransKind, strApprStatus, strReqUserName, strDlp, strTitle, strDlpApprove, strApprover, strDataType);
            return 0;
        }

        public int SendTransDetail(int groupid, string strUserID,string strTransSeq)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                sgSendData.RequestTransDetail(hsNetWork, groupid, strUserID,strTransSeq);
            return 0;
        }

        public int SendTransDaySize(int groupid, string strUserID)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                sgSendData.RequestTransDaySize(hsNetWork, groupid, strUserID);
            return 0;
        }

        public int SendApproveAlway(int groupid, string strUserID)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                sgSendData.RequestApproveAlway(hsNetWork, groupid, strUserID);
            return 0;
        }

        public int SendApproveBatch(int groupid, string strUserID, string strProcID, string strReason, string strApproveSeqs, string strApprover, string strApproveUserKind)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                sgSendData.RequestApproveBatch(hsNetWork, groupid, strUserID, strProcID, strReason, strApproveSeqs, strApprover, strApproveUserKind);
            return 0;
        }

        public int SendTransCancel(int groupid, string strUserID, string strTransSeq, string strAction, string strReason)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = GetConnectNetWork(groupid);
            if (hsNetWork != null)
                sgSendData.RequestSendCancel(hsNetWork, groupid, strUserID, strTransSeq, strAction, strReason);
            return 0;
        }
    }
}
