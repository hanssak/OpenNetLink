using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using HsNetWorkSG;
using OpenNetLinkApp.Data.SGDicData.SGUnitData;
using System.Threading;
using System.Runtime.InteropServices;
using OpenNetLinkApp.Data.SGNotify;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using static HsNetWorkSG.SGEnums;

namespace OpenNetLinkApp.Data.SGDicData
{
    /// <summary>
    /// 필요한 네트워크 요청을 위한 명령어 파라미터 생성 함수 모음
    /// </summary>
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

        CmdSendParser sendParser = new CmdSendParser();

        public int RequestSendEmailCancel(HsNetWork hsNet, int groupid, string strUserID, string emailSeq)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["EMAILSEQ"] = emailSeq;
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_EMAIL_SEND_CANCEL", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int RequestUserInfoEx(HsNetWork hsNet, int groupid, string strUserID)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_USERINFOEX", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int RequestUserInfoCheck(HsNetWork hsNet, int groupid, string strUserID)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_USERINFOCHECK", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int RequestApproveLine(HsNetWork hsNet, int groupid, string strUserID)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_APPROVELINE", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        /// <summary>
        /// 대결재 정보 요청(이 함수를 사용하면 한명의 대결재 정보만 리턴)
        /// </summary>
        /// <param name="hsNet"></param>
        /// <param name="groupid"></param>
        /// <param name="strUserID"></param>
        /// <param name="strTeamCode"></param>
        /// <returns></returns>
        public int RequestInstApprove(HsNetWork hsNet, int groupid, string strUserID, string strTeamCode)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["TEAMCODE"] = strTeamCode;
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_APPRINSTCUR", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }
        /// <summary>
        /// 사용자의 대결재 등록
        /// </summary>
        /// <param name="hsNet"></param>
        /// <param name="strUserID"></param>
        /// <param name="strQuery"></param>
        /// <returns></returns>
        public int RequestInstApproveReg(HsNetWork hsNet, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_DATABASEQUERY", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }
        public int RequestInstAgentHashLog(HsNetWork hsNet, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_DATABASEQUERY", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }
        public int RequestInstApproveClear(HsNetWork hsNet, string strUserID, string appruserId)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["APPR_USERID"] = appruserId;
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_APPRINSTCLEAR", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }
        public int RequestSystemEnv(HsNetWork hsNet, int groupid, string strUserID)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_SYSTEMENV", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }
        public int RequestSystemRunEnv(HsNetWork hsNet, int groupid, string strUserID)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_SYSTEMRUNENV", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int RequestUrlList(HsNetWork hsNet, int groupid, string strUserID)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_URLLIST", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }
        public int RequestChangePasswd(HsNetWork hsNet, int groupid, string strUserID, string strProtectedOldPassword, string strProtectedNewPassword)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["OLDPASSWORD"] = strProtectedOldPassword;
            dic["NEWPASSWORD"] = strProtectedNewPassword;
            SGEventArgs args = sendParser.RequestChangePW("CMD_STR_CHANGEPASSWORD", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int RequestOTPRegist(HsNetWork hsNet, int groupid, string strUserID, string otpNumber)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["OTP"] = otpNumber;
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_OTP", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int RequestDeptInfo(HsNetWork hsNet, int groupid, string strUserID)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_DEPTINFO", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int RequestFileTransList(HsNetWork hsNet, int groupid, string strUserID, string strFromDate, string strToDate, string strTransKind, string strTransStatus, string strApprStatus, string strDlp, string strTitle, string strDataType)
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
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_FILE_TRANSLIST", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int RequestFileApprList(HsNetWork hsNet, int groupid, string strUserID, string strFromDate, string strToDate, string strApprKind, string strTransKind, string strApproveStatus, string strReqUserName, string strDlp, string strTitle, string strDlpApprove, string strApprover, string strDataType)
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
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_FILE_APPROVE", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int RequestTransDetail(HsNetWork hsNet, int groupid, string strUserID, string strTransSeq)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["TRANSSEQ"] = strTransSeq;
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_TRANS_DETAIL", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }
        public int RequestDownloadCount(HsNetWork hsNet, int groupid, string strUserID, string strTransSeq)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["TRANSSEQ"] = strTransSeq;
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_DOWNLOAD_COUNT", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int RequestTransDaySize(HsNetWork hsNet, int groupid, string strUserID)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_TRANSFERDAYSIZE", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }
        public int RequestApproveAlway(HsNetWork hsNet, int groupid, string strUserID)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_APPROVEALWAY", dic, hsNet.stCliMem.GetProtectedSeedKey());
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
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_APPROVEBATCH", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }
        public int RequestEmailApproveBatch(HsNetWork hsNet, int groupid, string strUserID, string strProcID, string strReason, string strApproveSeqs, bool bUseSfm2Approve, bool bUsePrivacyApprove)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["PROCID"] = strProcID;
            dic["REASON"] = strReason;
            dic["EMAILAPPROVESEQS"] = strApproveSeqs;
            dic["APPROVER"] = (bUseSfm2Approve ? "1" : "0");
            dic["APPROVEUSERKIND"] = (bUsePrivacyApprove ? "1" : "0");

            SGEventArgs args = sendParser.RequestCmd("CMD_STR_EMAIL_APPROVE_BATCH", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int RequestSendCancel(HsNetWork hsNet, int groupid, string strUserID, string strTransSeq, string strAction, string strReason)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["TRANSSEQ"] = strTransSeq;
            dic["ACTION"] = strAction;
            dic["REASON"] = strReason;
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_SENDCANCEL", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }
        public int RequestForwardCancel(HsNetWork hsNet, int groupid, string strUserID, string strTransSeq)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["TRANSSEQ"] = strTransSeq;
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_FORWARD_CANCEL", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }
        public int RequestAutoDownload(HsNetWork hsNet, int groupid, string strUserID, string strTransSeq)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["TRANSSEQ"] = strTransSeq;
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_AUTODOWNLOAD", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }
        public int RequestManualDownload(HsNetWork hsNet, int groupid, string strUserID, string strTransSeq)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["TRANSSEQ"] = strTransSeq;
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_MANUALDOWNLOAD", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int RequestEmailPreviewInfo(HsNetWork hsNet, int groupid, string strUserID, string strEmailSeq)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["EMAILSEQ"] = strEmailSeq;
            CmdSendParser sendParser = new CmdSendParser();
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_EMAILPREVIEWINFO", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int RequestSendTransListCountQuery(HsNetWork hsNet, int groupid, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_FILETRANSLISTQUERYCOUNT", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }
        public int RequestSendCountQuery(HsNetWork hsNet, int groupid, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_COUNTQUERY", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }
        public int RequestSendListQuery(HsNetWork hsNet, int groupid, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_LISTQUERY", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }
        public int RequestSendDetailQuery(HsNetWork hsNet, int groupid, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_DETAIL_QUERY", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }
        public int RequestRecordExistCheckQuery(HsNetWork hsNet, int groupid, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_RECORDEXISTCHECK_QUERY", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }
        public int RequestSendTransListQuery(HsNetWork hsNet, int groupid, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_FILETRANSLISTQUERY", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }
        public int RequestSendApprListCountQuery(HsNetWork hsNet, int groupid, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_FILEAPPRLISTQUERYCOUNT", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }
        public int RequestSendApprListQuery(HsNetWork hsNet, int groupid, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_FILEAPPRLISTQUERY", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int RequestSendTransDetailQuery(HsNetWork hsNet, int groupid, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_FILETRANSDETAILQUERY", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int RequestSendApprDetailQuery(HsNetWork hsNet, int groupid, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_FILEAPPRDETAILQUERY", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int RequestSendDeptApprLineSearchQuery(HsNetWork hsNet, int groupid, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_DEPTAPPRLINESEARCHQUERY", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }
        public int RequestSecurityApproverQuery(HsNetWork hsNet, int groupid, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_SECURITY_APPROVER_QUERY", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int RequestSendFileTrans(HsNetWork hsNet, int groupid, string strUserID, string strMid, string strPolicyFlag,
            string strTitle, string strContents, bool bApprSendMail, bool bAfterApprove, int nDlp, string strRecvPos,
            string strZipPasswd, bool bPrivachApprove, string strSecureString, string strDataType, int nApprStep,
            string ApprLineSeq, List<HsStream> FileList, string strNetOver3info, string receiver, string strinterlockflagConfirmId = "")
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["MID"] = strMid;
            dic["POLICYFLAG"] = strPolicyFlag;
            dic["TITLE"] = strTitle;
            dic["CONTENT"] = strContents;
            if (bApprSendMail)
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
                dic["CONFIRMID"] = ApprLineSeq;
            }

            dic["RECVPOS"] = strRecvPos;
            dic["ZIPPASSWD"] = strZipPasswd;

            if (bPrivachApprove)
                dic["PRIVACYAPPROVE"] = "1";
            else
                dic["PRIVACYAPPROVE"] = "0";

            dic["SECURESTRING"] = strSecureString;

            if (strNetOver3info.Length > 0)
                dic["NETOVERDATA"] = "-";       // strNetOver3info

            dic["FILECOUNT"] = "-";
            dic["FILERECORD"] = "-";

            if ((receiver?.Length ?? 0) > 0)
                dic["FORWARDUSERID"] = receiver;

            dic["DATATYPE"] = strDataType;
            if ((strinterlockflagConfirmId?.Length ?? 0) > 0)
            {
                strinterlockflagConfirmId = strinterlockflagConfirmId.Replace('|', '\u0002');
                dic["INTERLOCKFLAGCONFIRMID"] = strinterlockflagConfirmId;
            }
            dic["GROUPID"] = groupid.ToString();
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_TRANSREQ", dic, hsNet.stCliMem.GetProtectedSeedKey());

            // 통신에서 transreq를 보낼때, '/' 문자로 나누어서 망개수 만큼 보냄 
            if (strNetOver3info.Length > 0)
            {
                args.errListParm = new List<string>();

                int nPos = -1;
                nPos = strNetOver3info.IndexOf("/");
                if (nPos < 0)
                    args.errListParm.Add(strNetOver3info);
                else
                {
                    String[] listOneNet = strNetOver3info.Split("/");
                    if (listOneNet.Count() > 1)
                    {
                        int nJdx = 0;
                        for (; nJdx < listOneNet.Count(); nJdx++)
                        {
                            args.errListParm.Add(listOneNet[nJdx]);
                        }
                    }
                }
            }

            src = new CancellationTokenSource();
            token = src.Token;

            return hsNet.SendMessage(args, FileList, token, null);
            // return -2;
        }

        public int RequestContinueSendFileTrans(HsNetWork hsNet, int groupid, Dictionary<string, string> values, string strNetOver3info, string hszFileName, int currentFileSize)
        {
            string FILEMD5 = values["FILEMD5"];
            string FILERECORD = values["FILERECORD"];
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_TRANSREQ", values, hsNet.stCliMem.GetProtectedSeedKey());
            args.MsgRecode["FILEMD5"] = FILEMD5;
            args.MsgRecode["FILERECORD"] = FILERECORD;

            FileStream fileStream = File.OpenRead(hszFileName);

            // 통신에서 transreq를 보낼때, '/' 문자로 나누어서 망개수 만큼 보냄 
            if (strNetOver3info.Length > 0)
            {
                args.errListParm = new List<string>();

                int nPos = -1;
                nPos = strNetOver3info.IndexOf("/");
                if (nPos < 0)
                    args.errListParm.Add(strNetOver3info);
                else
                {
                    String[] listOneNet = strNetOver3info.Split("/");
                    if (listOneNet.Count() > 1)
                    {
                        int nJdx = 0;
                        for (; nJdx < listOneNet.Count(); nJdx++)
                        {
                            args.errListParm.Add(listOneNet[nJdx]);
                        }
                    }
                }
            }

            src = new CancellationTokenSource();
            token = src.Token;

            return hsNet.ContinueSendFile(args, fileStream, token, currentFileSize, null);
            // return -2;
        }

        public int RequestSendFileTrans(HsNetWork hsNet, int groupid, string strUserID, string strMid, string strPolicyFlag,
            string strTitle, string strContents, bool bApprSendMail, bool bAfterApprove, int nDlp, string strRecvPos,
            string strZipPasswd, bool bPrivachApprove, string strSecureString, string strDataType, int nApprStep,
            List<string> ApprLineSeq, List<HsStream> FileList, string strNetOver3info, string receiver, string strinterlockflagConfirmId = "")
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["MID"] = strMid;
            dic["POLICYFLAG"] = strPolicyFlag;
            dic["TITLE"] = strTitle;
            dic["CONTENT"] = strContents;
            if (bApprSendMail)
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

            if (strNetOver3info.Length > 0)
                dic["NETOVERDATA"] = "-";       // strNetOver3info

            dic["FILECOUNT"] = "-";
            dic["FILERECORD"] = "-";

            if ((receiver?.Length ?? 0) > 0)
                dic["FORWARDUSERID"] = receiver;

            dic["DATATYPE"] = strDataType;

            if ((strinterlockflagConfirmId?.Length ?? 0) > 0)
            {
                strinterlockflagConfirmId = strinterlockflagConfirmId.Replace('|', '\u0002');
                dic["INTERLOCKFLAGCONFIRMID"] = strinterlockflagConfirmId;
            }
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_TRANSREQ", dic, hsNet.stCliMem.GetProtectedSeedKey());

            // 통신에서 transreq를 보낼때, '/' 문자로 나누어서 망개수 만큼 보냄 
            if (strNetOver3info.Length > 0)
            {
                args.errListParm = new List<string>();

                int nPos = -1;
                nPos = strNetOver3info.IndexOf("/");
                if (nPos < 0)
                    args.errListParm.Add(strNetOver3info);
                else
                {
                    String[] listOneNet = strNetOver3info.Split("/");
                    if (listOneNet.Count() > 1)
                    {
                        int nJdx = 0;
                        for (; nJdx < listOneNet.Count(); nJdx++)
                        {
                            args.errListParm.Add(listOneNet[nJdx]);
                        }
                    }
                }
            }

            src = new CancellationTokenSource();
            token = src.Token;
            return hsNet.SendMessage(args, FileList, token, null);
            // return -2;
        }

        public void RequestSendFileTransCancel()
        {
            src.Cancel();
        }

        public int RequestSendFilePrev(HsNetWork hsNet, int groupid, string strUserID, string strTransSeq, string strFileName, string strFileKey, string strFileSeq, string strOrgData)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["TRANSSEQ"] = strTransSeq;
            dic["FILENAME"] = strFileName;
            dic["FILEKEY"] = strFileKey;
            dic["FILESEQ"] = strFileSeq;
            dic["ORGDATA"] = strOrgData;

            SGEventArgs args = sendParser.RequestCmd("CMD_STR_FILEPREVIEW", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int RequestSendFileUploadInfo(HsNetWork hsNet, string strUserID, string mid, string totalPart, string totalSize)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["MID"] = mid;
            dic["TOTALPART"] = totalPart;
            dic["TOTALSIZE"] = totalSize;
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_FILEUPLOAD_INFO", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int RequestSendEmailDownload(HsNetWork hsNet, int groupid, string strUserID, string stEmailSeq, string sFileName, string filekey, string fileseq)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["EMAILSEQ"] = stEmailSeq;
            dic["FILENAME"] = sFileName;
            dic["FILEKEY"] = filekey;
            dic["FILESEQ"] = fileseq;
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_EMAIL_FILEDOWNLOAD64", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }
        public void RequestSendFilePrevCancel()
        {
        }
        public int RequestSendClipBoard(HsNetWork hsNet, string strUserID, int TotalCount, int CurCount, int DataType, int ClipboardSize, byte[] ClipData)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["TOTALCOUNT"] = TotalCount.ToString();
            dic["CURRENTCOUNT"] = CurCount.ToString();
            dic["DATATYPE"] = DataType.ToString();
            dic["CLIPBOARDSIZE"] = ClipboardSize.ToString();
            dic["CLIPBOARDDATA"] = "-";
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_CLIPBOARDTXT", dic, hsNet.stCliMem.GetProtectedSeedKey());
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
            if (str3NetDestSysID.Length > 0)
                dic["NETOVERDATA"] = str3NetDestSysID;
            dic["CLIPBOARDDATA"] = "-";

            // Clipboard 전송할곳 지정 : str3NetDestSysID
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_CLIPBOARDTXT", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessageClipBoard(args, ClipData);

        }


        public int SendUrlRedirectionData(HsNetWork hsNet, int groupid, string strUserID, int nTotalCount, int CurrentCount, int nSubDataType, string strUrlData)
        {

            if (strUrlData.Length < 1)
                return -1;

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["TOTALCOUNT"] = nTotalCount.ToString();
            dic["CURRENTCOUNT"] = CurrentCount.ToString();
            dic["SUBDATATYPE"] = nSubDataType.ToString();
            dic["SUBDATASIZE"] = strUrlData.ToString();
            dic["SUBDATA"] = strUrlData;

            // 3망 기능 지원안함
            // RequestCmd 에서 utf8로 인코딩함
            // Encoding.ASCII.GetByteCount(strUrlData);
            // Encoding.ASCII.GetBytes(strUrlData);
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_SUBDATAEXCHANGE", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        /// <summary>
        /// 업데이트 이력 전송
        /// </summary>
        /// <param name="clientInfo"></param>
        /// <param name="getStatus"></param>
        /// <param name="getClientIP"></param>
        /// <param name="getReason"></param>
        public int SendPatchHistory(HsNetWork hsNet, int getStatus, string getClientIP, string getReason)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["STATUS"] = getStatus.ToString();
            dic["CLIENTIP"] = getClientIP;
            dic["REASON"] = getReason;
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_AUDITORI", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }


        public int RequestSendAptConfirm(HsNetWork hsNet, string strUserID, string strTransSeq)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["TRANSSEQ"] = strTransSeq;
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_APT_CONFIRM", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int RequestSendVirusConfirm(HsNetWork hsNet, string strUserID, string strTransSeq)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["TRANSSEQ"] = strTransSeq;

            SGEventArgs args = sendParser.RequestCmd("CMD_STR_VIRUS_CONFIRM", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int RequestSendFileAddErr(HsNetWork hsNet, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            //SGEventArgs args = sendParser.RequestCmd("CMD_STR_DATABASEQUERY", dic);
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_FILEADDERROR", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }


        public int RequestSendUseDayFileTransInfo(HsNetWork hsNet, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_USEDAYFILETRANS", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }
        public int RequestSendUseDayClipboardInfo(HsNetWork hsNet, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_USEDAYCLIPTRANS", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int RequestSendLogOut(HsNetWork hsNet, string strUserID)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["REASON"] = "LOGOUT";
            SGEventArgs args = sendParser.RequestCmd("CMD_STR_LOGOUT", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }
        public int RequestSendScreenLockClear(HsNetWork hsNet, string strUserID, string strProtectedPasswd, string strLoginType)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["PASSWORD"] = strProtectedPasswd;
            dic["LOGINTYPE"] = strLoginType;
            SGEventArgs args = sendParser.RequestClientUnlock("CMD_STR_CLIENTUNLOCK", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int RequestSendZipDepthInfo(HsNetWork hsNet, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_ZIPDEPTHINFO", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }
        //public int RequestSendChangePassWD(HsNetWork hsNet, string strUserID, string strOldPassWD, string strNewPassWD)
        //{
        //    Dictionary<string, string> dic = new Dictionary<string, string>();
        //    dic["APPID"] = "0x00000000";
        //    dic["CLIENTID"] = strUserID;
        //    dic["OLDPASSWORD"] = strOldPassWD;
        //    dic["NEWPASSWORD"] = strNewPassWD;
        //    SGEventArgs args = sendParser.RequestChangePW("CMD_STR_CHANGEPASSWORD", dic, hsNet.stCliMem.GetProtectedSeedKey());
        //    return hsNet.SendMessage(args);
        //}

        public int RequestSendDashBoardCountQuery(HsNetWork hsNet, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_DASHBOARDCOUNT", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int RequestSendDashBoardTransReqCountQuery(HsNetWork hsNet, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_DASHBOARDTRANSREQCOUNT", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int RequestSendDashBoardApprWaitCountQuery(HsNetWork hsNet, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_DASHBOARDAPPRWAITCOUNT", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }
        public int RequestSendDashBoardApprConfirmCountQuery(HsNetWork hsNet, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_DASHBOARDAPPRCONFIRMCOUNT", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int RequestSendDashBoardApprRejectCountQuery(HsNetWork hsNet, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_DASHBOARDAPPRREJECTCOUNT", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int RequestSendPasswdChgDayQuery(HsNetWork hsNet, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_PASSWDCHGDAY", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }
        public int RequestSendBoardNotiSearch(HsNetWork hsNet, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_BOARDNOTIFYSEARCH", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }
        public int RequestSendBoardNotiConfirm(HsNetWork hsNet, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_BOARDNOTIFYCONFIRM", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }
        public void RequestSendSVRGPKIRegInfo(HsNetWork hsNet, string strGPKIList)
        {
            hsNet.getgpki(strGPKIList);
        }

        public int RequestSend_PRIVACY_CONTINUE(HsNetWork hsNet, string strUserID, string transSeq, string dlpApprove, string privacyConfirmSeq, string NetType)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["TRANSSEQ"] = transSeq;
            dic["DLPAPPROVE"] = dlpApprove;
            dic["PRIVACYCONFIRMID"] = privacyConfirmSeq;
            dic["NETOVERSYSTEM"] = NetType;

            SGEventArgs args = sendParser.RequestCmd("CMD_STR_PRIVACY_CONTINUE", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int RequestSendUrl(HsNetWork hsNet, int groupid, string strUserID, int nTotalCount, int nCurrentCount, int nSubDataType, string strUrlData)
        {
            if (strUrlData.Length < 1)
                return -1;

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["TOTALCOUNT"] = nTotalCount.ToString();
            dic["CURRENTCOUNT"] = nCurrentCount.ToString();
            dic["SUBDATATYPE"] = nSubDataType.ToString();

            dic["SUBDATASIZE"] = strUrlData.Length.ToString();
            dic["SUBDATA"] = strUrlData;

            SGEventArgs args = sendParser.RequestSubDataExchange("SUBDATAEXCHANGE", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int RequestCommonSendQuery(HsNetWork hsNet, eCmdList eCmd, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;

            string cmdStr = SGCmdDef.Instance.GetCmdIdToString(eCmd);
            SGEventArgs args = sendParser.RequestSendQuery(cmdStr, dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        public int SendGenericNotiType2(HsNetWork hsNet, string strUserID, string strUserName, string strUserDeptName, string fileName, string strPreworkType)
        {

            if (strUserID.Length < 1 || strUserName.Length < 1 || strUserDeptName.Length < 1 || fileName.Length < 1 || strPreworkType.Length < 1)
                return -1;

            Dictionary<string, string> Dic = new Dictionary<string, string>();

            Dic["APPID"] = "0x00000000";
            Dic["CLIENTID"] = strUserID;
            Dic["NOTITYPE"] = "2";
            Dic["USERNAME"] = strUserName;
            Dic["DEPTNAME"] = strUserDeptName;
            Dic["FILELIST"] = fileName;
            Dic["PREWORKTYPE"] = strPreworkType;
            Dic["REQDATE"] = DateTime.Now.ToString("yyyy-MM-dd").Base64EncodingStr();

            SGEventArgs args = sendParser.RequestCmd("CMD_STR_GENERIC_NOTI", Dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);

        }

        public int RequestSendOLEMimeListQuery(HsNetWork hsNet, string strUserID, string strQuery)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["APPID"] = "0x00000000";
            dic["CLIENTID"] = strUserID;
            dic["QUERY"] = strQuery;
            SGEventArgs args = sendParser.RequestSendQuery("CMD_STR_OLEMIMELISTQUERY", dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }


        //
        public int RequestRestReady(HsNetWork hsNet, string strAgentName, string strVersion, string strOSType, List<string> listGpkiCnList = null)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic["gpki_cn_list"] = (listGpkiCnList == null ? new List<string>() : listGpkiCnList);
            dic["agent_name"] = strAgentName;
            dic["version"] = strVersion;
            dic["os_type"] = strOSType;
            dic["rsa_public_key"] = SGCrypto.GetRSAPublicKey_Base64Enc(hsNet.GetGroupID().ToString());

            SGEventArgs args = sendParser.RequestRestCmd(eAdvancedCmdList.ePostReady, null, dic, hsNet.stCliMem.GetProtectedSeedKey()); // api-key 사용
            return hsNet.RequestRest(args);

            //return hsNet.SendMessage(args);
            //SGEventArgs args = sendParser.RequestReady(dic);
            //return hsNet.RequestRest(args);
        }

        /// <summary>
        /// Post-User/Login
        /// </summary>
        /// <param name="hsNet"></param>
        /// <param name="loginType">택 1: origin, ad, ldap, pw_otp, otp, sso</param>
        /// <param name="userId"></param>
        /// <param name="protectedUserPassword">메모리 암호화된 비밀번호</param>
        /// <param name="checkPassword"></param>
        /// <param name="strVersion"></param>
        /// <param name="notiType">Callback 방식. 택1: WebSocket, URL, None(Callback 미사용)</param>
        /// <param name="notiRootURL">URL 사용 시, Callback Root URL</param>
        /// <param name="hanssakOTP"></param>
        /// <returns></returns>
        public int RequestRestLogin(HsNetWork hsNet, int loginType, string userId, string protectedUserPassword, bool checkPassword, string strVersion, string notiRootURL, string hanssakOTP = "")
        {
            byte[] pw = null;
            byte[] pwTag = null;
            try
            {
                //hsNet.stCliMem.ExtLogin = loginType;
                //hsNet.stCliMem.UserId= userId;
                //hsNet.stCliMem.SetProtectedPassword(protectedUserPassword);

                SGCrypto.AESDecrypt256WithDEK(hsNet.stCliMem.GetProtectedPassword(), ref pw);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                Dictionary<string, object> auth_info = new Dictionary<string, object>();
                Dictionary<string, object> agent_info = new Dictionary<string, object>();
                Dictionary<string, object> user_conn_info = new Dictionary<string, object>();
                Dictionary<string, object> session_callback = new Dictionary<string, object>();

                dic.Add("hs_otp", hanssakOTP);
                dic.Add("auth_info", auth_info);
                dic.Add("agent_info", agent_info);
                dic.Add("user_conn_info", user_conn_info);
                user_conn_info.Add("session_callback", session_callback);

                //auth_info
                HsLoginType login = (HsLoginType)loginType;
                auth_info.Add("login_type", login.ToString().ToLower());
                auth_info.Add("user_id", userId);
                if (login == HsLoginType.LDAP) //LDAP,LDAPS
                {
                    pwTag = new byte[pw.Length + 1];
                    pwTag[0] = Convert.ToByte(2);
                    System.Buffer.BlockCopy(pw, 0, pwTag, 1, pw.Length);

                    //string newpwd = Convert.ToChar(2) + clientInfo.GetOriginalPassword();
                    auth_info.Add("user_password", SGCrypto.AESEncrypt256(pwTag, hsNet.stCliMem.GetProtectedSeedKey()).ByteToBase64String().Base64EncodingStr());
                }
                else
                    auth_info.Add("user_password", SGCrypto.AESEncrypt256(pw, hsNet.stCliMem.GetProtectedSeedKey()).ByteToBase64String().Base64EncodingStr());
                auth_info.Add("check_password", checkPassword);

                //agent_info
                agent_info.Add("version", strVersion);
                string uuid = "";
                SGCrypto.getUUID(ref uuid, false);

                agent_info.Add("uuid", uuid);
                agent_info.Add("hash", hsNet.GetAgentHash().Base64EncodingStr());

                //user_conn_info
                SgExtFunc.GetLocalIpMacAddress(out string strIPdata, out string strMacdata);
                //TODO 고도화 - LocalIP 사용해도 될지 검토 필요
                user_conn_info.Add("ip_addr", strIPdata.Base64EncodingStr());
                user_conn_info.Add("mac", SGCrypto.AESEncrypt256(strMacdata, hsNet.stCliMem.GetProtectedSeedKey()).ByteToBase64String().Base64EncodingStr());

                if (hsNet.NotiConnType == NotifyConnectionType.WebSocket)
                    session_callback.Add("type", "WebSocket");
                else if (hsNet.NotiConnType == NotifyConnectionType.CallBackURL)
                {
                    session_callback.Add("type", "URL");
                    session_callback.Add("root_url", notiRootURL);
                }
                else
                {
                    session_callback.Add("type", "None");
                }

                SGEventArgs args = sendParser.RequestRestCmd(eAdvancedCmdList.ePostLogin, null, dic, hsNet.stCliMem.GetProtectedSeedKey()); // api-key 사용
                return hsNet.RequestRest(args);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                pw?.hsClear(3);
                pwTag?.hsClear(3);
            }
        }

        /// <summary>
        /// Agent PC에 저장된 GPKI 리스트 기반으로 사용할 수 있는 GPKI 목록 요청
        /// </summary>
        /// <returns></returns>
        public int RequestRestGpkiCnList(HsNetWork hsNet, List<string> gpkiCNPCList)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("gpki_cn_list", gpkiCNPCList);

            SGEventArgs args = sendParser.RequestRestCmd(eAdvancedCmdList.eGetGpkiCnList, null, dic, hsNet.stCliMem.GetProtectedSeedKey()); // api-key 사용
            return hsNet.RequestRest(args);
        }

        public int RequestRestGpkiRandom(HsNetWork hsNet, string strUserID)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("gpki_uid", strUserID);

            SGEventArgs args = sendParser.RequestRestCmd(eAdvancedCmdList.ePostGPKIRandom, null, dic, hsNet.stCliMem.GetProtectedSeedKey()); // api-key 사용
            return hsNet.RequestRest(args);
        }

        public int RequestRestGPKILogin(HsNetWork hsNet, byte[] byteSignedDataHex)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            Dictionary<string, object> sign = new Dictionary<string, object>();
            dic.Add("gpki_sign", sign);
            sign.Add("data", byteSignedDataHex.ByteToBase64String());   // 정각과장과 협의
            sign.Add("length", byteSignedDataHex.Length.ToString());

            SGEventArgs args = sendParser.RequestRestCmd(eAdvancedCmdList.ePostGPKILogin, null, dic, hsNet.stCliMem.GetProtectedSeedKey()); // api-key 사용
            return hsNet.RequestRest(args);
        }

        public int RequestRestGpkiCnChange(HsNetWork hsNet, string strUserID, string strGpkiCn)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("gpki_cn", strGpkiCn);

            SGEventArgs args = sendParser.RequestRestCmd(eAdvancedCmdList.ePatchGPKICN, null, dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.SendMessage(args);
        }

        /// <summary>
        /// Post-User/Login
        /// </summary>
        /// <param name="hsNet"></param>
        /// <param name="loginType">택 1: origin, ad, ldap, pw_otp, otp, sso</param>
        /// <param name="userId"></param>
        /// <param name="protectedUserPassword">메모리 암호화된 비밀번호</param>
        /// <param name="checkPassword"></param>
        /// <param name="strVersion"></param>
        /// <param name="notiType">Callback 방식. 택1: WebSocket, URL, None(Callback 미사용)</param>
        /// <param name="notiRootURL">URL 사용 시, Callback Root URL</param>
        /// <param name="hanssakOTP"></param>
        /// <returns></returns>
        public int RequestRestLogin2Fa(HsNetWork hsNet, string gotpAuthNumber)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("gotp_auth_number", gotpAuthNumber);

            SGEventArgs args = sendParser.RequestRestCmd(eAdvancedCmdList.ePostLogin2Factor, null, dic, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.RequestRest(args);
        }

        public int RequestRestSendApproveBatch(HsNetWork hsNet, string strUserSeq, string strApproveType, string strDataType, string strApproveAction, string strstrDescription, List<Int64> listSeqData)
        {

            if ((strUserSeq?.Length ?? 0) < 1)
            {
                return -1;
            }

            Dictionary<string, object> dicApproveProcinfo = new Dictionary<string, object>();
            dicApproveProcinfo["approver_seq"] = Convert.ToInt64(strUserSeq);
            dicApproveProcinfo["approval_type"] = strApproveType;
            dicApproveProcinfo["data_type"] = strDataType;
            dicApproveProcinfo["approval_action"] = strApproveAction;
            dicApproveProcinfo["description"] = strstrDescription;


            Dictionary<string, object> dicApproveItem = new Dictionary<string, object>();
            dicApproveItem["approval_proc_info"] = dicApproveProcinfo;
            dicApproveItem["trans_seq_list"] = listSeqData.ToArray();

            SGEventArgs args = sendParser.RequestRestCmd(eAdvancedCmdList.ePostApprovalsConfirmation, null, dicApproveItem, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.RequestRest(args);
        }


        public int RequestRestDashBoardData(HsNetWork hsNet, string strFromToDate, int nItemCount)
        {
            Dictionary<string, string> dicSearch = new Dictionary<string, string>();
            dicSearch["pdate "] = strFromToDate;
            dicSearch["limit"] = nItemCount.ToString();

            //SGEventArgs args = sendParser.RequestApproveReject(dicSearch);
            //return hsNet.RequestRest(args);

            SGEventArgs args = sendParser.RequestRestCmd(eAdvancedCmdList.eGetDashboard, dicSearch, null, hsNet.stCliMem.GetProtectedSeedKey()); // api-key 사용
            return hsNet.RequestRest(args);
        }

        public int RequestRestSendTransCancel(HsNetWork hsNet, int groupid, string strTransSeq, string strDataType)
        {
            Dictionary<string, string> dicDoDelete = new Dictionary<string, string>();
            dicDoDelete["tseqs"] = strTransSeq;
            //dicDoDelete["data_type"] = strDataType;   // Server와 협의후 사용

            SGEventArgs args = sendParser.RequestRestCmd(eAdvancedCmdList.eDeleteTransferRequest, dicDoDelete, null, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.RequestRest(args);
        }

        public int RequestRestSendForwardCancel(HsNetWork hsNet, int groupid, string strTransSeq)
        {
            Dictionary<string, string> dicDoDelete = new Dictionary<string, string>();
            dicDoDelete["tseq"] = strTransSeq;

            SGEventArgs args = sendParser.RequestRestCmd(eAdvancedCmdList.eDeleteTransferRequestForward, dicDoDelete, null, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.RequestRest(args);
        }

        public int RequestRestSendBoardNotiConfirm(HsNetWork hsNet, List<Int64> listBoardSeq)
        {
            Dictionary<string, string> dicGetParamQuery = new Dictionary<string, string>();
            dicGetParamQuery["bseq"] = listBoardSeq.ToString();

            Dictionary<string, object> dicBody = new Dictionary<string, object>();
            dicBody["dept_seq"] = "";
            dicBody["read_again"] = false;

            SGEventArgs args = sendParser.RequestRestCmd(eAdvancedCmdList.ePostAnnouncementsReadDone, dicGetParamQuery, dicBody, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.RequestRest(args);
        }

        public int RequestRestUserSearch(HsNetWork hsNet, string strApproverName, string strDeptSeq, string strDeptName, UserApproveRightType ApproveType)
        {
            Dictionary<string, object> dicBody = new Dictionary<string, object>();
            dicBody["approver_type"] = ApproveType;
            dicBody["dept_seq"] = ((strDeptSeq?.Length ?? 0) > 0) ? Convert.ToInt64(strDeptSeq) : 0;
            dicBody["dept_name"] = strDeptName;
            dicBody["approver_name"] = strApproverName;

            SGEventArgs args = sendParser.RequestRestCmd(eAdvancedCmdList.ePostApproversRetrieval, null, dicBody, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.RequestRest(args);
        }

        public int RestRequestInstApproveRegChange(HsNetWork hsNet, string strUserID, string strQuery)
        {
            Dictionary<string, object> dicBody = new Dictionary<string, object>();
            dicBody["CLIENTID"] = strUserID;
            dicBody["QUERY"] = strQuery;

            SGEventArgs args = sendParser.RequestRestCmd(eAdvancedCmdList.ePostProxyApproversChange, null, dicBody, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.RequestRest(args);
        }


        public int RequestRestSendUrlRedirection(HsNetWork hsNet, int nTotalCount, int nCurrentCount, int nSubDataType, string strUrlData, bool bRetCallPos)
        {
            if (strUrlData.Length < 1)
                return -1;

            Dictionary<string, object> dicBody = new Dictionary<string, object>();
            dicBody["TOTALCOUNT"] = nTotalCount;
            dicBody["CURRENTCOUNT"] = nCurrentCount;
            dicBody["SUBDATATYPE"] = nSubDataType;
            dicBody["SUBDATA"] = strUrlData;

            SGEventArgs args = sendParser.RequestRestCmd(eAdvancedCmdList.ePostURLRedirection, null, dicBody, hsNet.stCliMem.GetProtectedSeedKey(), eCmdReponseType.CallPosReponse);
            return hsNet.RequestRest(args);
        }

        public int RequestRestAgentBlockReason(HsNetWork hsNet, List<AgentBlockData> listAgentBlock)
        {
            if ((listAgentBlock?.Count ?? 0) < 1)
                return -1;

            Dictionary<string, object> dicBody = new Dictionary<string, object>();
            dicBody["agent_block_list"] = listAgentBlock.ToArray();

            SGEventArgs args = sendParser.RequestRestCmd(eAdvancedCmdList.ePostAgentBlocks, null, dicBody, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.RequestRest(args);
        }

        public int RequestRestInstApproveClear(HsNetWork hsNet, eCmdReponseType reponseType)
        {
            SGEventArgs args = sendParser.RequestRestCmd(eAdvancedCmdList.eDeleteProxyApprovers, null, null, hsNet.stCliMem.GetProtectedSeedKey(), reponseType);
            return hsNet.RequestRest(args);
        }

        public int RequestRestInstApproveReg(HsNetWork hsNet, List<ProxyApprover> listProxyApprover)
        {
            if ((listProxyApprover?.Count ?? 0) < 1)
                return -1;

            Dictionary<string, object> dicBody = new Dictionary<string, object>();
            dicBody["proxy_approver_list"] = listProxyApprover.ToArray();

            SGEventArgs args = sendParser.RequestRestCmd(eAdvancedCmdList.ePostProxyApproversChange, null, null, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.RequestRest(args);
        }

        public int RequestRestInstApproveListGet(HsNetWork hsNet)
        {
            SGEventArgs args = sendParser.RequestRestCmd(eAdvancedCmdList.eGetProxyApprovers, null, null, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.RequestRest(args);
        }

        public int RequestRestLogOut(HsNetWork hsNet, eCmdReponseType reponseType)
        {
            Dictionary<string, object> dicBody = new Dictionary<string, object>();
            SGEventArgs args = sendParser.RequestRestCmd(eAdvancedCmdList.ePostLogout, null, dicBody, hsNet.stCliMem.GetProtectedSeedKey(), reponseType);
            return hsNet.RequestRest(args);
        }


        public int RequestRestSendDeptInfo(HsNetWork hsNet)
        {
            SGEventArgs args = sendParser.RequestRestCmd(eAdvancedCmdList.eGetDepts, null, null, hsNet.stCliMem.GetProtectedSeedKey());
            return hsNet.RequestRest(args);
        }

    }
}
