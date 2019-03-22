using Dapper;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Gaois.QueryLogger
{
    /// <summary>
    /// Stores log data in a SQL Server database
    /// </summary>
    public sealed class SqlLogStore : LogStore
    {
        private static readonly Lazy<SqlLogStore> _logStore = new Lazy<SqlLogStore>(() => new SqlLogStore(_settings));
        private static QueryLoggerSettings _settings = ConfigurationSettings.Settings;

        /// <summary>
        /// Stores log data in a SQL Server database
        /// </summary>
        public static SqlLogStore Instance { get => _logStore.Value; }

        private SqlLogStore(QueryLoggerSettings settings) : base(settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Logs query data to a data store
        /// </summary>
        /// <param name="queries">The <see cref="Query"/> object or objects to be logged</param>
        public override void WriteLog(params Query[] queries)
        {
            _ = queries ?? throw new ArgumentNullException(nameof(queries));

            using (var db = new SqlConnection(_settings.Store.ConnectionString))
                db.Execute(SqlQueries.WriteLog(_settings.Store.TableName), queries);
        }

        /// <summary>
        /// Logs query data in a data store asynchronously
        /// </summary>
        /// <param name="queries">The <see cref="Query"/> object or objects to be logged</param>
        public override async Task WriteLogAsync(params Query[] queries)
        {
            _ = queries ?? throw new ArgumentNullException(nameof(queries));

            using (var db = new SqlConnection(_settings.Store.ConnectionString))
                await db.ExecuteAsync(SqlQueries.WriteLog(_settings.Store.TableName), queries).ConfigureAwait(false);
        }
    }
}