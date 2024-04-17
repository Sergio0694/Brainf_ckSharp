using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Models;

namespace Brainf_ckSharp.Memory.Interfaces;

/// <summary>
/// A value-type enumerator for <see cref="IReadOnlyMachineState"/> instances.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public ref struct IReadOnlyMachineStateEnumerator
{
    /// <summary>
    /// The target <see cref="TuringMachineState"/> instance to enumerate
    /// </summary>
    private readonly TuringMachineState machineState;

    /// <summary>
    /// The size of <see cref="machineState"/>, used to skip an indirect memory access
    /// </summary>
    /// <remarks>On 64-bit, this field doesn't change the size of the type anyway</remarks>
    private readonly int size;

    /// <summary>
    /// The current index within <see cref="machineState"/>
    /// </summary>
    private int index;

    /// <summary>
    /// Creates a new <see cref="IReadOnlyMachineStateEnumerator"/> with the specified values
    /// </summary>
    /// <param name="machineState">The target <see cref="TuringMachineState"/> instance to enumerate</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal IReadOnlyMachineStateEnumerator(TuringMachineState machineState)
    {
        this.machineState = machineState;
        this.size = machineState.Size;
        this.index = -1;
    }

    /// <inheritdoc cref="IEnumerator{T}.Current"/>
    public readonly Brainf_ckMemoryCell Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.machineState[this.index];
    }

    /// <inheritdoc cref="System.Collections.IEnumerator.MoveNext"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        int index = this.index + 1;

        if (index < this.size)
        {
            this.index = index;

            return true;
        }

        return false;
    }
}
