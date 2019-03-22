using System.Configuration;

namespace Gaois.QueryLogger
{
    internal class ConfigurationSettings : ConfigurationSection
    {
        private static QueryLoggerSettings _settings => ConfigurationManager.GetSection("QueryLogger") as QueryLoggerSettings;

        public static QueryLoggerSettings Settings { get => _settings; }

        [ConfigurationProperty("applicationName")]
        public string ApplicationName => this["applicationName"] as string;

        [ConfigurationProperty("isEnabled")]
        public bool IsEnabled => (bool)this["isEnabled"];

        [ConfigurationProperty("Store")]
        public QueryLoggerStoreSettings Store => this["Store"] as QueryLoggerStoreSettings;
        public class QueryLoggerStoreSettings : ConfigurationElement
        {
            [ConfigurationProperty("connectionString")]
            public string ConnectionString => this["connectionString"] as string;

            [ConfigurationProperty("connectionStringName")]
            public string ConnectionStringName => this["connectionStringName"] as string;

            [ConfigurationProperty("tableName")]
            public string TableName => this["tableName"] as string;

            internal void Populate(QueryLoggerSettings settings)
            {
                var store = settings.Store;

                if (!string.IsNullOrWhiteSpace(ConnectionString))
                    store.ConnectionString = ConnectionString;

                if (!string.IsNullOrWhiteSpace(ConnectionStringName))
                {
                    store.ConnectionString = ConfigurationManager.ConnectionStrings[ConnectionStringName]?.ConnectionString
                        ?? throw new ConfigurationErrorsException($"A connection string was not found for the connection string name provided: {ConnectionStringName}");
                }
            }

            [ConfigurationProperty("storeClientIPAddress")]
            public bool StoreClientIPAddress => (bool)this["storeClientIPAddress"];

            [ConfigurationProperty("anonymizeIPAddress")]
            public IPAddressAnonymizationLevel AnonymizeIPAddress => (IPAddressAnonymizationLevel)this["anonymizeIPAddress"];
        }
    }
}