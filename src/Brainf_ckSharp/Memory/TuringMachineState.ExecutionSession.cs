using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Brainf_ckSharp.Memory.Interfaces;
using static System.Diagnostics.Debug;

#pragma warning disable IDE0032

namespace Brainf_ckSharp.Memory;

/// <summary>
/// A <see langword="class"/> that represents the state of a Turing machine
/// </summary>
internal sealed partial class TuringMachineState
{
    /// <summary>
    /// Gets an execution session of the specified type
    /// </summary>
    /// <typeparam name="TExecutionContext">The type of execution context to retrieve</typeparam>
    /// <returns>An execution session of the specified type</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ExecutionSession<TExecutionContext> CreateExecutionSession<TExecutionContext>()
        where TExecutionContext : struct, IMachineStateExecutionContext
    {
        return new ExecutionSession<TExecutionContext>(this);
    }

    /// <summary>
    /// A <see langword="struct"/> implementing an execution session with a specified mode
    /// </summary>
    public readonly ref struct ExecutionSession<TExecutionContext>
        where TExecutionContext : struct, IMachineStateExecutionContext
    {
        /// <summary>
        /// The <see cref="GCHandle"/> used to pin the target buffer
        /// </summary>
        /// <remarks>
        /// This handle is allocated to pin the target buffer and allow operations
        /// to be executed without bound checks from each mode-specific execution
        /// context. This is done because C# currently lacks support for ref fields,
        /// as well as ref structs being used as type parameter.
        /// Allocating this handle from and disposing it as soon as the processing
        /// completes minimizes the time the target buffer remains pinned in memory.
        /// </remarks>
        private readonly GCHandle handle;

        /// <summary>
        /// The <see cref="TuringMachineState"/> instance in use
        /// </summary>
        private readonly TuringMachineState machineState;

        /// <summary>
        /// The <typeparamref name="TExecutionContext"/> instance for the current session
        /// </summary>
        public readonly TExecutionContext ExecutionContext;

        /// <summary>
        /// Creates a new <see cref="ExecutionSession{TExecutionContext}"/> instance with the specified value
        /// </summary>
        /// <param name="state">The <see cref="TuringMachineState"/> instance to use</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ExecutionSession(TuringMachineState state)
        {
            Assert(state.buffer != null);

            this.handle = GCHandle.Alloc(state.buffer);
            this.machineState = state;
            this.ExecutionContext = state.GetExecutionContext<TExecutionContext>();
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            // Cast to a mutable reference to avoid the defensive copy
            Unsafe.AsRef(in this.handle).Free();

            this.machineState.position = this.ExecutionContext.Position;
        }
    }
}
