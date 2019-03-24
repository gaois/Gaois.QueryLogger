namespace Gaois.QueryLogger
{
    /// <summary>
    /// Describes the types of alerts that can be sent to users
    /// </summary>
    public static class AlertTypes
    {
        /// <summary>
        /// An error adding queries to the log queue
        /// </summary>
        public const string EnqueueError = "Enqueue error: A query or queries were not successfully queued for logging.";

        /// <summary>
        /// An error where the log queue is full
        /// </summary>
        public const string QueueFull = "Log queue full: The list of queries waiting to be logged has reached the max queue size. You may need to increase the max queue size or to investigate issues that may be preventing log writes.";

        /// <summary>
        /// An error writing logs to the data store
        /// </summary>
        public const string LogWriteError = "Log write error: A query or queries could not be written to the data store.";
    }
}