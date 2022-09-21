using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Security.Cryptography;
using System.IO;
using AgLogManager;
using Serilog;

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
