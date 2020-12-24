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
        public CancellationToken token;
        CancellationTokenSource src;
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
        public int RequestSendFileTrans(HsNetWork hsNet, int groupid, string strUserID, string strMid, string strPolicyFlag, 
            string strTitle, string strContents, bool bApprSendMail, bool bAfterApprove, int nDlp, string strRecvPos, 
            string strZipPasswd, bool bPrivachApprove, string strSecureString, string strDataType, int nApprStep, 
            List<string> ApprLineSeq, List<HsStream> FileList, string strNetOver3info)
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
                    char Sep = (char)'\u0002';
                    if (nApprStep == 0)
                    {
                        // AND 결재
                        for (int i = 0; i < nApprCount; i++)
                        {
                            strApprLine += ApprLineSeq[i];
                            strApprLine += Sep;
                        }
                    }
                    else if (nApprStep == 1)
                    {
                        // OR 결재 구현 필요
                        for (int i = 0; i < nApprCount; i++)
                        {
                            strApprLine += ApprLineSeq[i];
                            strApprLine += Sep;
                        }
                    }
                    else if (nApprStep == 2)
                    {
                        // ANDOR 결재 구현 필요
                        for (int i = 0; i < nApprCount; i++)
                        {
                            strApprLine += ApprLineSeq[i];
                            strApprLine += Sep;
                        }
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
            dic["NETOVERDATA"] = strNetOver3info;
            dic["FILECOUNT"] = "-";
            dic["FILERECORD"] = "-";
            dic["FORWARDUSERID"] = "";
            dic["DATATYPE"] = strDataType;


            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_TRANSREQ", dic);
            src = new CancellationTokenSource();
            token = src.Token;
            return hsNet.SendMessage(args,FileList, token, null);
           // return -2;
        }

        public void RequestSendFileTransCancel()
        {
            src.Cancel();
        }

        public int RequestSendFilePrev(HsNetWork hsNet, int groupid, string strUserID, string strTransSeq, string strFileName, string strFileKey, string strFileSeq,string strOrgData)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["TRANSSEQ"] = strTransSeq;
            dic["FILENAME"] = strFileName;
            dic["FILEKEY"] = strFileKey;
            dic["FILESEQ"] = strFileSeq;
            dic["ORGDATA"] = strOrgData;

            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_FILEPREVIEW", dic);
            return hsNet.SendMessage(args);
        }

        public void RequestSendFilePrevCancel()
        {
        }
        public int RequestSendClipBoard(HsNetWork hsNet, string strUserID,int TotalCount, int CurCount, int DataType, int ClipboardSize, byte[] ClipData)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["TOTALCOUNT"] = TotalCount.ToString();
            dic["CURRENTCOUNT"] = CurCount.ToString();
            dic["DATATYPE"] = DataType.ToString();
            dic["CLIPBOARDSIZE"] = ClipboardSize.ToString();
            dic["CLIPBOARDDATA"] = "-";

            
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_CLIPBOARDTXT", dic);
            return hsNet.SendMessageClipBoard(args, ClipData);
            
        }
        public int RequestSendClipBoard(HsNetWork hsNet, string str3NetDestSysID, string strUserID, int TotalCount, int CurCount, int DataType, int ClipboardSize, byte[] ClipData)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["TOTALCOUNT"] = TotalCount.ToString();
            dic["CURRENTCOUNT"] = CurCount.ToString();
            dic["DATATYPE"] = DataType.ToString();
            dic["CLIPBOARDSIZE"] = ClipboardSize.ToString();
            dic["CLIPBOARDDATA"] = "-";
            if (str3NetDestSysID.Length > 0)
                dic["NETOVERDATA"] = str3NetDestSysID;

            // KKW - Clipboard 전송할곳 지정 : str3NetDestSysID

            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_CLIPBOARDTXT", dic);
            return hsNet.SendMessageClipBoard(args, ClipData);

        }

        public int RequestSendAptConfirm(HsNetWork hsNet, string strUserID, string strTransSeq)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["TRANSSEQ"] = strTransSeq;

            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_APT_CONFIRM", dic);
            return hsNet.SendMessage(args);
        }
        public int RequestSendVirusConfirm(HsNetWork hsNet, string strUserID, string strTransSeq)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["TRANSSEQ"] = strTransSeq;

            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_VIRUS_CONFIRM", dic);
            return hsNet.SendMessage(args);
        }

        public int RequestSendFileAddErr(HsNetWork hsNet,string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            //SGEventArgs args = sendParser.RequestCmd("CMD_STR_DATABASEQUERY", dic);
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_FILEADDERROR", dic);
            return hsNet.SendMessage(args);
        }


        public int RequestSendUseDayFileTransInfo(HsNetWork hsNet,string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_USEDAYFILETRANS", dic);
            return hsNet.SendMessage(args);
        }
        public int RequestSendUseDayClipboardInfo(HsNetWork hsNet, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_USEDAYCLIPTRANS", dic);
            return hsNet.SendMessage(args);
        }

        public int RequestSendLogOut(HsNetWork hsNet, string strUserID)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["REASON"] = "LOGOUT";
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_LOGOUT", dic);
            return hsNet.SendMessage(args);
        }
        public int RequestSendScreenLockClear(HsNetWork hsNet, string strUserID, string strPasswd,string strLoginType)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["PASSWORD"] = strPasswd;
            dic["LOGINTYPE"] = strLoginType;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestClientUnlock("CMD_STR_CLIENTUNLOCK", dic);
            return hsNet.SendMessage(args);
        }

        public int RequestSendZipDepthInfo(HsNetWork hsNet, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_ZIPDEPTHINFO", dic);
            return hsNet.SendMessage(args);
        }
        public int RequestSendChangePassWD(HsNetWork hsNet, string strUserID, string strOldPassWD, string strNewPassWD)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["OLDPASSWORD"] = strOldPassWD;
            dic["NEWPASSWORD"] = strNewPassWD;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestChangePW("CMD_STR_CHANGEPASSWORD", dic);
            return hsNet.SendMessage(args);
        }

        public int RequestSendDashBoardCountQuery(HsNetWork hsNet,string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_DASHBOARDCOUNT", dic);
            return hsNet.SendMessage(args);
        }

        public int RequestSendDashBoardTransReqCountQuery(HsNetWork hsNet, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_DASHBOARDTRANSREQCOUNT", dic);
            return hsNet.SendMessage(args);
        }

        public int RequestSendDashBoardApprWaitCountQuery(HsNetWork hsNet, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_DASHBOARDAPPRWAITCOUNT", dic);
            return hsNet.SendMessage(args);
        }
        public int RequestSendDashBoardApprConfirmCountQuery(HsNetWork hsNet, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_DASHBOARDAPPRCONFIRMCOUNT", dic);
            return hsNet.SendMessage(args);
        }

        public int RequestSendDashBoardApprRejectCountQuery(HsNetWork hsNet, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_DASHBOARDAPPRREJECTCOUNT", dic);
            return hsNet.SendMessage(args);
        }

        public int RequestSendPasswdChgDayQuery(HsNetWork hsNet, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_PASSWDCHGDAY", dic);
            return hsNet.SendMessage(args);
        }
        public int RequestSendBoardNotiSearch(HsNetWork hsNet, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_BOARDNOTIFYSEARCH", dic);
            return hsNet.SendMessage(args);
        }
        public int RequestSendBoardNotiConfirm(HsNetWork hsNet, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_BOARDNOTIFYCONFIRM", dic);
            return hsNet.SendMessage(args);
        }
        public void RequestSendSVRGPKIRegInfo(HsNetWork hsNet,string strGPKIList)
        {
            hsNet.getgpki(strGPKIList);
        }

        public void RequestSendSVRGPKIRandom(HsNetWork hsNet, string strUserID)
        {
            /*Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;

            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey()); // 통신단에서 seedkey 받아서 처리
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_GPKIRANDOM", dic);*/

            hsNet.Gpki_Random(strUserID);
        }

        public int RequestSendSVRGPKICert(HsNetWork hsNet, string strUserID, string sessionKey, byte[] byteSignedDataHex)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;    // 통신단에서 Utf8로 변환해서 전송해야됨
            dic["SESSIONKEY"] = sessionKey;
            dic["SIGNLEN"] = byteSignedDataHex.Length.ToString();
            dic["SIGNDATA"] = byteSignedDataHex.ByteToBase64String();   // 정각과장과 협의

            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_GPKICERT", dic);
            return hsNet.SendMessage(args);
        }

        public int RequestSendSVRGPKIRegChange(HsNetWork hsNet, string strUserID, string strGpkiCN)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["GPKI_CN"] = strGpkiCN;

            CmdSendParser sendParser = new CmdSendParser();
            sendParser.SetSessionKey(hsNet.GetSeedKey());
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_CHANGEGPKI_CN", dic);
            return hsNet.SendMessage(args);
        }


    }
}
