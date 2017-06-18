namespace Brainf_ck_sharp_UWP.Messages.Actions
{
    /// <summary>
    /// A message that indicates a user request to either undo or redo the latest IDE changes
    /// </summary>
    public sealed class IDEUndoRedoRequestMessage
    {
        /// <summary>
        /// Gets the requested operation to perform
        /// </summary>
        public UndoRedoOperation Operation { get; }

        /// <summary>
        /// Creates a new request for the given operation
        /// </summary>
        /// <param name="operation">Indicates which operation was requested by the user</param>
        public IDEUndoRedoRequestMessage(UndoRedoOperation operation) => Operation = operation;
    }

    /// <summary>
    /// Indicates the requested undo/redo operation for the IDE
    /// </summary>
    public enum UndoRedoOperation
    {
        Undo,
        Redo
    }
}
