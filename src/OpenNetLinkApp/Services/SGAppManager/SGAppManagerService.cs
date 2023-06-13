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

        /* To Manage NetworkInfo State */
        /// <summary>
        /// Declared: Network Info Service for UI & App Controlling, included ISGNetworkService(SGNetworkService)
        /// </summary>
        ISGNetworkService NetworkInfoService { get; }

        /* To Manage ControlSide State */
        /// <summary>
        /// Declared: Control Right SideBar Menu, Related App Environment Setting.
        /// </summary>
        ISGCtrlSideUIService CtrlSideUIService { get; }

        /* To Manage AppConfigInfo State */
        /// <summary>
        /// Declared: AppConfigInfo Service for UI & App Controlling, included ISGAppConfigService(SGAppConfigService)
        /// </summary>
        ISGAppConfigService AppConfigInfoService { get; }
        ISGVersionConfigService VersionConfigInfoService { get; }

        /* To Manage SiteConfigInfo State */
        /// <summary>
        /// Declared: SiteConfigInfo Service for UI & App Controlling, included ISGSiteConfigService(SGSiteConfigService)
        /// </summary>
        ISGopConfigService OpConfigInfoService { get; }

    }
    internal class SGAppManagerService : ISGAppManagerService
    {
        public SGAppManagerService()
        {
            //OP 파일 재 암호화 => 암복호화 실패 시 재설치
            //Network 파일 재 암호화 => 암복호화 실패 시 재설치

            HsNetWorkSG.SGCrypto.InitKeyGenerate();

            HeaderUIService = new SGHeaderUIService();
            FooterUIService = new SGFooterUIService();
            CorpIdUIService = new SGCorpIdUIService();
            UserInfoService = new SGUserInfoService();
            SideBarUIService = new SGSideBarUIService();
            NetworkInfoService = new SGNetworkService();
            AppConfigInfoService = new SGAppConfigService();
            OpConfigInfoService = new SGopConfigService();
            VersionConfigInfoService = new SGVersionConfigService();
            CtrlSideUIService = new SGCtrlSideUIService(ref AppConfigInfoService.AppConfigInfo, ref OpConfigInfoService.AppConfigInfo, ref VersionConfigInfoService.VersionConfigInfo, NetworkInfoService.NetWorkInfo);
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

        /* To Manage NetworkInfo State */
        public ISGNetworkService NetworkInfoService { get; private set; } = null;

        /* To Manage ControlSide State */
        public ISGCtrlSideUIService CtrlSideUIService { get; private set; } = null;

        /* To Manage AppConfigInfo State */
        public ISGAppConfigService AppConfigInfoService { get; private set; } = null;

        public ISGopConfigService OpConfigInfoService { get; private set; } = null;
        public ISGVersionConfigService VersionConfigInfoService { get; private set; } = null;
    }
}