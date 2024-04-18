using System.Collections.Generic;
using System.Threading.Tasks;
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
    /// Creates a new <see cref="AboutSubPageViewModel"/> instance
    /// </summary>
    /// <param name="gitHubService">The <see cref="IGitHubService"/> instance to use</param>
    public AboutSubPageViewModel(IGitHubService gitHubService)
    {
        this.gitHubService = gitHubService;
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
    /// Gets the name of the current build configuration
    /// </summary>
    public string BuildConfiguration
#if DEBUG
        => "DEBUG";
#else
        => "RELEASE";
#endif

    private static IEnumerable<User>? developers;

    /// <summary>
    /// Gets the list of lead developers to the Legere repository
    /// </summary>
    public IEnumerable<User>? Developers
    {
        get => developers;
        private set => SetProperty(ref developers, value);
    }

    private static IEnumerable<string>? featuredLinks;

    /// <summary>
    /// Gets the list of featured links to use
    /// </summary>
    public IEnumerable<string>? FeaturedLinks
    {
        get => featuredLinks;
        private set => SetProperty(ref featuredLinks, value);
    }

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

