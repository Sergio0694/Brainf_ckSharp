using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;

namespace Brainf_ckSharp.Shared.ViewModels.Controls.SubPages
{
    /// <summary>
    /// A view model for the review prompt in the app
    /// </summary>
    public sealed class ReviewPromptSubPageViewModel : ObservableObject
    {
        /// <summary>
        /// The <see cref="IEmailService"/> instance currently in use
        /// </summary>
        private readonly IEmailService EmailService = Ioc.Default.GetRequiredService<IEmailService>();

        /// <summary>
        /// The <see cref="IStoreService"/> instance currently in use
        /// </summary>
        private readonly IStoreService StoreService = Ioc.Default.GetRequiredService<IStoreService>();

        /// <summary>
        /// The <see cref="ISystemInformationService"/> instance currently in use
        /// </summary>
        private readonly ISystemInformationService SystemInformationService = Ioc.Default.GetRequiredService<ISystemInformationService>();

        /// <summary>
        /// Creates a new <see cref="ReviewPromptSubPageViewModel"/> instance
        /// </summary>
        public ReviewPromptSubPageViewModel()
        {
            ReviewCommand = new AsyncRelayCommand(StoreService.RequestReviewAsync);
            FeedbackEmailCommand = new AsyncRelayCommand(SendFeedbackEmailAsync);
        }

        /// <summary>
        /// Gets the <see cref="ICommand"/> responsible for requesting a Store review
        /// </summary>
        public ICommand ReviewCommand { get; }

        /// <summary>
        /// Gets the <see cref="ICommand"/> responsible for sending a feedback email
        /// </summary>
        public ICommand FeedbackEmailCommand { get; }

        /// <summary>
        /// Prepares and sends a feedback email
        /// </summary>
        private Task SendFeedbackEmailAsync()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine("==========================");
            builder.AppendLine($"[AppVersion]: {SystemInformationService.ApplicationVersion}");
            builder.AppendLine($"[CPU architecture]: {SystemInformationService.CpuArchitecture}");
            builder.AppendLine($"[OS]: {SystemInformationService.OperatingSystemVersion}");
            builder.AppendLine($"[OS build]: {SystemInformationService.OperatingSystemVersion}");

            string body = builder.ToString();

            return EmailService.TryComposeEmailAsync(DeveloperInfo.FeedbackEmail, "Brainf*ck# feedback", body);
        }
    }
}
