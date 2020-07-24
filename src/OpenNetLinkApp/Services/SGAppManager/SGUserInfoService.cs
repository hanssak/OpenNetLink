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
        private void NotifyStateChangedUserInfo() => OnChangeUserInfo?.Invoke();
        public void SetUserInfo(int groupID, SGLoginData sgLoginData, SGUserData sgUserData)
        {
            SGUserInfo sgUser = new SGUserInfo();
            sgUser.UserId = sgLoginData.GetUserID();                // ����� ID
            sgUser.UserName = sgUserData.GetUserName();             // ����� �̸�
            sgUser.DeptName = sgUserData.GetTeamName();             // �μ���
            sgUser.Position = sgUserData.GetUserPosition();         // ��å
            sgUser.Rank = sgUserData.GetRank();                     // ����
            sgUser.ManOrSteff = sgUserData.GetPartOwner();          // ����/���� ���� ( ���� : 1, ���� : 2)

            SGUserInfoAdded sgUserAdd = new SGUserInfoAdded();
            sgUserAdd.FileFilterExt = sgLoginData.GetFileFilter();     // ���� Ȯ���� ����.

            Int64 size = sgLoginData.GetFileLimitSize();                // ���� ���� ������ ���� (���� MB)
            if (size <= 0)
                size = 1536;
            sgUserAdd.FileSizeLimit = size;

            int count = sgLoginData.GetFileLimitCount();                // ���۰����� ������ �ִ� ���� 
            if (count <= 0)
                count = 1024;
            sgUserAdd.FileCountLimit = count;

            size = sgLoginData.GetDayFileTransferLimitSize();           // �Ϸ翡 ���� ������ ���� �ִ� ũ��
            if (size <= 0)
                size = 1536;
            sgUserAdd.DayFileSizeLimit = size;

            count = sgLoginData.GetDayFileTransferLimitCount();         // �Ϸ翡 ���� ������ ���� �ִ� ȸ�� 
            if (count <= 0)
                count = 1024;
            sgUserAdd.DayFileCountLimit = count;

            size = sgLoginData.GetClipboardLimitSize();                 // �ѹ��� ���� ������ Ŭ������ �ִ� ũ��
            if (size <= 0)
                size = 1536;
            sgUserAdd.ClipSizeLimit = size;

            size = sgLoginData.GetDayClipboardLimitSize();              // �Ϸ翡 ���� ������ Ŭ������ �ִ� ũ��.
            if (size <= 0)
                size = 1536;
            sgUserAdd.DayClipSizeLimit = size;

            count = sgLoginData.GetDayClipboardLimitCount();            // �Ϸ翡 ���� ������ Ŭ������ �ִ� ȸ��.
            if (count <= 0)
                count = 1024;
            sgUserAdd.DayClipCountLimit = count;

            count = sgLoginData.GetMaxDownCount();                      // �ٿ�ε� ���� Ƚ��
            if (count <= 0)
                count = 1;
            sgUserAdd.MaxDownloadCount = count;

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