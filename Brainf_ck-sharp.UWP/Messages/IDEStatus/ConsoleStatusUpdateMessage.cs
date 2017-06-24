using System;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Messages.IDEStatus
{
    /// <summary>
    /// A message that indicates a new status update for the console
    /// </summary>
    public sealed class ConsoleStatusUpdateMessage : IDEStatusUpdateMessageBase
    {
        /// <summary>
        /// Gets the character position in the console command
        /// </summary>
        public int Character { get; }

        /// <summary>
        /// Gets the position of the first wrong character, if possible
        /// </summary>
        public int ErrorPosition { get; }

        // Default constructor
        public ConsoleStatusUpdateMessage(IDEStatus status, [NotNull] String info, int character, int error) : base(status, info)
        {
            Character = character;
            ErrorPosition = error;
        }
    }
}
