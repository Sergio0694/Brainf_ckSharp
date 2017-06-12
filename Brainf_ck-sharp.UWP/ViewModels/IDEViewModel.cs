using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Text;
using Brainf_ck_sharp_UWP.Helpers;
using GalaSoft.MvvmLight;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.ViewModels
{
    public class IDEViewModel : ViewModelBase
    {
        // The current document that's linked to the view
        private readonly ITextDocument Document;

        public IDEViewModel([NotNull] ITextDocument document) => Document = document;

        public void ApplySyntaxHighlightOnLastCharacter()
        {
            ITextRange range = Document.GetRange(Document.Selection.StartPosition - 1, Document.Selection.StartPosition);
            range.CharacterFormat.ForegroundColor = Brainf_ckFormatterHelper.GetSyntaxHighlightColorFromChar(range.Character);
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
        }
    }
}
