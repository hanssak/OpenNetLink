using System;
using System.Collections.Generic;
using OpenNetLinkApp.Models.SGUserInfo;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGAppManagerService
    {
        /* To Manage CI State */
        /// <summary>
        /// CIPath is CI Image path.
        /// </summary>
        string CIPath { get; } 
        /// <summary>
        /// </summary>
        /// <param name="ciPath"> 
        /// </param>
        /// <returns>void</returns>
        void SetCIPath(string ciPath);

        /* To Manage User State */
        /// <summary>
        /// Config User Information.
        /// </summary>
        ISGUserInfo UserInfo { get; }
        /// <summary>
        /// </summary>
        /// <param name="userInfo"> 
        /// </param>
        /// <returns>void</returns>
        void SetUserInfo(ISGUserInfo userInfo);

        /* To Manage Active Menu State */
        /// <summary>
        /// Declared: Menu Action Service for UI SideBar, included ISGSideBarUI(SGSideBarUI)
        /// </summary>
        ISGSideBarUIService SideBarUIService { get; }

        /* To Manage Header State */
        /// <summary>
        /// get / set HeaderUI Property
        /// </summary>
        ISGHeaderUI Header { get; } 
        /// <summary>
        /// </summary>
        /// <param name="header"> 
        /// </param>
        /// <returns>void</returns>
        void SetHeaderUI(ISGHeaderUI header);
    }
    internal class SGAppManagerService : ISGAppManagerService
    {
        public SGAppManagerService()
        {
            SideBarUIService = new SGSideBarUIService();
        }

        /* To Manage CI State */
        public string CIPath { get; private set; } = String.Empty;
        public event Action OnChangeCI;
        private void NotifyStateChangedCI() => OnChangeCI?.Invoke();
        public void SetCIPath(string ciPath)
        {
            CIPath = ciPath;
            NotifyStateChangedCI();
        }

        /* To Manage User State */
        public ISGUserInfo UserInfo { get; private set; } = null;
        public event Action OnChangeUserInfo;
        private void NotifyStateChangedUserInfo() => OnChangeUserInfo?.Invoke();
        public void SetUserInfo(ISGUserInfo userInfo)
        {
            UserInfo = userInfo;
            NotifyStateChangedUserInfo();
        }

        /* To Manage Active Menu State */
        /// Declared: Menu Action Service for UI SideBar, included ISGSideBarUI(SGSideBarUI)
        public ISGSideBarUIService SideBarUIService { get; private set; } = null;

        /* To Manage Header State */
        public ISGHeaderUI Header { get; private set; } 
        public event Action OnChangeHeader;
        private void NotifyStateChangedHeader() => OnChangeHeader?.Invoke();
        public void SetHeaderUI(ISGHeaderUI header)
        {
            Header = header;
            NotifyStateChangedHeader();
        }
    }
}