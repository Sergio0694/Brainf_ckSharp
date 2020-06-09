using Brainf_ckSharp.Shared.Models.Ide;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace Brainf_ckSharp.Shared.Messages.Ide
{
    /// <summary>
    /// A message that signals whenever the user requests to open a file
    /// </summary>
    public sealed class LoadSourceCodeRequestMessage : ValueChangedMessage<SourceCode>
    {
        /// <summary>
        /// Creates a new <see cref="LoadSourceCodeRequestMessage"/> with the specified parameters
        /// </summary>
        /// <param name="sourceCode">The <see cref="SourceCode"/> instance to load</param>
        public LoadSourceCodeRequestMessage(SourceCode sourceCode) : base(sourceCode) { }
    }
}
