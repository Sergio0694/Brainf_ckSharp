using Brainf_ckSharp.UWP.Messages.Abstract;

namespace Brainf_ckSharp.UWP.Messages.InputPanel
{
    /// <summary>
    /// A request message for the current stdin buffer
    /// </summary>
    public sealed class StdinRequestMessage : RequestMessageBase<string> { }
}
