using Ansa.Extensions;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
            if (!settings.StoreClientIPAddress)
                return "PRIVATE";

            if (ip.IsNullOrWhiteSpace())
                return "UNKNOWN";

            string result = default(string);

            switch (settings.AnonymizeIPAddress)
            {
                case IPAddressAnonymizationLevel.None:
                    result = ip;
                    break;
                case IPAddressAnonymizationLevel.Partial:
                    result = PartiallyAnonymizeIP(ip);
                    break;
                default:
                    result = string.Empty;
                    break;
            }

            return result;
        }

        private static string PartiallyAnonymizeIP(string ip)
        {
            var anonymizedIP = default(string);

            if (IPAddress.TryParse(ip, out IPAddress address))
            {
                switch (address.AddressFamily)
                {
                    // IPv4
                    case AddressFamily.InterNetwork:
                        var lastPosition = ip.LastIndexOf(".");
                        anonymizedIP = (lastPosition > 0) ? ip.Substring(0, lastPosition) + ".0" : string.Empty;
                        break;
                    // IPv6
                    case AddressFamily.InterNetworkV6:
                        var fullIP = address.GetAddressBytes();
                        var abbreviatedIP = new byte[fullIP.Length - 10];
                        Array.Copy(fullIP, abbreviatedIP, 6);
                        anonymizedIP = Encoding.UTF8.GetString(abbreviatedIP) + "0000:0000:0000:0000:0000";
                        break;
                }
            }

            return anonymizedIP;
        }
    }
}