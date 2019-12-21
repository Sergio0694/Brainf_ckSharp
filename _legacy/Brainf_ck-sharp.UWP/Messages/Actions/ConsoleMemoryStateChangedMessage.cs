using Brainf_ck_sharp_UWP.Messages.Abstract;
using Brainf_ckSharp.Legacy.MemoryState;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Messages.Actions
{
    /// <summary>
    /// A message that signals whenever the console memory state is changed by a script
    /// </summary>
    public sealed class ConsoleMemoryStateChangedMessage : ValueChangedMessageBase<IReadonlyTouringMachineState>
    {
        /// <inheritdoc cref="ValueChangedMessageBase{T}"/>
        public ConsoleMemoryStateChangedMessage([NotNull] IReadonlyTouringMachineState state) : base(state) { }
    }
}
