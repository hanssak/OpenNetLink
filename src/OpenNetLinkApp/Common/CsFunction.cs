using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

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
}
