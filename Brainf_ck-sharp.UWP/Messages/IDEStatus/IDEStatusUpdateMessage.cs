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
        /// Gets whether or not the filename should be visible to the user
        /// </summary>
        public bool FilenameVisibile => !String.IsNullOrEmpty(Filename);

        /// <summary>
        /// Gets the current filename, if available
        /// </summary>
        [CanBeNull]
        public String Filename { get; }

        // Default constructor
        public IDEStatusUpdateMessage([NotNull] String info, int row, int column, [CanBeNull] String filename) 
            : base(IDEStatus.IDE, info)
        {
            Row = row;
            Column = column;
            Filename = filename;
        }

        // Faulted constructor
        public IDEStatusUpdateMessage([NotNull] String info, int row, int column, int errorRow, int errorColumn, [CanBeNull] String filename) 
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
