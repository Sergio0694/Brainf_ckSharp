using System;
using System.Collections.Generic;
using Brainf_ck_sharp.NET.MemoryState;

namespace Brainf_ck_sharp.NET.Interfaces
{
    /// <summary>
    /// An <see langword="interface"/> that represents a readonly state of a Turing machine
    /// </summary>
    public interface IReadOnlyTuringMachineState : IEquatable<IReadOnlyTuringMachineState>, IReadOnlyList<Brainf_ckMemoryCell>, ICloneable, IDisposable 
    {
        /// <summary>
        /// Gets the current position on the memory buffer
        /// </summary>
        int Position { get; }

        /// <summary>
        /// Gets the cell at the current memory position
        /// </summary>
        Brainf_ckMemoryCell Current { get; }
    }
}
