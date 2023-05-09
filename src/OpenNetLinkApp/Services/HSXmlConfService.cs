using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using OpenNetLinkApp.Models.SGNetwork;
using System.Text.Json;

namespace OpenNetLinkApp.Services
{
    public class XmlConfService
    {
        XmlDocument m_Xml;
        string m_StrLanguage;
        List<ISGNetwork> listNetworks;
        public XmlConfService()
        {
            m_Xml = new XmlDocument();
            LoadXmlFile("wwwroot/conf/HSText.xml");
            m_StrLanguage = "KR";
            // m_StrLanguage = "JP";
            listNetworks = new List<ISGNetwork>();
            NetWorkJsonLoad();
        }
        ~XmlConfService()
        {

        }

        public void LoadXmlFile(string strFileName)
        {
            string strXmlData = System.IO.File.ReadAllText(strFileName);
            m_Xml.LoadXml(strXmlData);
        }

        public string GetCommon(string strID)
        {
            string str = "";
            XmlNodeList xnList = m_Xml.GetElementsByTagName("COMMON");
            if (m_StrLanguage == null)
                m_StrLanguage = "KR";
            foreach (XmlNode xn in xnList)
            {
                str = xn[strID][m_StrLanguage].InnerText;
            }
            return str;
        }

        public string GetTitle(string strID)
        {
            string str = "";
            XmlNodeList xnList = m_Xml.GetElementsByTagName("TITLE");
            if (m_StrLanguage == null)
                m_StrLanguage = "KR";
            foreach(XmlNode xn in xnList)
            {
                str = xn[strID][m_StrLanguage].InnerText;
            }
            return str;
        }
        public string GetInfoMsg(string strID)
        {
            string str = "";
            XmlNodeList xnList = m_Xml.GetElementsByTagName("INFO");
            if (m_StrLanguage == null)
                m_StrLanguage = "KR";
            foreach (XmlNode xn in xnList)
            {
                if (xn[strID][m_StrLanguage].InnerText != null)
                {
                    str = xn[strID][m_StrLanguage].InnerText;
                }
            }
            return str;
        }
        public string GetErrMsg(string strID)
        {
            string str = "";
            XmlNodeList xnList = m_Xml.GetElementsByTagName("ERROR");
            if (m_StrLanguage == null)
                m_StrLanguage = "KR";
            foreach (XmlNode xn in xnList)
            {
                str = xn[strID][m_StrLanguage].InnerText;
            }
            return str;
        }
        public string GetWarnMsg(string strID)
        {
            string str = "";
            XmlNodeList xnList = m_Xml.GetElementsByTagName("WARNING");
            if (m_StrLanguage == null)
                m_StrLanguage = "KR";
            foreach (XmlNode xn in xnList)
            {
                str = xn[strID][m_StrLanguage].InnerText;
            }
            return str;
        }
        public void SetLangType(string strLanguage)
        {
            m_StrLanguage = strLanguage;
        }

        public void NetWorkJsonLoad()
        {
            string strNetworkFileName = "wwwroot/conf/NetWork.json";
            string jsonString = File.ReadAllText(strNetworkFileName);
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
        public void GetNetworkTitle(int groupID,out string strFromName, out string strToName)
        {
            string str1 = "-";
            string str2 = "-";
            int count = listNetworks.Count;
            if (count <= 0)
            {
                strFromName = str1;
                strToName = str2;
                return;
            }
            for (int i = 0; i < count; i++)
            {
                int gID = listNetworks[i].GroupID;
                if(gID==groupID)
                {
                    str1 = listNetworks[i].FromName;
                    str2 = listNetworks[i].ToName;
                    break;
                }
            }

            strFromName = str1;
            strToName = str2;
            return;
        }
        public string GetNetworkTitle(int groupID)
        {
            string str1 = "-";
            string str2 = "-";
            int count = listNetworks.Count;
            if (count <= 0)
            {
                return "-";
            }
            for (int i = 0; i < count; i++)
            {
                int gID = listNetworks[i].GroupID;
                if (gID == groupID)
                {
                    str1 = listNetworks[i].FromName;
                    str2 = listNetworks[i].ToName;
                    break;
                }
            }

            str1 = str1 + " → " + str2;
            return str1;
        }      
    }
}
