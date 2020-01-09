using Brainf_ckSharp.Uwp.Messages.Abstract;
using Brainf_ckSharp.Uwp.Models.Ide;

namespace Brainf_ckSharp.Uwp.Messages.Ide
{
    /// <summary>
    /// A message that signals whenever the user requests to open a file
    /// </summary>
    public sealed class LoadSourceCodeRequestMessage : ValueChangedMessageBase<SourceCode>
    {
        /// <summary>
        /// Creates a new <see cref="LoadSourceCodeRequestMessage"/> with the specified parameters
        /// </summary>
        /// <param name="sourceCode">The <see cref="SourceCode"/> instance to load</param>
        public LoadSourceCodeRequestMessage(SourceCode sourceCode) : base(sourceCode) { }
    }
}
