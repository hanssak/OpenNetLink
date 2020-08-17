using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using HsNetWorkSGData;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
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
        eLOGINTYPE_GOOGLE_OTP=10
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
            return m_DicTagData[strTag];
        }
        public void Copy(SGData data)
        {
            m_DicTagData = new Dictionary<string, string>(data.m_DicTagData);
            m_DicRecordData = new List<Dictionary<int, string>>(data.m_DicRecordData);
        }

        public eLoginType GetLoginType()
        {
            string strLoginType = GetSvrTagData("LOGINTYPE");
            if(!strLoginType.Equals(""))
            {
                return (eLoginType)Convert.ToInt32(strLoginType);
            }
            return eLoginType.eLOGINTYPE_ORIGIN;
        }

    }
}
