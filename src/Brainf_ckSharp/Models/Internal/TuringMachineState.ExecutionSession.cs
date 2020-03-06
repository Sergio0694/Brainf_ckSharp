using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

#pragma warning disable IDE0032

namespace Brainf_ckSharp.Models.Internal
{
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
        [Pure]
        private ExecutionSession<TExecutionContext> CreateExecutionSession<TExecutionContext>()
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
            /// The <typeparamref name="TExecutionContext"/> instance for the current session
            /// </summary>
            public readonly TExecutionContext ExecutionContext;

            /// <summary>
            /// The <see cref="TuringMachineState"/> instance in use
            /// </summary>
            public readonly TuringMachineState MachineState;

            /// <summary>
            /// Creates a new <see cref="ExecutionSession{TExecutionContext}"/> instance with the specified value
            /// </summary>
            /// <param name="state">The <see cref="TuringMachineState"/> instance to use</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ExecutionSession(TuringMachineState state)
            {
                ExecutionContext = state.GetExecutionContext<TExecutionContext>();
                MachineState = state;
            }

            /// <inheritdoc cref="IDisposable.Dispose"/>
            public void Dispose()
            {
                MachineState._Position = ExecutionContext.Position;
            }
        }
    }
}
