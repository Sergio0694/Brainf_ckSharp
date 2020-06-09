using Brainf_ck_sharp.Legacy.UWP.Messages.Requests.Abstract;
using Brainf_ckSharp.Legacy.MemoryState;

namespace Brainf_ck_sharp.Legacy.UWP.Messages.Requests
{
    /// <summary>
    /// A request message for the inputs for the background code executor
    /// </summary>
    public sealed class BackgroundExecutionInputRequestMessage : RequestMessageBase<(string Code, string Stdin, IReadonlyTouringMachineState state)> { }
}
