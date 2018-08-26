using Brainf_ck_sharp_UWP.Messages.Abstract;

namespace Brainf_ck_sharp_UWP.Messages.Actions
{
    /// <summary>
    /// A message that indicates a user request to either undo or redo the latest IDE changes
    /// </summary>
    public sealed class IDEUndoRedoRequestMessage : ValueChangedMessageBase<UndoRedoOperation>
    {
        /// <inheritdoc cref="ValueChangedMessageBase{T}"/>
        public IDEUndoRedoRequestMessage(UndoRedoOperation operation) : base(operation) { }
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
