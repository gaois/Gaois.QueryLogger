using Gaois.QueryLogger.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Gaois.QueryLogger.Tests.AspNetCore
{
    public class SqlLogStoreTests
    {
        [Fact]
        public void Enqueue()
        {
            var configuration = TestHelper.GetQueryLoggerConfiguration("appsettings.json");
            var serviceProvider = new ServiceCollection()
                .Configure<QueryLoggerSettings>(configuration)
                .AddTransient<IAlertService, EmailAlertService>()
                .AddSingleton<SqlLogStore>()
                .BuildServiceProvider();

            var store = serviceProvider.GetService<SqlLogStore>();

            // Add single query
            var query = new Query()
            {
                QueryTerms = "test"
            };

            // Test queue size
            store.Enqueue(new[] { query });
            Assert.Single(store.LogQueue);

            // Test query value
            var queuedQuery = store.LogQueue.Take();
            Assert.Equal("test", queuedQuery.QueryTerms);

            // Add multiple queries
            var query1 = new Query()
            {
                QueryTerms = "test1"
            };

            var query2 = new Query()
            {
                QueryTerms = "test2"
            };

            var query3 = new Query()
            {
                QueryTerms = "test3",
                IPAddress = "207.46.13.0"
            };

            // Test queue size
            // query3 contains an excluded IP address
            store.Enqueue(new[] { query1, query2, query3 });
            Assert.Equal(2, store.LogQueue.Count);

            // Test query values
            var queuedQuery1 = store.LogQueue.Take();
            var queuedQuery2 = store.LogQueue.Take();
            Assert.Equal("test1", queuedQuery1.QueryTerms);
            Assert.Equal("test2", queuedQuery2.QueryTerms);
        }
    }
}