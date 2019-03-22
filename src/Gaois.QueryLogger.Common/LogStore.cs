using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Gaois.QueryLogger
{
    /// <summary>
    /// Log data to persistent storage
    /// </summary>
    public abstract class LogStore
    {
        private readonly QueryLoggerSettings _settings;
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
                        Console.WriteLine(exception);
                        RetryEnqueue(query);
                    }
                }
            });
        }

        private void RetryEnqueue(Query query)
        {
            Enqueue(new[] { query });
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
                    LogQueue.TryAdd(query, TimeSpan.FromSeconds(_settings.Store.MaxQueueRetryTime));
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
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
    }
}