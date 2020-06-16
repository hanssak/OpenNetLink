using Microsoft.Extensions.DependencyInjection;
using WebWindows.Blazor;
using OpenNetLinkApp.Services;

namespace OpenNetLinkApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<SideBarService>();
            services.AddSingleton<DragAndDropService>();
        }

        public void Configure(DesktopApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
