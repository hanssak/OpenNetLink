using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using OpenNetLinkApp.Models.SGNetwork;
using System.Text.Json;
using AgLogManager;

namespace OpenNetLinkApp.Services
{
    public class XmlConfService
    {
        const string xmlFileName = "wwwroot/conf/HSText.xml";

        private static Serilog.ILogger CLog => Serilog.Log.ForContext<XmlConfService>();
        static XmlDocument m_Xml = null;
        static string m_StrLanguage = "KR";
        List<ISGNetwork> listNetworks = SGAppManager.SGNetworkService.NetWorkInfo;
        public XmlConfService()
        {
            //m_Xml = new XmlDocument();
            //LoadXmlFile("wwwroot/conf/HSText.xml");
            //m_StrLanguage = "KR";
            // m_StrLanguage = "JP";
            //listNetworks = new List<ISGNetwork>();
            //NetWorkJsonLoad();
        }
        ~XmlConfService()
        {

        }

        public void LoadXmlFile(string strFileName)
        {
            try
            {
                string strXmlData = System.IO.File.ReadAllText(strFileName);
                m_Xml = new XmlDocument();
                m_Xml.LoadXml(strXmlData);

                m_StrLanguage = SGAppManager.SGAppConfigService.AppConfigInfo.strLanguage;
                if (string.IsNullOrEmpty(m_StrLanguage))
                    m_StrLanguage = "KR";
            }
            catch (Exception ex)
            {
                CLog.Here().Error($"XmlConfService LoadFile(Path:{strFileName}) Exception :{ex.ToString()}");
                throw;
            }
        }

        public string GetCommon(string strID)
        {
            try
            {
                if (m_Xml == null)
                    LoadXmlFile(xmlFileName);

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
            catch (Exception ex)
            {
                CLog.Here().Error($"XmlConfService GetCommon(Text:{strID}) Exception :{ex.ToString()}");
                throw;
            }
        }

        public string GetTitle(string strID)
        {
            try
            {
                if (m_Xml == null)
                    LoadXmlFile(xmlFileName);

                string str = "";
                XmlNodeList xnList = m_Xml.GetElementsByTagName("TITLE");
                if (m_StrLanguage == null)
                    m_StrLanguage = "KR";
                foreach (XmlNode xn in xnList)
                {
                    str = xn[strID][m_StrLanguage].InnerText;
                }
                return str;
            }
            catch (Exception ex)
            {
                CLog.Here().Error($"XmlConfService GetTitle(Text:{strID}) Exception :{ex.ToString()}");
                throw;
            }
        }
        public string GetInfoMsg(string strID)
        {
            try
            {
                if (m_Xml == null)
                    LoadXmlFile(xmlFileName);

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
            catch (Exception ex)
            {
                CLog.Here().Error($"XmlConfService GetInfoMsg(Text:{strID}) Exception :{ex.ToString()}");
                throw;
            }
        }
        public string GetErrMsg(string strID)
        {
            try
            {
                if (m_Xml == null)
                    LoadXmlFile(xmlFileName);

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
            catch (Exception ex)
            {
                CLog.Here().Error($"XmlConfService GetErrMsg(Text:{strID}) Exception :{ex.ToString()}");
                throw;
            }

        }
        public string GetWarnMsg(string strID)
        {
            try
            {
                if (m_Xml == null)
                    LoadXmlFile(xmlFileName);

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
            catch (Exception ex)
            {
                CLog.Here().Error($"XmlConfService GetWarnMsg(Text:{strID}) Exception :{ex.ToString()}");
                throw;
            }
        }
        public void SetLangType(string strLanguage)
        {
            m_StrLanguage = strLanguage;
        }

        public void GetNetworkTitle(int groupID, out string strFromName, out string strToName)
        {
            try
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
                    if (gID == groupID)
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
            catch (Exception ex)
            {
                strFromName = strToName = "";
                CLog.Here().Error($"XmlConfService GetNetworkTitle Exception :{ex.ToString()}");
            }
        }
        public string GetNetworkTitle(int groupID)
        {
            try
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
            catch (Exception ex)
            {
                CLog.Here().Error($"XmlConfService GetNetworkTitle Exception :{ex.ToString()}");
                throw;
            }
        }

        public string[] GetMonthNamesGroup()
        {
            string group = GetTitle("T_DATAPICKER_MONTH_GROUP");
            return group.Split(",");
        }
        public string[] GetDayNamesGroup()
        {
            string group = GetTitle("T_DATAPICKER_DAY_GROUP");
            return group.Split(",");
        }
        public string[] GetDayMinNamesGroup()
        {
            string group = GetTitle("T_DATAPICKER_DAY_MIN_GROUP");
            return group.Split(",");
        }
    }
}
