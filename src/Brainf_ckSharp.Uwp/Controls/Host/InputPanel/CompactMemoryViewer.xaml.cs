using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Shared.ViewModels.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace Brainf_ckSharp.Uwp.Controls.Host.InputPanel;

/// <summary>
/// A compact memory viewer for the interactive REPL console
/// </summary>
public sealed partial class CompactMemoryViewer : UserControl
{
    public CompactMemoryViewer()
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetRequiredService<CompactMemoryViewerViewModel>();
    }

    /// <summary>
    /// Gets the <see cref="CompactMemoryViewerViewModel"/> instance currently in use
    /// </summary>
    public CompactMemoryViewerViewModel ViewModel => (CompactMemoryViewerViewModel)DataContext;
}
