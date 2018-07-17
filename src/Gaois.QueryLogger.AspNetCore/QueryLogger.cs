using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Gaois.QueryLogger.Data;

namespace Gaois.QueryLogger.AspNetCore
{
    /// <summary>
    /// Logs query data to a data store
    /// </summary>
    public partial class QueryLogger : IQueryLogger
    {
        private readonly IHttpContextAccessor Accessor;
        private readonly IOptions<QueryLoggerSettings> Settings;

        /// <summary>
        /// Logs query data to a data store
        /// </summary>
        public QueryLogger(IHttpContextAccessor accessor, IOptions<QueryLoggerSettings> settings)
        {
            Accessor = accessor;
            Settings = settings;
        }

        /// <summary>
        /// Logs query data to a data store
        /// </summary>
        /// <param name="queries">The <see cref="Query"/> object or objects to be logged</param>
        public void Log(params Query[] queries)
        {
            var context = Accessor.HttpContext;

            foreach (Query query in queries)
            {
                string host = context.Request.Host.ToString();
                string ipAddress = (String.IsNullOrEmpty(query.IPAddress)) ? context.Connection.RemoteIpAddress.ToString() : query.IPAddress;

                query.QueryID = (query.QueryID == null) ? Guid.NewGuid() : query.QueryID;
                query.Host = (String.IsNullOrEmpty(query.Host)) ? host : query.Host;
                query.IPAddress = IPAddressProcessor.Process(ipAddress, Settings.Value);
                query.LogDate = (query.LogDate == null) ? DateTime.UtcNow : query.LogDate;
            }

            try
            {
                LogStore.LogQuery(Settings.Value.ConnectionString, queries);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
