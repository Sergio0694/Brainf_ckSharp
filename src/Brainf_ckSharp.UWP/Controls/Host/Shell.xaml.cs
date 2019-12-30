using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Uwp.ViewModels;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Host
{
    /// <summary>
    /// The shell that aacts as root visual element for the application
    /// </summary>
    public sealed partial class Shell : UserControl
    {
        public Shell()
        {
            this.InitializeComponent();
            this.DataContext = new ShellViewModel();
        }

        /// <summary>
        /// Gets the <see cref="ShellViewModel"/> instance for the current view
        /// </summary>
        public ShellViewModel? ViewModel => this.DataContext as ShellViewModel;
    }
}
