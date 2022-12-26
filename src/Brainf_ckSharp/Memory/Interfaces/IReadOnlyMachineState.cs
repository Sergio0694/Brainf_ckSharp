using System;
using System.Collections.Generic;
using Brainf_ckSharp.Models;

namespace Brainf_ckSharp.Memory.Interfaces;

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

    /// <summary>
    /// Returns a value-type implementing an allocation-free enumerator of the memory cells in the current
    /// instance. The return type shouldn't be used directly: just use a <see langword="foreach"/> block on
    /// the <see cref="IReadOnlyMachineState"/> instance in use and the C# compiler will automatically invoke this
    /// method behind the scenes. This method takes precedence over the <see cref="IEnumerable{T}.GetEnumerator"/>
    /// implementation, which is still available when casting to one of the underlying interfaces.
    /// </summary>
    /// <returns>A new <see cref="IReadOnlyMachineStateEnumerator"/> instance mapping the current <see cref="Brainf_ckMemoryCell"/> values in use.</returns>
    new IReadOnlyMachineStateEnumerator GetEnumerator();
}
