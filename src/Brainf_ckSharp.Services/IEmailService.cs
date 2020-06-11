using System.Threading.Tasks;

namespace Brainf_ckSharp.Services
{
    /// <summary>
    /// The default <see langword="interface"/> for a service that handles emails
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Tries to open the compose window for a new email with the specified parameters
        /// </summary>
        /// <param name="address">The (optional) target address to send the email to</param>
        /// <param name="subject">The (optional) subject of the email</param>
        /// <param name="body">The (optional) body of the email</param>
        /// <returns>A <see cref="Task"/> that completes when the email has been completed</returns>
        Task TryComposeEmailAsync(string? address = null, string? subject = null, string? body = null);
    }
}
