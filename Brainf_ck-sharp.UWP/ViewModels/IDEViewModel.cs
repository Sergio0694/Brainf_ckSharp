using System;
using Windows.UI.Text;
using Brainf_ck_sharp;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Messages;
using Brainf_ck_sharp_UWP.Messages.Actions;
using Brainf_ck_sharp_UWP.Messages.IDEStatus;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.ViewModels
{
    public class IDEViewModel : ViewModelBase
    {
        // The current document that's linked to the view
        private readonly ITextDocument Document;

        public IDEViewModel([NotNull] ITextDocument document) => Document = document;

        private bool _IsEnabled;

        /// <summary>
        /// Gets or sets whether or not the instance is enabled and it is processing incoming messages
        /// </summary>
        public bool IsEnabled
        {
            get => _IsEnabled;
            set
            {
                if (Set(ref _IsEnabled, value))
                {
                    if (value)
                    {
                        Messenger.Default.Register<OperatorAddedMessage>(this, op => InsertSingleCharacter(op.Operator));
                        Messenger.Default.Register<ClearScreenMessage>(this, m => TryClearScreen());
                        Messenger.Default.Register<PlayScriptMessage>(this, m => PlayRequested?.Invoke(this, m.StdinBuffer));
                        SendMessages();
                    }
                    else Messenger.Default.Unregister(this);
                }
            }
        }

        public event EventHandler<String> PlayRequested;

        /// <summary>
        /// Sends the status info messages for the current state
        /// </summary>
        public void SendMessages([CanBeNull] String code = null)
        {
            if (code == null) Document.GetText(TextGetOptions.None, out code);
            Messenger.Default.Send(new ConsoleAvailableActionStatusChangedMessage(ConsoleAction.ClearScreen, code.Length > 1));
            (bool valid, int error) = Brainf_ckInterpreter.CheckSourceSyntax(code);
            (int row, int col) = code.FindCoordinates(Document.Selection.StartPosition);
            if (valid)
            {
                Messenger.Default.Send(new IDEStatusUpdateMessage(LocalizationManager.GetResource("Ready"), row, col, String.Empty));
            }
            else
            {
                (int y, int x) = code.FindCoordinates(error);
                Messenger.Default.Send(new IDEStatusUpdateMessage(LocalizationManager.GetResource("Warning"), row, col, y, x, String.Empty));
            }
        }

        /// <summary>
        /// Inserts a new character from the virtual keyboard and scrolls the current line into view, if needed
        /// </summary>
        /// <param name="c">The received character</param>
        private void InsertSingleCharacter(char c)
        {
            try
            {
                Document.Selection.SetText(TextSetOptions.None, c.ToString());
                Document.Selection.SetRange(Document.Selection.StartPosition + 1, Document.Selection.StartPosition + 1);
            }
            catch
            {
                //
            }
            Document.Selection.ScrollIntoView(PointOptions.NoHorizontalScroll);
        }

        /// <summary>
        /// Clears the current content in the document
        /// </summary>
        private void TryClearScreen()
        {
            Document.SetText(TextSetOptions.None, String.Empty);
            SendMessages();
        }
    }
}
