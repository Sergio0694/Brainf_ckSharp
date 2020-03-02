using Brainf_ckSharp.Interfaces;
using Brainf_ckSharp.Uwp.Messages.Abstract;

namespace Brainf_ckSharp.Uwp.Messages.Console.MemoryState
{
    /// <summary>
    /// A request message for the current memory state used in the console
    /// </summary>
    public sealed class MemoryStateRequestMessage : RequestMessageBase<IReadOnlyTuringMachineState> { }
}
