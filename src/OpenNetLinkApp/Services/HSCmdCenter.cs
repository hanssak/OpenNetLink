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

namespace OpenNetLinkApp.Services
{
    public class PageEventArgs : EventArgs
    {
        public string strMsg { get; set; }
    }
    public class HSCmdCenter
    {
        public delegate void LoginEvent(int groupid, PageEventArgs e);
        public event LoginEvent ELogin;

        private Dictionary<int, HsNetWork> m_DicNetWork = new Dictionary<int, HsNetWork>();
        public SGDicRecvData sgDicRecvData = new SGDicRecvData();
        public SGSendData sgSendData = new SGSendData();
        public HSCmdCenter()
        {
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

        private void SGDataRecv(int groupId, int cmd, SGData sgData)
        {
            int nRet = 0;
            nRet = sgData.GetResult();
            switch (cmd)
            {
                case 1000:                                                  // SEEDKEY_ACK : seed key 요청 응답
                    break;

                case 1001:                                                  // BIND_ACK : user bind(connect) 인증 응답
                    nRet = sgData.GetResult();
                    if (nRet == 0)
                    {
                        sgDicRecvData.SetLoginData(groupId, sgData);
                        SendUserInfoEx(groupId, sgData.GetUserID());
                        //ReceiveLogin(sender)
                    }
                    else
                    {
                        string strMsg = SGLoginData.LoginFailMessage(nRet);
                        //MessageBox.Show(strMsg);
                    }
                    break;

                case 1028:                                                  // URL 자동전환 리스트 요청 응답.
                    // FileMime.conf 요청하는 함수 구현 필요. 추후 개발 
                    break;

                case 1030:                                                  // USERINFOEX : 사용자 정보 응답.
                    if(nRet==0)
                        sgDicRecvData.SetUserData(groupId, sgData);
                    SendApproveLine(groupId, sgData.GetUserID());
                    break;

                case 1034:                                                  // 현재 등록된 대결재자 정보 요청 응답.
                    SendSystemEnv(groupId, sgData.GetUserID());
                    break;

                case 1062:                                                  // 시스템 환경정보 요청에 대한 응답.
                    SendUrlList(groupId, sgData.GetUserID());
                    break;

                case 1075:                                                  // 사용자기본결재정보조회 요청 응답.
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

        public HsNetWork ConnectNetWork(int groupid)
        {
            HsNetWork hsNetwork = null;
            hsNetwork = new HsNetWork();
            hsNetwork.Init("172.16.4.204", 3435, 0, SslProtocols.Tls);
            hsNetwork.SGData_EventReg(SGDataRecv);
            //hsNetwork.Init("172.16.4.204", 3435, 0, SslProtocols.Tls12);
            //hsNetwork.Init("172.16.4.206", 3435, 0, SslProtocols.Tls12);
            return hsNetwork;
        }

        public int Login(int groupid, string strID, string strPW)
        {
            HsNetWork hsNetwork = ConnectNetWork(groupid);
            hsNetwork.SetGroupID(groupid);
            int ret = hsNetwork.Login(strID, strPW);
            if(ret==0)
            {
                m_DicNetWork.Remove(groupid);
                m_DicNetWork[groupid] = hsNetwork;
            }
            return 0;
        }
        public int SendUserInfoEx(int groupid, string strUserID)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = m_DicNetWork[groupid];
            sgSendData.RequestUserInfoEx(hsNetWork, groupid, strUserID);
            return 0;
        }

        public int SendApproveLine(int groupid, string strUserID)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = m_DicNetWork[groupid];
            sgSendData.RequestApproveLine(hsNetWork, groupid, strUserID);
            return 0;
        }
        public int SendInstApprove(int groupid, string strUserID, string strTeamCode)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = m_DicNetWork[groupid];
            sgSendData.RequestInstApprove(hsNetWork, groupid, strUserID,strTeamCode);
            return 0;
        }

        public int SendSystemEnv(int groupid, string strUserID)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = m_DicNetWork[groupid];
            sgSendData.RequestSystemEnv(hsNetWork, groupid, strUserID);
            return 0;
        }

        public int SendUrlList(int groupid, string strUserID)
        {
            HsNetWork hsNetWork = null;
            hsNetWork = m_DicNetWork[groupid];
            sgSendData.RequestUrlList(hsNetWork, groupid, strUserID);
            return 0;
        }

    }
}
