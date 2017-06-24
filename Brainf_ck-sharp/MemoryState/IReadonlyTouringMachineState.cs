using System.Collections.Generic;

namespace Brainf_ck_sharp.MemoryState
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
    }
}