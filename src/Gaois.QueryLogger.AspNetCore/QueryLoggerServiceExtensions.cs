using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Gaois.QueryLogger
{
    /// <summary>
    /// Extension methods for configuring Gaois.QueryLogger
    /// </summary>
    public static class QueryLoggerServiceExtensions
    {
        /// <summary>
        /// Adds Gaois.QueryLogger configuration for logging queries
        /// </summary>
        /// <param name="services">The services collection to configure.</param>
        /// <param name="configureSettings">An <see cref="Action{QueryLoggerSettings}"/> to configure options for Gaois.QueryLogger.</param>
        /// <returns></returns>
        public static IServiceCollection AddQueryLogger(this IServiceCollection services, Action<QueryLoggerSettings> configureSettings = null)
        {
            if (configureSettings != null)
            {
                services.Configure(configureSettings);
            }

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IQueryLogger, QueryLoggerCore>();

            return services;
        }
    }
}