using System;
using System.Buffers;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Windows.UI.Text;
using Microsoft.Toolkit.HighPerformance.Buffers;

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    public sealed partial class Brainf_ckIde
    {
        /// <summary>
        /// The reference text currently in use
        /// </summary>
        private string _ReferenceText = "\r";

        /// <summary>
        /// Gets the source code currently displayed in the control
        /// </summary>
        /// <returns></returns>
        [Pure]
        public string GetText()
        {
            return CodeEditBox.PlainText;
        }

        /// <summary>
        /// Loads a given text file and starts using it as reference for the git diff indicators
        /// </summary>
        /// <param name="text"></param>
        public void LoadText(string text)
        {
            _ReferenceText = text.WithCarriageReturnLineEndings();

            _DiffIndicators.Span.Clear();

            CodeEditBox.Document.LoadFromString(text);
        }

        /// <summary>
        /// Marks the currently loaded text file as being saved and updates the git diff indicators
        /// </summary>
        public void MarkTextAsSaved()
        {
            _ReferenceText = CodeEditBox.PlainText;

            UpdateDiffInfo();

            IdeOverlaysCanvas.Invalidate();
        }

        /// <summary>
        /// Types a new character into the current document
        /// </summary>
        /// <param name="character">The character to type</param>
        public void TypeCharacter(char character)
        {
            CodeEditBox.Document.Selection.TypeText(character.ToString());
        }

        /// <summary>
        /// Deletes the last character as if the delete key had been pressed
        /// </summary>
        public void DeleteCharacter()
        {
            CodeEditBox.DeleteSelectionOrCharacter();
        }

        /// <summary>
        /// Gets a buffer with the existing breakpoints (their line numbers)
        /// </summary>
        /// <returns>A <see cref="MemoryOwner{T}"/> instance with the line numbers with a breakpoint.</returns>
        [Pure]
        public IMemoryOwner<int> GetBreakpoints()
        {
            int count = BreakpointIndicators.Count;

            if (count == 0) return MemoryOwner<int>.Empty;

            // Rent a buffer to copy the line numbers with a breakpoint
            MemoryOwner<int> buffer = MemoryOwner<int>.Allocate(count);

            ref int r0 = ref buffer.DangerousGetReference();
            int i = 0;

            // Copy the existing breakpoints
            foreach (var pair in BreakpointIndicators)
            {
                Unsafe.Add(ref r0, i++) = pair.Key;
            }

            return buffer;
        }
    }
}
