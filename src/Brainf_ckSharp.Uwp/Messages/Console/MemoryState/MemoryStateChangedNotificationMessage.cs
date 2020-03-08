using Brainf_ckSharp.Memory.Interfaces;
using Brainf_ckSharp.Uwp.Messages.Abstract;

namespace Brainf_ckSharp.Uwp.Messages.Console.MemoryState
{
    /// <summary>
    /// A message that notifies whenever the machine state for the Brainf*ck/PBrain console changes
    /// </summary>
    public sealed class MemoryStateChangedNotificationMessage : ValueChangedMessageBase<IReadOnlyMachineState>
    {
        /// <summary>
        /// Creates a new <see cref="MemoryStateChangedNotificationMessage"/> instance with the specified parameters
        /// </summary>
        /// <param name="value">The <see cref="IReadOnlyMachineState"/> instance with the new machine state</param>
        public MemoryStateChangedNotificationMessage(IReadOnlyMachineState value) : base(value) { }
    }
}
