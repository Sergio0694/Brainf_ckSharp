using System.Diagnostics.Contracts;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Helpers;
using Brainf_ckSharp.Uwp.Controls.Ide.Enums;

namespace Brainf_ckSharp.Uwp.Controls.Ide.Helpers
{
    /// <summary>
    /// A <see langword="class"/> that generates autocomplete snippets
    /// </summary>
    internal static class CodeGenerator
    {
        /// <summary>
        /// Creates the autocomplete text for an open bracket in a fast way
        /// </summary>
        /// <param name="style">The current formatting style in use</param>
        /// <param name="depth">The current indentation depth level</param>
        /// <returns>A formatted autocomplete text</returns>
        [Pure]
        public static unsafe string GetBracketAutocompleteText(BracketsFormattingStyle style, int depth)
        {
            DebugGuard.MustBeGreaterThanOrEqualTo(depth, 0, nameof(depth));

            /* This is the maximum length for the newline style.
             * It contains a series of 3 indentations, plus the space
             * for the 2 brackets and the 3 newline characters. */
            int length = depth * 3 + 6;
            char*
                p0 = stackalloc char[length],
                p1 = p0;

            /* If the style is with the open bracket on a new line,
             * add the newline and the initial tab characters, or
             * update the length offset for the rest of the method. */
            if (style == BracketsFormattingStyle.NewLine)
            {
                *p0 = '\r';

                for (int i = 0; i < depth; i++)
                    p0[i] = '\t';

                p1 = p0 + depth + 1;
            }
            else length -= depth + 1;

            // Bracket and new line
            p1[0] = Characters.LoopStart;
            p1[1] = '\r';

            // Middle tabs
            for (int i = 0; i < depth + 1; i++)
                p1[i + 2] = '\t';

            p1[depth + 3] = '\r';

            // End tabs
            for (int i = 0; i < depth; i++)
                p1[i + depth + 4] = '\t';

            p1[depth * 2 + 4] = Characters.LoopEnd;

            return new string(p0, 0, length);
        }
    }
}
