using System;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Messages
{
    /// <summary>
    /// A message that indicates a new status update for the IDE status bar
    /// </summary>
    public sealed class IDEStatusUpdateMessage
    {
        /// <summary>
        /// Gets the current IDE status (console mode, IDE in edit mode or IDE with error)
        /// </summary>
        public IDEStatus Status { get; }

        /// <summary>
        /// Gets the main info for the IDE status
        /// </summary>
        [NotNull]
        public String Info { get; }

        /// <summary>
        /// Gets the current row position in the editor, if possible
        /// </summary>
        public int Row { get; }

        /// <summary>
        /// Gets the character position in the editor, if possible
        /// </summary>
        public int Character { get; }

        /// <summary>
        /// Gets the current filename, if available
        /// </summary>
        [NotNull]
        public String Filename { get; }

        // Default constructor
        public IDEStatusUpdateMessage(IDEStatus status, [NotNull] String info, int row, int character, [NotNull] String filename)
        {
            Status = status;
            Info = info;
            Row = row;
            Character = character;
            Filename = filename;
        }
    }

    /// <summary>
    /// Indicates the current status of the IDE
    /// </summary>
    public enum IDEStatus
    {
        Console,
        IDE,
        FaultedIDE
    }
}
