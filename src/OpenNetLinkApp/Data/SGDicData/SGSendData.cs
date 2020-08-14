using System;
using System.Collections.Generic;
using System.Text;
using HsNetWorkSG;
using OpenNetLinkApp.Data.SGDicData.SGUnitData;
using System.Threading;
using System.Runtime.InteropServices;

namespace OpenNetLinkApp.Data.SGDicData
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
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_USERINFOEX", dic);
            return hsNet.SendMessage(args);
        }

        public int RequestApproveLine(HsNetWork hsNet,int groupid, string strUserID)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_APPROVELINE", dic);
            return hsNet.SendMessage(args);
        }
        public int RequestInstApprove(HsNetWork hsNet,int groupid, string strUserID, string strTeamCode)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["TEAMCODE"] = strTeamCode;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_APPRINSTCUR", dic);
            return hsNet.SendMessage(args);
        }

        public int RequestSystemEnv(HsNetWork hsNet,int groupid, string strUserID)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_SYSTEMENV", dic);
            return hsNet.SendMessage(args);
        }
        public int RequestSystemRunEnv(HsNetWork hsNet, int groupid, string strUserID)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_SYSTEMRUNENV", dic);
            return hsNet.SendMessage(args);
        }

        public int RequestUrlList(HsNetWork hsNet, int groupid, string strUserID)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_URLLIST", dic);
            return hsNet.SendMessage(args);
        }
        public int RequestChangePasswd(HsNetWork hsNet, int groupid, string strUserID, string strOldPassword, string strNewPassword)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["OLDPASSWORD"] = strOldPassword;
            dic["NEWPASSWORD"] = strNewPassword;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestChangePW("CMD_STR_CHANGEPASSWORD", dic);
            return hsNet.SendMessage(args);
        }
        public int RequestDeptInfo(HsNetWork hsNet, int groupid, string strUserID)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_DEPTINFO", dic);
            return hsNet.SendMessage(args);
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
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_FILE_TRANSLIST", dic);
            return hsNet.SendMessage(args);
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
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_FILE_APPROVE", dic);
            return hsNet.SendMessage(args);
        }

        public int RequestTransDetail(HsNetWork hsNet, int groupid, string strUserID, string strTransSeq)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["TRANSSEQ"] = strTransSeq;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_TRANS_DETAIL", dic);
            return hsNet.SendMessage(args);
        }

        public int RequestTransDaySize(HsNetWork hsNet, int groupid, string strUserID)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_TRANSFERDAYSIZE", dic);
            return hsNet.SendMessage(args);
        }
        public int RequestApproveAlway(HsNetWork hsNet, int groupid, string strUserID)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_APPROVEALWAY", dic);
            return hsNet.SendMessage(args);
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
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_APPROVEBATCH", dic);
            return hsNet.SendMessage(args);
        }

        public int RequestSendCancel(HsNetWork hsNet,int groupid, string strUserID, string strTransSeq, string strAction, string strReason)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["TRANSSEQ"] = strTransSeq;
            dic["ACTION"] = strAction;
            dic["REASON"] = strReason;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_SENDCANCEL", dic);
            return hsNet.SendMessage(args);
        }
        public int RequestSendTransListCountQuery(HsNetWork hsNet, int groupid, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_FILETRANSLISTQUERYCOUNT", dic);
            return hsNet.SendMessage(args);
        }
        public int RequestSendTransListQuery(HsNetWork hsNet, int groupid, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_FILETRANSLISTQUERY", dic);
            return hsNet.SendMessage(args);
        }
        public int RequestSendApprListCountQuery(HsNetWork hsNet, int groupid, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_FILEAPPRLISTQUERYCOUNT", dic);
            return hsNet.SendMessage(args);
        }
        public int RequestSendApprListQuery(HsNetWork hsNet, int groupid, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_FILEAPPRLISTQUERY", dic);
            return hsNet.SendMessage(args);
        }

        public int RequestSendTransDetailQuery(HsNetWork hsNet, int groupid, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_FILETRANSDETAILQUERY", dic);
            return hsNet.SendMessage(args);
        }

        public int RequestSendApprDetailQuery(HsNetWork hsNet, int groupid, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_FILEAPPRDETAILQUERY", dic);
            return hsNet.SendMessage(args);
        }

        public int RequestSendDeptApprLineSearchQuery(HsNetWork hsNet, int groupid, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_DEPTAPPRLINESEARCHQUERY", dic);
            return hsNet.SendMessage(args);
        }
        public int RequestSendFileTrans(HsNetWork hsNet, int groupid, string strUserID, string strMid, string strPolicyFlag, string strTitle, string strContents, bool bApprSendMail, bool bAfterApprove, int nDlp, string strRecvPos, string strZipPasswd, bool bPrivachApprove, string strSecureString, string strDataType, int nApprStep, List<string> ApprLineSeq, List<HsStream> FileList)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["MID"] = strMid;
            dic["POLICYFLAG"] = strPolicyFlag;
            dic["TITLE"] = strTitle;
            dic["CONTENT"] = strContents;
            if(bApprSendMail)
                dic["EMAIL"] = "Y";
            else
                dic["EMAIL"] = "N";
            if (bAfterApprove)
                dic["APPROVEKIND"] = "1";
            else
                dic["APPROVEKIND"] = "0";

            dic["DLP"] = nDlp.ToString();

            dic["FILEKEY"] = "-";
            dic["FILEMD5"] = "-";
            dic["FILESIZE"] = "-";
            dic["FILEDATE"] = "-";

            if (ApprLineSeq == null)
                dic["CONFIRMID"] = "";
            else
            {
                string strApprLine = "";
                int nApprCount = ApprLineSeq.Count;
                if (nApprCount <= 0)
                    dic["CONFIRMID"] = "";
                else
                {
                    char Sep = (char)'\u0001';
                    if (nApprStep == 0)
                    {
                        for (int i = 0; i < nApprCount; i++)
                        {
                            strApprLine += ApprLineSeq[i];
                            strApprLine += Sep;
                        }
                    }
                    else if (nApprStep == 1)
                    {

                    }
                    else if (nApprStep == 2)
                    {

                    }
                    else
                        strApprLine = "";

                    dic["CONFIRMID"] = strApprLine;
                }
            }

            dic["RECVPOS"] = strRecvPos;
            dic["ZIPPASSWD"] = strZipPasswd;

            if (bPrivachApprove)
                dic["PRIVACYAPPROVE"] = "1";
            else
                dic["PRIVACYAPPROVE"] = "0";

            dic["SECURESTRING"] = strSecureString;
            dic["FILECOUNT"] = "-";
            dic["FILERECORD"] = "-";
            dic["FORWARDUSERID"] = "";
            dic["DATATYPE"] = strDataType;

            

            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_TRANSREQ", dic);
            CancellationToken ct = new CancellationToken();
            return hsNet.SendMessage(args,FileList, ct, null);
        }
    }
}
