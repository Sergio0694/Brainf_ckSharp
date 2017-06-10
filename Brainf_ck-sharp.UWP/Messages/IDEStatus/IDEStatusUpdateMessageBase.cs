using System;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Messages.IDEStatus
{
    /// <summary>
    /// A message that indicates a new status update for the IDE status bar
    /// </summary>
    public abstract class IDEStatusUpdateMessageBase
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

        // Default constructor
        protected IDEStatusUpdateMessageBase(IDEStatus status, [NotNull] String info)
        {
            Status = status;
            Info = info;
        }
    }

    /// <summary>
    /// Indicates the current status of the IDE
    /// </summary>
    public enum IDEStatus
    {
        Console,
        FaultedConsole,
        IDE,
        FaultedIDE
    }
}
