using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Brainf_ck_sharp.Legacy.UWP.Helpers.WindowsAPIs
{
    /// <summary>
    /// Static class with the helper methods to send emails from the app
    /// </summary>
    public static class EmailHelper
    {
        /// <summary>
        /// Developer feedback email
        /// </summary>
        public const string FeedbackEmail = "apps.sergiopedri@outlook.com";

        /// <summary>
        /// Sends a feedback email to the developer
        /// </summary>
        public static async Task SendFeedbackEmail()
        {
            EmailMessage email = new EmailMessage();
            email.To.Add(new EmailRecipient(FeedbackEmail, "Sergio Pedri"));
            email.Subject = "Brainf*ck# feedback";
            await EmailManager.ShowComposeNewEmailAsync(email);
        }

        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="address">The email address to use</param>
        /// <param name="subject"></param>
        /// <param name="body">The optional body of the message</param>
        /// <param name="attachment">The optional attachment of the email message</param>
        public static async Task<bool> SendEmail(string address, string subject = null, string body = null, StorageFile attachment = null)
        {
            // Create the email message and prepare its info
            EmailMessage email = new EmailMessage();
            if (!string.IsNullOrEmpty(address)) email.To.Add(new EmailRecipient(address));
            if (subject != null) email.Subject = subject;
            if (body != null) email.Body = body;
            if (attachment != null)
            {
                RandomAccessStreamReference stream = RandomAccessStreamReference.CreateFromFile(attachment);
                email.Attachments.Add(new EmailAttachment(attachment.Name, stream));
            }

            // Try to send the email message
            try
            {
                await EmailManager.ShowComposeNewEmailAsync(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
