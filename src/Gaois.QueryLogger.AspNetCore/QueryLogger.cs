using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Gaois.QueryLogger.Data;

namespace Gaois.QueryLogger
{
    /// <summary>
    /// Logs query data to a data store
    /// </summary>
    public partial class QueryLoggerCore : IQueryLogger
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOptions<QueryLoggerSettings> _settings;

        /// <summary>
        /// Logs query data to a data store
        /// </summary>
        public QueryLoggerCore(
            IHttpContextAccessor contextAccessor, 
            IOptions<QueryLoggerSettings> settings)
        {
            _contextAccessor = contextAccessor;
            _settings = settings;
        }

        /// <summary>
        /// Logs query data to a data store
        /// </summary>
        /// <param name="queries">The <see cref="Query"/> object or objects to be logged</param>
        public void Log(params Query[] queries)
        {
            var context = _contextAccessor.HttpContext;

            foreach (Query query in queries)
            {
                string host = context.Request.Host.ToString();
                string ipAddress = (String.IsNullOrEmpty(query.IPAddress)) 
                    ? context.Connection.RemoteIpAddress.ToString() 
                    : query.IPAddress;

                query.QueryID = (query.QueryID == null) ? Guid.NewGuid() : query.QueryID;
                query.Host = (String.IsNullOrEmpty(query.Host)) ? host : query.Host;
                query.IPAddress = IPAddressProcessor.Process(ipAddress, _settings.Value);
                query.LogDate = (query.LogDate == null) ? DateTime.UtcNow : query.LogDate;
            }

            try
            {
                LogStore.LogQuery(_settings.Value.Store.ConnectionString, queries);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
