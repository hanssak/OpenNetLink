using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace OpenNetLinkService
{
    public class Worker : BackgroundService
    {
        private static Serilog.ILogger CLog => Serilog.Log.ForContext<HttpWatcher>();
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                Thread thread = new Thread(HttpWatcher.Run);
                thread.Start();
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000 * 60, stoppingToken);
                }
            }
            catch(Exception ex)
            {
                CLog.Error($"OpenNetLink Service Worker Exception - {ex.Message}");
            }
        }
    }
}
