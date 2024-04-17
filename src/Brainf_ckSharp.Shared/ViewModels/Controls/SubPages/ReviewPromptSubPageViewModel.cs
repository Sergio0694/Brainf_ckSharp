using System.Text;
using System.Threading.Tasks;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Brainf_ckSharp.Shared.ViewModels.Controls.SubPages;

/// <summary>
/// A view model for the review prompt in the app
/// </summary>
public sealed partial class ReviewPromptSubPageViewModel : ObservableObject
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
        this.EmailService = emailService;
        this.StoreService = storeService;
        this.SystemInformationService = systemInformationService;
    }

    /// <inheritdoc cref="IStoreService.RequestReviewAsync"/>
    [RelayCommand]
    private Task RequestReviewAsync()
    {
        return this.StoreService.RequestReviewAsync();
    }

    /// <summary>
    /// Prepares and sends a feedback email
    /// </summary>
    [RelayCommand]
    private Task SendFeedbackEmailAsync()
    {
        StringBuilder builder = new();

        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine("==========================");
        builder.AppendLine($"[AppVersion]: {this.SystemInformationService.ApplicationVersion}");
        builder.AppendLine($"[CPU architecture]: {this.SystemInformationService.CpuArchitecture}");
        builder.AppendLine($"[OS]: {this.SystemInformationService.OperatingSystemVersion}");
        builder.AppendLine($"[OS build]: {this.SystemInformationService.OperatingSystemVersion}");

        string body = builder.ToString();

        return this.EmailService.TryComposeEmailAsync(DeveloperInfo.FeedbackEmail, "Brainf*ck# feedback", body);
    }
}
