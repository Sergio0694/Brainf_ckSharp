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
                        SendMessages();
                    }
                    else Messenger.Default.Unregister(this);
                }
            }
        }

        /// <summary>
        /// Sends the status info messages for the current state
        /// </summary>
        private void SendMessages()
        {
            Document.GetText(TextGetOptions.None, out String code);
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

        public void ApplySyntaxHighlightOnLastCharacter()
        {
            ITextRange range = Document.GetRange(Document.Selection.StartPosition - 1, Document.Selection.StartPosition);
            range.CharacterFormat.ForegroundColor = Brainf_ckFormatterHelper.GetSyntaxHighlightColorFromChar(range.Character);
            SendMessages();
        }

        public void InsertSingleCharacter(char c)
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
            ApplySyntaxHighlightOnLastCharacter();
            Document.Selection.ScrollIntoView(PointOptions.NoHorizontalScroll);


            String a;
            Document.GetText(TextGetOptions.None, out a);
            ITextRange range = Document.GetRange(0, int.MaxValue);
            int r = range.FindText("\r", int.MaxValue, FindOptions.None);
            System.Diagnostics.Debug.WriteLine(r);

            SendMessages();
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
