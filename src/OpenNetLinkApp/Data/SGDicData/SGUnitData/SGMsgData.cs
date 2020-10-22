using System;
using System.Collections.Generic;
using System.Text;
using HsNetWorkSGData;
using HsNetWorkSG;
using OpenNetLinkApp.Services;
using WebWindows;
using OpenNetLinkApp.Models.SGSideBar;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    [Flags]
    public enum eMsgType
    {
        eNone = 0,
        eMsgBoardNoti,                  // [공지사항]
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
    public enum eAlarmType
    {
        eNone = 0,
        eAlarmVirus,                        // [바이러스]
        eAlarmBoardNoti,                      // [공지사항]
        eAlarmApt,                            // [APT]
        eAlarmApprWait,                       // [승인대기]
        eAlarmSecureApprWait,                 // [보안승인대기]
        eAlarmEMailApprWait,                  // [메일승인대기]
        eAlarmApprConfirm,                    // [승인완료]
        eAlarmApprReject,                     // [반려완료]
        eAlarmLogin,                           // [로그인]
        eAlarmLogout                            // [로그아웃]
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

        public LSIDEBAR GetConvertOSNotiMenuCategory(OS_NOTI osNoti)
        {
            LSIDEBAR sidebar;
            switch (osNoti)
            {
                case OS_NOTI.WAIT_APPR:                                     // 승인대기
                    sidebar = LSIDEBAR.MENU_CATE_FILE;
                    break;
                case OS_NOTI.SECURE_APPR:                                   // 보안결재 승인대기
                    sidebar = LSIDEBAR.MENU_CATE_FILE;
                    break;
                case OS_NOTI.MAIL_APPR:                                     // 메일결재 승인대기
                    sidebar = LSIDEBAR.MENU_CATE_MAIL;
                    break;
                default:
                    sidebar = LSIDEBAR.MENU_CATE_ROOT;
                    break;
            }
            return sidebar;
        }

        public eAlarmType GetConvertOSNotiAlarmTitle(OS_NOTI osNoti)
        {
            eAlarmType eAType;
            switch(osNoti)
            {
                case OS_NOTI.ONLINE:                                        // 온라인
                    eAType = eAlarmType.eAlarmLogin;
                    break;
                case OS_NOTI.OFFLINE:                                       // 오프라인
                    eAType = eAlarmType.eAlarmLogout;
                    break;
                case OS_NOTI.WAIT_APPR:                                     // 승인대기
                    eAType = eAlarmType.eAlarmApprWait;
                    break;
                case OS_NOTI.SECURE_APPR:                                   // 보안결재 승인대기
                    eAType = eAlarmType.eAlarmSecureApprWait;
                    break;
                case OS_NOTI.MAIL_APPR:                                     // 메일결재 승인대기
                    eAType = eAlarmType.eAlarmEMailApprWait;
                    break;
                case OS_NOTI.CONFIRM_APPR:                                  // 승인완료
                    eAType = eAlarmType.eAlarmApprConfirm;
                    break;
                case OS_NOTI.REJECT_APPR:                                   // 반려 알림
                    eAType = eAlarmType.eAlarmApprReject;
                    break;
                default:
                    eAType = eAlarmType.eNone;
                    break;
            }
            return eAType;
        }
        public string GetConvertAlarmTitle(eAlarmType eAlarm)
        {
            string strTitle = "";
            switch(eAlarm)
            {
                case eAlarmType.eAlarmVirus:
                    strTitle = xconf.GetTitle("T_ALARM_VIRUS");                 // [바이러스]
                    break;
                case eAlarmType.eAlarmBoardNoti:
                    strTitle = xconf.GetTitle("T_ALARM_BOARDNOTI");             // [공지사항]
                    break;
                case eAlarmType.eAlarmApt:
                    strTitle = xconf.GetTitle("T_ALARM_APT");                   // [APT]
                    break;
                case eAlarmType.eAlarmApprWait:
                    strTitle = xconf.GetTitle("T_ALARM_APPRWAIT");              // [승인대기]
                    break;
                case eAlarmType.eAlarmSecureApprWait:
                    strTitle = xconf.GetTitle("T_ALARM_SECUREAPPRWAIT");        // [보안승인대기]
                    break;
                case eAlarmType.eAlarmEMailApprWait:
                    strTitle = xconf.GetTitle("T_ALARM_EMAILAPPRWAIT");         // [메일승인대기]
                    break;
                case eAlarmType.eAlarmApprConfirm:
                    strTitle = xconf.GetTitle("T_ALARM_APPRCONFIRM");           // [승인완료]
                    break;
                case eAlarmType.eAlarmApprReject:
                    strTitle = xconf.GetTitle("T_ALARM_APPRREJCT");             // [반려완료]
                    break;
                case eAlarmType.eAlarmLogin:
                    strTitle = xconf.GetTitle("T_ALARM_LOGIN");                 // [로그인]
                    break;
                default:
                    break;
            }
            return strTitle;
        }
        public string GetConvertMessageTitle(eMsgType eTitle)
        {
            string strTitle = "";
            switch(eTitle)
            {
                case eMsgType.eMsgBoardNoti:                   
                    strTitle = xconf.GetTitle("T_MESSAGE_BOARDNOTI");           // [공지사항]
                    break;
                case eMsgType.eMsgFileTrans:
                    strTitle = xconf.GetTitle("T_MESSAGE_FILETRANS");           // [파일전송]
                    break;
                case eMsgType.eMsgFileRecv:
                    strTitle = xconf.GetTitle("T_MESSAGE_FILERECV");            // [파일수신]
                    break;
                case eMsgType.eMsgClipSend:
                    strTitle = xconf.GetTitle("T_MESSAGE_CLIPSEND");            // [클립보드전송]
                    break;
                case eMsgType.eMsgClipRecv:
                    strTitle = xconf.GetTitle("T_MESSAGE_CLIPRECV");            // [클립보드수신]
                    break;
                case eMsgType.eMsgFileCancel:
                    strTitle = xconf.GetTitle("T_MESSAGE_TRANSCANCLE");             // [전송취소]
                    break;
                case eMsgType.eMsgApprConfirm:
                    strTitle = xconf.GetTitle("T_MESSAGE_APPRCONFIRM");             // [승인]
                    break;
                case eMsgType.eMsgApprReject:
                    strTitle = xconf.GetTitle("T_MESSAGE_APPRREJECT");              // [반려]
                    break;
                case eMsgType.eMsgEmailApprConfirm:        
                    strTitle = xconf.GetTitle("T_MESSAGE_MAILAPPRCONFIRM");         // [메일승인]
                    break;
                case eMsgType.eMsgEmailApprReject:
                    strTitle = xconf.GetTitle("T_MESSAGE_MAILAPPRREJECT");          // [메일반려]
                    break;
                case eMsgType.eMsgEmailCancel:
                    strTitle = xconf.GetTitle("T_MESSAGE_MAILCANCEL");              // [발송취소]
                    break;

                default:
                    break;
            }
         
            return strTitle;
        }
    }
}
