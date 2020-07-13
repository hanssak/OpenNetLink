using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenNetLinkApp.Services
{
    class XmlConfService
    {
        XmlDocument m_Xml;
        string m_StrLanguage;
        public XmlConfService()
        {
            m_Xml = new XmlDocument();
            LoadXmlFile("wwwroot/conf/HSText.xml");
            m_StrLanguage = "KR";
           // m_StrLanguage = "JP";
        }
        ~XmlConfService()
        {

        }

        public void LoadXmlFile(string strFileName)
        {
            string strXmlData = System.IO.File.ReadAllText(strFileName);
            m_Xml.LoadXml(strXmlData);
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
    }
}
