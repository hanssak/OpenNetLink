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


        private static void LoadFile()
        {
            string strNetworkFileName = "wwwroot/conf/NetWork.json";
            byte[] contents = File.ReadAllBytes(strNetworkFileName);
            try
            {
                string strContents = Encoding.UTF8.GetString(contents);
                bool isOriFile = strContents.ToUpper().Contains("NETWORKS");

                if (isOriFile == false)
                {
                    byte[] decContents = new byte[0];
                    SGCrypto.AESDecrypt256WithDEK(contents, ref decContents);
                    strContents = Encoding.UTF8.GetString(decContents);
                }

                try
                {
                    _netWorkInfo = networkParsing(strContents);
                }
                catch (Exception ex)
                {
                    CLog.Here().Error($"NetworkParsing err : Change ADDomain Format in Network.json  - {ex.ToString()}");
                    string[] strNetwork = strContents.Split(Environment.NewLine);
                    for (int i = 0; i < strNetwork.Length; i++)
                    {
                        if (strNetwork[i].Contains("ADDomain") && !(strNetwork[i].Contains("[") && strNetwork[i].Contains("]")))
                        {
                            string element = strNetwork[i].Split(':')[0];
                            string value = strNetwork[i].Split(':')[1];
                            strNetwork[i] = $"{element}: [ {value} ]";
                        }
                    }

                    strContents = string.Join(Environment.NewLine, strNetwork);

                    //포맷변경하여 Network.json 다시 저장
                    if (isOriFile)
                        File.WriteAllText(strNetworkFileName, strContents);
                    else
                    {
                        byte[] saveContetns = new byte[0];
                        SGCrypto.AESEncrypt256WithDEK(Encoding.UTF8.GetBytes(strContents), ref saveContetns);
                        File.WriteAllBytes(strNetworkFileName, saveContetns);
                    }
                    _netWorkInfo = networkParsing(strContents);
                }
            }
            catch (Exception ex)
            {
                HsLog.err("Load Exception : " + ex.ToString());
                throw;
            }
        }


        static List<ISGNetwork> networkParsing(string jsonString)
        {
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
            return listNetworks;
        }


    }
}