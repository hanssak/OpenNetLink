using System;
using System.Collections.Generic;
using OpenNetLinkApp.Models.SGUserInfo;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGAppManagerService
    {
        /* To Manage Header State */
        /// <summary>
        /// Declared: Header Action Service for UI Header, included ISGHeaderUI(SGHeaderUI)
        /// </summary>
        ISGHeaderUIService HeaderUIService { get; }

        /* To Manage Corporate Identity State */
        /// <summary>
        /// Declared: Corporate Identity(CI) Service for CI Info/Image.
        /// </summary>
        ISGCorpIdUIService CorpIdUIService { get; }

        /* To Manage User Info State */
        /// <summary>
        /// Declared: User Info & Configuration Service for UI & App Controlling, included ISGUserInfo(SGUserInfo)
        /// </summary>
        ISGUserInfoService UserInfoService { get; }

        /* To Manage Active Menu State */
        /// <summary>
        /// Declared: Menu Action Service for UI SideBar, included ISGSideBarUI(SGSideBarUI)
        /// </summary>
        ISGSideBarUIService SideBarUIService { get; }
    }
    internal class SGAppManagerService : ISGAppManagerService
    {
        public SGAppManagerService()
        {
            SideBarUIService = new SGSideBarUIService();
        }

        /* To Manage Header State */
        public ISGHeaderUIService HeaderUIService { get; private set; } = null;

        /* To Manage Corporate Identity State */
        public ISGCorpIdUIService CorpIdUIService { get; private set; } = null;

        /* To Manage User Info State */
        public ISGUserInfoService UserInfoService { get; private set; } = null;

        /* To Manage Active Menu State */
        public ISGSideBarUIService SideBarUIService { get; private set; } = null;
    }
}