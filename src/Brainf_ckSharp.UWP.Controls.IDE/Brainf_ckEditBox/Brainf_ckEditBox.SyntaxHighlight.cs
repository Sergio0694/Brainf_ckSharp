using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.UI.Text;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Helpers;
using Brainf_ckSharp.Uwp.Controls.Ide.Enums;
using Brainf_ckSharp.Uwp.Controls.Ide.Helpers;
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
            using (FormattingLock.For(this))
            {
                string text = Document.GetText();

                int
                    textLength = text.Length,
                    selectionLength = Document.Selection.Length,
                    selectionStart = Document.Selection.StartPosition;

                if (textLength == _Text.Length + 1 ||
                    _SelectionLength > 1 && selectionLength == 0 && selectionStart > 0)
                {
                    FormatSingleCharacter(ref text, selectionStart);
                }
                else
                {
                    FormatRange(text, 0, textLength);
                }


                _Text = text;
                _IsSyntaxValid = Brainf_ckParser.ValidateSyntax(text).IsSuccessOrEmptyScript;
            }
        }

        /// <summary>
        /// Formats and applies the syntax highlight a single character being inserted by the user
        /// </summary>
        /// <param name="text">The current source code, which will be updated in case of an autocomplete</param>
        /// <param name="start">The current index for the formatting operation</param>
        private void FormatSingleCharacter(ref string text, int start)
        {
            DebugGuard.MustBeGreaterThan(start, 0, nameof(start));
            DebugGuard.MustBeLessThan(start, text.Length, nameof(start));

            char c = text[start - 1];

            if (c == Characters.LoopStart && _IsSyntaxValid)
            {
                int depth = text.CalculateIndentationDepth(start - 1);
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
                    autocomplete = CodeGenerator.GetBracketAutocompleteText(BracketsFormattingStyle.NewLine, depth);
                }
                else autocomplete = CodeGenerator.GetBracketAutocompleteText(BracketsFormattingStyle.SameLine, depth);

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
            else if (c == '\r' && _IsSyntaxValid)
            {
                int depth = text.CalculateIndentationDepth(start);
                string autocomplete = new string('\t', depth);

                // Simply insert the tabs at the current selection, then collapse it
                Document.Selection.Text = autocomplete;
                Document.Selection.StartPosition = Document.Selection.EndPosition;

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
        private void FormatRange(string text, int start, int end)
        {
            DebugGuard.MustBeGreaterThanOrEqualTo(start, 0, nameof(start));
            DebugGuard.MustBeGreaterThan(end, 0, nameof(end));
            DebugGuard.MustBeLessThan(start, end, nameof(start));
            DebugGuard.MustBeLessThan(start, text.Length, nameof(start));
            DebugGuard.MustBeLessThanOrEqualTo(end, text.Length, nameof(end));

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
        /// Shifts the current text selection forward by adding leading tabs
        /// </summary>
        private void ShiftForward()
        {
            DebugGuard.MustBeGreaterThanOrEqualTo(Math.Abs(Document.Selection.Length), 2, nameof(Document.Selection));

            using (FormattingLock.For(this))
            {
                // Get the current selection text and range
                var bounds = Document.Selection.GetBounds();
                string text = Document.Selection.GetText();
                ref char r0 = ref MemoryMarshal.GetReference(text.AsSpan());
                int
                    max = text.Length - 1,
                    count = 2; // Initial \t, +1 after each \r

                // Initial tab
                Document.GetRangeAt(bounds.Start).Text = "\t";

                // Insert a tab before each new line character
                for (int i = 0; i < max; i++)
                    if (Unsafe.Add(ref r0, i) == '\r')
                        Document.GetRangeAt(bounds.Start + i + count++).Text = "\t";

                _Text = Document.GetText();
            }
        }
    }
}
