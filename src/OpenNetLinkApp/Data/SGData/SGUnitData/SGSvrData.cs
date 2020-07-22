using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using HsNetWorkSGData;

namespace OpenNetLinkApp.Data
{
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
    }
}
