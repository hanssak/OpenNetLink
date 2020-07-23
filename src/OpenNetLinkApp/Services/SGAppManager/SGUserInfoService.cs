using System;
using OpenNetLinkApp.Data.SGDicData.SGUnitData;
using OpenNetLinkApp.Models.SGUserInfo;
using System.Collections.Generic;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGUserInfoService
    {
        /* To Manage User Info State */
        /// <summary>
        /// Config User Information.
        /// </summary>
        Dictionary<int, ISGUserInfo> DicUserInfo { get; set; }
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
        void SetUserInfo(int groupID, SGLoginData sgLoginData, SGUserData sgUserData);
        //void SetUserInfo(ISGUserInfo userInfo);

        ISGUserInfo GetUserInfo(int groupID);
    }
    internal class SGUserInfoService : ISGUserInfoService
    {
        public SGUserInfoService()
        {
        }

        /* To Manage User Info State */
        public Dictionary<int, ISGUserInfo> DicUserInfo { get; set; } = new Dictionary<int, ISGUserInfo>();
        public event Action OnChangeUserInfo;
        //private void NotifyStateChangedUserInfo() => OnChangeUserInfo?.Invoke();
        public void SetUserInfo(int groupID, SGLoginData sgLoginData, SGUserData sgUserData)
        {
            SGUserInfo sgUser = new SGUserInfo();
            sgUser.UserId = sgLoginData.GetUserID();                // 사용자 ID
            sgUser.UserName = sgUserData.GetUserName();             // 사용자 이름
            sgUser.DeptName = sgUserData.GetTeamName();             // 부서명
            sgUser.Position = sgUserData.GetUserPosition();         // 직책
            sgUser.Rank = sgUserData.GetRank();                     // 직위
            sgUser.ManOrSteff = sgUserData.GetPartOwner();          // 팀원/팀장 여부 ( 팀원 : 1, 팀장 : 2)

            SGUserInfoAdded sgUserAdd = new SGUserInfoAdded();
            sgUserAdd.FileFilterExt = sgLoginData.GetFileFilter();     // 파일 확장자 제한.

            Int64 size = sgLoginData.GetFileLimitSize();                // 파일 전송 사이즈 제한 (단위 MB)
            if (size <= 0)
                size = 1536;
            sgUserAdd.FileSizeLimit = size;

            int count = sgLoginData.GetFileLimitCount();                // 전송가능한 파일의 최대 개수 
            if (count <= 0)
                count = 1024;
            sgUserAdd.FileCountLimit = count;

            size = sgLoginData.GetDayFileTransferLimitSize();           // 하루에 전송 가능한 파일 최대 크기
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

            sgUser.UserInfoAdded = sgUserAdd;

            DicUserInfo[groupID] = sgUser;
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