using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;

#nullable enable

namespace Brainf_ckSharp.Services.Uwp.Email
{
    /// <summary>
    /// A <see langword="class"/> that handles emails
    /// </summary>
    public sealed class EmailService : IEmailService
    {
        /// <inheritdoc/>
        public async Task TryComposeEmailAsync(string? address = null, string? subject = null, string? body = null)
        {
            EmailMessage email = new();

            if (!(address is null)) email.To.Add(new EmailRecipient(address));
            if (!(subject is null)) email.Subject = subject;
            if (!(body is null)) email.Body = body;

            // Try to send the email message
            try
            {
                await EmailManager.ShowComposeNewEmailAsync(email);
            }
            catch
            {
                // Ignore errors
            }
        }
    }
}
