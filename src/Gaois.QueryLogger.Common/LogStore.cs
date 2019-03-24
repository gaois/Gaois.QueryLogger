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
        /// <summary>
        /// Gets the queue of logs waiting to be written
        /// </summary>
        public abstract BlockingCollection<Query> LogQueue { get; }

        private void RetryEnqueue(Query query)
        {
            Enqueue(new[] { query });
        }

        /// <summary>
        /// Consumes the log queue and writes logs to the data store in a separate thread
        /// </summary>
        protected void ConsumeQueue()
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

        /// <summary>
        /// Sends an alert to designated users using the configured alert services
        /// </summary>
        /// <param name="alert"></param>
        protected abstract void SendAlert(Alert alert);

        /// <summary>
        /// Queues query data for logging to a data store
        /// </summary>
        /// <param name="queries">The <see cref="Query"/> object or objects to be logged</param>
        public abstract void Enqueue(Query[] queries);

        /// <summary>
        /// Writes query log to a data store
        /// </summary>
        /// <param name="queries">The <see cref="Query"/> object or objects to be logged</param>
        public abstract void WriteLog(params Query[] queries);

        /// <summary>
        /// Writes query log to a data store asynchronously
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