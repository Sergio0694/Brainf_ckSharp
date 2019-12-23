using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.UWP.ViewModels;

#nullable enable

namespace Brainf_ckSharp.UWP.Views
{
    /// <summary>
    /// A view for an interactive REPL console for Brainf*ck/PBrain
    /// </summary>
    public sealed partial class ConsoleView : UserControl
    {
        public ConsoleView()
        {
            this.InitializeComponent();
            this.DataContext = new ConsoleViewModel();
        }

        /// <summary>
        /// Gets the <see cref="ConsoleViewModel"/> instance for the current view
        /// </summary>
        public ConsoleViewModel? ViewModel => this.DataContext as ConsoleViewModel;
    }
}
