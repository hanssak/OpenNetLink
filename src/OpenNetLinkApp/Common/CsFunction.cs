using OpenNetLinkApp.Data.SGDicData.SGUnitData;
using OpenNetLinkApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetLinkApp.Common
{
    public class CsFunction
    {


        public static XmlConfService _xmlConfInstance = new XmlConfService();

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

        public static void SetFilterString(ref string str, string value, Func<Task> function)
        {
            str = value;

            Task.Run(async () => { await function(); });
        }

        public static List<string> GetOptionValue(string option, Dictionary<string, SGNetOverData> dic = null)
        {
            List<string> values = new List<string>();
            if (option == "TransKind")
            {
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_ALL"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_IMPORT"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_EXPORT"));
            }
            else if (option == "TransStatus")
            {
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_ALL"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_TRANSWAIT"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_TRANSCANCLE"));
                values.Add(_xmlConfInstance.GetTitle("T_TRANS_COMPLETE"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_TRANSFAIL"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_TRANSCHECKING"));
            }
            else if (option == "MailTransStatus")
            {
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_ALL"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_TRANSWAIT"));     //전송대기
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_TRANS_SUCCESS")); //전송완료
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_TRANSFAIL"));     //전송실패
            }
            else if (option == "ApproveStatus")
            {
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_ALL"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_APPROVE_WAIT"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_APPROVE"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_REJECTION"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_REQUESTCANCEL"));
            }
            else if (option == "MailApproveStatus")
            {
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_ALL"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_APPROVE_WAIT"));
                values.Add(_xmlConfInstance.GetTitle("T_DASH_APPROVE_COMPLETE"));
                values.Add(_xmlConfInstance.GetTitle("T_DASH_APPROVE_REJECT"));
            }
            else if (option == "ApproveKind")
            {
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_ALL"));
                values.Add(_xmlConfInstance.GetTitle("T_DETAIL_BEFORE_APPROVE"));
                values.Add(_xmlConfInstance.GetTitle("T_DETAIL_AFTER_APPROVE"));
            }
            else if (option == "TransType")
            {
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_ALL"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_IMPORT"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_EXPORT"));
            }
            else if (option == "DlpValue")
            {
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_ALL"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_DLP_INCLUSION"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_DLP_NOTINCLUSION"));
            }
            else if (option == "DataType")
            {
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_ALL"));
                values.Add(_xmlConfInstance.GetTitle("T_DATA_TYPE_TEXT"));
                values.Add(_xmlConfInstance.GetTitle("T_DATA_TYPE_IMAGE"));
            }
            else if(option == "DestNetwork")
            {
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_ALL"));
                if (dic != null)
                {
                    foreach (string str in dic.Keys)
                    {
                        values.Add(str);
                    }
                }
            }

            return values;
        }
    }
}
