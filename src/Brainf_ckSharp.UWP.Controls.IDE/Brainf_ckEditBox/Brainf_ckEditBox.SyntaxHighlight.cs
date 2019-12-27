using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.UI.Text;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Uwp.Themes;

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    public sealed partial class Brainf_ckEditBox
    {
        private string _Text = "\r";

        private int _TextLength = 1;

        private int _SelectionLength = 0;

        private int _SelectionStart = 0;

        /// <summary>
        /// Applies the syntaxt highlight to the current document, using less resources as possible
        /// </summary>
        private void ApplySyntaxHighlight()
        {
            string text = Document.GetText();

            int
                textLength = text.Length,
                selectionLength = Document.Selection.Length,
                selectionStart = Document.Selection.StartPosition;

            if (textLength == _TextLength + 1 ||
                _SelectionLength > 1 && selectionLength == 0 && selectionStart > 0)
            {
                TryFormatSingleCharacter(text, selectionStart);
                //ApplySyntaxHighlight(text, selectionStart - 1, selectionStart);
            }
            else
            {
                ApplySyntaxHighlight(text, 0, textLength);
            }


            _Text = text;
            _TextLength = textLength;
        }

        private static int CalculateIndentationDepth(string text, int start)
        {
            ref char r0 = ref MemoryMarshal.GetReference(text.AsSpan());

            int depth = 0;

            for (int i = 0; i < start; i++)
            {
                switch (Unsafe.Add(ref r0, i))
                {
                    case Characters.LoopStart: depth++; break;
                    case Characters.LoopEnd: depth--; break;
                }
            }

            return depth;
        }

        private void TryFormatSingleCharacter(string text, int start)
        {
            char c = text[start - 1];

            if (c == Characters.LoopStart)
            {
                ITextRange range = Document.GetRange(start - 1, start);
                range.CharacterFormat.ForegroundColor = SyntaxHighlightTheme.GetColor('[');

                Document.Selection.Text = "TEST]";

                start += 5;
                range = Document.GetRange(start - 1, start);
                range.CharacterFormat.ForegroundColor = SyntaxHighlightTheme.GetColor(']');

                Document.Selection.StartPosition = Document.Selection.EndPosition;
            }
            else ApplySyntaxHighlight(text, start - 1, start);
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

            /* Iterate over the current range from the input text,
             * applying the highlight to all the available characters.
             * To improve performance, adjacent characters with the same
             * color are aggregated into a single text range, to minimize
             * the number of interactions with the RTF document. */
            for (int i = start, j = i; j < end; i = j)
            {
                char c = Unsafe.Add(ref r0, i);

                // Find the edge of the current chunk of characters
                while (++j < end)
                    if (!Brainf_ckTheme.HaveSameColor(c, Unsafe.Add(ref r0, j)))
                        break;

                // Highlight the current range
                ITextRange range = Document.GetRange(i, j);
                range.CharacterFormat.ForegroundColor = SyntaxHighlightTheme.GetColor(c);
            }
        }

        /// <summary>
        /// Applies the syntax highlight to a specified range in the current text document
        /// </summary>
        /// <param name="text">The plain text currently displayed in the control</param>
        /// <param name="offset">The offset of <paramref name="text"/> into the current document</param>
        private void ApplySyntaxHighlight(string text, int offset)
        {
            int end = text.Length;
            ref char r0 = ref MemoryMarshal.GetReference(text.AsSpan());

            /* Iterate over the current string with offset and apply the
             * highlight to all the available characters. Like with the
             * first overload, compatible characters are aggregated. */
            for (int i = 0, j = i; j < end; i = j)
            {
                char c = Unsafe.Add(ref r0, i);

                // Find the edge of the current chunk of characters
                while (++j < end)
                    if (!Brainf_ckTheme.HaveSameColor(c, Unsafe.Add(ref r0, j)))
                        break;

                // Highlight the current range, adding back the offset
                ITextRange range = Document.GetRange(offset + i, offset + j);
                range.CharacterFormat.ForegroundColor = SyntaxHighlightTheme.GetColor(c);
            }
        }
    }
}
