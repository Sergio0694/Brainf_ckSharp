using System.Buffers;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages;
using Brainf_ckSharp.Uwp.Controls.SubPages.Interfaces;
using Microsoft.Extensions.DependencyInjection;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.SubPages.Views;

/// <summary>
/// A sub page that displays the result of a script being executed from the IDE
/// </summary>
public sealed partial class IdeResultSubPage : UserControl, IConstrainedSubPage
{
    /// <summary>
    /// Creates a new <see cref="IdeResultSubPage"/> instance with the specified parameters
    /// </summary>
    /// <param name="script">The script to execute</param>
    /// <param name="breakpoints">The optional breakpoints to use</param>
    public IdeResultSubPage(string script, IMemoryOwner<int>? breakpoints = null)
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetRequiredService<IdeResultSubPageViewModel>();
        this.Unloaded += (s, e) => ViewModel.IsActive = false;

        ViewModel.Script = script;
        ViewModel.Breakpoints = breakpoints;
    }

    /// <summary>
    /// Gets the <see cref="IdeResultSubPageViewModel"/> instance currently in use
    /// </summary>
    public IdeResultSubPageViewModel ViewModel => (IdeResultSubPageViewModel)DataContext;

    /// <inheritdoc/>
    public double MaxExpandedWidth { get; } = 540;

    /// <inheritdoc/>
    public double MaxExpandedHeight { get; } = double.PositiveInfinity;
}
