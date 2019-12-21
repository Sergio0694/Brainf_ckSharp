using Brainf_ck_sharp.Legacy.UWP.Messages.Abstract;

namespace Brainf_ck_sharp.Legacy.UWP.Messages.Actions
{
    /// <summary>
    /// A message that signals whenever the user requests to save a source code he's working on
    /// </summary>
    public sealed class SaveSourceCodeRequestMessage : ValueChangedMessageBase<CodeSaveType>
    {
        /// <inheritdoc cref="ValueChangedMessageBase{T}"/>
        public SaveSourceCodeRequestMessage(CodeSaveType request) : base(request) { }
    }

    /// <summary>
    /// Indicates the request type for the source code to save
    /// </summary>
    public enum CodeSaveType
    {
        Save,
        SaveAs
    }
}
