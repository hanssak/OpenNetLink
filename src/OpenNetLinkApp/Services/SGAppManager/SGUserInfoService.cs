using System;
using OpenNetLinkApp.Data.SGDicData.SGUnitData;
using OpenNetLinkApp.Models.SGUserInfo;
using System.Collections.Generic;
using HsNetWorkSGData;

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
        void SetUserInfo(int groupID, SGLoginData sgLoginData, SGUserData sgUserData, SGData sfmData);
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
        private void NotifyStateChangedUserInfo() => OnChangeUserInfo?.Invoke();
        public void SetUserInfo(int groupID, SGLoginData sgLoginData, SGUserData sgUserData, SGData sfmData)
        {
            SGUserInfo sgUser = new SGUserInfo();
            sgUser.UserId = sgLoginData.GetUserID();                // 사용자 ID
            sgUser.UserName = sgUserData.GetUserName();             // 사용자 이름
            sgUser.UserSeq = sgUserData.GetUserSequence();          // 사용자 SEQ
            sgUser.DeptName = sgUserData.GetTeamName();             // 부서명
            sgUser.Position = sgUserData.GetUserPosition();         // 직책
            sgUser.Rank = sgUserData.GetRank();                     // 직위
            sgUser.ManOrSteff = sgUserData.GetPartOwner();          // 팀원/팀장 여부 ( 팀원 : 1, 팀장 : 2)
            sgUser.ApprPos = sgUserData.GetUserApprpos();           // 일반사용자, 결재자, 전결자

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
            string headUserName = sgUserData.GetUserName();
            if (sfmData != null)
            {
                List<Dictionary<int, string>> listDicSfmdata = null;
                listDicSfmdata = sfmData.GetSvrRecordData("RECORD");
                if (listDicSfmdata == null || listDicSfmdata.Count == 0)
                {
                    isMySelfSFM = true;
                    sfmRight = 0;
                }
                else
                {
                    if(listDicSfmdata.Count > 1)
                    {
                        headUserName = $"{sgUserData.GetUserName()}({listDicSfmdata[0][3]} {listDicSfmdata[0][4]} 대결재 외 {listDicSfmdata.Count - 1})";
                    }
                    else
                    {
                        headUserName = $"{sgUserData.GetUserName()}({listDicSfmdata[0][3]} {listDicSfmdata[0][4]} 대결재)";
                    }

                    foreach(Dictionary<int, string> value in listDicSfmdata)
                    {
                        if (sfmRight < Convert.ToInt32(value[0]))
                            sfmRight = Convert.ToInt32(value[0]);
                    }

                    isMySelfSFM = false;
                }
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