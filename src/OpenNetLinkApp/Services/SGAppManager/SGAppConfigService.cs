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
        List<ISGAppConfig> UserConfigInfo { get { return UserConfigInfo; } }
    }
    internal class SGAppConfigService : ISGAppConfigService
    {
        public List<SGAppConfig> UserConfigInfo { get; set; } = null;
        public SGAppConfigService()
        {

        }
    
    }
}