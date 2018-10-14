using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace Gaois.QueryLogger.Data
{
    /// <summary>
    /// Stores log data in a SQL Server database asynchronously
    /// </summary>
    public static partial class LogStore
    {
        /// <summary>
        /// Logs query data in a data store asynchronously
        /// </summary>
        /// <param name="queries">The <see cref="Query"/> object or objects to be logged</param>
        /// <param name="connectionString">The connection string for a SQL Server database</param>
        /// <returns>The number of queries successfully logged</returns>
        public static async Task<int> LogQueryAsync(string connectionString, params Query[] queries)
        {
            using (SqlConnection db = new SqlConnection(connectionString))
            {
                const string sql = @"INSERT INTO QueryLogs (QueryID, ApplicationName, QueryCategory, 
                    QueryTerms, QueryText, Host, IPAddress, ExecutedSuccessfully, ExecutionTime, ResultCount, LogDate, JsonData) 
                VALUES (@QueryID, @ApplicationName, @QueryCategory, @QueryTerms, @QueryText, @Host, @IPAddress, @ExecutedSuccessfully, 
                    @ExecutionTime, @ResultCount, @LogDate, @JsonData);";

                try
                {
                    db.Open();
                }
                catch (SqlException exception)
                {
                    throw exception;
                }

                try
                {
                    var count = await db.ExecuteAsync(sql, queries);
                    return count;
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
        }
    }
}