using System;
using System.Collections.Generic;
using System.Text;
using HsNetWorkSGData;
using HsNetWorkSG;
using OpenNetLinkApp.Services;
using WebWindows;
using OpenNetLinkApp.Models.SGSideBar;
using OpenNetLinkApp.Models.SGNotify;
using System.Globalization;
using System.Linq;

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
    public class BoardItem
    {
        public int ItemType; //1:알람, 2:메시지, 3:공지
        public string Title;
        public bool IsNew;
        public string RegDate;
        public string strBoardSeq;

        public BoardItem()
        {
            ItemType = 0;
            strBoardSeq=Title = RegDate = "";
            IsNew = false;
        }

        public BoardItem(int type, string title, bool isnew, string reg, string strSeq)
        {
            ItemType = type;
            Title = title;
            IsNew = isnew;
            RegDate = reg;
            strBoardSeq = strSeq;
        }

        public string getTypeTag()
        {
            if (ItemType == 1)
                return @"<span class='notify_1'>[알람]</span>";
            else if (ItemType == 2)
                return @"<span class='message_1'>[메시지]</span>";
            else 
                return @"<span class='notice_1'>[공지]</span>";
        }

        public string getNewTag()
        {
            if (IsNew)
                return @"<span class='newtxt'>NEW</span>";
            else
                return "";
        }
    }
    public class SGMsgData : SGData
    {
        private readonly XmlConfService xconf = new XmlConfService();
        List<BoardItem> listItem = new List<BoardItem>();
        public SGMsgData()
        {

        }
        ~SGMsgData()
        {

        }

        public List<BoardItem> GetRecentList(List<SGNotiData> listNotiData, List<SGAlarmData> listAlarmData,int nLimitCount=0)
        {
            listItem.Clear();

            if (((listNotiData == null) || (listNotiData.Count <= 0)) && ((listAlarmData == null) || (listAlarmData.Count <= 0)))
                return null;

            else if (((listNotiData == null) || (listNotiData.Count <= 0)) && ((listAlarmData != null) && (listAlarmData.Count > 0)))
                return GetAlarmList(listAlarmData,nLimitCount);

            else if (((listNotiData != null) && (listNotiData.Count > 0)) && ((listAlarmData == null) || (listAlarmData.Count <= 0)))
                return GetNotiList(listNotiData, nLimitCount);

            else if (((listNotiData != null) || (listNotiData.Count > 0)) && ((listAlarmData != null) || (listAlarmData.Count > 0)))
            {
                List<BoardItem> listAlarmItem= GetAlarmList(listAlarmData, nLimitCount);
                List<BoardItem> listNotiItem = GetNotiList(listNotiData, nLimitCount);

                List<BoardItem> listTmpItem = new List<BoardItem>();
                listTmpItem.AddRange(listAlarmItem);
                listTmpItem.AddRange(listNotiItem);

                listTmpItem = listTmpItem.OrderByDescending(x => x.RegDate).ToList();
                listTmpItem = listTmpItem.OrderByDescending(x => x.strBoardSeq).ToList();

                if (nLimitCount>0)
                {
                    if (nLimitCount >= listTmpItem.Count)
                        listItem = listTmpItem;
                    else
                    {
                        listItem.Clear();
                        for(int i=0;i< nLimitCount; i++)
                        {
                            BoardItem item = listTmpItem[i];
                            listItem.Add(item);
                        }
                    }
                }
            }
            else
                return null;

            return new List<BoardItem>(listItem);
        }
        public List<BoardItem> GetBoardList(List<SGNotiData> listNotiData, int nLimitCount = 0)
        {
            listItem.Clear();
            if (listNotiData == null)
                return null;
            int DataCount = 0;
            for (int i = 0; i < listNotiData.Count; i++)
            {
                if(nLimitCount>0)
                {
                    if (nLimitCount <= DataCount)
                        break;
                }
                if (listNotiData[i].Type == NOTI_TYPE.SYSTEM)
                //if (!listNotiData[i].Seq.Equals("0"))
                {
                    BoardItem boardItem = new BoardItem();
                    boardItem.ItemType = 3;                                     // 공지사항
                    boardItem.Title = listNotiData[i].Head;
                    if (boardItem.Title.Length > 24)
                    {
                        boardItem.Title = boardItem.Title.Substring(0, 24);
                        boardItem.Title = boardItem.Title + "...";
                    }
                    boardItem.RegDate = " " + listNotiData[i].Time?.ToString("yyyy-MM-dd");
                    string strToday = DateTime.Now.ToString("yyyy-MM-dd");
                    string strTmpDay = boardItem.RegDate.Trim();
                    if (strToday.Equals(strTmpDay))
                        boardItem.IsNew = true;
                    else
                        boardItem.IsNew = false;

                    boardItem.strBoardSeq = listNotiData[i].Seq;
                    listItem.Add(boardItem);
                    DataCount++;
                }
            }
            return new List<BoardItem>(listItem);
        }
        public List<BoardItem> GetBoardExceptionList(List<SGNotiData> listNotiData, int nLimitCount = 0)
        {
            listItem.Clear();
            if (listNotiData == null)
                return null;

            int DataCount = 0;
            for (int i = 0; i < listNotiData.Count; i++)
            {
                if (nLimitCount > 0)
                {
                    if (nLimitCount <= DataCount)
                        break;
                }
                if (listNotiData[i].Type != NOTI_TYPE.SYSTEM)
                //if (listNotiData[i].Seq.Equals("0"))
                {
                    BoardItem boardItem = new BoardItem();
                    boardItem.ItemType = 2;                                         // 메시지         
                    boardItem.Title = listNotiData[i].Head;
                    if (boardItem.Title.Length > 24)
                    {
                        boardItem.Title = boardItem.Title.Substring(0, 24);
                        boardItem.Title = boardItem.Title + "...";
                    }
                    boardItem.RegDate = " " + listNotiData[i].Time?.ToString("yyyy-MM-dd");
                    string strToday = DateTime.Now.ToString("yyyy-MM-dd");
                    string strTmpDay = boardItem.RegDate.Trim();
                    if (strToday.Equals(strTmpDay))
                        boardItem.IsNew = true;
                    else
                        boardItem.IsNew = false;
                    boardItem.strBoardSeq = listNotiData[i].Seq;
                    listItem.Add(boardItem);
                    DataCount++;
                }
            }
            return new List<BoardItem>(listItem);
        }

        public List<BoardItem> GetAlarmList(List<SGAlarmData> listAlarmData, int nLimitCount = 0)
        {
            listItem.Clear();
            if (listAlarmData == null)
                return null;

            int DataCount = 0;
            for(int i=0;i<listAlarmData.Count;i++)
            {
                if (nLimitCount > 0)
                {
                    if (nLimitCount <= DataCount)
                        break;
                }
                BoardItem boardItem = new BoardItem();
                boardItem.ItemType = 1;                                         // 알람         
                boardItem.Title = listAlarmData[i].Head;
                if (boardItem.Title.Length > 24)
                {
                    boardItem.Title = boardItem.Title.Substring(0, 24);
                    boardItem.Title = boardItem.Title + "...";
                }
                boardItem.RegDate = " " + listAlarmData[i].Time?.ToString("yyyy-MM-dd");
                string strToday = DateTime.Now.ToString("yyyy-MM-dd");
                string strTmpDay = boardItem.RegDate.Trim();
                if (strToday.Equals(strTmpDay))
                    boardItem.IsNew = true;
                else
                    boardItem.IsNew = false;
                boardItem.strBoardSeq = "0";
                listItem.Add(boardItem);
                DataCount++;
            }
            return new List<BoardItem>(listItem);
        }

        public List<BoardItem> GetNotiList(List<SGNotiData> listNotiData, int nLimitCount = 0)
        {
            listItem.Clear();
            if (listNotiData == null)
                return null;

            int DataCount = 0;
            for (int i = 0; i < listNotiData.Count; i++)
            {
                if (nLimitCount > 0)
                {
                    if (nLimitCount <= DataCount)
                        break;
                }
                BoardItem boardItem = new BoardItem();
                if (listNotiData[i].Type != NOTI_TYPE.SYSTEM)
                //if (listNotiData[i].Seq.Equals("0"))
                    boardItem.ItemType = 2;                                         // 메시지
                else
                    boardItem.ItemType = 3;                                         // 공지사항
                boardItem.Title = listNotiData[i].Head;
                if (boardItem.Title.Length > 24)
                {
                    boardItem.Title = boardItem.Title.Substring(0, 24);
                    boardItem.Title = boardItem.Title + "...";
                }
                boardItem.RegDate = " " + listNotiData[i].Time?.ToString("yyyy-MM-dd");
                string strToday = DateTime.Now.ToString("yyyy-MM-dd");
                string strTmpDay = boardItem.RegDate.Trim();
                if (strToday.Equals(strTmpDay))
                    boardItem.IsNew = true;
                else
                    boardItem.IsNew = false;
                boardItem.strBoardSeq = listNotiData[i].Seq;
                listItem.Add(boardItem);
                DataCount++;
            }
            return new List<BoardItem>(listItem);
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
        public string GetConvertAlarmTitle(eAlarmType eAlarm,string title="")
        {
            string strTitle = "";
            switch(eAlarm)
            {
                case eAlarmType.eAlarmVirus:
                    strTitle = xconf.GetTitle("T_ALARM_VIRUS");                 // 바이러스 검출
                    break;
                case eAlarmType.eAlarmBoardNoti:
                    strTitle = title;                                            // 공지사항 제목
                    break;
                case eAlarmType.eAlarmApt:
                    strTitle = xconf.GetTitle("T_ALARM_APT");                   // 악성코드 검출
                    break;
                case eAlarmType.eAlarmApprWait:
                    strTitle = xconf.GetTitle("T_ALARM_APPRWAIT");              // 결재 승인대기
                    break;
                case eAlarmType.eAlarmSecureApprWait:
                    strTitle = xconf.GetTitle("T_ALARM_SECUREAPPRWAIT");        // 보안결재 승인대기
                    break;
                case eAlarmType.eAlarmEMailApprWait:
                    strTitle = xconf.GetTitle("T_ALARM_EMAILAPPRWAIT");         // 메일결재 승인대기
                    break;
                case eAlarmType.eAlarmApprConfirm:
                    strTitle = xconf.GetTitle("T_ALARM_APPRCONFIRM");           // 결재 승인 완료
                    break;
                case eAlarmType.eAlarmApprReject:
                    strTitle = xconf.GetTitle("T_ALARM_APPRREJCT");             // 결재 반려 완료
                    break;
                case eAlarmType.eAlarmLogin:
                    strTitle = xconf.GetTitle("T_ALARM_LOGIN");                 // 로그인
                    break;
                default:
                    break;
            }
            return strTitle;
        }
        public string GetConvertMessageTitle(eMsgType eMsg, string title = "")
        {
            string strTitle = "";
            switch(eMsg)
            {
                case eMsgType.eMsgBoardNoti:                   
                    strTitle = title;                                           // 공지사항 제목
                    break;
                case eMsgType.eMsgFileTrans:
                    strTitle = xconf.GetTitle("T_MESSAGE_FILETRANS");           // 파일전송 완료
                    break;
                case eMsgType.eMsgFileRecv:
                    strTitle = xconf.GetTitle("T_MESSAGE_FILERECV");            // 파일수신 완료
                    break;
                case eMsgType.eMsgClipSend:
                    strTitle = xconf.GetTitle("T_MESSAGE_CLIPSEND");            // 클립보드 전송
                    break;
                case eMsgType.eMsgClipRecv:
                    strTitle = xconf.GetTitle("T_MESSAGE_CLIPRECV");            // 클립보드 수신
                    break;
                case eMsgType.eMsgFileCancel:
                    strTitle = xconf.GetTitle("T_MESSAGE_TRANSCANCLE");             // 파일 전송취소
                    break;
                case eMsgType.eMsgApprConfirm:
                    strTitle = xconf.GetTitle("T_MESSAGE_APPRCONFIRM");             // 결재 승인완료
                    break;
                case eMsgType.eMsgApprReject:
                    strTitle = xconf.GetTitle("T_MESSAGE_APPRREJECT");              // 결재 반려완료
                    break;
                case eMsgType.eMsgEmailApprConfirm:        
                    strTitle = xconf.GetTitle("T_MESSAGE_MAILAPPRCONFIRM");         // 메일결재 승인완료
                    break;
                case eMsgType.eMsgEmailApprReject:
                    strTitle = xconf.GetTitle("T_MESSAGE_MAILAPPRREJECT");          // 메일결재 반려완료
                    break;
                case eMsgType.eMsgEmailCancel:
                    strTitle = xconf.GetTitle("T_MESSAGE_MAILCANCEL");              // 메일 발송취소
                    break;

                default:
                    break;
            }
         
            return strTitle;
        }
    }
}
