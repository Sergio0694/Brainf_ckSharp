using Brainf_ckSharp.Uwp.Messages.Abstract;

namespace Brainf_ckSharp.Uwp.Messages.InputPanel
{
    /// <summary>
    /// A request message for the current stdin buffer
    /// </summary>
    public sealed class StdinRequestMessage : RequestMessageBase<string> { }
}
