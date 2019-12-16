using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Brainf_ck_sharp.NET.Buffers;
using Brainf_ck_sharp.NET.Buffers.IO;
using Brainf_ck_sharp.NET.Enums;
using Brainf_ck_sharp.NET.Extensions.Types;
using Brainf_ck_sharp.NET.Models.Internal;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Brainf_ck_sharp.NET.Models
{
    /// <summary>
    /// A <see langword="class"/> that represents an interpreter session executing on a given script
    /// </summary>
    public sealed class InterpreterSession : IEnumerator<InterpreterResult>
    {
        /// <summary>
        /// The sequence of parsed operators to execute
        /// </summary>
        private readonly UnsafeMemoryBuffer<byte> Operators;

        /// <summary>
        /// The table of breakpoints for the current executable
        /// </summary>
        private readonly UnsafeMemoryBuffer<bool> Breakpoints;

        /// <summary>
        /// The jump table for loops and function declarations
        /// </summary>
        private readonly UnsafeMemoryBuffer<int> JumpTable;

        /// <summary>
        /// The mapping of functions for the current execution
        /// </summary>
        private readonly UnsafeMemoryBuffer<Range> Functions;

        /// <summary>
        /// The lookup table to check which functions are defined
        /// </summary>
        private readonly UnsafeMemoryBuffer<ushort> Definitions;

        /// <summary>
        /// The sequence of stack frames for the current execution
        /// </summary>
        private readonly UnsafeMemoryBuffer<StackFrame> StackFrames;

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
        /// The total number of executed operators
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
        /// Creates a new <see cref="InterpreterSession"/> with the specified parameters
        /// </summary>
        /// <param name="operators">The sequence of parsed operators to execute</param>
        /// <param name="breakpoints">The table of breakpoints for the current executable</param>
        /// <param name="jumpTable">The jump table for loops and function declarations</param>
        /// <param name="functions">The mapping of functions for the current execution</param>
        /// <param name="definitions">The lookup table to check which functions are defined</param>
        /// <param name="stackFrames">The sequence of stack frames for the current execution</param>
        /// <param name="stdin">The input <see cref="string"/> to read characters from</param>
        /// <param name="memorySize">The size of the state machine to create to run the script</param>
        /// <param name="overflowMode">The overflow mode to use in the state machine used to run the script</param>
        /// <param name="executionToken">A <see cref="CancellationToken"/> that can be used to halt the execution</param>
        /// <param name="debugToken">A <see cref="CancellationToken"/> that is used to ignore/respect existing breakpoints</param>
        internal InterpreterSession(
            UnsafeMemoryBuffer<byte> operators,
            UnsafeMemoryBuffer<bool> breakpoints,
            UnsafeMemoryBuffer<int> jumpTable,
            UnsafeMemoryBuffer<Range> functions,
            UnsafeMemoryBuffer<ushort> definitions,
            UnsafeMemoryBuffer<StackFrame> stackFrames,
            string stdin,
            int memorySize,
            OverflowMode overflowMode,
            CancellationToken executionToken,
            CancellationToken debugToken)
        {
            Operators = operators;
            Breakpoints = breakpoints;
            JumpTable = jumpTable;
            Functions = functions;
            Definitions = definitions;
            StackFrames = stackFrames;
            StdinBuffer = new StdinBuffer(stdin);
            StdoutBuffer = new StdoutBuffer();
            MachineState = new TuringMachineState(memorySize, overflowMode);
            ExecutionToken = executionToken;
            DebugToken = debugToken;
            Stopwatch = new Stopwatch();
            SourceCode = Brainf_ckParser.ExtractSource(operators.Memory);
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
            if (Current != null &&
                (Current.ExitCode.HasFlag(ExitCode.Failure) ||
                 !Current.ExitCode.HasFlag(ExitCode.BreakpointReached)))
            {
                return false;
            }

            Stopwatch.Start();

            // Execute the new interpreter debug step
            ExitCode exitCode = Brainf_ckInterpreter.TryRun(
                Operators.Memory,
                Breakpoints.Memory,
                JumpTable.Memory,
                Functions.Memory,
                Definitions.Memory,
                MachineState,
                StdinBuffer,
                StdoutBuffer,
                StackFrames.Memory,
                ref _Depth,
                ref _TotalOperations,
                ref _TotalFunctions,
                ExecutionToken,
                DebugToken);

            Stopwatch.Stop();

            // Prepare the stack frames
            string[] stackTrace = Brainf_ckInterpreter.LoadStackTrace(
                Operators.Memory,
                StackFrames.Memory,
                _Depth);

            // Build the collection of defined functions
            FunctionDefinition[] functionDefinitions = Brainf_ckInterpreter.LoadFunctionDefinitions(
                Operators.Memory,
                Functions.Memory,
                Definitions.Memory,
                _TotalFunctions);

            // Update the current interpreter result
            _Current = new InterpreterResult(
                SourceCode,
                exitCode,
                stackTrace,
                (TuringMachineState)MachineState.Clone(),
                functionDefinitions,
                StdinBuffer.ToString(),
                StdoutBuffer.ToString(),
                Stopwatch.Elapsed,
                _TotalOperations);

            return true;
        }

        /// <inheritdoc/>
        public void Reset() => throw new NotSupportedException("An interpreter session can't be reset");

        ~InterpreterSession() => Dispose();

        /// <inheritdoc/>
        public void Dispose()
        {
            Operators.Dispose();
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
