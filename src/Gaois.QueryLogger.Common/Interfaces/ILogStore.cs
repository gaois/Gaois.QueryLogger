using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Gaois.QueryLogger
{
    /// <summary>
    /// Log data to persistent storage
    /// </summary>
    public interface ILogStore
    {
        /// <summary>
        /// Gets the queue of logs waiting to be written
        /// </summary>
        BlockingCollection<Query> LogQueue { get; }

        /// <summary>
        /// Verifies that the log queue is being processed; if not, initializes queue consumption.
        /// </summary>
        void ProcessQueue();

        /// <summary>
        /// Queues query data for logging to a data store
        /// </summary>
        /// <param name="queries">The <see cref="Query"/> object or objects to be logged</param>
        void Enqueue(Query[] queries);

        /// <summary>
        /// Writes query log to a data store
        /// </summary>
        /// <param name="queries">The <see cref="Query"/> object or objects to be logged</param>
        void WriteLog(params Query[] queries);

        /// <summary>
        /// Writes query log to a data store asynchronously
        /// </summary>
        /// <param name="queries">The <see cref="Query"/> object or objects to be logged</param>
        Task WriteLogAsync(params Query[] queries);

        /// <summary>
        /// Alerts designated users in case of possible issues with the query logger service
        /// </summary>
        /// <param name="alert"></param>
        void Alert(Alert alert);
    }
}