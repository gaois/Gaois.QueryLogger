using System.Threading.Tasks;

namespace Gaois.QueryLogger
{
    /// <summary>
    /// Logs query data to a data store
    /// </summary>
    public interface IQueryLogger
    {
        /// <summary>
        /// Logs query data to a data store
        /// </summary>
        /// <param name="queries">The <see cref="Query"/> object or objects to be logged</param>
        /// <returns>The number of queries successfully logged</returns>
        int Log(params Query[] queries);

        /// <summary>
        /// Logs query data to a data store asynchronously
        /// </summary>
        /// <param name="queries">The <see cref="Query"/> object or objects to be logged</param>
        /// <returns>The number of queries successfully logged</returns>
        Task<int> LogAsync(params Query[] queries);
    }
}