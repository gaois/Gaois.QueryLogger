using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Gaois.QueryLogger.Tests.AspNetCore
{
    internal class MockLogStore : ILogStore
    {
        private readonly IOptionsMonitor<QueryLoggerSettings> _settings;
        private readonly IAlertService _alertService;
        private BlockingCollection<Query> _logQueue;

        public MockLogStore(
            IOptionsMonitor<QueryLoggerSettings> settings,
            IAlertService alertService)
        {
            _settings = settings;
            _alertService = alertService;
        }

        public BlockingCollection<Query> LogQueue => _logQueue ??
            (_logQueue = new BlockingCollection<Query>(_settings.CurrentValue.Store.MaxQueueSize));

        public void Alert(Alert alert) => throw new NotImplementedException();

        public void Enqueue(Query[] queries)
        {
            foreach (var query in queries)
            {
                if (_settings.CurrentValue.ExcludedIPAddresses != null
                    && _settings.CurrentValue.ExcludedIPAddresses.Find(x => x.IPAddress == query.IPAddress) != null)
                    continue;

                LogQueue.Add(query);
            }
        }

        public void ProcessQueue()
        {
            return;
        }

        public void WriteLog(params Query[] queries) => throw new NotImplementedException();

        public Task WriteLogAsync(params Query[] queries) => throw new NotImplementedException();
    }
}