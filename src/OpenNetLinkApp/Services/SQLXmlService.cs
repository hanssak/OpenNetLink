using HsNetWorkSG;
using OpenNetLinkApp.Data.SGQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenNetLinkApp.Services
{
    class SQLXmlService
    {
        private byte[] _sqlContent;
        private static SQLXmlService _sqlXmlService = null;
        public static SQLXmlService Instanse { 
            get {

                if (_sqlXmlService != null)
                    return _sqlXmlService;
                else
                {
                    _sqlXmlService = new SQLXmlService();
                    return _sqlXmlService;
                }
            } 
        }
        public SQLXmlService()
        {
            byte[] sqlContent = System.IO.File.ReadAllBytes("wwwroot/conf/SqlQuery.xml");
            SGCrypto.AESEncrypt256WithDEK(sqlContent, ref _sqlContent);
            sqlContent.hsClear(3);
            int i = 0;
            //_sqlContent = System.IO.File.ReadAllBytes("wwwroot/conf/SqlQuery.xml");
        }
        ~SQLXmlService()
        {
            Array.Clear(_sqlContent, 0x0, _sqlContent.Length);
        }

        public string GetSqlQuery(string statementId, Dictionary<string, string> param)
        {
            StringBuilder sb = new StringBuilder();
            SQLMapping.GetSqlQuery(ref _sqlContent, statementId, param, ref sb);

            string sql = sb.ToString();
            return SGCrypto.AESEncrypt256WithDEK(ref sql);
        }

        public void GetSqlQuery(string statementId, Dictionary<string, string> param, ref StringBuilder sb)
        {
            SQLMapping.GetSqlQuery(ref _sqlContent, statementId, param, ref sb);
        }

        public Dictionary<string, string> ConvertClassToDictionary(object T)
        {
            PropertyInfo[] infos = T.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            Dictionary<string, string> param = new Dictionary<string, string>();
            foreach (PropertyInfo info in infos)
            {
                if (info.GetValue(T, null) != null)
                {
                    if (info.GetValue(T, null).GetType().IsEnum)
                    {
                        param.Add(info.Name, ((int)info.GetValue(T, null)).ToString());
                    }
                    else if (info.GetValue(T, null).GetType().Name == "Boolean")
                    {
                        param.Add(info.Name, Convert.ToInt32(info.GetValue(T, null)).ToString());
                    }
                    else
                        param.Add(info.Name, info.GetValue(T, null).ToString());
                }
                else
                {
                    param.Add(info.Name, "");
                }
            }

            return param;
        }
    }
}
