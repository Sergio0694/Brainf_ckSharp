using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using GitHub.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Launcher = Windows.System.Launcher;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.DataTemplates;

/// <summary>
/// A template for a GitHub user
/// </summary>
public sealed partial class DeveloperTemplate : UserControl
{
    public DeveloperTemplate()
    {
        this.InitializeComponent();
        this.DataContextChanged += (s, e) => this.Bindings.Update();
    }

    /// <summary>
    /// Gets the <see cref="User"/> instance for the current view
    /// </summary>
    public User? ViewModel => DataContext as User;

    // Hides the progress ring
    private void ImageExBase_OnImageExOpened(object sender, ImageExOpenedEventArgs e) => this.LoadingRing.Visibility = Visibility.Collapsed;

    // Opens the profile page of the current contributor
    private void Contributor_Clicked(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(ViewModel?.ProfilePageUrl))
        {
            _ = Launcher.LaunchUriAsync(new Uri(ViewModel!.ProfilePageUrl, UriKind.Absolute));

            App.Current.Services.GetRequiredService<IAnalyticsService>().Log(EventNames.GitHubProfileOpened);
        }
    }
}
