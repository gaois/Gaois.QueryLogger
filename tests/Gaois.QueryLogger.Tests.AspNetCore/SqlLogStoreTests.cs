using Gaois.QueryLogger.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gaois.QueryLogger.Tests.AspNetCore
{
    public class SqlLogStoreTests
    {
        private readonly ILogStore _store;

        public SqlLogStoreTests()
        {
            var configuration = TestHelper.GetQueryLoggerConfiguration("appsettings.json");
            var serviceProvider = new ServiceCollection()
                .Configure<QueryLoggerSettings>(configuration)
                .AddTransient<IAlertService, EmailAlertService>()
                .AddSingleton<ILogStore, SqlLogStore>()
                .BuildServiceProvider();
            _store = serviceProvider.GetService<ILogStore>();
        }

        [Fact]
        public void Enqueue()
        {
            // Add single query
            var query = new Query()
            {
                QueryTerms = "test"
            };

            // Test queue size
            _store.Enqueue(new[] { query });
            Assert.Single(_store.LogQueue);

            // Test query value
            var queuedQuery = _store.LogQueue.Take();
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
            };

            // Test queue size
            // query3 contains an excluded IP address
            _store.Enqueue(new[] { query1, query2, query3 });
            Assert.Equal(3, _store.LogQueue.Count);

            // Test query values
            var queuedQuery1 = _store.LogQueue.Take();
            var queuedQuery2 = _store.LogQueue.Take();
            var queuedQuery3 = _store.LogQueue.Take();
            Assert.Equal("test1", queuedQuery1.QueryTerms);
            Assert.Equal("test2", queuedQuery2.QueryTerms);
            Assert.Equal("test3", queuedQuery3.QueryTerms);
        }

        [Fact]
        public void EnqueueWithExcludedIPAddresses()
        {
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

            // Test queue size: query3 contains an excluded IP address
            _store.Enqueue(new[] { query1, query2, query3 });
            Assert.Equal(2, _store.LogQueue.Count);
        }
    }
}