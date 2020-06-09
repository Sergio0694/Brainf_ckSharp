using System;

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    /// <summary>
    /// A <see langword="class"/> that contains info to signal when the cursor position changes
    /// </summary>
    public sealed class CursorPositionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new <see cref="CursorPositionChangedEventArgs"/> instance with the specified parameters
        /// </summary>
        /// <param name="row">The current row position</param>
        /// <param name="column">The current column position</param>
        internal CursorPositionChangedEventArgs(int row, int column)
        {
            Row = row;
            Column = column;
        }

        /// <summary>
        /// Gets the current row position (1-based)
        /// </summary>
        public int Row { get; }

        /// <summary>
        /// Gets the current column position (1-based)
        /// </summary>
        public int Column { get; }
    }
}
