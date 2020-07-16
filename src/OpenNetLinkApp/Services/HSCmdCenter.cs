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
    
    public class HSCmdCenter
    {
        private Dictionary<int, HsNetWork> m_DicNetWork = new Dictionary<int, HsNetWork>();
        public SGDicData sgDicData = new SGDicData();
        public HSCmdCenter()
        {
        }

        ~HSCmdCenter()
        {

        }

        public SGData GetSGSvrData(int groupid)
        {
            SGData data = null;
            data = sgDicData.GetSvrData(groupid);
            return data;
        }
        public SGData GetLoginData(int groupid)
        {
            SGData data = null;
            data = sgDicData.GetLoginData(groupid);
            return data;
        }

        public SGData GetUserData(int groupid)
        {
            SGData data = null;
            data = sgDicData.GetUserData(groupid);
            return data;
        }

        private void SGDataRecv(int groupId, int cmd, SGData sgData)
        {
            switch (cmd)
            {
                case 1000:                                                  // SEEDKEY_ACK : seed key 요청 응답
                    break;

                case 1001:                                                  // BIND_ACK : user bind(connect) 인증 응답
                    sgDicData.SetLoginData(groupId, sgData);
                    break;
            }

            return;
        }

        private void SGSvrRecv(int groupId, int cmd, SGData sgData)
        {
            SGData sgSvrData = GetSGSvrData(groupId);
            if (sgSvrData == null)
                sgSvrData = new SGData();

            switch (cmd)
            {
                case 2005:                                                              // usertype, logintype, systemid, tlsversion
                    sgSvrData.m_DicTagData["USERTYPE"] = sgData.m_DicTagData["USERTYPE"];
                    sgSvrData.m_DicTagData["LOGINTYPE"] = sgData.m_DicTagData["LOGINTYPE"];
                    sgSvrData.m_DicTagData["SYSTEMID"] = sgData.m_DicTagData["SYSTEMID"];
                    sgSvrData.m_DicTagData["TLSVERSION"] = sgData.m_DicTagData["TLSVERSION"];
                    break;
                case 2102:                                                              // gpki_cn
                    sgSvrData.m_DicTagData["GPKI_CN"] = sgData.m_DicTagData["GPKI_CN"];
                    break;
                case 2103:                                                              // filemime.conf

                    break;

            }
            sgDicData.SetSvrData(groupId, sgData);
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
    }
}
