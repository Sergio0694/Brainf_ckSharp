using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Brainf_ckSharp.Buffers;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Memory;
using Brainf_ckSharp.Memory.Interfaces;
using Brainf_ckSharp.Models.Internal;
using Brainf_ckSharp.Opcodes;
using Microsoft.Toolkit.HighPerformance.Buffers;
using Range = Brainf_ckSharp.Models.Internal.Range;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Brainf_ckSharp.Models
{
    /// <summary>
    /// A <see langword="class"/> that represents an interpreter session executing on a given script
    /// </summary>
    public sealed class InterpreterSession : IEnumerator<InterpreterResult>
    {
        /// <summary>
        /// The sequence of parsed opcodes to execute
        /// </summary>
        private readonly MemoryOwner<Brainf_ckOperator> Opcodes;

        /// <summary>
        /// The table of breakpoints for the current executable
        /// </summary>
        private readonly MemoryOwner<bool> Breakpoints;

        /// <summary>
        /// The jump table for loops and function declarations
        /// </summary>
        private readonly MemoryOwner<int> JumpTable;

        /// <summary>
        /// The mapping of functions for the current execution
        /// </summary>
        private readonly MemoryOwner<Range> Functions;

        /// <summary>
        /// The lookup table to check which functions are defined
        /// </summary>
        private readonly MemoryOwner<ushort> Definitions;

        /// <summary>
        /// The sequence of stack frames for the current execution
        /// </summary>
        private readonly MemoryOwner<StackFrame> StackFrames;

        /// <summary>
        /// The input buffer to read characters from
        /// </summary>
        private readonly StdinBuffer StdinBuffer;

        /// <summary>
        /// The output buffer to write characters to
        /// </summary>
        private readonly StdoutBuffer StdoutBuffer;

        /// <summary>
        /// The target <see cref="TuringMachineState"/> instance to execute the code on
        /// </summary>
        private readonly TuringMachineState MachineState;

        /// <summary>
        /// The current stack depth
        /// </summary>
        private int _Depth;

        /// <summary>
        /// The total number of executed opcodes
        /// </summary>
        private int _TotalOperations;

        /// <summary>
        /// The total number of defined functions
        /// </summary>
        private int _TotalFunctions;

        /// <summary>
        /// A <see cref="CancellationToken"/> that can be used to halt the execution
        /// </summary>
        private readonly CancellationToken ExecutionToken;

        /// <summary>
        /// A <see cref="CancellationToken"/> that is used to ignore/respect existing breakpoints
        /// </summary>
        private readonly CancellationToken DebugToken;

        /// <summary>
        /// A stopwatch used to keep track of the elapsed time during the execution of the script
        /// </summary>
        private readonly Stopwatch Stopwatch;

        /// <summary>
        /// The original source code for the interpreted script
        /// </summary>
        private readonly string SourceCode;

        /// <summary>
        /// Indicates whether or not the current instance has already been disposed
        /// </summary>
        private bool _Disposed;

        /// <summary>
        /// Creates a new <see cref="InterpreterSession"/> with the specified parameters
        /// </summary>
        /// <param name="opcodes">The sequence of parsed opcodes to execute</param>
        /// <param name="breakpoints">The table of breakpoints for the current executable</param>
        /// <param name="jumpTable">The jump table for loops and function declarations</param>
        /// <param name="functions">The mapping of functions for the current execution</param>
        /// <param name="definitions">The lookup table to check which functions are defined</param>
        /// <param name="stackFrames">The sequence of stack frames for the current execution</param>
        /// <param name="stdin">The input <see cref="string"/> to read characters from</param>
        /// <param name="machineState">The target machine state to use to run the script</param>
        /// <param name="executionToken">A <see cref="CancellationToken"/> that can be used to halt the execution</param>
        /// <param name="debugToken">A <see cref="CancellationToken"/> that is used to ignore/respect existing breakpoints</param>
        internal InterpreterSession(
            MemoryOwner<Brainf_ckOperator> opcodes,
            MemoryOwner<bool> breakpoints,
            MemoryOwner<int> jumpTable,
            MemoryOwner<Range> functions,
            MemoryOwner<ushort> definitions,
            MemoryOwner<StackFrame> stackFrames,
            string stdin,
            TuringMachineState machineState,
            CancellationToken executionToken,
            CancellationToken debugToken)
        {
            Opcodes = opcodes;
            Breakpoints = breakpoints;
            JumpTable = jumpTable;
            Functions = functions;
            Definitions = definitions;
            StackFrames = stackFrames;
            StdinBuffer = new StdinBuffer(stdin);
            StdoutBuffer = StdoutBuffer.Allocate();
            MachineState = machineState;
            ExecutionToken = executionToken;
            DebugToken = debugToken;
            Stopwatch = new Stopwatch();
            SourceCode = Brainf_ckParser.ExtractSource(opcodes.Span);
        }

        private InterpreterResult? _Current;

        /// <inheritdoc/>
        public InterpreterResult Current => _Current ?? throw new InvalidOperationException("The session has not been initialized yet");

        /// <inheritdoc/>
        object IEnumerator.Current => Current;

        /// <inheritdoc/>
        public bool MoveNext()
        {
            // Check whether the current session can go ahead by one step
            if (_Current != null &&
                (_Current.ExitCode.HasFlag(ExitCode.Failure) ||
                 !_Current.ExitCode.HasFlag(ExitCode.BreakpointReached)))
            {
                return false;
            }

            // Execute the mode specific implementation
            switch (MachineState.Mode)
            {
                case OverflowMode.UshortWithNoOverflow: MoveNext<TuringMachineState.UshortWithNoOverflowExecutionContext>(); break;
                case OverflowMode.UshortWithOverflow: MoveNext<TuringMachineState.UshortWithOverflowExecutionContext>(); break;
                case OverflowMode.ByteWithNoOverflow: MoveNext<TuringMachineState.ByteWithNoOverflowExecutionContext>(); break;
                case OverflowMode.ByteWithOverflow: MoveNext<TuringMachineState.ByteWithOverflowExecutionContext>(); break;
                default: throw new ArgumentOutOfRangeException(nameof(MachineState.Mode), $"Invalid execution mode: {MachineState.Mode}");
            };

            return true;
        }

        /// <summary>
        /// Implements the <see cref="MoveNext"/> logic with a specific execution mode
        /// </summary>
        /// <typeparam name="TExecutionContext">The type implementing <see cref="IMachineStateExecutionContext"/> to use</typeparam>
        private void MoveNext<TExecutionContext>()
            where TExecutionContext : struct, IMachineStateExecutionContext
        {
            ExitCode exitCode;

            using (TuringMachineState.ExecutionSession<TExecutionContext> session = MachineState.CreateExecutionSession<TExecutionContext>())
            {
                Stopwatch.Start();

                // Execute the new interpreter debug step
                exitCode = Brainf_ckInterpreter.Debug.Run(
                    ref Unsafe.AsRef(session.ExecutionContext),
                    ref Opcodes.DangerousGetReference(),
                    ref Breakpoints.DangerousGetReference(),
                    ref JumpTable.DangerousGetReference(),
                    ref Functions.DangerousGetReference(),
                    ref Definitions.DangerousGetReference(),
                    ref StackFrames.DangerousGetReference(),
                    ref _Depth,
                    ref _TotalOperations,
                    ref _TotalFunctions,
                    ref Unsafe.AsRef(StdinBuffer),
                    ref Unsafe.AsRef(StdoutBuffer),
                    ExecutionToken,
                    DebugToken);

                Stopwatch.Stop();
            }

            // Prepare the debug info
            HaltedExecutionInfo? debugInfo = Brainf_ckInterpreter.LoadDebugInfo(
                Opcodes.Span,
                StackFrames.Span,
                _Depth);

            // Build the collection of defined functions
            FunctionDefinition[] functionDefinitions = Brainf_ckInterpreter.LoadFunctionDefinitions(
                Opcodes.Span,
                Functions.Span,
                Definitions.Span,
                _TotalFunctions);

            // Update the current interpreter result
            _Current = new InterpreterResult(
                SourceCode,
                exitCode,
                debugInfo,
                (TuringMachineState)MachineState.Clone(),
                functionDefinitions,
                StdinBuffer.ToString(),
                StdoutBuffer.ToString(),
                Stopwatch.Elapsed,
                _TotalOperations);
        }

        /// <inheritdoc/>
        public void Reset() => throw new NotSupportedException("An interpreter session can't be reset");

        ~InterpreterSession() => Dispose();

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_Disposed) return;

            _Disposed = true;

            Opcodes.Dispose();
            Breakpoints.Dispose();
            JumpTable.Dispose();
            Functions.Dispose();
            Definitions.Dispose();
            StackFrames.Dispose();
            StackFrames.Dispose();
            StdoutBuffer.Dispose();
            Stopwatch.Stop();
        }
    }
}
