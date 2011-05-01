namespace Assman.BuildSupport
{
    /// <summary>
    /// Represents an interface for a build logger.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs a message to the logger.
        /// </summary>
        /// <param name="message"></param>
        void LogMessage(string message);
    }
}