using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using HsNetWorkSGData;

namespace OpenNetLinkApp.Models.Data
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
    }
}
