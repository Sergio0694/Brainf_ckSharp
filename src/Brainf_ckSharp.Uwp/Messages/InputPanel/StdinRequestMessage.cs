using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace Brainf_ckSharp.Uwp.Messages.InputPanel
{
    /// <summary>
    /// A request message for the current stdin buffer
    /// </summary>
    public sealed class StdinRequestMessage : RequestMessage<string> { }
}
