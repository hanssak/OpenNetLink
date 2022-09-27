using OpenNetLinkApp.Data.SGDicData.SGUnitData;
using OpenNetLinkApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Security.Cryptography;
using System.IO;
using AgLogManager;
using Serilog;
using HsNetWorkSG;

namespace OpenNetLinkApp.Common
{

    public sealed class AsyncLock
    {

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public async Task<IDisposable> LockAsync()
        {
            await _semaphore.WaitAsync();
            return new Handler(_semaphore);
        }

        private sealed class Handler : IDisposable
        {
            private readonly SemaphoreSlim _semaphore;
            private bool _disposed = false;

            public Handler(SemaphoreSlim semaphore)
            {
                _semaphore = semaphore;
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _semaphore.Release();
                    _disposed = true;
                }
            }

        }

    }


    public class CsFunction
    {


        private static XmlConfService _xmlConfInstance = new XmlConfService();

        public static XmlConfService XmlConf
        {
            get
            {
                if(_xmlConfInstance != null)
                    return _xmlConfInstance;
                else
                {
                    _xmlConfInstance = new XmlConfService();
                    return _xmlConfInstance;
                }
            }
        }

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
            string strRet = "";
            strRet = SgExtFunc.hsFileRename(bMode, strFileName);
            return strRet;
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

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }
    }

    public class CsSeqFunc
    {
        public static bool isDeptSeq(string strSeq)
        {
            if ((strSeq?.Length ?? 0) != 18)
                return false;

            if (strSeq.Substring(8, 2) != "16")
                return false;



            return true;
        }

    }

    public class CsFileFunc
    {
        public static long GetFileSize(string filePath)
        {
            long lSize = 0;

            using (FileStream fileStream = File.OpenRead(filePath))
            {
                try
                {
                    lSize = fileStream.Length;
                    return lSize;
                }
                catch (Exception e)
                {
                    Log.Information($"GetFileSize - Exception - msg : {e.Message}, path : {filePath}");
                    lSize = -1;
                }
                finally
                {
                    fileStream?.Dispose();
                }
            }

            return lSize;
        }
    }

    public class CsHashFunc
    {

        public static string SHA256CheckSum(string filePath)
        {
            try
            {
                using (SHA256 SHA256 = SHA256Managed.Create())
                {
                    using (FileStream fileStream = File.OpenRead(filePath))
                        return Convert.ToBase64String(SHA256.ComputeHash(fileStream));
                }
            }
            catch (Exception e)
            {
                Log.Information($"SHA256CheckSum - Exception - msg : {e.Message}, path : {filePath}");
                //CLog.Here().Information($"FileInfo Get(#####) - Exception - msg : {e.Message}, path : {filePath}");
            }

            return "";
        }

        public static string SHA384BinBase64(string filePath)
        {
            try
            {
                using (SHA384 sha384 = SHA384Managed.Create())
                {
                    using (FileStream fileStream = File.OpenRead(filePath))
                    {
                        byte[] pbyte = null;
                        pbyte = new byte[fileStream.Length];
                        fileStream.Read(pbyte, 0, (int)fileStream.Length);

                        return Convert.ToBase64String(SHA384.HashData(pbyte));
                    }
                }
            }
            catch (Exception e)
            {
                Log.Information($"SHA384BinBase64 - Exception - msg : {e.Message}, path : {filePath}");
                //CLog.Here().Information($"FileInfo Get(#####) - Exception - msg : {e.Message}, path : {filePath}");
            }

            return "";
        }

    }

}
