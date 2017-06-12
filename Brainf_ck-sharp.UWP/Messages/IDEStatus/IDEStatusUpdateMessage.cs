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
        /// Gets the error Y position, if present
        /// </summary>
        public int ErrorRow { get; }

        /// <summary>
        /// Gets the error X position, if present
        /// </summary>
        public int ErrorColumn { get; }

        /// <summary>
        /// Gets the current filename, if available
        /// </summary>
        [NotNull]
        public String Filename { get; }

        // Default constructor
        public IDEStatusUpdateMessage([NotNull] String info, int row, int column, [NotNull] String filename) 
            : base(IDEStatus.IDE, info)
        {
            Row = row;
            Column = column;
            Filename = filename;
        }

        // Faulted constructor
        public IDEStatusUpdateMessage([NotNull] String info, int row, int column, int errorRow, int errorColumn, [NotNull] String filename) 
            : base(IDEStatus.FaultedIDE, info)
        {
            Row = row;
            Column = column;
            ErrorRow = errorRow;
            ErrorColumn = errorColumn;
            Filename = filename;
        }
    }
}
