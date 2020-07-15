using System;
using System.Collections.Generic;
using System.Text;
using HsNetWorkSG;
using OpenNetLinkApp.Models.SGData;
using OpenNetLinkApp.Models.SGNetwork;
using OpenNetLinkApp.Services.SGAppManager;

namespace OpenNetLinkApp.Services
{
    
    public class HSCmdCenter
    {
        public event SGSvrEvent CmdSvr_Event;
        public event SGDataEvent CmdCenter_Event;
        private Dictionary<int, HsNetWork> m_DicNetWork = new Dictionary<int, HsNetWork>();
        public Dictionary<int, SGSvrData> m_DicSvrData = new Dictionary<int, SGSvrData>();
        public SGSvrData sgData = new SGSvrData();
        public HSCmdCenter()
        {
        }

        ~HSCmdCenter()
        {

        }

        public SGSvrData GetSGSvrData(int groupid)
        {
            return m_DicSvrData[groupid];
        }

        private void HSCmdCenter_SGDataEvent(int groupId, SGData.SGDicData sgData)
        {
        }

        private void HSCmdCenter_SGSvrEvent(int groupId, int cmd, SGData.SGDicData sgData)
        {
            SGSvrData sgSvrData = GetSGSvrData(groupId);
            if (sgSvrData == null)
                sgSvrData = new SGSvrData();

            switch (cmd)
            {
                case 2005:                                                              // usertype, logintype, systemid, tlsversion
                    sgSvrData.m_Data["USERTYPE"] = sgData.m_DicTagData["USERTYPE"];
                    sgSvrData.m_Data["LOGINTYPE"] = sgData.m_DicTagData["LOGINTYPE"];
                    sgSvrData.m_Data["SYSTEMID"] = sgData.m_DicTagData["SYSTEMID"];
                    sgSvrData.m_Data["TLSVERSION"] = sgData.m_DicTagData["TLSVERSION"];
                    break;
                case 2102:                                                              // gpki_cn
                    sgSvrData.m_Data["GPKI_CN"] = sgData.m_DicTagData["GPKI_CN"];
                    break;
                case 2103:                                                              // filemime.conf

                    break;

            }
            
        }

        public HsNetWork ConnectNetWork(int groupid)
        {
            HsNetWork hsNetwork = null;
            hsNetwork = m_DicNetWork[groupid];
            if (hsNetwork == null)
            {
                hsNetwork = new HsNetWork();
                m_DicNetWork[groupid] = hsNetwork;
            }

            return hsNetwork;
        }

        public int Login(int groupid, string strID, string strPW)
        {
            HsNetWork hsNetwork = ConnectNetWork(groupid);
            hsNetwork.SetGroupID(groupid);
            hsNetwork.Login(strID, strPW);
            return 0;
        }
    }
}
