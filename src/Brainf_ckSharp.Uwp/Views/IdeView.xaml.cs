using System;
using System.Buffers;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Shared.Enums.Settings;
using Brainf_ckSharp.Shared.ViewModels.Views;
using Brainf_ckSharp.Uwp.Controls.Ide;
using Brainf_ckSharp.Uwp.Controls.SubPages.Views;
using Brainf_ckSharp.Uwp.Messages.Navigation;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using TextChangedEventArgs = Brainf_ckSharp.Uwp.Controls.Ide.TextChangedEventArgs;

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

            CodeEditor.SyntaxHighlightTheme = Settings.GetCurrentTheme();

            Messenger.Default.Register<ValueChangedMessage<VirtualKey>>(this, m => CodeEditor.Move(m.Value));
            Messenger.Default.Register<ValueChangedMessage<IdeTheme>>(this, m => CodeEditor.SyntaxHighlightTheme = Settings.GetCurrentTheme());
        }

        /// <summary>
        /// Requests the execution of the currently displayed code in RELEASE mode
        /// </summary>
        /// <param name="sender">The current <see cref="IdeViewModel"/> instance</param>
        /// <param name="e">The empty <see cref="EventArgs"/> instance for this event</param>
        private void IdeViewModel_OnScriptRunRequested(object sender, EventArgs e)
        {
            Messenger.Default.Send(SubPageNavigationRequestMessage.To(new IdeResultSubPage(CodeEditor.Text)));
        }

        /// <summary>
        /// Requests the execution of the currently displayed code in DEBUG mode
        /// </summary>
        /// <param name="sender">The current <see cref="IdeViewModel"/> instance</param>
        /// <param name="e">The empty <see cref="EventArgs"/> instance for this event</param>
        private void IdeViewModel_OnScriptDebugRequested(object sender, EventArgs e)
        {
            string source = CodeEditor.Text;
            IMemoryOwner<int> breakpoints = CodeEditor.GetBreakpoints();

            Messenger.Default.Send(SubPageNavigationRequestMessage.To(new IdeResultSubPage(source, breakpoints)));
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

            Messenger.Default.Send<SubPageCloseRequestMessage>();
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

        /// <summary>
        /// Updates the currently displayed text
        /// </summary>
        /// <param name="sender">The sender <see cref="Brainf_ckIde"/> control</param>
        /// <param name="args">The <see cref="TextChangedEventArgs"/> instance for the current event</param>
        private void CodeEditor_OnTextChanged(Brainf_ckIde sender, TextChangedEventArgs args)
        {
            ViewModel.Text = args.PlainText.AsMemory(0, args.PlainText.Length - 1);
            ViewModel.ValidationResult = args.ValidationResult;
        }

        /// <summary>
        /// Updates the current cursor position
        /// </summary>
        /// <param name="sender">The sender <see cref="Brainf_ckIde"/> control</param>
        /// <param name="args">The <see cref="CursorPositionChangedEventArgs"/> instance for the current event</param>
        private void CodeEditor_OnCursorPositionChanged(Brainf_ckIde sender, CursorPositionChangedEventArgs args)
        {
            ViewModel.Row = args.Row;
            ViewModel.Column = args.Column;
        }
    }
}
