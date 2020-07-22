using System;
using System.Collections.Generic;
using OpenNetLinkApp.Models.SGUserInfo;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGUserInfoService
    {
        /* To Manage User Info State */
        /// <summary>
        /// Config User Information.
        /// </summary>
        Dictionary<int, ISGUserInfo> DicUserInfo { get; }
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
        void SetUserInfo(int groupNo, ISGUserInfo userInfo);
    }
    internal class SGUserInfoService : ISGUserInfoService
    {
        public SGUserInfoService()
        {
        }

        /* To Manage User Info State */
        public Dictionary<int, ISGUserInfo> DicUserInfo { get; set; } = null;
        public event Action OnChangeUserInfo;
        private void NotifyStateChangedUserInfo() => OnChangeUserInfo?.Invoke();
        public void SetUserInfo(int groupNo, ISGUserInfo userInfo )
        {
            DicUserInfo[groupNo] = userInfo;
            NotifyStateChangedUserInfo();
        }
    }
}