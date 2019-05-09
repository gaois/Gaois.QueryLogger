namespace Gaois.QueryLogger
{
    /// <summary>
    /// Describes status of last sent alert
    /// </summary>
    public enum AlertStatus
    {
        /// <summary>
        /// No alert status has been set
        /// </summary>
        NotSet = 0,
        /// <summary>
        /// An alert has been recently sent - wait to send again
        /// </summary>
        Wait = 1,
        /// <summary>
        /// No alert was sent recently - send alert
        /// </summary>
        DoSend = 2
    }
}