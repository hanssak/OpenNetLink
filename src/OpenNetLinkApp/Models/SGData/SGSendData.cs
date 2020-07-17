using System;
using System.Collections.Generic;
using System.Text;
using HsNetWorkSG;

namespace OpenNetLinkApp.Models.Data
{
    public class SGSendData
    {
        public SGSendData()
        {

        }
        ~SGSendData()
        {

        }

        public int RequestUserInfoEx(HsNetWork hsNet, int groupid, string strUserID)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            CmdSendParser sendParser = new CmdSendParser();
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_USERINFOEX", dic);
            hsNet.SendMessage(args);
            return 0;
        }

        public int RequestApproveLine(HsNetWork hsNet,int groupid, string strUserID)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            CmdSendParser sendParser = new CmdSendParser();
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_APPROVELINE", dic);
            hsNet.SendMessage(args);
            return 0;
        }
        public int RequestInstApprove(HsNetWork hsNet,int groupid, string strUserID, string strTeamCode)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["TEAMCODE"] = strTeamCode;
            CmdSendParser sendParser = new CmdSendParser();
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_APPRINSTCUR", dic);
            hsNet.SendMessage(args);
            return 0;
        }

        public int RequestSystemEnv(HsNetWork hsNet,int groupid, string strUserID)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            CmdSendParser sendParser = new CmdSendParser();
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_SYSTEMENV", dic);
            hsNet.SendMessage(args);
            return 0;
        }

        public int RequestUrlList(HsNetWork hsNet, int groupid, string strUserID)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            CmdSendParser sendParser = new CmdSendParser();
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_URLLIST", dic);
            hsNet.SendMessage(args);
            return 0;
        }
    }
}
