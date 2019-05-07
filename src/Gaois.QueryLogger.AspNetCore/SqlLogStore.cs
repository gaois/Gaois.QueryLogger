using Dapper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Gaois.QueryLogger.AspNetCore
{
    /// <summary>
    /// Stores log data in a SQL Server database
    /// </summary>
    public sealed class SqlLogStore : LogStore, ILogStore
    {
        private readonly IOptionsMonitor<QueryLoggerSettings> _settings;
        private readonly IAlertService _alertService;
        private DateTime? _lastAlertTime;
        private BlockingCollection<Query> _logQueue;
        private bool _isConsumingQueue;

        /// <summary>
        /// Stores log data in a SQL Server database
        /// </summary>
        public SqlLogStore(
            IOptionsMonitor<QueryLoggerSettings> settings,
            IAlertService alertService)
        {
            _settings = settings;
            _alertService = alertService;
        }

        /// <summary>
        /// Gets the queue of logs waiting to be written
        /// </summary>
        public override BlockingCollection<Query> LogQueue => _logQueue ?? 
            (_logQueue = new BlockingCollection<Query>(_settings.CurrentValue.Store.MaxQueueSize));

        /// <summary>
        /// Sends an alert to designated users using the configured alert services
        /// </summary>
        /// <param name="alert"></param>
        protected override void SendAlert(Alert alert)
        {
            if (_lastAlertTime is null)
                _lastAlertTime = DateTime.UtcNow;

            if (_lastAlertTime > DateTime.UtcNow - TimeSpan.FromMilliseconds(_settings.CurrentValue.AlertInterval))
                return;

            try
            {
                Alert(alert);
            }
            catch (Exception exception)
            {
                // Last port of call if there is an error and we also can't send an alert.
                // We catch the exception as we don't want the logger to tank its parent application by throwing exceptions continuously if alert service is not available.
                // We write the exception to a trace to provide some degree of visibility for the error.
                Trace.WriteLine(exception);
            }
        }

        /// <summary>
        /// Adds query data to a queue for logging to a data store
        /// </summary>
        /// <param name="queries">The <see cref="Query"/> object or objects to be logged</param>
        public override void Enqueue(Query[] queries)
        {
            foreach (var query in queries)
            {
                if (_settings.CurrentValue.ExcludedIPAddresses != null
                    && _settings.CurrentValue.ExcludedIPAddresses.Find(x => x.IPAddress == query.IPAddress) != null)
                    continue;

                try
                {
                    if (!LogQueue.TryAdd(query, _settings.CurrentValue.Store.MaxQueueRetryTime))
                    {
                        var alert = new Alert
                        {
                            Type = AlertTypes.QueueFull,
                            Query = query
                        };

                        SendAlert(alert);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception exception)
                {
                    var alert = new Alert
                    {
                        Type = AlertTypes.EnqueueError,
                        Query = query,
                        Exception = exception
                    };

                    SendAlert(alert);
                }
            }
        }

        /// <summary>
        /// Verifies that the log queue is being processed; if not, initializes queue consumption.
        /// </summary>
        public override void ProcessQueue()
        {
            if (_isConsumingQueue)
                return;

            ConsumeQueue();
            _isConsumingQueue = true;
        }

        /// <summary>
        /// Alerts designated users in case of possible issues with the query logger service
        /// </summary>
        /// <param name="alert">The <see cref="Alert"/> to be sent</param>
        public override void Alert(Alert alert)
        {
            _ = alert ?? throw new ArgumentNullException(nameof(alert));
            _alertService.Alert(alert);
        }

        /// <summary>
        /// Writes query log to a data store
        /// </summary>
        /// <param name="queries">The <see cref="Query"/> object or objects to be logged</param>
        public override void WriteLog(params Query[] queries)
        {
            _ = queries ?? throw new ArgumentNullException(nameof(queries));

            using (var db = new SqlConnection(_settings.CurrentValue.Store.ConnectionString))
                db.Execute(SqlQueries.WriteLog(_settings.CurrentValue.Store.TableName), queries);
        }

        /// <summary>
        /// Writes query log to a data store asynchronously
        /// </summary>
        /// <param name="queries">The <see cref="Query"/> object or objects to be logged</param>
        public override async Task WriteLogAsync(params Query[] queries)
        {
            _ = queries ?? throw new ArgumentNullException(nameof(queries));

            using (var db = new SqlConnection(_settings.CurrentValue.Store.ConnectionString))
                await db.ExecuteAsync(SqlQueries.WriteLog(_settings.CurrentValue.Store.TableName), queries).ConfigureAwait(false);
        }
    }
}