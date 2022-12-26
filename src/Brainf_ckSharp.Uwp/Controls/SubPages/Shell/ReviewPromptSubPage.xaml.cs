using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages;
using Brainf_ckSharp.Uwp.Controls.SubPages.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Brainf_ckSharp.Uwp.Controls.SubPages.Shell;

public sealed partial class ReviewPromptSubPage : UserControl, IConstrainedSubPage
{
    public ReviewPromptSubPage()
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetRequiredService<ReviewPromptSubPageViewModel>();
    }

    /// <summary>
    /// Gets the <see cref="ReviewPromptSubPageViewModel"/> instance currently in use
    /// </summary>
    public ReviewPromptSubPageViewModel ViewModel => (ReviewPromptSubPageViewModel)DataContext;

    /// <inheritdoc/>
    public double MaxExpandedWidth { get; } = 400;

    /// <inheritdoc/>
    public double MaxExpandedHeight { get; } = 280;

    /// <summary>
    /// Requests to close the current sub page when an action is selected
    /// </summary>
    /// <param name="sender">The <see cref="Button"/> that was clicked</param>
    /// <param name="e">The empty <see cref="RoutedEventArgs"/> instance for the current event</param>
    private void ActionButton_OnClick(object sender, RoutedEventArgs e)
    {
        App.Current.SubPageHost.CloseSubFramePage();
    }
}
