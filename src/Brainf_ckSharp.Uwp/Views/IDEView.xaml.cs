using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Uwp.ViewModels.Views;

#nullable enable

namespace Brainf_ckSharp.Uwp.Views
{
    /// <summary>
    /// A view for a Brainf*ck/PBrain IDE
    /// </summary>
    public sealed partial class IdeView : UserControl
    {
        public IdeView()
        {
            this.InitializeComponent();
            this.DataContext = new IdeViewModel();
        }

        /// <summary>
        /// Gets the <see cref="IdeViewModel"/> instance for the current view
        /// </summary>
        public IdeViewModel? ViewModel => this.DataContext as IdeViewModel;
    }
}
