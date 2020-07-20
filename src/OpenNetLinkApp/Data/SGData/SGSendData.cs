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
        public int RequestChangePasswd(HsNetWork hsNet, int groupid, string strUserID, string strOldPassword, string strNewPassword)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["OLDPASSWORD"] = strOldPassword;
            dic["NEWPASSWORD"] = strNewPassword;
            CmdSendParser sendParser = new CmdSendParser();
            SGEventArgs args = sendParser.RequestChangePW("CMD_STR_CHANGEPASSWORD", dic);
            hsNet.SendMessage(args);
            return 0;
        }
        public int RequestDeptInfo(HsNetWork hsNet, int groupid, string strUserID)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            CmdSendParser sendParser = new CmdSendParser();
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_DEPTINFO", dic);
            hsNet.SendMessage(args);
            return 0;
        }

        public int RequestFileTransList(HsNetWork hsNet, int groupid, string strUserID,string strFromDate, string strToDate, string strTransKind, string strTransStatus, string strApprStatus, string strDlp, string strTitle, string strDataType)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["FROMDATE"] = strFromDate;
            dic["TODATE"] = strToDate;
            dic["TRANSKIND"] = strTransKind;
            dic["TRANSSTATUS"] = strTransStatus;
            dic["APPROVESTATUS"] = strApprStatus;
            dic["DLP"] = strDlp;
            dic["TITLE"] = strTitle;
            dic["DATATYPE"] = strDataType;
            CmdSendParser sendParser = new CmdSendParser();
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_FILE_TRANSLIST", dic);
            hsNet.SendMessage(args);
            return 0;
        }

        public int RequestFileApprList(HsNetWork hsNet, int groupid, string strUserID, string strFromDate, string strToDate, string strApprKind, string strTransKind, string strApproveStatus, string strReqUserName, string strDlp, string strTitle, string strDlpApprove, string strApprover,string strDataType) 
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["FROMDATE"] = strFromDate;
            dic["TODATE"] = strToDate;
            dic["APPROVKIND"] = strApprKind;
            dic["TRANSKIND"] = strTransKind;
            dic["APPROVESTATUS"] = strApproveStatus;
            dic["REQUSERNAME"] = strReqUserName;
            dic["DLP"] = strDlp;
            dic["TITLE"] = strTitle;
            dic["DLPAPPROVE"] = strDlpApprove;
            dic["APPROVER"] = strApprover;
            dic["DATATYPE"] = strDataType;
            CmdSendParser sendParser = new CmdSendParser();
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_FILE_APPROVE", dic);
            hsNet.SendMessage(args);
            return 0;
        }

        public int RequestTransDetail(HsNetWork hsNet, int groupid, string strUserID, string strTransSeq)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["TRANSSEQ"] = strTransSeq;
            CmdSendParser sendParser = new CmdSendParser();
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_TRANS_DETAIL", dic);
            hsNet.SendMessage(args);
            return 0;
        }

        public int RequestTransDaySize(HsNetWork hsNet, int groupid, string strUserID)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            CmdSendParser sendParser = new CmdSendParser();
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_TRANSFERDAYSIZE", dic);
            hsNet.SendMessage(args);
            return 0;
        }
        public int RequestApproveAlway(HsNetWork hsNet, int groupid, string strUserID)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            CmdSendParser sendParser = new CmdSendParser();
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_APPROVEALWAY", dic);
            hsNet.SendMessage(args);
            return 0;
        }
        public int RequestApproveBatch(HsNetWork hsNet, int groupid, string strUserID, string strProcID, string strReason, string strApproveSeqs, string strApprover, string strApproveUserKind)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["PROCID"] = strProcID;
            dic["REASON"] = strReason;
            dic["APPROVESEQS"] = strApproveSeqs;
            dic["APPROVER"] = strApprover;
            dic["APPROVEUSERKIND"] = strApproveUserKind;
            CmdSendParser sendParser = new CmdSendParser();
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_APPROVEBATCH", dic);
            hsNet.SendMessage(args);
            return 0;
        }
    }
}
