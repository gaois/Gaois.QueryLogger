using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Gaois.QueryLogger.AspNetCore
{
    /// <summary>
    /// 
    /// </summary>
    public static class QueryLoggerServiceExtensions
    {
        public static IServiceCollection AddQueryLogger(this IServiceCollection services, Action<QueryLoggerSettings> configureSettings = null)
        {
            if (configureSettings != null)
            {
                services.Configure(configureSettings);
            }

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IQueryLogger, QueryLogger>();

            return services;
        }
    }
}