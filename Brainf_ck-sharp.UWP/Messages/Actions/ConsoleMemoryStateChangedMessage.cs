using Brainf_ck_sharp.MemoryState;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Messages.Actions
{
    /// <summary>
    /// A message that signals whenever the console memory state is changed by a script
    /// </summary>
    public class ConsoleMemoryStateChangedMessage
    {
        /// <summary>
        /// Gets the updated memory state for this message
        /// </summary>
        [NotNull]
        public IReadonlyTouringMachineState State { get; }

        /// <summary>
        /// Creates a new instance with the given state
        /// </summary>
        /// <param name="state">The new state to propagate</param>
        public ConsoleMemoryStateChangedMessage([NotNull] IReadonlyTouringMachineState state) => State = state;
    }
}
