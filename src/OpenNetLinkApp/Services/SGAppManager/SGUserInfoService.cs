using System;
using OpenNetLinkApp.Data.SGDicData.SGUnitData;
using OpenNetLinkApp.Models.SGUserInfo;
using System.Collections.Generic;
using System.Collections.Concurrent;
using HsNetWorkSGData;
using OpenNetLinkApp.Common;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGUserInfoService
    {
        /* To Manage User Info State */
        /// <summary>
        /// Config User Information.
        /// </summary>
        ConcurrentDictionary<int, ISGUserInfo> DicUserInfo { get; set; }
        //ISGUserInfo UserInfo { get; }
        /// <summary>
        /// User Info Event Delegate, Modified by User or System.
        /// </summary>
        event Action OnChangeUserInfo;
        /// <summary>
        /// To Set UserInfo Config or Information 
        /// </summary>
        /// <param name="userInfo"> 
        /// </param>
        /// <returns>void</returns>
        void SetUserInfo(int groupID, SGLoginData sgLoginData);
        //void SetUserInfo(ISGUserInfo userInfo);

        ISGUserInfo GetUserInfo(int groupID);
    }
    internal class SGUserInfoService : ISGUserInfoService
    {
        public SGUserInfoService()
        {
        }

        /* To Manage User Info State */
        public ConcurrentDictionary<int, ISGUserInfo> DicUserInfo { get; set; } = new ConcurrentDictionary<int, ISGUserInfo>();
        public event Action OnChangeUserInfo;
        private void NotifyStateChangedUserInfo() => OnChangeUserInfo?.Invoke();
        public void SetUserInfo(int groupID, SGLoginData sgLoginData)
        {
            SGUserInfo sgUser = new SGUserInfo();
            sgUser.UserId = sgLoginData.GetUserID();                // 사용자 ID
            sgUser.UserName = sgLoginData.GetUserName();             // 사용자 이름
            sgUser.UserSeq = sgLoginData.GetUserSequence();          // 사용자 SEQ
            sgUser.DeptName = sgLoginData.GetTeamName();             // 부서명
            sgUser.Position = sgLoginData.GetUserPosition();         // 직책
            sgUser.Rank = sgLoginData.GetRank();                     // 직위
            sgUser.ApprPos = sgLoginData.GetUserApprpos();           // 일반사용자, 결재자, 전결자

            SGUserInfoAdded sgUserAdd = new SGUserInfoAdded();
            sgUserAdd.FileFilterExt = sgLoginData.GetFileFilter();     // 파일 확장자 제한.

            Int64 size = sgLoginData.GetFileLimitSize();                // 파일 전송 사이즈 제한 (단위 MB)
            if (size <= 0)
                size = 1536;
            sgUserAdd.FileSizeLimit = size;

            int count = sgLoginData.GetFileLimitCount();                 // 전송가능한 파일의 최대 개수
            if (count <= 0)
                count = 1024;
            sgUserAdd.FileCountLimit = count;

            size = sgLoginData.GetDayFileTransferLimitSize();          // 하루에 전송 가능한 파일 최대 크기
            if (size <= 0)
                size = 1536;
            sgUserAdd.DayFileSizeLimit = size;

            count = sgLoginData.GetDayFileTransferLimitCount();         // 하루에 전송 가능한 파일 최대 회수 
            if (count <= 0)
                count = 1024;
            sgUserAdd.DayFileCountLimit = count;

            size = sgLoginData.GetClipboardLimitSize();                 // 한번에 전송 가능한 클립보드 최대 크기
            if (size <= 0)
                size = 1536;
            sgUserAdd.ClipSizeLimit = size;

            size = sgLoginData.GetDayClipboardLimitSize();              // 하루에 전송 가능한 클립보드 최대 크기.
            if (size <= 0)
                size = 1536;
            sgUserAdd.DayClipSizeLimit = size;

            count = sgLoginData.GetDayClipboardLimitCount();            // 하루에 전송 가능한 클립보드 최대 회수.
            if (count <= 0)
                count = 1024;
            sgUserAdd.DayClipCountLimit = count;

            count = sgLoginData.GetMaxDownCount();                      // 다운로드 가능 횟수
            if (count <= 0)
                count = 1;
            sgUserAdd.MaxDownloadCount = count;

            //대결재자 관련 추가 부분

            bool isMySelfSFM = true;
            int sfmRight = 0;
            string headUserName = sgLoginData.GetUserName();
            List<(string Name, string Rank, int Right)> listDicSfmdata = sgLoginData.GetApproverWhoGaveMeProxyList();
            if (listDicSfmdata == null || listDicSfmdata.Count == 0)
            {
                isMySelfSFM = true;
                sfmRight = 0;
            }
            else
            {
                if (listDicSfmdata.Count > 1)
                {
                    headUserName = String.Format(CsFunction.XmlConf.GetTitle("T_PROXY_USERNAME_COUNT"), sgLoginData.GetUserName(), listDicSfmdata[0].Name, listDicSfmdata[0].Rank, listDicSfmdata.Count - 1);
                }
                else
                {
                    headUserName = String.Format(CsFunction.XmlConf.GetTitle("T_PROXY_USERNAME"), sgLoginData.GetUserName(), listDicSfmdata[0].Name, listDicSfmdata[0].Rank);
                }

                foreach ((string Name, string Rank, int Right) value in listDicSfmdata)
                {
                    if (sfmRight < Convert.ToInt32(value.Rank))
                        sfmRight = Convert.ToInt32(value.Rank);
                }

                isMySelfSFM = false;
            }

            sgUser.HeaderUserName = headUserName;
            sgUserAdd.IsMySelfSFM = isMySelfSFM;
            sgUserAdd.SFMRight = sfmRight;


            sgUser.UserInfoAdded = sgUserAdd;

            DicUserInfo[groupID] = sgUser;
            NotifyStateChangedUserInfo();
        }
        /*
        public void SetUserInfo(ISGUserInfo userInfo)
        {
            UserInfo = userInfo;
            NotifyStateChangedUserInfo();
        }
        */

        public ISGUserInfo GetUserInfo(int groupID)
        {
            ISGUserInfo sgUserInfo;
            if (DicUserInfo.TryGetValue(groupID, out sgUserInfo) == true)
                sgUserInfo = DicUserInfo[groupID];
            return sgUserInfo;
        }
    }
}