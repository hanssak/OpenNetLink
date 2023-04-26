using System;
using OpenNetLinkApp.Models.SGNetwork;
using System.IO;
using System.Text.Json;
using System.Text;
using System.Collections.Generic;
using HsNetWorkSG;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGNetworkService
    {
        List<ISGNetwork> NetWorkInfo { get { return NetWorkInfo; } }
    }
    internal class SGNetworkService : ISGNetworkService
    {
        public List<ISGNetwork> NetWorkInfo { get; set; } = null;
        public SGNetworkService()
        {
            string strNetworkFileName = "wwwroot/conf/NetWork.json";
            string jsonString = File.ReadAllText(strNetworkFileName);
            List<ISGNetwork> listNetworks = new List<ISGNetwork>();
            
            //ADdomain 포맷 변경으로 오류 발생 시, Network.json 파일을 수정 후 파싱 시도하도록 수정
            try { _networkParsing(); }
            catch (Exception ex)
            {
                string[] strNetwork = jsonString.Split("\r\n");
                for (int i = 0; i < strNetwork.Length; i++)
                {
                    if (strNetwork[i].Contains("ADDomain") && !(strNetwork[i].Contains("[") && strNetwork[i].Contains("]")))
                    {
                        string element = strNetwork[i].Split(':')[0];
                        string value = strNetwork[i].Split(':')[1];
                        strNetwork[i] = $"{element}: [ {value} ]";
                    }
                }
                File.WriteAllText(strNetworkFileName, string.Join("\r\n", strNetwork));
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
            NetWorkInfo = listNetworks;
        }

    }
}