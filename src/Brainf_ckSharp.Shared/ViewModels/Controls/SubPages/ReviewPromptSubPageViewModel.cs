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
    private readonly IEmailService emailService;

    /// <summary>
    /// The <see cref="IStoreService"/> instance currently in use
    /// </summary>
    private readonly IStoreService storeService;

    /// <summary>
    /// The <see cref="ISystemInformationService"/> instance currently in use
    /// </summary>
    private readonly ISystemInformationService systemInformationService;

    /// <summary>
    /// Creates a new <see cref="ReviewPromptSubPageViewModel"/> instance
    /// </summary>
    /// <param name="emailService">The <see cref="IEmailService"/> instance to use</param>
    /// <param name="storeService">The <see cref="IStoreService"/> instance to use</param>
    /// <param name="systemInformationService">The <see cref="ISystemInformationService"/> instance to use</param>
    public ReviewPromptSubPageViewModel(IEmailService emailService, IStoreService storeService, ISystemInformationService systemInformationService)
    {
        this.emailService = emailService;
        this.storeService = storeService;
        this.systemInformationService = systemInformationService;
    }

    /// <inheritdoc cref="IStoreService.RequestReviewAsync"/>
    [RelayCommand]
    private Task RequestReviewAsync()
    {
        return this.storeService.RequestReviewAsync();
    }

    /// <summary>
    /// Prepares and sends a feedback email
    /// </summary>
    [RelayCommand]
    private Task SendFeedbackEmailAsync()
    {
        string body = $"""


            ==========================
            [AppVersion]: {this.systemInformationService.ApplicationVersion}
            [CPU architecture]: {this.systemInformationService.CpuArchitecture}
            [OS]: {this.systemInformationService.OperatingSystemVersion}
            [OS build]: {this.systemInformationService.OperatingSystemVersion}
            """;

        return this.emailService.TryComposeEmailAsync(DeveloperInfo.FeedbackEmail, "Brainf*ck# feedback", body);
    }
}
