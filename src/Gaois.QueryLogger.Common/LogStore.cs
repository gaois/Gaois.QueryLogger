using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Gaois.QueryLogger
{
    /// <summary>
    /// Log data to persistent storage
    /// </summary>
    public abstract class LogStore
    {
        private readonly QueryLoggerSettings _settings;
        private DateTime? _lastAlertTime;
        private BlockingCollection<Query> _logQueue;
        private bool _isConsumingQueue;

        /// <summary>
        /// Log data to persistent storage
        /// </summary>
        public LogStore(QueryLoggerSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Gets the queue of logs waiting to be written
        /// </summary>
        public BlockingCollection<Query> LogQueue => _logQueue ?? (_logQueue = new BlockingCollection<Query>(_settings.Store.MaxQueueSize));

        private void ConsumeQueue()
        {
            // Consume the log queue, and write to data store, in a separate thread
            Task.Run(async () =>
            {
                foreach (var query in LogQueue.GetConsumingEnumerable())
                {
                    try
                    {   
                        await WriteLogAsync(query).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception exception)
                    {
                        var alert = new Alert
                        {
                            Type = AlertTypes.LogWriteError,
                            Query = query,
                            Exception = exception
                        };

                        RetryEnqueue(query);
                        SendAlert(alert);
                    }
                }
            });
        }

        private void RetryEnqueue(Query query)
        {
            Enqueue(new[] { query });
        }

        private void SendAlert(Alert alert)
        {
            if (_lastAlertTime is null)
                _lastAlertTime = DateTime.UtcNow;

            if (_lastAlertTime > DateTime.UtcNow - TimeSpan.FromMilliseconds(_settings.AlertInterval))
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
        /// Logs query data to a data store
        /// </summary>
        /// <param name="queries">The <see cref="Query"/> object or objects to be logged</param>
        public void Enqueue(Query[] queries)
        {
            foreach (var query in queries)
            {
                try
                {
                    if (!LogQueue.TryAdd(query, _settings.Store.MaxQueueRetryTime))
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

            if (_isConsumingQueue)
                return;

            ConsumeQueue();
            _isConsumingQueue = true;
        }

        /// <summary>
        /// Logs query data in a data store
        /// </summary>
        /// <param name="queries">The <see cref="Query"/> object or objects to be logged</param>
        public abstract void WriteLog(params Query[] queries);

        /// <summary>
        /// Logs query data in a data store asynchronously
        /// </summary>
        /// <param name="queries">The <see cref="Query"/> object or objects to be logged</param>
        public abstract Task WriteLogAsync(params Query[] queries);

        /// <summary>
        /// Alerts designated users in case of possible issues with the query logger service
        /// </summary>
        /// <param name="alert"></param>
        public abstract void Alert(Alert alert);
    }
}