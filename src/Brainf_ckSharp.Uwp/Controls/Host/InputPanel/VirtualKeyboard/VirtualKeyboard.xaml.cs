using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Shared.ViewModels.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace Brainf_ckSharp.Uwp.Controls.Host.InputPanel
{
    /// <summary>
    /// A virtual keyboard with the Brainf*ck/PBrain operators
    /// </summary>
    public sealed partial class VirtualKeyboard : UserControl
    {
        public VirtualKeyboard()
        {
            this.InitializeComponent();
            this.DataContext = App.Current.Services.GetRequiredService<VirtualKeyboardViewModel>();
        }

        /// <summary>
        /// Gets the <see cref="VirtualKeyboardViewModel"/> instance currently in use
        /// </summary>
        public VirtualKeyboardViewModel ViewModel => (VirtualKeyboardViewModel)DataContext;
    }
}
