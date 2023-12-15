using System;
using OpenNetLinkApp.Models.SGNetwork;
using System.IO;
using System.Text.Json;
using System.Text;
using System.Collections.Generic;
using HsNetWorkSG;
using AgLogManager;


namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGNetworkService
    {
        List<ISGNetwork> NetWorkInfo { get { return GetSGNetworkService(); } }
        List<ISGNetwork> GetSGNetworkService();
        public void SaveIPAndReload(string IP);
    }
    internal class SGNetworkService : ISGNetworkService
    {
        /// <summary>ISGNetworkService 에서 사용</summary>
        public List<ISGNetwork> GetSGNetworkService() => NetWorkInfo;

        private static List<ISGNetwork> _netWorkInfo { get; set; } = null;
        public static List<ISGNetwork> NetWorkInfo
        {
            get
            {
                if (_netWorkInfo == null) LoadFile();
                return _netWorkInfo;
            }
        }

        private static Serilog.ILogger CLog => Serilog.Log.ForContext<SGNetworkService>();
        //public List<ISGNetwork> NetWorkInfo { get; set; } = null;

        public static void LoadFile()
        {
            loadNetworkFile();
        }

        public void SaveIPAndReload(string IP)
        {
            string strNetworkFileName = "wwwroot/conf/NetWork.json";

            string jsonString = File.ReadAllText(strNetworkFileName);

            int i = 0;
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
                    if(  NetWorkInfo.Count > i)
                    {
                        NetWorkInfo[i].FromName = sgNet.FromName;
                        NetWorkInfo[i].ToName = sgNet.ToName;
                    }
                    i++;
                }
            }

            NetWorkInfo[0].IPAddress = IP;
            

            SGNetworkForSave saveFormat = new SGNetworkForSave();
            saveFormat.NETWORKS = NetWorkInfo;

            var opt = new JsonSerializerOptions() { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            var json = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes<SGNetworkForSave>(saveFormat, opt);

            jsonString = Encoding.UTF8.GetString(json);

            File.WriteAllText(strNetworkFileName, jsonString);

            loadNetworkFile(true);
        }

        private static void loadNetworkFile(bool isReload = false)
        {
            string strNetworkFileName = "wwwroot/conf/NetWork.json";
            try
            {
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
                
                if(isReload)
                {
                    //Reload 시에는 이미 해당 변수를 참조하는 곳이 있으므로,
                    //객체를 재생성하지 않고 유지
                    _netWorkInfo?.Clear();
                    _netWorkInfo?.AddRange(listNetworks);
                }
                else
                {
                    _netWorkInfo = listNetworks;
                }
                
            }
            catch (Exception ex)
            {
                CLog.Here().Error($"SGNetworkService loadNetworkFile(Path:{strNetworkFileName}) Exception :{ex.ToString()}");
            }
        }

    }
}