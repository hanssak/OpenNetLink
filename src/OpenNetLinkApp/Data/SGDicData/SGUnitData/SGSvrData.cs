using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using HsNetWorkSGData;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{

    [Flags]
    public enum eLoginType
    {
        eLOGINTYPE_ORIGIN = 0,
        eLOGINTYPE_AD = 1,
        eLOGINTYPE_AD_LDAP = 2,
        eLOGINTYPE_LDAP = 3,
        eLOGINTYPE_PW_OTP = 4,
        eLOGINTYPE_OTP=5,
        eLOGINTYPE_SSO=6,
        eLOGINTYPE_NAC=7,
        eLOGINTYPE_SSO2=8,
        eLOGINTYPE_GPKI=9,
        eLOGINTYPE_GOOGLE_OTP=10,
        eLOGINTYPE_CUSTOM = 11,
    }
    public class SGSvrData : SGData
    {
        public SGSvrData()
        {

        }
        ~SGSvrData()
        {

        }

        public string GetSvrTagData(string strTag)
        {
            return m_DicTagData[strTag].ToString();
        }
        public void Copy(SGData data)
        {
            m_DicTagData = new Dictionary<string, object>(data.m_DicTagData);
            m_DicRecordData = new List<Dictionary<int, string>>(data.m_DicRecordData);
        }

        public eLoginType GetLoginType()
        {
            //string strLoginType = GetSvrTagData("LOGINTYPE");
            string strLoginType = GetBasicTagData("LOGINTYPE");
            if(!strLoginType.Equals(""))
            {
                return (eLoginType)Convert.ToInt32(strLoginType);
            }
            return eLoginType.eLOGINTYPE_ORIGIN;
        }

        public string GetLoginTypeStr()
        {
            //string strLoginType = GetSvrTagData("LOGINTYPE");
            string strLoginType = GetBasicTagData("LOGINTYPE");
            if(!strLoginType.Equals(""))
            {
                switch((eLoginType)Convert.ToInt32(strLoginType))
                {
                    case eLoginType.eLOGINTYPE_ORIGIN:     return "ORIGIN";
                    case eLoginType.eLOGINTYPE_AD:         return "AD";
                    case eLoginType.eLOGINTYPE_AD_LDAP:    return "AD_LDAP";
                    case eLoginType.eLOGINTYPE_LDAP:       return "LDAP";
                    case eLoginType.eLOGINTYPE_PW_OTP:     return "PW_OTP";
                    case eLoginType.eLOGINTYPE_OTP:        return "OTP";
                    case eLoginType.eLOGINTYPE_SSO:        return "SSO";
                    case eLoginType.eLOGINTYPE_NAC:        return "NAC";
                    case eLoginType.eLOGINTYPE_SSO2:       return "SSO2";
                    case eLoginType.eLOGINTYPE_GPKI:       return "GPKI";
                    case eLoginType.eLOGINTYPE_GOOGLE_OTP: return "GOOGLE_OTP";
                    case eLoginType.eLOGINTYPE_CUSTOM:     return "CUSTOM";
                    default:                               return "ORIGIN";
                }
            }
            return "ORIGIN";
        }
    }
}
