using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Memory.Interfaces;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Base;
using Brainf_ckSharp.Models.Internal;
using Brainf_ckSharp.Opcodes;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.HighPerformance.Buffers;
using static System.Diagnostics.Debug;

#pragma warning disable IDE0032

namespace Brainf_ckSharp.Memory
{
    /// <summary>
    /// A <see langword="class"/> that represents the state of a Turing machine
    /// </summary>
    internal sealed partial class TuringMachineState<T>
    {
        /// <inheritdoc/>
        public InterpreterResult Run(Span<Brainf_ckOperation> opcodes, string stdin, CancellationToken executionToken)
        {
            Assert(opcodes.Length >= 0);

            return Mode switch
            {
                OverflowMode.ByteWithOverflow => Brainf_ckInterpreter.Release.Run<T, ByteWithOverflowExecutionContext>(opcodes, stdin, this, executionToken),
                OverflowMode.ByteWithNoOverflow => Brainf_ckInterpreter.Release.Run<T, ByteWithNoOverflowExecutionContext>(opcodes, stdin, this, executionToken),
                OverflowMode.UshortWithOverflow => Brainf_ckInterpreter.Release.Run<T, UshortWithOverflowExecutionContext>(opcodes, stdin, this, executionToken),
                OverflowMode.UshortWithNoOverflow => Brainf_ckInterpreter.Release.Run<T, UshortWithNoOverflowExecutionContext>(opcodes, stdin, this, executionToken),
                _ => ThrowHelper.ThrowArgumentOutOfRangeException<InterpreterResult>(nameof(Mode), "Invalid execution mode")
            };
        }

        /// <inheritdoc/>
        public Option<IEnumerator<InterpreterResult>> TryCreateSession(
            ReadOnlySpan<char> source,
            ReadOnlySpan<int> breakpoints,
            string stdin,
            CancellationToken executionToken,
            CancellationToken debugToken)
        {
            MemoryOwner<Brainf_ckOperator> opcodes = Brainf_ckParser.TryParse<Brainf_ckOperator>(source, out SyntaxValidationResult validationResult)!;

            if (!validationResult.IsSuccess)
            {
                return Option<IEnumerator<InterpreterResult>>.From(validationResult);
            }

            // Initialize the temporary buffers
            MemoryOwner<bool> breakpointsTable = Brainf_ckInterpreter.Debug.LoadBreakpointsTable(source, validationResult.OperatorsCount, breakpoints);
            MemoryOwner<int> jumpTable = Brainf_ckInterpreter.Debug.LoadJumpTable(opcodes.Span, out int functionsCount);
            MemoryOwner<Range> functions = MemoryOwner<Range>.Allocate(ushort.MaxValue, AllocationMode.Clear);
            MemoryOwner<ushort> definitions = Brainf_ckInterpreter.Debug.LoadDefinitionsTable(functionsCount);
            MemoryOwner<StackFrame> stackFrames = MemoryOwner<StackFrame>.Allocate(Specs.MaximumStackSize);

            // Initialize the root stack frame
            stackFrames.DangerousGetReference() = new StackFrame(new Range(0, opcodes.Length), 0);

            // Create the interpreter session
            InterpreterSession<T> session = new InterpreterSession<T>(
                opcodes,
                breakpointsTable,
                jumpTable,
                functions,
                definitions,
                stackFrames,
                stdin,
                this,
                executionToken,
                debugToken);

            return Option<IEnumerator<InterpreterResult>>.From(validationResult, session);
        }

        /// <summary>
        /// Gets an execution session of the specified type
        /// </summary>
        /// <typeparam name="TExecutionContext">The type of execution context to retrieve</typeparam>
        /// <returns>An execution session of the specified type</returns>
        [Pure]
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
            private readonly GCHandle Handle;

            /// <summary>
            /// The <see cref="TuringMachineState{T}"/> instance in use
            /// </summary>
            private readonly TuringMachineState<T> MachineState;

            /// <summary>
            /// The <typeparamref name="TExecutionContext"/> instance for the current session
            /// </summary>
            public readonly TExecutionContext ExecutionContext;

            /// <summary>
            /// Creates a new <see cref="ExecutionSession{TExecutionContext}"/> instance with the specified value
            /// </summary>
            /// <param name="state">The <see cref="TuringMachineState{T}"/> instance to use</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ExecutionSession(TuringMachineState<T> state)
            {
                Assert(state._Buffer != null);

                Handle = GCHandle.Alloc(state._Buffer);
                MachineState = state;
                ExecutionContext = state.GetExecutionContext<TExecutionContext>();
            }

            /// <inheritdoc cref="IDisposable.Dispose"/>
            public void Dispose()
            {
                // Cast to a mutable reference to avoid the defensive copy
                Unsafe.AsRef(Handle).Free();

                MachineState._Position = ExecutionContext.Position;
            }
        }
    }
}
