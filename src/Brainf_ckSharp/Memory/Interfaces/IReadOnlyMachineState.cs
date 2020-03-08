using System;
using System.Collections.Generic;
using Brainf_ckSharp.Models;

namespace Brainf_ckSharp.Memory.Interfaces
{
    /// <summary>
    /// An <see langword="interface"/> that represents a readonly machine state
    /// </summary>
    public interface IReadOnlyMachineState : IEquatable<IReadOnlyMachineState>, IReadOnlyList<Brainf_ckMemoryCell>, ICloneable, IDisposable 
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
