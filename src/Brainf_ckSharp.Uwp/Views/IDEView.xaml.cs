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

            ViewModel!.OperatorAdded += ViewModel_OperatorAdded;
        }

        /// <summary>
        /// Gets the <see cref="IdeViewModel"/> instance for the current view
        /// </summary>
        public IdeViewModel? ViewModel => this.DataContext as IdeViewModel;

        /// <summary>
        /// Types a new operator character into the IDE when requested by the user
        /// </summary>
        /// <param name="sender">The current <see cref="IdeViewModel"/> instance</param>
        /// <param name="e">The operator character to add to the text</param>
        private void ViewModel_OperatorAdded(object sender, char e) => CodeEditor.TypeCharacter(e);
    }
}
