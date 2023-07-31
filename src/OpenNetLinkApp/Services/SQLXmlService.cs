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
        public static SQLXmlService Instanse
        {
            get
            {

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
            byte[] sqlContent = new byte[0];
            byte[] compare = Encoding.UTF8.GetBytes("<statements>");
            byte[] find = new byte[compare.Length];
            try
            {
                sqlContent = System.IO.File.ReadAllBytes("wwwroot/conf/SqlQuery.xml");
                bool isOriFile = false;
                for (int i = 0; i < (sqlContent.Length / 5); i++)
                {
                    Array.Copy(sqlContent, i, find, 0, find.Length);
                    if (find.SequenceEqual(compare))
                    {
                        isOriFile = true;
                        break;
                    }
                }

                if (isOriFile == false)
                {
                    _sqlContent = new byte[sqlContent.Length];
                    Array.Copy(sqlContent, _sqlContent, sqlContent.Length);
                }
                else
                {
                    SGCrypto.AESEncrypt256WithDEK(sqlContent, ref _sqlContent);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                sqlContent.hsClear(3);
                find.hsClear(3);
            }
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
            byte[] input = null;
            char[] temp = new char[sb.Length];
            try
            {
                //string sql = sb.ToString();
                for (int i = 0; i < sb.Length; i++)
                {
                    temp[i] = sb[i];
                }
                input = ASCIIEncoding.UTF8.GetBytes(temp);
                return SGCrypto.AESEncrypt256WithDEK(input);
            }
            catch (Exception ex)
            {
                return "";
            }
            finally
            {
                for (int i = 0; i < sb.Length; i++)
                {
                    sb[i] = '0';
                    temp[i] = '0';
                }
                if (input != null)
                    input.hsClear();
            }
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
