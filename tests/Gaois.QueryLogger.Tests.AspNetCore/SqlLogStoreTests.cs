using Gaois.QueryLogger.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;
using Xunit.Abstractions;

namespace Gaois.QueryLogger.Tests.AspNetCore
{
    public class SqlLogStoreTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public SqlLogStoreTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public void Enqueue()
        {
            var configuration = TestHelper.GetConfiguration("appsettings.json");
            var serviceProvider = new ServiceCollection()
                .Configure<QueryLoggerSettings>(configuration.GetSection("QueryLogger"))
                .AddOptions()
                .AddTransient<IAlertService, EmailAlertService>()
                .BuildServiceProvider();

            var alertService = serviceProvider.GetService<IAlertService>();
            var settings = serviceProvider.GetService<IOptionsMonitor<QueryLoggerSettings>>();

            _outputHelper.WriteLine(settings.CurrentValue.ApplicationName);

            var store = new SqlLogStore(settings, alertService);

            // Add single query
            var query = new Query()
            {

            };

            // Test queue size
            store.Enqueue(new[] { query });
            Assert.Single(store.LogQueue);

            // Test query value
            var takeQuery = store.LogQueue.Take();
            Assert.Equal("RecordsApp", takeQuery.ApplicationName);
        }
    }
}