using System;
using System.Data.SqlClient;
using Dapper;

namespace Gaois.QueryLogger.Data
{
    /// <summary>
    /// Stores log data in a SQL Server database
    /// </summary>
    public static class LogStore
    {
        /// <summary>
        /// Logs query data in a data store
        /// </summary>
        /// <param name="queries">The <see cref="Query"/> object or objects to be logged</param>
        /// <param name="connectionString">The connection string for a SQL Server database</param>
        public static void LogQuery(string connectionString, params Query[] queries)
        {
            using (SqlConnection db = new SqlConnection(connectionString))
            {
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
                    db.Execute(@"INSERT INTO QueryLog (QueryID, ApplicationName, QueryCategory, QueryText, Host, IPAddress, 
                            ExecutedSuccessfully, ExecutionTime, ResultCount, LogDate, JsonData) 
                        VALUES (@QueryID, @ApplicationName, @QueryCategory, @QueryText, @Host, @IPAddress, 
                            @ExecutedSuccessfully, @ExecutionTime, @ResultCount, @LogDate, @JsonData)", queries);
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
        } 
    }
}