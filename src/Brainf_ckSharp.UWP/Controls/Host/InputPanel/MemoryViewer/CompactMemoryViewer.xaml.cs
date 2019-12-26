using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.UWP.ViewModels.Controls;

namespace Brainf_ckSharp.UWP.Controls.Host.InputPanel.MemoryViewer
{
    /// <summary>
    /// A compact memory viewer for the interactive REPL console
    /// </summary>
    public sealed partial class CompactMemoryViewer : UserControl
    {
        public CompactMemoryViewer()
        {
            this.InitializeComponent();
            this.DataContext = new CompactMemoryViewerViewModel();
        }

        /// <summary>
        /// Gets the <see cref="CompactMemoryViewerViewModel"/> instance for the current view
        /// </summary>
        public CompactMemoryViewerViewModel? ViewModel => this.DataContext as CompactMemoryViewerViewModel;
    }
}
