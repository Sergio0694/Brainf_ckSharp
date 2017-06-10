using System;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Messages.IDEStatus
{
    /// <summary>
    /// A message that indicates a new status update for the IDE status bar
    /// </summary>
    public sealed class IDEStatusUpdateMessage : IDEStatusUpdateMessageBase
    {
        /// <summary>
        /// Gets the current row position in the editor, if possible
        /// </summary>
        public int Row { get; }

        /// <summary>
        /// Gets the character position in the editor, if possible
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Gets the current filename, if available
        /// </summary>
        [NotNull]
        public String Filename { get; }

        // Default constructor
        public IDEStatusUpdateMessage(IDEStatus status, [NotNull] String info, int row, int column, [NotNull] String filename) : base(status, info)
        {
            Row = row;
            Column = column;
            Filename = filename;
        }
    }
}
