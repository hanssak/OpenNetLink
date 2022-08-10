using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetLinkApp.Common
{
    public class CsFunction
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Size"></param>
        /// <returns></returns>
        public static string GetSizeStr(long Size)
        {
            string rtn = "";
            if (Size == 0)
            {
                rtn = "0 Byte";
            }
            if (Size > 1024 * 1024 * 1024)
            {
                float nSize = (float)Size / (1024 * 1024 * 1024);
                rtn = nSize.ToString("####0.0") + "GB";
            }
            else if (Size > 1024 * 1024)
            {
                float nSize = (float)Size / (1024 * 1024);
                rtn = nSize.ToString("####0.0") + "MB";
            }
            else if (Size > 1024)
            {
                float nSize = (float)Size / (1024);
                rtn = nSize.ToString("####0.0") + "KB";
            }
            else if (Size > 0)
                rtn = Size + " Byte";
            return rtn;
        }

        public static string GetChangeNewLineToN(string sql)
        {
            return sql.Replace(Environment.NewLine, "\n");
        }

        public static string GetFileRename(bool bMode, string strFileName)
        {
            if (bMode == true)
            {
                strFileName = strFileName.Replace("`", "^TD^");
                strFileName = strFileName.Replace("&", "^AP^");
                strFileName = strFileName.Replace("%", "^PC^");
                strFileName = strFileName.Replace("!", "^EM^");
                strFileName = strFileName.Replace("@", "^AT^");

                strFileName = strFileName.Replace("#", "^SH^");
                strFileName = strFileName.Replace("$", "^DL^");
                strFileName = strFileName.Replace("*", "^AS^");
                strFileName = strFileName.Replace("(", "^LR^");
                strFileName = strFileName.Replace(")", "^RR^");

                strFileName = strFileName.Replace("-", "^DS^");
                strFileName = strFileName.Replace("+", "^PL^");
                strFileName = strFileName.Replace("=", "^EQ^");
                strFileName = strFileName.Replace(";", "^SC^");
                strFileName = strFileName.Replace("'", "^SQ^");
            }
            else
            {
                strFileName = strFileName.Replace("^TD^", "`");
                strFileName = strFileName.Replace("^AP^", "&");
                strFileName = strFileName.Replace("^PC^", "%");
                strFileName = strFileName.Replace("^EM^", "!");
                strFileName = strFileName.Replace("^AT^", "@");

                strFileName = strFileName.Replace("^SH^", "#");
                strFileName = strFileName.Replace("^DL^", "$");
                strFileName = strFileName.Replace("^AS^", "*");
                strFileName = strFileName.Replace("^LR^", "(");
                strFileName = strFileName.Replace("^RR^", ")");

                strFileName = strFileName.Replace("^DS^", "-");
                strFileName = strFileName.Replace("^PL^", "+");
                strFileName = strFileName.Replace("^EQ^", "=");
                strFileName = strFileName.Replace("^SC^", ";");
                strFileName = strFileName.Replace("^SQ^", "'");
            }

            return strFileName;
        }

    }
}
