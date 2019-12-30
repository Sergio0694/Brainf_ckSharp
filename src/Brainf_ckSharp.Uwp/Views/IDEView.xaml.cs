using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Uwp.ViewModels.Views;

#nullable enable

namespace Brainf_ckSharp.Uwp.Views
{
    /// <summary>
    /// A view for a Brainf*ck/PBrain IDE
    /// </summary>
    public sealed partial class IDEView : UserControl
    {
        public IDEView()
        {
            this.InitializeComponent();
            this.DataContext = new IDEViewModel();
        }

        /// <summary>
        /// Gets the <see cref="IDEViewModel"/> instance for the current view
        /// </summary>
        public IDEViewModel? ViewModel => this.DataContext as IDEViewModel;
    }
}
