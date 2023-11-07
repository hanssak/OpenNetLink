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
            NetWorkInfo[0].IPAddress = IP;

            SGNetworkForSave saveFormat = new SGNetworkForSave();
            saveFormat.NETWORKS = NetWorkInfo;

            var opt = new JsonSerializerOptions() { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            var json = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes<SGNetworkForSave>(saveFormat, opt);

            string jsonString = Encoding.UTF8.GetString(json);

            File.WriteAllText(strNetworkFileName, jsonString);

            loadNetworkFile();
        }

        private static void loadNetworkFile()
        {
            string strNetworkFileName = "wwwroot/conf/NetWork.json";
            try
            {
                string jsonString = File.ReadAllText(strNetworkFileName);
                List<ISGNetwork> listNetworks = new List<ISGNetwork>();

                //ADDomain 이 string 타입인 Network.json은 List<string> 타입으로 수정
                try
                {
                    _networkParsing();
                }
                catch (Exception ex)
                {
                    CLog.Here().Error($"NetworkParsing err : Change ADDomain Format in Network.json  - {ex.ToString()}");
                    string[] strNetwork = jsonString.Split(Environment.NewLine);
                    for (int i = 0; i < strNetwork.Length; i++)
                    {
                        if (strNetwork[i].Contains("ADDomain") && !(strNetwork[i].Contains("[") && strNetwork[i].Contains("]")))
                        {
                            string element = strNetwork[i].Split(':')[0];
                            string value = strNetwork[i].Split(':')[1];
                            strNetwork[i] = $"{element}: [ {value} ]";
                        }
                    }
                    File.WriteAllText(strNetworkFileName, string.Join(Environment.NewLine, strNetwork));
                    jsonString = string.Join(Environment.NewLine, strNetwork);
                    _networkParsing();
                }

                void _networkParsing()
                {
                    listNetworks.Clear();
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
                }
                _netWorkInfo = listNetworks;
            }
            catch (Exception ex)
            {
                CLog.Here().Error($"SGNetworkService loadNetworkFile(Path:{strNetworkFileName}) Exception :{ex.ToString()}");
            }
        }

    }
}