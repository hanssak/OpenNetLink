using Microsoft.Extensions.DependencyInjection;
using System;

namespace OpenNetLinkApp.Services.SGAppUpdater
{
    public static class SetupSGAppUpdater
    {
        /// <summary>
        /// Adds <see cref="ISGAppUpdaterService"/> as a scoped service
        /// to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddSGAppUpdaterService(this IServiceCollection services)
        {
            services.AddScoped<ISGAppUpdaterService, SGAppUpdaterService>();
            return services;
        }
    }
}