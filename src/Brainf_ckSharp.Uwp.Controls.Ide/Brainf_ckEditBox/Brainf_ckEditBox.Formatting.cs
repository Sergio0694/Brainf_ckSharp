using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.UI.Text;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Uwp.Controls.Ide.Enums;
using Brainf_ckSharp.Uwp.Controls.Ide.Extensions.System;
using Brainf_ckSharp.Uwp.Controls.Ide.Extensions.Windows.UI.Text;
using Brainf_ckSharp.Uwp.Controls.Ide.Helpers;
using Brainf_ckSharp.Uwp.Themes;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    public sealed partial class Brainf_ckEditBox
    {
        /// <summary>
        /// The syntax validation result for the currently displayed text
        /// </summary>
        private SyntaxValidationResult _SyntaxValidationResult = Brainf_ckParser.ValidateSyntax(string.Empty);

        /// <summary>
        /// The start position of the current selection
        /// </summary>
        private int _SelectionStart;

        /// <summary>
        /// The length of the current selection
        /// </summary>
        private int _SelectionLength;

        /// <summary>
        /// Indicates whether the delete key was pressed
        /// </summary>
        private bool _IsDeleteRequested;

        /// <summary>
        /// Applies the syntaxt highlight to the current document, using less resources as possible
        /// </summary>
        private void ApplySyntaxHighlight()
        {
            using (FormattingLock.For(this))
            {
                string
                    oldText = Text,
                    newText = Document.GetText();

                int
                    textLength = newText.Length,
                    selectionLength = Document.Selection.Length,
                    selectionStart = Document.Selection.StartPosition;

                // Handle all the possible cases individually.
                //   - If the current selection has a length of 0 and is not the first position in
                //     the text, it means that the user might have typed a single character, or
                //     replaced a selection with a single character. A single character is typed
                //     if the previous selection had a length of 0 as well, and the current text
                //     has one more character than the previous one. Otherwise, check whether the
                //     previous selection had a length of at least a single character.
                //   - If CTRL + Z was pressed and it resulted in a single typed character being
                //     deleted, ignore this case to avoid formatting the entire text again.
                //   - If a deletion is requested, just skip the formatting entirely. If no new
                //     characters have been typed, there is no need to highlight anything.
                //   - As a last resort, just format the entire text from start to finish.
                if (selectionLength == 0 &&
                    selectionStart > 0 &&
                    (_SelectionLength == 0 && textLength == oldText.Length + 1) ||
                    _SelectionLength > 1)
                {
                    FormatSingleCharacter(ref newText, selectionStart);
                }
                else if (selectionLength == 0 &&
                         _SelectionLength == 0 &&
                         textLength == oldText.Length - 1 &&
                         selectionStart == _SelectionStart)
                {
                    // This branch also captures single character deletions.
                    // Set the property to false to make sure the formatting
                    // isn't compromised in future runs by the property
                    // remaining true because this branch took precedence.
                    _IsDeleteRequested = false;
                }
                else if (_IsDeleteRequested) _IsDeleteRequested = false;
                else FormatRange(newText, 0, textLength);

                _SyntaxValidationResult = Brainf_ckParser.ValidateSyntax(newText);
                Text = newText;
            }
        }

        /// <summary>
        /// Formats and applies the syntax highlight a single character being inserted by the user
        /// </summary>
        /// <param name="text">The current source code, which will be updated in case of an autocomplete</param>
        /// <param name="start">The current index for the formatting operation</param>
        private void FormatSingleCharacter(ref string text, int start)
        {
            Debug.Assert(start > 0);
            Debug.Assert(start < text.Length);

            char c = text[start - 1];

            if (c == Characters.LoopStart && _SyntaxValidationResult.IsSuccessOrEmptyScript)
            {
                int depth = text.CalculateIndentationDepth(start - 1);
                ITextRange range = Document.GetRange(start - 1, start);
                string autocomplete;

                // Handle the two possible bracket formatting options. If the bracket needs
                // to go on a new line, check for edge cases and then prepare the new text
                // with the leading new line. Otherwise, simply replace the open bracket
                // on the same line and insert the autocomplete text. Using a single replacement
                // improves performance, as the color can be applied in a single pass.
                if (BracketsFormattingStyle == BracketsFormattingStyle.NewLine &&
                    start > 1 &&
                    text[start - 2] != Characters.CarriageReturn)
                {
                    autocomplete = CodeGenerator.GetBracketAutocompleteText(BracketsFormattingStyle.NewLine, depth);
                }
                else autocomplete = CodeGenerator.GetBracketAutocompleteText(BracketsFormattingStyle.SameLine, depth);

                // Set the autocomplete text and color it
                range.SetText(TextSetOptions.None, autocomplete);
                range.CharacterFormat.ForegroundColor = SyntaxHighlightTheme.GetColor(Characters.LoopStart);

                // Move the selection at the end of the added line between the brackets.
                // Start is the position right after the first inserted bracket, which was replaced.
                // It gets shifted ahead by the length of the replacement text, minus the number of
                // tabs before the closing bracket, and 3 which is the number of additional characters
                // that were inserted with respect to the end: the first replaced bracket,
                // the closing bracket, and the new line before the line with the last bracket.
                Document.Selection.StartPosition = Document.Selection.EndPosition = start + autocomplete.Length - (depth + 3);

                text = Document.GetText();
            }
            else if (c == Characters.CarriageReturn && _SyntaxValidationResult.IsSuccessOrEmptyScript)
            {
                int depth = text.CalculateIndentationDepth(start);
                string autocomplete = new string(Characters.Tab, depth);

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
            Debug.Assert(start >= 0);
            Debug.Assert(end > 0);
            Debug.Assert(start < end);
            Debug.Assert(start < text.Length);
            Debug.Assert(end <= text.Length);

            ref char r0 = ref MemoryMarshal.GetReference(text.AsSpan());

            // Iterate over the current range from the input text,
            // applying the highlight to all the available characters.
            // To improve performance, adjacent characters with the same
            // color are aggregated into a single text range, to minimize
            // the number of interactions with the RTF document.
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
        /// Programmatically deletes the current character, or the current selection
        /// </summary>
        public void DeleteSelectionOrCharacter()
        {
            int
                selectionLength = Document.Selection.Length,
                selectionEnd = Document.Selection.EndPosition;

            if (selectionEnd == 0) return;

            using (FormattingLock.For(this))
            {
                // Remove text in the current selection, or delete the previous character
                if (selectionLength > 0) Document.Selection.Text = string.Empty;
                else
                {
                    // Delete and adjust the selection
                    Document.GetRange(selectionEnd - 1, selectionEnd).Text = string.Empty;
                    Document.Selection.StartPosition = Document.Selection.EndPosition = selectionEnd - 1;
                }

                // Update the current syntax validation
                string text = Document.GetText();

                _SyntaxValidationResult = Brainf_ckParser.ValidateSyntax(text);
                Text = text;
            }
        }

        /// <summary>
        /// Shifts the current text selection forward by adding leading tabs
        /// </summary>
        private void ShiftForward()
        {
            Debug.Assert(Math.Abs(Document.Selection.Length) >= 2);

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
                    if (Unsafe.Add(ref r0, i) == Characters.CarriageReturn)
                        Document.GetRangeAt(bounds.Start + i + count++).Text = "\t";

                Text = Document.GetText();
            }
        }

        /// <summary>
        /// Shifts the current text selection backward by removing leading tabs
        /// </summary>
        private void ShiftBackward()
        {
            Debug.Assert(Math.Abs(Document.Selection.Length) >= 2);

            using (FormattingLock.For(this))
            {
                // Get the current selection text and range
                var bounds = Document.Selection.GetBounds();
                string text = Document.Selection.GetText();
                ref char r0 = ref MemoryMarshal.GetReference(text.AsSpan());
                int
                    length = text.Length,
                    count = 0;

                // Remove leading \t from each line, if present
                for (int i = 0; i < length; i++)
                {
                    // Remove the current \t
                    if (Unsafe.Add(ref r0, i) == Characters.Tab)
                    {
                        int offset = bounds.Start + i - count++;
                        Document.GetRange(offset, offset + 1).Text = string.Empty;
                    }

                    // Move to the following \r
                    i++;
                    while (i < length && Unsafe.Add(ref r0, i) != Characters.CarriageReturn)
                        i++;
                }

                Text = Document.GetText();
            }
        }

        /// <summary>
        /// Inserts a given source <see cref="string"/> into the current selection
        /// </summary>
        /// <param name="source">The source text to insert</param>
        public void InsertText(string source)
        {
            if (source.Length == 0) return;

            ContextFlyout?.Hide();

            // Adjust the line endings
            source = source.WithCarriageReturnLineEndings();

            using (FormattingLock.For(this))
            {
                Document.Selection.Text = source;

                // Update the current syntax validation
                string text = Document.GetText();

                _SyntaxValidationResult = Brainf_ckParser.ValidateSyntax(text);
                Text = text;

                int
                    sourceLength = source.Length,
                    selectionStart = Document.Selection.StartPosition,
                    selectionEnd = selectionStart + sourceLength;

                // Only format the inserted text
                FormatRange(text, selectionStart, selectionEnd);

                // Set the selection after the pasted text
                Document.Selection.StartPosition = Document.Selection.EndPosition;
            }
        }
    }
}
