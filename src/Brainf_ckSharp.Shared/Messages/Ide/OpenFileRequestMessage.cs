using Brainf_ckSharp.Services;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace Brainf_ckSharp.Shared.Messages.Ide
{
    /// <summary>
    /// A message that signals whenever the user requests to open a file
    /// </summary>
    public sealed class OpenFileRequestMessage : ValueChangedMessage<IFile>
    {
        /// <summary>
        /// Creates a new <see cref="OpenFileRequestMessage"/> instance with the specified parameters
        /// </summary>
        /// <param name="file">The file to open</param>
        public OpenFileRequestMessage(IFile file) : base(file)
        {
        }
    }
}
