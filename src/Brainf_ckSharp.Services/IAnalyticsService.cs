namespace Brainf_ckSharp.Services
{
    /// <summary>
    /// The default <see langword="interface"/> for an analytics service
    /// </summary>
    public interface IAnalyticsService
    {
        /// <summary>
        /// Initializes the current <see cref="IAnalyticsService"/> instance
        /// </summary>
        /// <param name="secret">The secret token to use to initialize the service</param>
        void Initialize(string secret);

        /// <summary>
        /// Logs an event with a specified title and optional properties
        /// </summary>
        /// <param name="title">The title of the event to track</param>
        /// <param name="data">The optional event properties</param>
        void Log(string title, params (string Property, string Value)[]? data);
    }
}
