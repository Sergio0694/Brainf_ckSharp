using System;

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    /// <summary>
    /// A <see langword="class"/> that contains info to signal when the cursor position changes
    /// </summary>
    public sealed class BreakpointToggleEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new <see cref="BreakpointToggleEventArgs"/> instance with the specified parameters
        /// </summary>
        /// <param name="row">The row where the breakpoint was added or removed</param>
        /// <param name="count">The updated count of existing breakpoints</param>
        internal BreakpointToggleEventArgs(int row, int count)
        {
            Row = row;
            Count = count;
        }

        /// <summary>
        /// Gets the current row position (1-based)
        /// </summary>
        public int Row { get; }

        /// <summary>
        /// Gets the current column position (1-based)
        /// </summary>
        public int Count { get; }
    }
}
