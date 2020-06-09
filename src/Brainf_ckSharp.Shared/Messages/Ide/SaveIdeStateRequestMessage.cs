using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace Brainf_ckSharp.Shared.Messages.Ide
{
    /// <summary>
    /// A request message to save the current IDE state
    /// </summary>
    public sealed class SaveIdeStateRequestMessage : RequestMessage<Task>
    {
    }
}
