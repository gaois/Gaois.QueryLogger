using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;

namespace Gaois.QueryLogger.Tests.AspNetCore
{
    internal class TestHelper
    {
        public static string BasePath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string SettingsBasePath => Path.Combine(BasePath, "Settings");

        public static IConfigurationRoot GetConfiguration(string jsonFileName)
        {
            return new ConfigurationBuilder()
                .SetBasePath(SettingsBasePath)
                .AddJsonFile(jsonFileName, optional: true)
                .Build();
        }

        public static QueryLoggerSettings GetQueryLoggerConfiguration(string jsonFileName)
        {
            var configuration = new QueryLoggerSettings();
            var configRoot = GetConfiguration(jsonFileName);
            configRoot.GetSection("QueryLogger").Bind(configuration);
            return configuration;
        }
    }
}