using System;
using OpenNetLinkApp.Models.SGConfig;
using System.IO;
using System.Text.Json;
using System.Text;
using System.Collections.Generic;
using HsNetWorkSG;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGSiteConfigService
    {
        List<ISGSiteConfig> SiteConfigInfo { get { return SiteConfigInfo; } }
    }
    internal class SGSiteConfigService : ISGUserConfigService
    {
        public List<SGSiteConfig> SiteConfigInfo { get; set; } = null;
        public SGSiteConfigService()
        {
        }
    
    }
}