using System.Collections.Generic;
using JetBrains.Annotations;

namespace Brainf_ckSharp.Legacy.MemoryState
{
    /// <summary>
    /// An interface that exposes a readonly memory state for a Touring machine
    /// </summary>
    public interface IReadonlyTouringMachineState : IReadOnlyList<Brainf_ckMemoryCell>
    {
        /// <summary>
        /// Gets the current position on the memory array
        /// </summary>
        int Position { get; }

        /// <summary>
        /// Gets the current value for the memory state
        /// </summary>
        Brainf_ckMemoryCell Current { get; }

        /// <summary>
        /// Returns a new instance where all the cells have been updated to be lower than 255
        /// </summary>
        [NotNull]
        IReadonlyTouringMachineState ApplyByteOverflow();
    }
}