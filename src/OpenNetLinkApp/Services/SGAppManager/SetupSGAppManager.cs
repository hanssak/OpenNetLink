using Microsoft.Extensions.DependencyInjection;
using System;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public static class SetupSGAppManager
    {
        /// <summary>
        /// Adds <see cref="ISGAppManagerService"/> as a scoped service
        /// to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddSGAppManagerService(this IServiceCollection services)
        {
            services.AddScoped<ISGAppManagerService, SGAppManagerService>();
            return services;
        }
    }
}