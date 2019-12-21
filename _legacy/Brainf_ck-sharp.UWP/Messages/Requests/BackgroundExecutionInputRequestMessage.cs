using Brainf_ck_sharp.MemoryState;
using Brainf_ck_sharp_UWP.Messages.Requests.Abstract;

namespace Brainf_ck_sharp_UWP.Messages.Requests
{
    /// <summary>
    /// A request message for the inputs for the background code executor
    /// </summary>
    public sealed class BackgroundExecutionInputRequestMessage : RequestMessageBase<(string Code, string Stdin, IReadonlyTouringMachineState state)> { }
}
