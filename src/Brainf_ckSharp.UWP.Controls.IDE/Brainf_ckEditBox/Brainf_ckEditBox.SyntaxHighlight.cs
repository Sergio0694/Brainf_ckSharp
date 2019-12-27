using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.UI.Text;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Uwp.Controls.Ide.Enums;
using Brainf_ckSharp.Uwp.Themes;

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    public sealed partial class Brainf_ckEditBox
    {
        private string _Text = "\r";

        private int _SelectionLength = 0;

        private int _SelectionStart = 0;

        private bool _IsSyntaxValid = true;

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

            if (textLength == _Text.Length + 1 ||
                _SelectionLength > 1 && selectionLength == 0 && selectionStart > 0)
            {
                TryFormatSingleCharacter(ref text, selectionStart);
                //ApplySyntaxHighlight(text, selectionStart - 1, selectionStart);
            }
            else
            {
                ApplySyntaxHighlight(text, 0, textLength);
            }


            _Text = text;
            _IsSyntaxValid = Brainf_ckParser.ValidateSyntax(text).IsSuccessOrEmptyScript;
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

        private void TryFormatSingleCharacter(ref string text, int start)
        {
            char c = text[start - 1];

            if (c == Characters.LoopStart && _IsSyntaxValid)
            {
                int depth = CalculateIndentationDepth(text, start - 1);
                ITextRange range = Document.GetRange(start - 1, start);
                string autocomplete;

                /* Handle the two possible bracket formatting options. If the bracket needs
                 * to go on a new line, check for edge cases and then prepare the new text
                 * with the leading new line. Otherwise, simply replace the open bracket
                 * on the same line and insert the autocomplete text. Using a single replacement
                 * improves performance, as the color can be applied in a single pass. */
                if (BracketsFormattingStyle == BracketsFormattingStyle.NewLine &&
                    start > 1 &&
                    text[start - 2] != '\r')
                {
                    autocomplete = $"\r{new string('\t', depth)}{Characters.LoopStart}\r{new string('\t', depth + 1)}\r{new string('\t', depth)}{Characters.LoopEnd}";
                }
                else autocomplete = $"{Characters.LoopStart}\r{new string('\t', depth + 1)}\r{new string('\t', depth)}{Characters.LoopEnd}";

                // Set the autocomplete text and color it
                range.SetText(TextSetOptions.None, autocomplete);
                range.CharacterFormat.ForegroundColor = SyntaxHighlightTheme.GetColor(Characters.LoopStart);

                /* Move the selection at the end of the added line between the brackets.
                 * Start is the position right after the first inserted bracket, which was replaced.
                 * It gets shifted ahead by the length of the replacement text, minus the number of
                 * tabs before the closing bracket, and 3 which is the number of additional characters
                 * that were inserted with respect to the end: the first replaced bracket,
                 * the closing bracket, and the new line before the line with the last bracket. */
                Document.Selection.StartPosition = Document.Selection.EndPosition = start + autocomplete.Length - (depth + 3);

                text = Document.GetText();
            }
            else Document.SetRangeColor(start - 1, start, SyntaxHighlightTheme.GetColor(c));
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
                Document.SetRangeColor(i, j, SyntaxHighlightTheme.GetColor(c));
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
                Document.SetRangeColor(offset + i, offset + j, SyntaxHighlightTheme.GetColor(c));
            }
        }
    }
}
