using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Serilog;
using Serilog.Events;
using AgLogManager;
using System.Collections.Generic;
using System.Linq;

namespace OpenNetLinkApp.Data.SGDicData.SGNpki
{ 
    
    public class NPKICA
    {
        Dictionary<string, string> DicNpkiCA = new Dictionary<string, string>();
        public void SetData(string tag, string value)
        {
            string strTmp = "";
            if (DicNpkiCA.TryGetValue(tag, out strTmp) == true)
            {
                DicNpkiCA.Remove(tag);
            }
            DicNpkiCA[tag] = value;
        }
        public string GetData(string tag)
        {
            string strTmp = "";
            if (DicNpkiCA.TryGetValue(tag, out strTmp) != true)
            {
                return strTmp;
            }
            return DicNpkiCA[tag];
        }
    }
    public class NPKIFileInfo
    {
        public string m_strFileName;            // GPKI 인증서 파일(Cer)명
        public string m_strKeyFilePath;            // GPKI 인증서 파일(Key)명
        public string m_strUserID;              // GPKI 인증서 ID
        public string m_strExpiredDate;         // GPKI 인증서 만료일자
        public string m_strKeyUse;              // GPKI 인증서 사용 용도.
        public string m_strOrg;                 // GPKI 인증서 발급기관.
        //public GPKICA m_gpkiCA;                 // GPKI 인증서 발급기관 상세정보.
        //public string m_strOID;                   // GPKI 인증서 OID
        public int m_nRemainDay;                  // GPKI 인증서 남은 유효 기간
        public string m_selected { get; set; }
        public bool m_bIsRegisteredServer;         // Server에 CN이 등록되어 있는지 유무
        public byte[] m_pKeyData;


        public NPKIFileInfo()
        {
            m_strFileName = m_strUserID = m_strKeyUse = m_strOrg = "";
            m_nRemainDay = 0;
            m_selected = "";
            //m_strOID = "";
            //m_gpkiCA = new GPKICA();
            m_pKeyData = null;
        }
        public void SetNPKIInfo(string userID, string expiredDate, string KeyUse, string Org, int nRemainDay)
        {
            m_strUserID = userID;               // GPKI 인증서 사용자 계정.
            m_strExpiredDate = expiredDate;     // GPKI 인증서 만료일자
            m_strKeyUse = KeyUse;               // GPKI 인증서 사용 용도.
            m_strOrg = Org;                     // GPKI 인증서 발급 기관.
            m_nRemainDay = nRemainDay;          // GPKI 인증서 사용가능 날짜
        }

    }

}
