using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.System;
using Windows.UI.Text;
using Brainf_ckSharp.UWP.Constants;
using Brainf_ckSharp.UWP.Models.Themes;

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
            else
            {
                ApplySyntaxHighlight(text, 0, textLength);
            }

            _TextLength = textLength;
        }

        /// <summary>
        /// Applies the syntax highlight to a specified range in the current text document
        /// </summary>
        /// <param name="text">The plain text currently displayed in the control</param>
        /// <param name="start">The initial position to apply the highlight to</param>
        /// <param name="end">The final position to apply the highlight to</param>
        private void ApplySyntaxHighlight(string text, int start, int end)
        {
            ref char r0 = ref MemoryMarshal.GetReference(text.AsSpan());

            /* Iterate over the current text selection and apply the
             * highlight to all the available characters. To improve
             * performances, adjacent characters with the same color
             * are aggregated into a single text range, to minimize
             * the number of interactions with the RTF document. */
            for (int i = start, j = i + 1; j < end; i = j, j++)
            {
                char c = Unsafe.Add(ref r0, i);

                // Find the edge of the current chunk of characters
                while (j < end)
                    if (!ThemeInfo.HaveSameColor(c, Unsafe.Add(ref r0, j++)))
                        break;
                j--;

                // Highlight the current range
                ITextRange range = Document.GetRange(i, j);
                range.CharacterFormat.ForegroundColor = Settings.Theme.GetColor(c);
            }
        }
    }
}
