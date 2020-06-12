using System;
using System.Buffers;
using System.Linq;
using System.Reflection;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared;
using Brainf_ckSharp.Shared.Enums.Settings;
using Brainf_ckSharp.Shared.Messages.Settings;
using Brainf_ckSharp.Shared.Models.Ide;
using Brainf_ckSharp.Shared.ViewModels.Views;
using Brainf_ckSharp.Uwp.Controls.Ide;
using Brainf_ckSharp.Uwp.Controls.SubPages.Views;
using Brainf_ckSharp.Uwp.Messages.Navigation;
using Brainf_ckSharp.Uwp.Themes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
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

            CodeEditor.RenderWhitespaceCharacters = Ioc.Default.GetRequiredService<ISettingsService>().GetValue<bool>(SettingsKeys.RenderWhitespaces);
            CodeEditor.SyntaxHighlightTheme = Ioc.Default.GetRequiredService<ISettingsService>().GetValue<IdeTheme>(SettingsKeys.IdeTheme).AsBrainf_ckTheme();

            Messenger.Default.Register<ValueChangedMessage<VirtualKey>>(this, m => CodeEditor.Move(m.Value));
            Messenger.Default.Register<IdeThemeSettingChangedMessage>(this, m => CodeEditor.SyntaxHighlightTheme = m.Value.AsBrainf_ckTheme());
            Messenger.Default.Register<RenderWhitespacesSettingChangedMessage>(this, m => CodeEditor.RenderWhitespaceCharacters = m.Value);
        }

        /// <summary>
        /// Restores the IDE state when it is loaded
        /// </summary>
        /// <param name="sender">The current <see cref="IdeViewModel"/> instance</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance for this event</param>
        private void IdeView_OnLoaded(object sender, RoutedEventArgs e)
        {
            _ = App.Current.TryExtractRequestedFile(out IFile? file);

            _ = ViewModel.RestoreStateAsync(file);
        }

        /// <summary>
        /// Requests the execution of the currently displayed code in RELEASE mode
        /// </summary>
        /// <param name="sender">The current <see cref="IdeViewModel"/> instance</param>
        /// <param name="e">The empty <see cref="EventArgs"/> instance for this event</param>
        private void IdeViewModel_OnScriptRunRequested(object sender, EventArgs e)
        {
            if (!ViewModel.ValidationResult.IsSuccessOrEmptyScript)
            {
                CodeEditor.TryShowSyntaxErrorToolTip();

                return;
            }

            Messenger.Default.Send(SubPageNavigationRequestMessage.To(new IdeResultSubPage(CodeEditor.Text)));
        }

        /// <summary>
        /// Requests the execution of the currently displayed code in DEBUG mode
        /// </summary>
        /// <param name="sender">The current <see cref="IdeViewModel"/> instance</param>
        /// <param name="e">The empty <see cref="EventArgs"/> instance for this event</param>
        private void IdeViewModel_OnScriptDebugRequested(object sender, EventArgs e)
        {
            if (!ViewModel.ValidationResult.IsSuccessOrEmptyScript)
            {
                CodeEditor.TryShowSyntaxErrorToolTip();

                return;
            }

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
            App.Current.RegisterFilePath(ViewModel.Code.File);

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
        /// Updates the UI when the state is restored
        /// </summary>
        /// <param name="sender">The current <see cref="IdeViewModel"/> instance</param>
        /// <param name="e">The empty <see cref="EventArgs"/> instance for the event</param>
        private void ViewModel_OnStateRestored(object sender, IdeState e)
        {
            App.Current.RegisterFilePath(ViewModel.Code.File);

            CodeEditor.LoadText(e.Text);
            CodeEditor.Move(e.Row, e.Column);
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

        /// <summary>
        /// Inserts the text of a selected code snippet
        /// </summary>
        /// <param name="sender">The sender <see cref="Button"/> for the snippet</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> for the current event</param>
        private void CodeSnippet_Clicked(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is string snippet)
            {
                string name = (
                    from fieldInfo in typeof(CodeSnippets).GetFields(BindingFlags.Public | BindingFlags.Static)
                    let fieldValue = (string)fieldInfo.GetRawConstantValue()
                    where fieldValue == snippet
                    select fieldInfo.Name).First();

                Ioc.Default.GetRequiredService<IAnalyticsService>().Log(Shared.Constants.Events.InsertCodeSnippet, (nameof(CodeSnippets), name));

                CodeEditor.InsertText(snippet);
            }
        }

        /// <summary>
        /// Notifies whenever a breakpoint is added to the IDE
        /// </summary>
        /// <param name="sender">The sender <see cref="Brainf_ckIde"/> control</param>
        /// <param name="args">The <see cref="BreakpointToggleEventArgs"/> instance for the event</param>
        private void CodeEditor_OnBreakpointAdded(Brainf_ckIde sender, BreakpointToggleEventArgs args)
        {
            Ioc.Default.GetRequiredService<IAnalyticsService>().Log(
                Shared.Constants.Events.BreakpointAdded,
                (nameof(BreakpointToggleEventArgs.Row), args.Row.ToString()),
                (nameof(BreakpointToggleEventArgs.Count), args.Count.ToString()));
        }

        /// <summary>
        /// Notifies whenever a breakpoint is removed from the IDE
        /// </summary>
        /// <param name="sender">The sender <see cref="Brainf_ckIde"/> control</param>
        /// <param name="args">The <see cref="BreakpointToggleEventArgs"/> instance for the event</param>
        private void CodeEditor_OnBreakpointRemoved(Brainf_ckIde sender, BreakpointToggleEventArgs args)
        {
            Ioc.Default.GetRequiredService<IAnalyticsService>().Log(
                Shared.Constants.Events.BreakpointRemoved,
                (nameof(BreakpointToggleEventArgs.Row), args.Row.ToString()),
                (nameof(BreakpointToggleEventArgs.Count), args.Count.ToString()));
        }

        /// <summary>
        /// Notifies whenever all breakpoints are removed from the IDE
        /// </summary>
        /// <param name="sender">The sender <see cref="Brainf_ckIde"/> control</param>
        /// <param name="args">The number of removed breakpoints</param>
        private void CodeEditor_OnBreakpointsCleared(Brainf_ckIde sender, int args)
        {
            Ioc.Default.GetRequiredService<IAnalyticsService>().Log(Shared.Constants.Events.BreakpointsCleared, (nameof(ItemCollection.Count), args.ToString()));
        }
    }
}
