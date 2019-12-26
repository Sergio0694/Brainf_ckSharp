using Brainf_ckSharp.Interfaces;
using Brainf_ckSharp.UWP.Messages.Abstract;

namespace Brainf_ckSharp.UWP.Messages.Console.MemoryState
{
    /// <summary>
    /// A request message for the current memory state used in the console
    /// </summary>
    public sealed class MemoryStateRequestMessage : RequestMessageBase<IReadOnlyTuringMachineState> { }
}
