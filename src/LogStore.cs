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
        /// <param name="query">The <see cref="Query"/> object to be logged</param>
        /// <param name="connectionString">The connection string for a SQL Server database</param>
        public static void LogQuery(Query query, string connectionString)
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
                            @ExecutedSuccessfully, @ExecutionTime, @ResultCount, @LogDate, @JsonData)", 
                        new { QueryID = query.QueryID, ApplicationName = query.ApplicationName, QueryCategory = query.QueryCategory, 
                            QueryText = query.QueryText, Host = query.Host, IPAddress = query.IPAddress, 
                            ExecutedSuccessfully = query.ExecutedSuccessfully, ExecutionTime = query.ExecutionTime, 
                            ResultCount = query.ResultCount, LogDate = query.LogDate, JsonData = query.JsonData });
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
        } 
    }
}