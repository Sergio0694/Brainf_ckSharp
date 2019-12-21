using Brainf_ck_sharp_UWP.Messages.Abstract;
using Brainf_ckSharp.Legacy.ReturnTypes;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Messages.UI
{
    /// <summary>
    /// A message that signals the result of a script run in the background
    /// </summary>
    public sealed class BackgroundExecutionStatusChangedMessage : ValueChangedMessageBase<InterpreterResult>
    {
        /// <inheritdoc cref="ValueChangedMessageBase{T}"/>
        public BackgroundExecutionStatusChangedMessage([NotNull] InterpreterResult result) : base(result) { }
    }
}
