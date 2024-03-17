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
        eLOGINTYPE_OTP = 5,
        eLOGINTYPE_SSO = 6,
        eLOGINTYPE_NAC = 7,
        eLOGINTYPE_SSO2 = 8,
        eLOGINTYPE_GPKI = 9,
        eLOGINTYPE_GOOGLE_OTP = 10,
        eLOGINTYPE_CUSTOM = 11,
    }
    public class SGReadyData : SGData
    {
        public SGReadyData()
        {

        }
        ~SGReadyData()
        {

        }
        public void Copy(SGData data)
        {
            m_DicTagData = new Dictionary<string, object>(data.m_DicTagData);
            m_DicRecordData = new List<Dictionary<int, string>>(data.m_DicRecordData);
        }
        /// <summary>
        /// 서버에서 처리하는 망 종류. 택1: IN(내부), EX(외부).
        /// </summary>
        /// <returns>true 내부, false 외부</returns>
        public string GetSystemPositionString() => GetTagData("server_info", "sg_net", "type");

        /// <summary>
        /// 접속된 서버의 SgNetType
        /// <para>"server_info", "sg_net", "type"</para>
        /// </summary>
        /// <returns></returns>
        public string GetSgNetType() => GetTagData("server_info", "sg_net", "type");
        /// <summary>
        /// 현재 내부망에 접속되어 있는지 여부를 반환
        /// </summary>
        /// <returns>true 내부, false 외부</returns>
        public bool GetSystemPosition()
        {
            //서버에서 처리하는 망 종류. 택1: IN(내부), EX(외부).
            string strData = GetSystemPositionString();
            return strData.StartsWith("I");
        }



        public eLoginType GetLoginType()
        {
            switch (GetLoginTypeStr())
            {

                case "ORIGIN": return eLoginType.eLOGINTYPE_ORIGIN;
                case "AD": return eLoginType.eLOGINTYPE_AD;
                case "AD_LDAP": return eLoginType.eLOGINTYPE_AD_LDAP;
                case "LDAP": return eLoginType.eLOGINTYPE_LDAP;
                case "PW_OTP": return eLoginType.eLOGINTYPE_PW_OTP;
                case "OTP": return eLoginType.eLOGINTYPE_OTP;
                case "SSO": return eLoginType.eLOGINTYPE_SSO;
                case "NAC": return eLoginType.eLOGINTYPE_NAC;
                case "SSO2": return eLoginType.eLOGINTYPE_SSO2;
                case "GPKI": return eLoginType.eLOGINTYPE_GPKI;
                case "GOOGLE_OTP": return eLoginType.eLOGINTYPE_GOOGLE_OTP;
                case "CUSTOM": return eLoginType.eLOGINTYPE_CUSTOM;
                default: return eLoginType.eLOGINTYPE_ORIGIN;
            }
        }

        /// <summary>
        /// Json에서는 소문자로 구성되어 있으나, 기존UI사용을 위해 대문자로 자동변환하여 반환
        /// </summary>
        /// <returns></returns>
        public string GetLoginTypeStr()
        {
            //string strLoginType = GetSvrTagData("LOGINTYPE");
            string strLoginType = GetTagData("server_info", "login_type");
            return strLoginType.ToUpper();
        }

        /// <summary>
        /// 검사 결과 예외 처리 결재 추가 사용 여부
        /// </summary>
        /// <returns></returns>
        public bool GetExApprovalUse()
        {
            string strData = GetTagData("server_info", "ex_approval_use");
            return (strData.ToUpper() == "TRUE");
        }

        /// <summary>
        /// 서버명을 반환
        /// </summary>
        /// <returns>서버명</returns>
        public string GetServName()
        {
            string strData = GetTagData("server_info", "name");
            return strData;
        }



    }
}
