using Brainf_ckSharp.Interfaces;
using Brainf_ckSharp.UWP.Messages.Abstract;

namespace Brainf_ckSharp.UWP.Messages.Console.MemoryState
{
    /// <summary>
    /// A message that notifies whenever the machine state for the Brainf*ck/PBrain console changes
    /// </summary>
    public sealed class MemoryStateChangedNotificationMessage : ValueChangedMessageBase<IReadOnlyTuringMachineState>
    {
        /// <summary>
        /// Creates a new <see cref="MemoryStateChangedNotificationMessage"/> instance with the specified parameters
        /// </summary>
        /// <param name="value">The <see cref="IReadOnlyTuringMachineState"/> instance with the new machine state</param>
        public MemoryStateChangedNotificationMessage(IReadOnlyTuringMachineState value) : base(value) { }
    }
}
