using Microsoft.Extensions.DependencyInjection;
using WebWindows.Blazor;
using OpenNetLinkApp.Services;
using Blazor.FileReader;

namespace OpenNetLinkApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<SideBarService>();
            services.AddSingleton<DragAndDropService>();
            services.AddFileReaderService(options => options.InitializeOnFirstCall = true);
        }

        public void Configure(DesktopApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
