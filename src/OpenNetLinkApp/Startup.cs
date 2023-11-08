using Microsoft.Extensions.DependencyInjection;
using WebWindows.Blazor;
using OpenNetLinkApp.Services;
using Blazor.FileReader;
using OpenNetLinkApp.Services.SGAppManager;
using OpenNetLinkApp.Services.SGAppUpdater;

namespace OpenNetLinkApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddSingleton<XmlConfService>();
                services.AddSGAppManagerService();
                services.AddSGAppUpdaterService();
                services.AddSingleton<DragAndDropService>();
                services.AddSingleton<HSCmdCenter>();
                services.AddSingleton<PageStatusService>();
                services.AddSingleton<OSXcmdService>();
                services.AddFileReaderService(options => {
                    options.InitializeOnFirstCall = true;
                    options.UseWasmSharedBuffer = false;
                });
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Startup ConfigureServices Exception : {ex.ToString()}");
            }
        }

        public void Configure(DesktopApplicationBuilder app)
        {
            try
            {
                app.AddComponent<App>("app");
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Startup Configure Exception : {ex.ToString()}");
            }
            
        }
    }
}
