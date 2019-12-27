using Brainf_ckSharp.UWP.Constants;

namespace Brainf_ckSharp.UWP.Controls.IDE
{
    public sealed partial class Brainf_ckEditBox
    {
        private int _TextLength = 1;

        /// <summary>
        /// Applies the syntaxt highlight to the current document, using less resources as possible
        /// </summary>
        /// <param name="text">The plain text currently displayed in the control</param>
        private void ApplySyntaxHighlight(string text)
        {
            int
                textLength = text.Length,
                selectionStart = Document.Selection.StartPosition;

            if (textLength == _TextLength + 1)
            {
                Document.GetRange(selectionStart - 1, selectionStart).CharacterFormat.ForegroundColor = Settings.Theme.GetColor(text[selectionStart - 1]);
            }

            _TextLength = textLength;
        }
    }
}
