using Brainf_ckSharp.Memory.Interfaces;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace Brainf_ckSharp.Uwp.Messages.Console.MemoryState
{
    /// <summary>
    /// A request message for the current memory state used in the console
    /// </summary>
    public sealed class MemoryStateRequestMessage : RequestMessage<IReadOnlyMachineState> { }
}
