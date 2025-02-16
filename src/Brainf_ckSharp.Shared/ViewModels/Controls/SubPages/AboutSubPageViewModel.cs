using System.Collections.Generic;
using System.Threading.Tasks;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GitHub;
using User = GitHub.Models.User;

#pragma warning disable IDE0290

namespace Brainf_ckSharp.Shared.ViewModels.Controls.SubPages;

/// <summary>
/// A view model for the about page in the app
/// </summary>
public sealed partial class AboutSubPageViewModel : ObservableObject
{
    /// <summary>
    /// The <see cref="IGitHubService"/> instance currently in use
    /// </summary>
    private readonly IGitHubService gitHubService;

    /// <summary>
    /// The <see cref="IGitHubService"/> instance currently in use
    /// </summary>
    private readonly ISystemInformationService systemInformationService;

    /// <summary>
    /// Creates a new <see cref="AboutSubPageViewModel"/> instance
    /// </summary>
    /// <param name="gitHubService">The <see cref="IGitHubService"/> instance to use</param>
    /// <param name="systemInformationService">The <see cref="ISystemInformationService"/> instance to use</param>
    public AboutSubPageViewModel(
        IGitHubService gitHubService,
        ISystemInformationService systemInformationService)
    {
        this.gitHubService = gitHubService;
        this.systemInformationService = systemInformationService;
    }

    /// <summary>
    /// Forwards the <see cref="ThisAssembly.Git.Branch"/> property
    /// </summary>
    public string GitBranch => ThisAssembly.Git.Branch;

    /// <summary>
    /// Forwards the <see cref=" ThisAssembly.Git.Commit"/> property
    /// </summary>
    public string GitCommit => ThisAssembly.Git.Commit;

    /// <summary>
    /// Gets the current app version.
    /// </summary>
    public string AppVersion => this.systemInformationService.ApplicationVersion;

    /// <summary>
    /// Gets the name of the current build configuration
    /// </summary>
    public string BuildConfiguration
#if DEBUG
        => "DEBUG";
#else
        => "RELEASE";
#endif

    /// <summary>
    /// Gets the list of lead developers to the Legere repository
    /// </summary>
    [ObservableProperty]
    private static IEnumerable<User>? developers;

    /// <summary>
    /// Gets the list of featured links to use
    /// </summary>
    [ObservableProperty]
    private static IEnumerable<string>? featuredLinks;

    /// <summary>
    /// Loads all the necessary data for the view model
    /// </summary>
    [RelayCommand]
    public async Task LoadDataAsync()
    {
        if (Developers != null)
        {
            return;
        }

        try
        {
            Developers = [await this.gitHubService.GetUserAsync(DeveloperInfo.GitHubUsername)];
            FeaturedLinks = [DeveloperInfo.PayPalMeUrl];
        }
        catch
        {
            // Whoops!
        }
    }
}

