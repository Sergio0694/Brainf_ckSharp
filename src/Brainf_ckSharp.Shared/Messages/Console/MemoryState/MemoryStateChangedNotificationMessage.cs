using Brainf_ckSharp.Memory.Interfaces;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace Brainf_ckSharp.Shared.Messages.Console.MemoryState
{
    /// <summary>
    /// A message that notifies whenever the machine state for the Brainf*ck/PBrain console changes
    /// </summary>
    public sealed class MemoryStateChangedNotificationMessage : ValueChangedMessage<IReadOnlyMachineState>
    {
        /// <summary>
        /// Creates a new <see cref="MemoryStateChangedNotificationMessage"/> instance with the specified parameters
        /// </summary>
        /// <param name="value">The <see cref="IReadOnlyMachineState"/> instance with the new machine state</param>
        public MemoryStateChangedNotificationMessage(IReadOnlyMachineState value) : base(value) { }
    }
}
