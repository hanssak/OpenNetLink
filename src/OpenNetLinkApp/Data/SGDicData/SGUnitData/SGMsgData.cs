using System;
using System.Collections.Generic;
using System.Text;
using HsNetWorkSGData;
using HsNetWorkSG;
using OpenNetLinkApp.Services;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    [Flags]
    public enum eMsgTitle
    {
        eMsgBoardNoti = 0,                  // [공지사항]
        eMsgFileTrans,                      // [파일전송]
        eMsgFileRecv,                       // [파일수신]
        eMsgClipSend,                       // [클립보드전송]
        eMsgClipRecv,                       // [클립보드수신]
        eMsgFileCancel,                     // [전송취소]
        eMsgApprConfirm,                    // [승인]
        eMsgApprReject,                     // [반려]
        eMsgEmailApprConfirm,               // [메일승인]
        eMsgEmailApprReject,                // [메일반려]
        eMsgEmailCancel                     // [발송취소]
    }
    [Flags]
    public enum eAlarmTitle
    {
        eAlarmVirus=0,                        // [바이러스]
        eAlarmBoardNoti,                      // [공지사항]
        eAlarmApt,                            // [APT]
        eAlarmApprWait,                       // [승인대기]
        eAlarmSecureApprWait,                 // [보안승인대기]
        eAlarmEMailApprWait,                  // [메일승인대기]
        eAlarmApprConfirm,                    // [승인완료]
        eAlarmApprReject,                     // [반려완료]
        eAlarmLogin                           // [로그인]
    }
    public class SGMsgData : SGData
    {
        private readonly XmlConfService xconf = new XmlConfService();
        public SGMsgData()
        {

        }
        ~SGMsgData()
        {

        }

        public string GetConvertAlarmTitle(eAlarmTitle eAlarm)
        {
            string strTitle = "";
            switch(eAlarm)
            {
                case eAlarmTitle.eAlarmVirus:
                    strTitle = xconf.GetTitle("T_ALARM_VIRUS");                 // [바이러스]
                    break;
                case eAlarmTitle.eAlarmBoardNoti:
                    strTitle = xconf.GetTitle("T_ALARM_BOARDNOTI");             // [공지사항]
                    break;
                case eAlarmTitle.eAlarmApt:
                    strTitle = xconf.GetTitle("T_ALARM_APT");                   // [APT]
                    break;
                case eAlarmTitle.eAlarmApprWait:
                    strTitle = xconf.GetTitle("T_ALARM_APPRWAIT");              // [승인대기]
                    break;
                case eAlarmTitle.eAlarmSecureApprWait:
                    strTitle = xconf.GetTitle("T_ALARM_SECUREAPPRWAIT");        // [보안승인대기]
                    break;
                case eAlarmTitle.eAlarmEMailApprWait:
                    strTitle = xconf.GetTitle("T_ALARM_EMAILAPPRWAIT");         // [메일승인대기]
                    break;
                case eAlarmTitle.eAlarmApprConfirm:
                    strTitle = xconf.GetTitle("T_ALARM_APPRCONFIRM");           // [승인완료]
                    break;
                case eAlarmTitle.eAlarmApprReject:
                    strTitle = xconf.GetTitle("T_ALARM_APPRREJCT");             // [반려완료]
                    break;
                case eAlarmTitle.eAlarmLogin:
                    strTitle = xconf.GetTitle("T_ALARM_LOGIN");                 // [로그인]
                    break;
                default:
                    break;
            }
            return strTitle;
        }
        public string GetConvertMessageTitle(eMsgTitle eTitle)
        {
            string strTitle = "";
            switch(eTitle)
            {
                case eMsgTitle.eMsgBoardNoti:                   
                    strTitle = xconf.GetTitle("T_MESSAGE_BOARDNOTI");           // [공지사항]
                    break;
                case eMsgTitle.eMsgFileTrans:
                    strTitle = xconf.GetTitle("T_MESSAGE_FILETRANS");           // [파일전송]
                    break;
                case eMsgTitle.eMsgFileRecv:
                    strTitle = xconf.GetTitle("T_MESSAGE_FILERECV");            // [파일수신]
                    break;
                case eMsgTitle.eMsgClipSend:
                    strTitle = xconf.GetTitle("T_MESSAGE_CLIPSEND");            // [클립보드전송]
                    break;
                case eMsgTitle.eMsgClipRecv:
                    strTitle = xconf.GetTitle("T_MESSAGE_CLIPRECV");            // [클립보드수신]
                    break;
                case eMsgTitle.eMsgFileCancel:
                    strTitle = xconf.GetTitle("T_MESSAGE_TRANSCANCLE");             // [전송취소]
                    break;
                case eMsgTitle.eMsgApprConfirm:
                    strTitle = xconf.GetTitle("T_MESSAGE_APPRCONFIRM");             // [승인]
                    break;
                case eMsgTitle.eMsgApprReject:
                    strTitle = xconf.GetTitle("T_MESSAGE_APPRREJECT");              // [반려]
                    break;
                case eMsgTitle.eMsgEmailApprConfirm:        
                    strTitle = xconf.GetTitle("T_MESSAGE_MAILAPPRCONFIRM");         // [메일승인]
                    break;
                case eMsgTitle.eMsgEmailApprReject:
                    strTitle = xconf.GetTitle("T_MESSAGE_MAILAPPRREJECT");          // [메일반려]
                    break;
                case eMsgTitle.eMsgEmailCancel:
                    strTitle = xconf.GetTitle("T_MESSAGE_MAILCANCEL");              // [발송취소]
                    break;

                default:
                    break;
            }
         
            return strTitle;
        }
    }
}
