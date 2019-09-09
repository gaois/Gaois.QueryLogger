using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using Xunit;

namespace Gaois.QueryLogger.Tests.AspNetCore
{
    public class QueryLoggerTests
    {
        private readonly IHttpContextData _context;

        public QueryLoggerTests()
        {
            _context = new MockHttpContextData();
        }

        /*[Fact]
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
        public void LoggingEnabled()
        {
            var configuration = TestHelper.GetQueryLoggerConfiguration("typicalsettings.json");
            var serviceProvider = GetServiceProvider(configuration);
            var store = serviceProvider.GetService<ILogStore>();
            var queryLogger = serviceProvider.GetService<IQueryLogger>();
            var settings = serviceProvider.GetService<IOptionsMonitor<QueryLoggerSettings>>();

            var category = "TestCategory";

            var autoPopulatedQuery = new Query()
            {
                QueryTerms = "test1",
                QueryCategory = category
            };

            var userSpecifiedQueryID = Guid.NewGuid();

            var userSpecifiedQuery = new Query()
            {
                QueryID = userSpecifiedQueryID,
                ApplicationName = "MyTestApp",
                QueryTerms = "test2",
                Host = "www.example.com",
                IPAddress = "987.65.43.0"
            };
            
            queryLogger.Log(autoPopulatedQuery, userSpecifiedQuery);

            var queueCount = store.LogQueue.Count;
            Assert.Equal(2, queueCount);

            var queuedAutoPopulatedQuery = store.LogQueue.Take();
            var processedContextIP = IPAddressProcessor.Process(_context.IPAddress, settings.CurrentValue);
            Assert.NotNull(queuedAutoPopulatedQuery.QueryID);
            Assert.Equal("RecordsApp", queuedAutoPopulatedQuery.ApplicationName);
            Assert.Equal(category, queuedAutoPopulatedQuery.QueryCategory);
            Assert.Equal(_context.Host, queuedAutoPopulatedQuery.Host);
            Assert.Equal(processedContextIP, queuedAutoPopulatedQuery.IPAddress);

            var queuedUserSpecifiedQuery = store.LogQueue.Take();
            Assert.Equal(userSpecifiedQueryID, queuedUserSpecifiedQuery.QueryID);
            Assert.Equal("MyTestApp", queuedUserSpecifiedQuery.ApplicationName);
            Assert.Null(queuedUserSpecifiedQuery.QueryCategory);
            Assert.NotEqual(_context.Host, queuedUserSpecifiedQuery.Host);
            Assert.NotEqual(processedContextIP, queuedUserSpecifiedQuery.IPAddress);
        }*/

        private ServiceProvider GetServiceProvider(IConfigurationSection configuration) =>
            new ServiceCollection()
                .Configure<QueryLoggerSettings>(configuration)
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSingleton<ILogStore, MockLogStore>()
                .AddTransient<IAlertService, EmailAlertService>()
                .AddTransient<IHttpContextData, MockHttpContextData>()
                .AddTransient<IQueryLogger, QueryLogger>()
                .BuildServiceProvider();

        private ServiceProvider GetServiceProvider(Action<QueryLoggerSettings> settings) =>
            new ServiceCollection()
                .Configure(settings)
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSingleton<ILogStore, MockLogStore>()
                .AddTransient<IAlertService, EmailAlertService>()
                .AddTransient<IHttpContextData, MockHttpContextData>()
                .AddTransient<IQueryLogger, QueryLogger>()
                .BuildServiceProvider();
    }
}