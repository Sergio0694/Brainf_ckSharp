using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages;
using Brainf_ckSharp.Uwp.Controls.SubPages.Interfaces;
using Microsoft.Extensions.DependencyInjection;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.SubPages.Views;

/// <summary>
/// A sub page that displays the library of recent and sample source codes
/// </summary>
public sealed partial class CodeLibrarySubPage : UserControl, IConstrainedSubPage
{
    public CodeLibrarySubPage()
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetRequiredService<CodeLibrarySubPageViewModel>();
    }

    /// <summary>
    /// Gets the <see cref="CodeLibrarySubPageViewModel"/> instance currently in use
    /// </summary>
    public CodeLibrarySubPageViewModel ViewModel => (CodeLibrarySubPageViewModel)DataContext;

    /// <inheritdoc/>
    public double MaxExpandedWidth { get; } = 520;

    /// <inheritdoc/>
    public double MaxExpandedHeight { get; } = double.PositiveInfinity;
}
