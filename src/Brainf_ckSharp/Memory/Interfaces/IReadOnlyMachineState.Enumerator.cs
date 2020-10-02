using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Models;

namespace Brainf_ckSharp.Memory.Interfaces
{
    /// <summary>
    /// A value-type enumerator for <see cref="IReadOnlyMachineState"/> instances.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ref struct IReadOnlyMachineStateEnumerator
    {
        /// <summary>
        /// The target <see cref="IReadOnlyMachineState"/> instance to enumerate
        /// </summary>
        private readonly IReadOnlyMachineState MachineState;

        /// <summary>
        /// The size of <see cref="MachineState"/>, used to skip an indirect memory access
        /// </summary>
        /// <remarks>On 64-bit, this field doesn't change the size of the type anyway</remarks>
        private readonly int Size;

        /// <summary>
        /// The current index within <see cref="MachineState"/>
        /// </summary>
        private int _Index;

        /// <summary>
        /// Creates a new <see cref="IReadOnlyMachineStateEnumerator"/> with the specified values
        /// </summary>
        /// <param name="machineState">The target <see cref="IReadOnlyMachineState"/> instance to enumerate</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal IReadOnlyMachineStateEnumerator(IReadOnlyMachineState machineState)
        {
            MachineState = machineState;
            Size = machineState.Count;
            _Index = -1;
        }

        /// <inheritdoc cref="IEnumerator{T}.Current"/>
        public Brainf_ckMemoryCell Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MachineState[_Index];
        }

        /// <inheritdoc cref="System.Collections.IEnumerator.MoveNext"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            int index = _Index + 1;

            if (index < Size)
            {
                _Index = index;

                return true;
            }

            return false;
        }
    }
}
