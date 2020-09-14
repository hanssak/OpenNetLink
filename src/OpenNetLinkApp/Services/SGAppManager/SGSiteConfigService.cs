using System;
using OpenNetLinkApp.Models.SGConfig;
using System.IO;
using System.Text.Json;
using System.Text;
using System.Collections.Generic;
using HsNetWorkSG;
using OpenNetLinkApp.Models.SGNetwork;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGSiteConfigService
    {
        public List<ISGSiteConfig> SiteConfigInfo { get;}
    }
    internal class SGSiteConfigService : ISGSiteConfigService
    {
        public List<ISGSiteConfig> SiteConfigInfo { get; set; } = null;
        public SGSiteConfigService()
        {
            SiteConfigInfo = new List<ISGSiteConfig>();
            string strNetworkFileName = "wwwroot/conf/NetWork.json";
            string jsonString = File.ReadAllText(strNetworkFileName);
            List<ISGNetwork> listNetworks = new List<ISGNetwork>();
            using (JsonDocument document = JsonDocument.Parse(jsonString))
            {
                JsonElement root = document.RootElement;
                JsonElement NetWorkElement = root.GetProperty("NETWORKS");
                //JsonElement Element;
                foreach (JsonElement netElement in NetWorkElement.EnumerateArray())
                {
                    SGNetwork sgNet = new SGNetwork();
                    string strJsonElement = netElement.ToString();
                    var options = new JsonSerializerOptions
                    {
                        ReadCommentHandling = JsonCommentHandling.Skip,
                        AllowTrailingCommas = true,
                        PropertyNameCaseInsensitive = true,
                    };
                    sgNet = JsonSerializer.Deserialize<SGNetwork>(strJsonElement, options);
                    listNetworks.Add(sgNet);
                }
            }
            int count = listNetworks.Count;
            for(int i=0;i<count; i++)
            {
                SGSiteConfig sgSiteConfig = new SGSiteConfig();
                sgSiteConfig.nPWChangeApplyCnt = 9;
                SiteConfigInfo.Add(sgSiteConfig);
            }
            
        }
    
    }
}