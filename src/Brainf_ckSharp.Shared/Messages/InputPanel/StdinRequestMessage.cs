using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace Brainf_ckSharp.Shared.Messages.InputPanel
{
    /// <summary>
    /// A request message for the current stdin buffer
    /// </summary>
    public sealed class StdinRequestMessage : RequestMessage<string> { }
}
