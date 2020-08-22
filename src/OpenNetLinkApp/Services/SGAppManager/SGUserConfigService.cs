using System;
using OpenNetLinkApp.Models.SGConfig;
using System.IO;
using System.Text.Json;
using System.Text;
using System.Collections.Generic;
using HsNetWorkSG;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGUserConfigService
    {
        List<ISGUserConfig> UserConfigInfo { get { return UserConfigInfo; } }
    }
    internal class SGUserConfigService : ISGUserConfigService
    {
        public List<SGUserConfig> UserConfigInfo { get; set; } = null;
        public SGUserConfigService()
        {

        }
    
    }
}