namespace Gaois.QueryLogger
{
    /// <summary>
    /// Represents different IP anonymization policies
    /// </summary>
    public enum IPAddressAnonymizationLevel
    {
        /// <summary>
        /// IP address will not be anonymized
        /// </summary>
        None = 0,

        /// <summary>
        /// The final octect of an IPv4 IP address will be removed
        /// </summary>
        Partial = 1
    }
}