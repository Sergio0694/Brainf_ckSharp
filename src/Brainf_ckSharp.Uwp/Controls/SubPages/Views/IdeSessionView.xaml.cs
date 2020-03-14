using Windows.UI.Xaml.Controls;

namespace Brainf_ckSharp.Uwp.Controls.SubPages.Views
{
    public sealed partial class IdeSessionView : UserControl
    {
        /// <summary>
        /// Creates a new <see cref="IdeSessionView"/> instance with the specified parameters
        /// </summary>
        /// <param name="script">The script to execute</param>
        /// <param name="stdin">The stdin buffer to use</param>
        public IdeSessionView(string script, string stdin)
        {
            this.InitializeComponent();

            ViewModel.Script = script;
            ViewModel.Stdin = stdin;
        }
    }
}
