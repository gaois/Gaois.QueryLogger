using System;

namespace Gaois.QueryLogger
{
    /// <summary>
    /// Provides methods to process and anonymize IP addresses
    /// </summary>
    public static class IPAddressProcessor
    {
        /// <summary>
        /// Assesses whether the IP address is to be anonymized and excutes anonymization if necessary
        /// </summary>
        /// <param name="ip">An IP address</param>
        /// <param name="settings">The <see cref="QueryLoggerSettings"/> to configure the processor with</param>
        public static string Process(string ip, QueryLoggerSettings settings)
        {
            if (settings.StoreClientIPAddress == false)
            {
                return "PRIVATE";
            }

            if (String.IsNullOrEmpty(ip))
            {
                return "UNKNOWN";
            }

            string result;

            switch (settings.AnonymizeIPAddress)
            {
                case IPAddressAnonymizationLevel.None:
                    result = ip;
                    break;
                case IPAddressAnonymizationLevel.Partial:
                    result = PartiallyAnonymizeIP(ip);
                    break;
                default:
                    result = String.Empty;
                    break;
            }

            return result;
        }

        private static string PartiallyAnonymizeIP(string ip)
        {
            int lastPosition = ip.LastIndexOf(".");
            return (lastPosition > 0) ? ip.Substring(0, lastPosition) : ip;
        }
    }
}
