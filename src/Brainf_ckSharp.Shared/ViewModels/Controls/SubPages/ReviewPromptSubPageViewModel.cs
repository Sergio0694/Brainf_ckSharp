using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Brainf_ckSharp.Shared.ViewModels.Controls.SubPages;

/// <summary>
/// A view model for the review prompt in the app
/// </summary>
public sealed class ReviewPromptSubPageViewModel : ObservableObject
{
    /// <summary>
    /// The <see cref="IEmailService"/> instance currently in use
    /// </summary>
    private readonly IEmailService EmailService;

    /// <summary>
    /// The <see cref="IStoreService"/> instance currently in use
    /// </summary>
    private readonly IStoreService StoreService;

    /// <summary>
    /// The <see cref="ISystemInformationService"/> instance currently in use
    /// </summary>
    private readonly ISystemInformationService SystemInformationService;

    /// <summary>
    /// Creates a new <see cref="ReviewPromptSubPageViewModel"/> instance
    /// </summary>
    /// <param name="emailService">The <see cref="IEmailService"/> instance to use</param>
    /// <param name="storeService">The <see cref="IStoreService"/> instance to use</param>
    /// <param name="systemInformationService">The <see cref="ISystemInformationService"/> instance to use</param>
    public ReviewPromptSubPageViewModel(IEmailService emailService, IStoreService storeService, ISystemInformationService systemInformationService)
    {
        EmailService = emailService;
        StoreService = storeService;
        SystemInformationService = systemInformationService;

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
        StringBuilder builder = new();

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
