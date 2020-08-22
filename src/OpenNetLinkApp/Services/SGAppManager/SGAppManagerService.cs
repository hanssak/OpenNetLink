using System;
using System.Collections.Generic;
using OpenNetLinkApp.Models.SGUserInfo;
using OpenNetLinkApp.Models.SGNetwork;
using OpenNetLinkApp.Models.SGConfig;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGAppManagerService
    {
        /* To Manage Header State */
        /// <summary>
        /// Declared: Header Action Service for UI Header, included ISGHeaderUI(SGHeaderUI)
        /// </summary>
        ISGHeaderUIService HeaderUIService { get; }

        /* To Manage Footer State */
        /// <summary>
        /// Declared: Footer Action Service for UI Footer, included ISGFooterUI(SGFooterUI)
        /// </summary>
        ISGFooterUIService FooterUIService { get; }

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

        ISGNetworkService NetworkInfoService { get; }

        ISGUserConfigService UserConfigInfoService { get; }
        ISGSiteConfigService SiteConfigInfoService { get; }
    }
    internal class SGAppManagerService : ISGAppManagerService
    {
        public SGAppManagerService()
        {
            HeaderUIService = new SGHeaderUIService();
            FooterUIService = new SGFooterUIService();
            CorpIdUIService = new SGCorpIdUIService();
            UserInfoService = new SGUserInfoService();
            SideBarUIService = new SGSideBarUIService();
            NetworkInfoService = new SGNetworkService();
            UserConfigInfoService = (ISGUserConfigService)new SGUserConfigService();
            SiteConfigInfoService = (ISGSiteConfigService)new SGSiteConfigService();
        }

        /* To Manage Header State */
        public ISGHeaderUIService HeaderUIService { get; private set; } = null;

        /* To Manage Footer State */
        public ISGFooterUIService FooterUIService { get; private set; } = null;

        /* To Manage Corporate Identity State */
        public ISGCorpIdUIService CorpIdUIService { get; private set; } = null;

        /* To Manage User Info State */
        public ISGUserInfoService UserInfoService { get; private set; } = null;

        /* To Manage Active Menu State */
        public ISGSideBarUIService SideBarUIService { get; private set; } = null;

        public ISGNetworkService NetworkInfoService { get; private set; } = null;

        public ISGUserConfigService UserConfigInfoService { get; private set; } = null;
        public ISGSiteConfigService SiteConfigInfoService { get; private set; } = null;
    }
}