namespace Gaois.QueryLogger
{
    /// <summary>
    /// SQL queries for log persistence
    /// </summary>
    public static class SqlQueries
    {
        /// <summary>
        /// SQL insert string that writes log to a given table
        /// </summary>
        /// <param name="tableName">The name of the database table that will be written to</param>
        public static string WriteLog(string tableName) => $@"INSERT INTO {tableName} (QueryID, ApplicationName, QueryCategory, 
                QueryTerms, QueryText, Host, IPAddress, ExecutedSuccessfully, ExecutionTime, ResultCount, LogDate, JsonData) 
            VALUES (@QueryID, @ApplicationName, @QueryCategory, @QueryTerms, @QueryText, @Host, @IPAddress, @ExecutedSuccessfully, 
                @ExecutionTime, @ResultCount, @LogDate, @JsonData);";
    }
}