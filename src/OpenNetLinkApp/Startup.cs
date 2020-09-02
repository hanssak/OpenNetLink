using Microsoft.Extensions.DependencyInjection;
using WebWindows.Blazor;
using OpenNetLinkApp.Services;
using Blazor.FileReader;
using OpenNetLinkApp.Services.SGAppManager;

namespace OpenNetLinkApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<XmlConfService>();
            services.AddSGAppManagerService();
            services.AddSingleton<DragAndDropService>();
            services.AddSingleton<HSCmdCenter>();
            services.AddSingleton<PageStatusService>();
            services.AddFileReaderService(options => options.InitializeOnFirstCall = true);
        }

        public void Configure(DesktopApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
