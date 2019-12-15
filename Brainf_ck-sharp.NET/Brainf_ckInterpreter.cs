using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using Brainf_ck_sharp.NET.Buffers;
using Brainf_ck_sharp.NET.Buffers.IO;
using Brainf_ck_sharp.NET.Constants;
using Brainf_ck_sharp.NET.Enums;
using Brainf_ck_sharp.NET.Extensions.Types;
using Brainf_ck_sharp.NET.Helpers;
using Brainf_ck_sharp.NET.Interfaces;
using Brainf_ck_sharp.NET.Models;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Brainf_ck_sharp.NET
{
    /// <summary>
    /// A <see langword="class"/> responsible for interpreting and debugging Brainf*ck/PBrain scripts
    /// </summary>
    public static class Brainf_ckInterpreter
    {
        /// <summary>
        /// The default memory size for machine states used to run scripts
        /// </summary>
        public const int DefaultMemorySize = 128;

        /// <summary>
        /// The default overflow mode for running scripts
        /// </summary>
        public const OverflowMode DefaultOverflowMode = OverflowMode.ByteWithNoOverflow;

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <returns>An <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        public static InterpreterResult Run(string source)
        {
            return Run(source, string.Empty, DefaultMemorySize, DefaultOverflowMode);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="memorySize">The size of the state machine to create to run the script</param>
        /// <returns>An <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        public static InterpreterResult Run(string source, string stdin, int memorySize)
        {
            return Run(source, stdin, memorySize, DefaultOverflowMode);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="memorySize">The size of the state machine to create to run the script</param>
        /// <param name="overflowMode">The overflow mode to use in the state machine used to run the script</param>
        /// <returns>An <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        public static InterpreterResult Run(string source, string stdin, int memorySize, OverflowMode overflowMode)
        {
            Guard.MustBeGreaterThanOrEqualTo(memorySize, 32, nameof(memorySize));
            Guard.MustBeLessThanOrEqualTo(memorySize, 1024, nameof(memorySize));

            using UnsafeMemoryBuffer<byte>? operators = Brainf_ckParser.TryParse(source, out SyntaxValidationResult validationResult);

            if (!validationResult.IsSuccess) return null;

            TuringMachineState machineState = new TuringMachineState(memorySize, overflowMode);

            return Run(operators!, stdin, machineState);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="initialState">The initial state machine to use to start running the script from</param>
        /// <returns>An <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        public static InterpreterResult Run(string source, IReadOnlyTuringMachineState initialState)
        {
            return Run(source, string.Empty, initialState);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="initialState">The initial state machine to use to start running the script from</param>
        /// <returns>An <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        public static InterpreterResult Run(string source, string stdin, IReadOnlyTuringMachineState initialState)
        {
            DebugGuard.MustBeTrue(initialState is TuringMachineState, nameof(initialState));

            using UnsafeMemoryBuffer<byte>? operators = Brainf_ckParser.TryParse(source, out SyntaxValidationResult validationResult);

            if (!validationResult.IsSuccess) return null;

            TuringMachineState machineState = (TuringMachineState)((TuringMachineState)initialState).Clone();

            return Run(operators!, stdin, machineState);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="operators">The executable to run</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="machineState">The target machine state to use to run the script</param>
        /// <returns>An <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        private static InterpreterResult Run(
            UnsafeMemoryBuffer<byte> operators,
            string stdin,
            TuringMachineState machineState)
        {
            DebugGuard.MustBeGreaterThanOrEqualTo(operators.Size, 0, nameof(operators));
            DebugGuard.MustBeGreaterThanOrEqualTo(machineState.Size, 0, nameof(machineState));

            // Initialize the temporary buffers
            using UnsafeMemoryBuffer<bool> breakpoints = UnsafeMemoryBuffer<bool>.Allocate(operators.Size, true);
            using UnsafeMemoryBuffer<int> jumpTable = LoadJumpTable(operators);
            using UnsafeMemoryBuffer<Range> functions = UnsafeMemoryBuffer<Range>.Allocate(ushort.MaxValue, true);
            using UnsafeMemoryBuffer<ushort> definitions = UnsafeMemoryBuffer<ushort>.Allocate(operators.Size, true);
            using UnsafeMemoryBuffer<StackFrame> stackFrames = UnsafeMemoryBuffer<StackFrame>.Allocate(MaximumStackSize, false);
            using StdoutBuffer stdout = new StdoutBuffer();

            // Shared counters
            int depth = 0;
            int totalOperations = 0;
            int totalFunctions = 0;

            // Manually set the initial stack frame to the entire script
            stackFrames[0] = new StackFrame(new Range(0, operators.Size), 0);

            Stopwatch stopwatch = Stopwatch.StartNew();

            // Start the interpreter
            InterpreterWorkingData data = TryRun(
                operators.Memory,
                breakpoints.Memory,
                jumpTable.Memory,
                functions.Memory,
                definitions.Memory,
                machineState,
                new StdinBuffer(stdin),
                stdout, stackFrames.Memory,
                ref depth,
                ref totalOperations,
                ref totalFunctions,
                CancellationToken.None,
                CancellationToken.None);

            stopwatch.Stop();

            // Rebuild the compacted source code
            string sourceCode = Brainf_ckParser.ExtractSource(operators.Memory);

            // Build the collection of defined functions
            FunctionDefinition[] functionDefinitions = LoadFunctionDefinitions(
                operators.Memory,
                functions.Memory,
                definitions.Memory,
                totalFunctions);

            return new InterpreterResult(
                sourceCode,
                data.ExitCode,
                machineState,
                functionDefinitions,
                stdin,
                stdout.ToString(),
                stopwatch.Elapsed,
                totalOperations);
        }

        /// <summary>
        /// Loads the function definitions with the given executable and parameters
        /// </summary>
        /// <param name="operators">The sequence of parsed operators to execute</param>
        /// <param name="functions">The mapping of functions for the current execution</param>
        /// <param name="definitions">The lookup table to check which functions are defined</param>
        /// <param name="totalFunctions">The total number of defined functions</param>
        /// <returns>An array of <see cref="FunctionDefinition"/> instance with the defined functions</returns>
        [Pure]
        private static FunctionDefinition[] LoadFunctionDefinitions(
            UnsafeMemory<byte> operators,
            UnsafeMemory<Range> functions,
            UnsafeMemory<ushort> definitions,
            int totalFunctions)
        {
            DebugGuard.MustBeGreaterThanOrEqualTo(operators.Size, 0, nameof(operators));
            DebugGuard.MustBeEqualTo(functions.Size, ushort.MaxValue, nameof(functions));
            DebugGuard.MustBeEqualTo(definitions.Size, operators.Size, nameof(definitions));
            DebugGuard.MustBeGreaterThanOrEqualTo(totalFunctions, 0, nameof(totalFunctions));

            // No declared functions
            if (totalFunctions == 0) return Array.Empty<FunctionDefinition>();

            FunctionDefinition[] result = new FunctionDefinition[totalFunctions];
            ref FunctionDefinition r0 = ref result[0];

            // Process all the declared functions
            for (int i = 0, j = 0; j < totalFunctions; j++)
            {
                ushort key = definitions[j];

                if (key == 0) continue;

                // Extract the source for the current function
                Range range = functions[key];
                UnsafeMemory<byte> memory = operators.Slice(range.Start, range.End);
                string body = Brainf_ckParser.ExtractSource(memory);

                Unsafe.Add(ref r0, i) = new FunctionDefinition(key, i, j, body);
            }

            return result;
        }

        /// <summary>
        /// The maximum number of recursive calls that can be performed by a script
        /// </summary>
        public const int MaximumStackSize = 512;

        /// <summary>
        /// Loads the jump table for loops and functions from a given executable
        /// </summary>
        /// <param name="operators">The sequence of parsed operators to inspect</param>
        /// <returns>The resulting precomputed jump table for the input executable</returns>
        [Pure]
        private static UnsafeMemoryBuffer<int> LoadJumpTable(UnsafeMemoryBuffer<byte> operators)
        {
            DebugGuard.MustBeGreaterThanOrEqualTo(operators.Size, 0, nameof(operators));

            UnsafeMemoryBuffer<int> jumpTable = UnsafeMemoryBuffer<int>.Allocate(operators.Size, false);

            /* Temporarily allocate two buffers to store the indirect indices to build the jump table.
             * The two temporary buffers are initialized with a size of half the length of the input
             * executable, because that is the maximum number of open square brackets in a valid source file.
             * The two temporary buffers are used to implement an indirect indexing system while building
             * the table, which allows to reduce the complexity of the operation from O(N^2) to O(N) */
            int tempBuffersLength = operators.Size / 2 + 1;
            int[] rootTempIndices = ArrayPool<int>.Shared.Rent(tempBuffersLength);
            int[] functionTempIndices = ArrayPool<int>.Shared.Rent(tempBuffersLength);
            ref int rootTempIndicesRef = ref rootTempIndices[0];
            ref int functionTempIndicesRef = ref functionTempIndices[0];

            // Go through the executable to build the jump table for each open parenthesis or square bracket
            for (int r = 0, f = -1, i = 0; i < operators.Size; i++)
            {
                switch (operators[i])
                {
                    /* When a loop start, the current index is stored in the right
                     * temporary buffer, depending on whether or not the current
                     * part of the executable is within a function definition */
                    case Operators.LoopStart:
                        if (f == -1) Unsafe.Add(ref rootTempIndicesRef, r++) = i;
                        else Unsafe.Add(ref functionTempIndicesRef, f++) = i;
                        break;

                    /* When a loop ends, the index of the corresponding open
                     * square bracket is retrieved from the right temporary
                     * buffer, and the current index is stored at that location
                     * in the final jump table being built. The inverse mapping is
                     * stored too, so that each closing square bracket can reference the
                     * corresponding open bracket at the start of the loop. */
                    case Operators.LoopEnd:
                        int start = f == -1
                            ? Unsafe.Add(ref rootTempIndicesRef, r--)
                            : Unsafe.Add(ref functionTempIndicesRef, f--);
                        jumpTable[start] = i;
                        jumpTable[i] = start;
                        break;

                    /* When a function definition starts, the offset into the
                     * temporary buffer for the function indices is set to 1.
                     * This is because in this case a 1-based indexing is used:
                     *the first location in the temporary buffer is used to store
                     * the index of the open parenthesis for the function definition */
                    case Operators.FunctionStart:
                        f = 1;
                        functionTempIndicesRef = i;
                        break;
                    case Operators.FunctionEnd:
                        f = -1;
                        jumpTable[functionTempIndicesRef] = i;
                        break;
                }
            }

            /* ArrayPool<T> is used directly here instead of UnsafeMemoryBuffer<T> to save the cost of
             * allocating a GCHandle and pinning each temporary buffer, which is not necessary since
             * both buffers are only used in the scope of this method, and then disposed */
            ArrayPool<int>.Shared.Return(rootTempIndices);
            ArrayPool<int>.Shared.Return(functionTempIndices);

            return jumpTable;
        }

        /// <summary>
        /// Tries to run a given input Brainf*ck/PBrain executable
        /// </summary>
        /// <param name="operators">The sequence of parsed operators to execute</param>
        /// <param name="breakpoints">The table of breakpoints for the current executable</param>
        /// <param name="jumpTable">The jump table for loops and function declarations</param>
        /// <param name="functions">The mapping of functions for the current execution</param>
        /// <param name="definitions">The lookup table to check which functions are defined</param>
        /// <param name="state">The target <see cref="TuringMachineState"/> instance to execute the code on</param>
        /// <param name="stdin">The input buffer to read characters from</param>
        /// <param name="stdout">The output buffer to write characters to</param>
        /// <param name="stackFrames">The sequence of stack frames for the current execution</param>
        /// <param name="depth">The current stack depth</param>
        /// <param name="totalOperations">The total number of executed operators</param>
        /// <param name="totalFunctions">The total number of defined functions</param>
        /// <param name="executionToken">A <see cref="CancellationToken"/> that can be used to halt the execution</param>
        /// <param name="debugToken">A <see cref="CancellationToken"/> that is used to ignore/respect existing breakpoints</param>
        /// <returns>An <see cref="IEnumerator{T}"/> that produces <see cref="InterpreterWorkingData"/> instances for the execution results</returns>
        private static InterpreterWorkingData TryRun(
            UnsafeMemory<byte> operators,
            UnsafeMemory<bool> breakpoints,
            UnsafeMemory<int> jumpTable,
            UnsafeMemory<Range> functions,
            UnsafeMemory<ushort> definitions,
            TuringMachineState state,
            StdinBuffer stdin,
            StdoutBuffer stdout,
            UnsafeMemory<StackFrame> stackFrames,
            ref int depth,
            ref int totalOperations,
            ref int totalFunctions,
            CancellationToken executionToken,
            CancellationToken debugToken)
        {
            DebugGuard.MustBeTrue(operators.Size > 0, nameof(operators));
            DebugGuard.MustBeEqualTo(breakpoints.Size, operators.Size, nameof(breakpoints));
            DebugGuard.MustBeEqualTo(jumpTable.Size, operators.Size, nameof(jumpTable));
            DebugGuard.MustBeEqualTo(functions.Size, ushort.MaxValue, nameof(functions));
            DebugGuard.MustBeEqualTo(definitions.Size, operators.Size, nameof(definitions));
            DebugGuard.MustBeEqualTo(stackFrames.Size, MaximumStackSize, nameof(stackFrames));
            DebugGuard.MustBeGreaterThanOrEqualTo(depth, 0, nameof(depth));
            DebugGuard.MustBeGreaterThanOrEqualTo(totalOperations, 0, nameof(totalOperations));
            DebugGuard.MustBeGreaterThanOrEqualTo(totalFunctions, 0, nameof(totalFunctions));

            // Outer loop to go through the existing stack frames
            for (StackFrame frame = stackFrames[depth]; depth >= 0; depth--)
            {
                /* This label is used when a function call is performed: a new stack frame
                 * is pushed in the frames collection and then a goto is used to jump out
                 * of both the switch case and the inner loop. This is faster than using
                 * another variable to manually handle the two consecutive breaks to
                 * reach the start of the inner loop from a switch case. */
                StackFrameLoop:

                // Iterate over the current operators
                for (int i = frame.Offset; i < frame.Range.End; i++)
                {
                    // Check if a breakpoint has been reached
                    if (breakpoints[i] && !debugToken.IsCancellationRequested)
                    {
                        return new InterpreterWorkingData(ExitCode.BreakpointReached, operators.Slice(frame.Range.Start, i + 1), i, totalOperations);
                    }

                    // Execute the current operator
                    switch (operators[i])
                    {
                        // ptr++
                        case Operators.ForwardPtr:
                            if (state.TryMoveNext()) totalOperations++;
                            else
                            {
                                return new InterpreterWorkingData(ExitCode.UpperBoundExceeded, operators.Slice(frame.Range.Start, i + 1), i, totalOperations + 1);
                            }
                            break;

                        // ptr--
                        case Operators.BackwardPtr:
                            if (state.TryMoveBack()) totalOperations++;
                            else
                            {
                                return new InterpreterWorkingData(ExitCode.LowerBoundExceeded, operators.Slice(frame.Range.Start, i + 1), i, totalOperations + 1);
                            }
                            break;

                        // (*ptr)++
                        case Operators.Plus:
                            if (state.TryIncrement()) totalOperations++;
                            else
                            {
                                return new InterpreterWorkingData(ExitCode.MaxValueExceeded, operators.Slice(frame.Range.Start, i + 1), i, totalOperations + 1);
                            }
                            break;

                        // (*ptr)--
                        case Operators.Minus:
                            if (state.TryDecrement()) totalOperations++;
                            else
                            {
                                return new InterpreterWorkingData(ExitCode.NegativeValue, operators.Slice(frame.Range.Start, i + 1), i, totalOperations + 1);
                            }
                            break;

                        // putch(*ptr)
                        case Operators.PrintChar:
                            if (stdout.TryWrite((char)state.Current)) totalOperations++;
                            else
                            {
                                return new InterpreterWorkingData(ExitCode.StdoutBufferLimitExceeded, operators.Slice(frame.Range.Start, i + 1), i, totalOperations + 1);
                            }
                            break;

                        // *ptr = getch()
                        case Operators.ReadChar:
                            if (stdin.TryRead(out char c))
                            {
                                // Check if the input character can be stored in the current cell
                                if (state.TryInput(c)) totalOperations++;
                                else
                                {
                                    return new InterpreterWorkingData(ExitCode.MaxValueExceeded, operators.Slice(frame.Range.Start, i + 1), i, totalOperations + 1);
                                }
                            }
                            else
                            {
                                return new InterpreterWorkingData(ExitCode.StdinBufferExhausted, operators.Slice(frame.Range.Start, i + 1), i, totalOperations + 1);
                            }
                            break;

                        // while (*ptr) {
                        case Operators.LoopStart:

                            // Check whether the loop is active
                            if (state.Current == 0)
                            {
                                i = jumpTable[i];
                                totalOperations++;
                            }
                            else if (jumpTable[i] == i + 2 &&
                                     operators[i + 1] == Operators.Minus &&
                                     (!breakpoints[i + 1] &&
                                      !breakpoints[i + 2] ||
                                      debugToken.IsCancellationRequested))
                            {
                                // Fast path for [-] loops
                                state.ResetCell();
                                totalOperations += state.Current * 2 + 1;
                                i += 2;
                            }
                            else if (executionToken.IsCancellationRequested)
                            {
                                // Check whether the code can still be executed before starting an active loop
                                return new InterpreterWorkingData(ExitCode.ThresholdExceeded, operators.Slice(frame.Range.Start, i + 1), i, totalOperations);
                            }
                            break;

                        // {
                        case Operators.LoopEnd:
                            if (state.Current > 0) i = jumpTable[i] - 1;
                            totalOperations++;
                            break;

                        // f[*ptr] = []() {
                        case Operators.FunctionStart:
                        {
                            // Check for duplicate function definitions
                            if (functions[state.Current].Length != 0)
                            {
                                return new InterpreterWorkingData(ExitCode.DuplicateFunctionDefinition, operators.Slice(frame.Range.Start, i + 1), i, totalOperations + 1);
                            }

                            // Check that the current function has not been defined before
                            if (definitions[i] != 0)
                            {
                                return new InterpreterWorkingData(ExitCode.FunctionAlreadyDefined, operators.Slice(frame.Range.Start, i + 1), i, totalOperations + 1);
                            }

                            // Save the new function definition
                            Range function = new Range(i + 1, jumpTable[i]);
                            functions[state.Current] = function;
                            definitions[i] = state.Current;
                            totalFunctions++;
                            totalOperations++;
                            i += function.Length;
                            break;
                        }

                        // }
                        case Operators.FunctionEnd:
                            totalOperations++;
                            break;

                        // f[*ptr]()
                        case Operators.FunctionCall:
                        {
                            // Try to retrieve the function to invoke
                            Range function = functions[state.Current];
                            if (function.Length == 0)
                            {
                                return new InterpreterWorkingData(ExitCode.UndefinedFunctionCalled, operators.Slice(frame.Range.Start, i + 1), i, totalOperations + 1);
                            }

                            // Ensure the stack has space for the new function invocation
                            if (depth == MaximumStackSize - 1)
                            {
                                return new InterpreterWorkingData(ExitCode.StackLimitExceeded, operators.Slice(frame.Range.Start, i + 1), i, totalOperations + 1);
                            }

                            // Add the new stack fraame for the function call
                            stackFrames[++depth] = new StackFrame(function, i);
                            totalOperations++;
                            goto StackFrameLoop;
                        }
                    }
                }
            }

            // Return a new state for a successful execution
            bool hasOutput = stdout.Length > 0;
            ExitCode code = hasOutput ? ExitCode.TextOutput : ExitCode.NoOutput;
            return new InterpreterWorkingData(code, operators, operators.Size - 1, totalOperations);
        }
    }
}
