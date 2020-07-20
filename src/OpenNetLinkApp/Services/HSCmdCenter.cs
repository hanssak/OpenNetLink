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

        private void SGDataRecv(int groupId, eCmdList cmd, SGData sgData)
        {
            int nRet = 0;
            nRet = sgData.GetResult();
            switch (cmd)
            {
                case eCmdList.eSEEDKEY:                                                  // SEEDKEY_ACK : seed key 요청 응답
                    break;

                case eCmdList.eBIND:                                                  // BIND_ACK : user bind(connect) 인증 응답
                    nRet = sgData.GetResult();
                    string strMsg = "";
                    if (nRet == 0)
                    {
                        sgDicRecvData.SetLoginData(groupId, sgData);
                        //SendUserInfoEx(groupId, sgData.GetUserID());
                    }
                    else
                    {
                        strMsg = SGLoginData.LoginFailMessage(nRet);
                    }
                    PageEventArgs e = new PageEventArgs();
                    e.result = nRet;
                    e.strMsg = strMsg;
                    LoginResult_Event(groupId, e);
                    break;

                case eCmdList.eCHANGEPASSWD:                                                  // 비밀번호 변경 요청 응답.
                    break;

                case eCmdList.eDEPTINFO:                                                  // 부서정보 조회 요청 응답.
                    break;

                case eCmdList.eURLLIST:                                                  // URL 자동전환 리스트 요청 응답.
                    // FileMime.conf 요청하는 함수 구현 필요. 추후 개발 
                    break;

                case eCmdList.eUSERINFOEX:                                                  // USERINFOEX : 사용자 정보 응답.
                    if(nRet==0)
                        sgDicRecvData.SetUserData(groupId, sgData);
                    SendApproveLine(groupId, sgData.GetUserID());
                    break;

                case eCmdList.eAPPRINSTCUR:                                                  // 현재 등록된 대결재자 정보 요청 응답.
                    SendSystemEnv(groupId, sgData.GetUserID());
                    break;

                case eCmdList.eFILETRANSLIST:                                                  // 전송관리 조회 리스트 데이터 요청 응답.
                    break;

                case eCmdList.eFILEAPPROVE:                                                  // 결재관리 조회 리스트 데이터 요청 응답.
                    break;

                case eCmdList.eSYSTEMRUNENV:                                                  // 시스템 환경정보 요청에 대한 응답.
                    SendUrlList(groupId, sgData.GetUserID());
                    break;

                case eCmdList.eSESSIONCOUNT:                                                  // 사용자가 현재 다른 PC 에 로그인되어 있는지 여부 확인 요청에 대한 응답.
                    break;

                case eCmdList.eAPPROVEDEFAULT:                                                  // 사용자기본결재정보조회 요청 응답.
                    if (nRet == 0)
                        sgDicRecvData.SetApprLineData(groupId, sgData);
                    SendInstApprove(groupId, sgData.GetUserID(), sgData.GetTeamCode());
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
            if(hsNetWork!=null)
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

    }
}
