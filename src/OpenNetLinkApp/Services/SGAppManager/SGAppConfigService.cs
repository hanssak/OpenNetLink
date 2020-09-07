using System;
using OpenNetLinkApp.Models.SGConfig;
using System.IO;
using System.Text.Json;
using System.Text;
using System.Collections.Generic;
using HsNetWorkSG;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGAppConfigService
    {
        ISGAppConfig AppConfigInfo { get; }
    }
    internal class SGAppConfigService : ISGAppConfigService
    {
        public ISGAppConfig AppConfigInfo { get; private set; } = null;
        public SGAppConfigService()
        {
            AppConfigInfo = new SGAppConfig();
        }
    
    }
}