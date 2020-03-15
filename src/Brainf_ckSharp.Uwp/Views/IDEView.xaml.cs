using System;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Uwp.Controls.SubPages.Views;
using Brainf_ckSharp.Uwp.Messages.Navigation;
using Brainf_ckSharp.Uwp.ViewModels.Views;
using GalaSoft.MvvmLight.Messaging;

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
        }

        /// <summary>
        /// Requests the execution of the currently displayed code
        /// </summary>
        /// <param name="sender">The current <see cref="IdeViewModel"/> instance</param>
        /// <param name="e">The empty <see cref="EventArgs"/> instance for this event</param>
        private void IdeViewModel_OnScriptRunRequested(object sender, EventArgs e)
        {
            string
                source = CodeEditor.GetText(),
                stdin = string.Empty;

            Messenger.Default.Send(SubPageNavigationRequestMessage.To(new IdeResultSubPage(source, stdin)));
        }

        /// <summary>
        /// Types a new operator character into the IDE when requested by the user
        /// </summary>
        /// <param name="sender">The current <see cref="IdeViewModel"/> instance</param>
        /// <param name="e">The operator character to add to the text</param>
        private void ViewModelCharacterAdded(object sender, char e)
        {
            CodeEditor.TypeCharacter(e);
        }

        /// <summary>
        /// Deletes the last character from the IDE when requested by the user
        /// </summary>
        /// <param name="sender">The current <see cref="IdeViewModel"/> instance</param>
        /// <param name="e">The empty <see cref="EventArgs"/> instance for this event</param>
        private void ViewModel_CharacterDeleted(object sender, EventArgs e)
        {
            CodeEditor.DeleteCharacter();
        }

        /// <summary>
        /// Loads a given source code into the IDE
        /// </summary>
        /// <param name="sender">The current <see cref="IdeViewModel"/> instance</param>
        /// <param name="e">The <see cref="string"/> with the code to load</param>
        private void ViewModel_CodeLoaded(object sender, string e)
        {
            CodeEditor.LoadText(e);
        }

        /// <summary>
        /// Marks the current source code as saved in the IDE
        /// </summary>
        /// <param name="sender">The current <see cref="IdeViewModel"/> instance</param>
        /// <param name="e">The empty <see cref="EventArgs"/> instance for the event</param>
        private void ViewModel_CodeSaved(object sender, EventArgs e)
        {
            CodeEditor.MarkTextAsSaved();
        }
    }
}
