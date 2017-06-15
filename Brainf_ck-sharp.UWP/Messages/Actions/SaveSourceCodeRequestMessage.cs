namespace Brainf_ck_sharp_UWP.Messages.Actions
{
    /// <summary>
    /// A message that signals whenever the user requests to save a source code he's working on
    /// </summary>
    public sealed class SaveSourceCodeRequestMessage
    {
        /// <summary>
        /// Gets the type of request made by the user
        /// </summary>
        public CodeSaveType RequestType { get; }

        /// <summary>
        /// Creates a new instance of the given type
        /// </summary>
        /// <param name="request">The request type to wrap</param>
        public SaveSourceCodeRequestMessage(CodeSaveType request) => RequestType = request;
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
