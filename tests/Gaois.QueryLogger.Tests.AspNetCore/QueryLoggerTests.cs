using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace Gaois.QueryLogger.Tests.AspNetCore
{
    public class QueryLoggerTests
    {
        [Fact]
        public void LoggingDisabled()
        {
            var serviceProvider = GetServiceProvider(settings => settings.IsEnabled = false);
            var queryLogger = serviceProvider.GetService<IQueryLogger>();
            var store = serviceProvider.GetService<ILogStore>();

            var query1 = new Query()
            {
                QueryTerms = "test1"
            };

            var query2 = new Query()
            {
                QueryTerms = "test2"
            };

            queryLogger.Log(query1, query2);
            Assert.Empty(store.LogQueue);
        }
        
        [Fact]
        public void LogWithConfigurationSettings()
        {
            var configuration = TestHelper.GetQueryLoggerConfiguration("appsettings.json");
            var serviceProvider = GetServiceProvider(configuration);
            var store = serviceProvider.GetService<ILogStore>();
            var queryLogger = serviceProvider.GetService<IQueryLogger>();

            var query1 = new Query()
            {
                QueryTerms = "test1"
            };

            var query2 = new Query()
            {
                QueryTerms = "test2"
            };
            
            // Need to mock HttpContextAccessor

            /*
            queryLogger.Log(query1, query2);

            var queuedQuery = store.LogQueue.Count;
            Assert.Equal(2, queuedQuery);
            Assert.Equal("RecordsApp", queuedQuery.ApplicationName);
            */
        }

        [Fact]
        public void LogWithConfigurationSettingsOverridden()
        {

        }

        private ServiceProvider GetServiceProvider(IConfigurationSection configuration) =>
            new ServiceCollection()
                .Configure<QueryLoggerSettings>(configuration)
                .AddTransient<IAlertService, EmailAlertService>()
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSingleton<ILogStore, TestLogStore>()
                .AddTransient<IQueryLogger, QueryLogger>()
                .BuildServiceProvider();

        private ServiceProvider GetServiceProvider(Action<QueryLoggerSettings> settings) =>
            new ServiceCollection()
                .Configure(settings)
                .AddTransient<IAlertService, EmailAlertService>()
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSingleton<ILogStore, TestLogStore>()
                .AddTransient<IQueryLogger, QueryLogger>()
                .BuildServiceProvider();
    }
}