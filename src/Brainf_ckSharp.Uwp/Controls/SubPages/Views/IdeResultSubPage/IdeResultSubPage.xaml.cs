using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Uwp.Controls.SubPages.Interfaces;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.SubPages.Views
{
    /// <summary>
    /// A sub page that displays the result of a script being executed from the IDE
    /// </summary>
    public sealed partial class IdeResultSubPage : UserControl, IConstrainedSubPage
    {
        /// <summary>
        /// Creates a new <see cref="IdeResultSubPage"/> instance with the specified parameters
        /// </summary>
        /// <param name="script">The script to execute</param>
        public IdeResultSubPage(string script)
        {
            this.InitializeComponent();
            this.Unloaded += (s, e) => ViewModel.IsActive = false;

            ViewModel.Script = script;
        }

        /// <inheritdoc/>
        public double MaxExpandedWidth { get; } = 540;

        /// <inheritdoc/>
        public double MaxExpandedHeight { get; } = double.PositiveInfinity;
    }
}
