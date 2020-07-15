using System;
using OpenNetLinkApp.Models.SGNetwork;
using System.IO;
using System.Text.Json;
using System.Text;
using System.Collections.Generic;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGNetworkService
    {
        List<SGNetwork> NetWorkInfo { get; }
    }
    internal class SGNetworkService : ISGNetworkService
    {
        public SGNetworkService()
        {
            string strNetworkFileName = "wwwroot/conf/NetWork.json";
            string jsonString = File.ReadAllText(strNetworkFileName);
            List<SGNetwork> listNetworks = new List<SGNetwork>();
            using (JsonDocument document = JsonDocument.Parse(jsonString))
            {
                JsonElement root = document.RootElement;
                JsonElement NetWorkElement = root.GetProperty("NETWORKS");
                //JsonElement Element;
                foreach (JsonElement netElement in NetWorkElement.EnumerateArray())
                {
                    SGNetwork sgNet = new SGNetwork();
                    string strJsonElement= netElement.ToString();
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
            NetWorkInfo = listNetworks;
        }

        /* To Manage User Info State */
        public List<SGNetwork> NetWorkInfo { get; set; } = null;
    }
}