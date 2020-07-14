using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using HsNetWorkSG;

namespace SGData
{
    class SGDicData
    {
        public Dictionary<string, string> m_DicTagData = new Dictionary<string, string>();
        public List<Dictionary<int, string>> m_DicRecordData = new List<Dictionary<int, string>>();

        public void SetTagData(string strKey, string strVal)
        {
            m_DicTagData[strKey] = strVal;
        }
        public bool TagValueEncExeception(string strTag)
        {
            bool bTag = true;
            if (
                (String.Compare(strTag, "APPID", true) == 0)
                || (String.Compare(strTag, "REASON", true) == 0)
                || (String.Compare(strTag, "RESULT", true) == 0)
                )
                bTag = false;
            return bTag;
        }
        public string GetTagData(string strKey)
        {
            bool bTag = TagValueEncExeception(strKey);
            string strVal = "";
            if (bTag)
            {
                SGCrypto sgCrypt = new SGCrypto();
                //sgCrypt.SetSessionKey();
                string strEncVal = m_DicTagData[strKey];
                sgCrypt.Aes256TagValueDecrypt(strEncVal, out strVal);
            }
            else
                strVal = m_DicTagData[strKey];

            return strVal;
        }

        public List<Dictionary<int, string>> GetRecordData(string strKey)
        {
            m_DicRecordData.Clear();

            string strRecordVal = m_DicTagData[strKey];

            char sep = (char)0x02;

            string[] strRecord = strRecordVal.Split(sep);

            foreach (var item in strRecord)
            {
                Dictionary<int, string> pair = new Dictionary<int, string>();
                char subSep = (char)0x03;
                string[] strData = item.Split(subSep);
                int idx = 0;
                foreach(var subItem in strData)
                {
                    pair[idx++] = subItem;
                }

                m_DicRecordData.Add(pair);
            }

            return m_DicRecordData;
        }

        public void Clear()
        {
            m_DicTagData.Clear();
        }

    }
}
