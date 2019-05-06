using Ansa.Extensions;
using AutoMapper;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace Gaois.QueryLogger
{
    internal class ConfigurationSettings : ConfigurationSection
    {
        private static readonly Lazy<IMapper> _lazyMapper = new Lazy<IMapper>(() =>
        {
            var configuration = new MapperConfiguration(config => {
                config.ShouldMapProperty = p => p.GetMethod.IsPublic || p.GetMethod.IsAssembly;
                config.CreateMap<ConfigurationSettings, QueryLoggerSettings>();
                config.CreateMap<ExcludedIPAddressSettings, ExcludedIPAddress>();
            });

            return configuration.CreateMapper();
        });

        private static IMapper _mapper => _lazyMapper.Value;
        private static ConfigurationSettings _settings => ConfigurationManager.GetSection("QueryLogger") as ConfigurationSettings;

        public static QueryLoggerSettings Settings => _mapper.Map<QueryLoggerSettings>(_settings);

        [ConfigurationProperty("applicationName")]
        public string ApplicationName => this["applicationName"] as string;

        [ConfigurationProperty("isEnabled", DefaultValue = true)]
        public bool? IsEnabled => (bool?)this["isEnabled"];

        [ConfigurationProperty("Store")]
        public QueryLoggerStoreSettings Store => this["Store"] as QueryLoggerStoreSettings;
        public class QueryLoggerStoreSettings : ConfigurationElement
        {
            [ConfigurationProperty("connectionStringName")]
            public string ConnectionStringName => this["connectionStringName"] as string;

            [ConfigurationProperty("connectionString")]
            public string ConnectionString => (this["connectionString"] as string).HasValue() 
                ? this["connectionString"] as string
                : ConnectionStringName.HasValue()
                    ? ConfigurationManager.ConnectionStrings[ConnectionStringName]?.ConnectionString 
                        ?? throw new ConfigurationErrorsException($"A connection string was not found for the connection string name provided: {ConnectionStringName}")
                    : null;

            [ConfigurationProperty("maxQueueRetryTime", DefaultValue = 30000)]
            public int? MaxQueueRetryTime => (int?)this["maxQueueRetryTime"];

            [ConfigurationProperty("maxQueueSize", DefaultValue = 1000)]
            public int? MaxQueueSize => (int?)this["maxQueueSize"];

            [ConfigurationProperty("tableName", DefaultValue = "QueryLogs")]
            public string TableName => this["tableName"] as string;
        }

        [ConfigurationProperty("storeClientIPAddress", DefaultValue = true)]
        public bool StoreClientIPAddress => (bool)this["storeClientIPAddress"];

        [ConfigurationProperty("anonymizeIPAddress", DefaultValue = IPAddressAnonymizationLevel.Partial)]
        public IPAddressAnonymizationLevel AnonymizeIPAddress => (IPAddressAnonymizationLevel)this["anonymizeIPAddress"];

        [ConfigurationProperty("alertInterval", DefaultValue = 300000)]
        public int? AlertInterval => (int?)this["alertInterval"];

        [ConfigurationProperty("Email")]
        public EmailSettings Email => this["Email"] as EmailSettings;
        public class EmailSettings : ConfigurationElement
        {
            public MailAddress FromMailAddress => GetMailAddress();

            public NetworkCredential SMTPCredentials => GetCredentials();

            [ConfigurationProperty("toAddress")]
            public string ToAddress => this["toAddress"] as string;
            
            [ConfigurationProperty("fromAddress")]
            public string FromAddress => this["fromAddress"] as string;
            
            [ConfigurationProperty("fromDisplayName")]
            public string FromDisplayName => this["fromDisplayName"] as string;

            [ConfigurationProperty("smtpHost")]
            public string SMTPHost => this["smtpHost"] as string;

            [ConfigurationProperty("smtpPort")]
            public int? SMTPPort => (int?)this["smtpPort"];
            
            [ConfigurationProperty("smtpUserName")]
            public string SMTPUserName => this["smtpUserName"] as string;
            
            [ConfigurationProperty("smtpPassword")]
            public string SMTPPassword => this["smtpPassword"] as string;

            [ConfigurationProperty("smtpEnableSSL", DefaultValue = false)]
            public bool? SMTPEnableSSL => (bool?)this["smtpEnableSSL"];

            private MailAddress GetMailAddress()
            {
                try
                {
                    return FromDisplayName.HasValue()
                        ? new MailAddress(FromAddress, FromDisplayName)
                        : new MailAddress(FromAddress);
                }
                catch
                {
                    return null;
                }
            }

            private NetworkCredential GetCredentials() =>
                SMTPUserName.HasValue() && SMTPPassword.HasValue()
                    ? new NetworkCredential(SMTPUserName, SMTPPassword)
                    : null;
        }
        
        [ConfigurationProperty("ExcludedIPAddresses", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(ExcludedIPAddressCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public ExcludedIPAddressCollection ExcludedIPAddresses => this["ExcludedIPAddresses"] as ExcludedIPAddressCollection;
        public class ExcludedIPAddressSettings : ConfigurationElement
        {
            [ConfigurationProperty("name")]
            public string Name => this["name"] as string;

            [ConfigurationProperty("ipAddress", IsRequired = true, IsKey = true)]
            public string IPAddress => this["ipAddress"] as string;
        }

        public class ExcludedIPAddressCollection : ConfigurationElementCollection
        {
            public ExcludedIPAddressSettings this[int index]
            {
                get => (ExcludedIPAddressSettings)BaseGet(index);
                set
                {
                    if (BaseGet(index) != null)
                        BaseRemoveAt(index);
                    BaseAdd(index, value);
                }
            }

            public void Add(ExcludedIPAddressSettings settings) => BaseAdd(settings);

            public void Clear() => BaseClear();

            protected override ConfigurationElement CreateNewElement() => new ExcludedIPAddressSettings();

            protected override object GetElementKey(ConfigurationElement element) => ((ExcludedIPAddressSettings)element).IPAddress;

            public void Remove(ExcludedIPAddressSettings settings) => BaseRemove(settings.IPAddress);

            public void RemoveAt(int index) => BaseRemoveAt(index);

            public void Remove(string name) => BaseRemove(name);
        }
    }
}