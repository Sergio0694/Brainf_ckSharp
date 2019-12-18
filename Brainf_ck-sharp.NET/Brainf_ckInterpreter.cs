using System;
using System.Buffers;
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
using Brainf_ck_sharp.NET.Models.Base;
using Brainf_ck_sharp.NET.Models.Internal;
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
        private const int DefaultMemorySize = 128;

        /// <summary>
        /// The default overflow mode for running scripts
        /// </summary>
        private const OverflowMode DefaultOverflowMode = OverflowMode.ByteWithNoOverflow;

        /// <summary>
        /// The maximum number of recursive calls that can be performed by a script
        /// </summary>
        private const int MaximumStackSize = 512;

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        public static Option<InterpreterResult> TryRun(string source)
        {
            return TryRun(source, string.Empty, DefaultMemorySize, DefaultOverflowMode);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        public static Option<InterpreterResult> TryRun(string source, string stdin)
        {
            return TryRun(source, stdin, DefaultMemorySize, DefaultOverflowMode);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="memorySize">The size of the state machine to create to run the script</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        public static Option<InterpreterResult> TryRun(string source, string stdin, int memorySize)
        {
            return TryRun(source, stdin, memorySize, DefaultOverflowMode);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="memorySize">The size of the state machine to create to run the script</param>
        /// <param name="overflowMode">The overflow mode to use in the state machine used to run the script</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        public static Option<InterpreterResult> TryRun(string source, string stdin, int memorySize, OverflowMode overflowMode)
        {
            Guard.MustBeGreaterThanOrEqualTo(memorySize, 32, nameof(memorySize));
            Guard.MustBeLessThanOrEqualTo(memorySize, 1024, nameof(memorySize));

            using UnsafeMemoryBuffer<byte>? operators = Brainf_ckParser.TryParse(source, out SyntaxValidationResult validationResult);

            if (!validationResult.IsSuccess) return Option<InterpreterResult>.From(validationResult);

            TuringMachineState machineState = new TuringMachineState(memorySize, overflowMode);
            InterpreterResult result = Run(operators!, stdin, machineState);

            return Option<InterpreterResult>.From(validationResult, result);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="initialState">The initial state machine to use to start running the script from</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        public static Option<InterpreterResult> TryRun(string source, IReadOnlyTuringMachineState initialState)
        {
            return TryRun(source, string.Empty, initialState);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="initialState">The initial state machine to use to start running the script from</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        public static Option<InterpreterResult> TryRun(string source, string stdin, IReadOnlyTuringMachineState initialState)
        {
            DebugGuard.MustBeTrue(initialState is TuringMachineState, nameof(initialState));

            using UnsafeMemoryBuffer<byte>? operators = Brainf_ckParser.TryParse(source, out SyntaxValidationResult validationResult);

            if (!validationResult.IsSuccess) return Option<InterpreterResult>.From(validationResult);

            TuringMachineState machineState = (TuringMachineState)initialState.Clone();
            InterpreterResult result = Run(operators!, stdin, machineState);

            return Option<InterpreterResult>.From(validationResult, result);
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
            ExitCode exitCode = Run(
                operators.Memory,
                breakpoints.Memory,
                jumpTable.Memory,
                functions.Memory,
                definitions.Memory,
                machineState,
                new StdinBuffer(stdin),
                stdout,
                stackFrames.Memory,
                ref depth,
                ref totalOperations,
                ref totalFunctions,
                CancellationToken.None,
                CancellationToken.None);

            stopwatch.Stop();

            // Rebuild the compacted source code
            string sourceCode = Brainf_ckParser.ExtractSource(operators.Memory);

            // Prepare the stack frames
            string[] stackTrace = LoadStackTrace(
                operators.Memory,
                stackFrames.Memory,
                depth);

            // Build the collection of defined functions
            FunctionDefinition[] functionDefinitions = LoadFunctionDefinitions(
                operators.Memory,
                functions.Memory,
                definitions.Memory,
                totalFunctions);

            return new InterpreterResult(
                sourceCode,
                exitCode,
                stackTrace,
                machineState,
                functionDefinitions,
                stdin,
                stdout.ToString(),
                stopwatch.Elapsed,
                totalOperations);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="breakpoints">The sequence of indices for the breakpoints to apply to the script</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterSession"/> instance with the results of the execution</returns>
        public static Option<InterpreterSession> TryCreateSession(string source, ReadOnlySpan<int> breakpoints)
        {
            return TryCreateSession(source, breakpoints, string.Empty, DefaultMemorySize, DefaultOverflowMode);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="breakpoints">The sequence of indices for the breakpoints to apply to the script</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterSession"/> instance with the results of the execution</returns>
        public static Option<InterpreterSession> TryCreateSession(string source, ReadOnlySpan<int> breakpoints, string stdin)
        {
            return TryCreateSession(source, breakpoints, stdin, DefaultMemorySize, DefaultOverflowMode);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="breakpoints">The sequence of indices for the breakpoints to apply to the script</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="memorySize">The size of the state machine to create to run the script</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterSession"/> instance with the results of the execution</returns>
        public static Option<InterpreterSession> TryCreateSession(string source, ReadOnlySpan<int> breakpoints, string stdin, int memorySize)
        {
            return TryCreateSession(source, breakpoints, stdin, memorySize, DefaultOverflowMode);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="breakpoints">The sequence of indices for the breakpoints to apply to the script</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="memorySize">The size of the state machine to create to run the script</param>
        /// <param name="overflowMode">The overflow mode to use in the state machine used to run the script</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterSession"/> instance with the results of the execution</returns>
        public static Option<InterpreterSession> TryCreateSession(string source, ReadOnlySpan<int> breakpoints, string stdin, int memorySize, OverflowMode overflowMode)
        {
            Guard.MustBeGreaterThanOrEqualTo(memorySize, 32, nameof(memorySize));
            Guard.MustBeLessThanOrEqualTo(memorySize, 1024, nameof(memorySize));

            UnsafeMemoryBuffer<byte>? operators = Brainf_ckParser.TryParse(source, out SyntaxValidationResult validationResult);

            if (!validationResult.IsSuccess) return Option<InterpreterSession>.From(validationResult);

            // Initialize the temporary buffers
            UnsafeMemoryBuffer<bool> breakpointsTable = LoadBreakpointsTable(source, validationResult.OperatorsCount, breakpoints);
            UnsafeMemoryBuffer<int> jumpTable = LoadJumpTable(operators!);
            UnsafeMemoryBuffer<Range> functions = UnsafeMemoryBuffer<Range>.Allocate(ushort.MaxValue, true);
            UnsafeMemoryBuffer<ushort> definitions = UnsafeMemoryBuffer<ushort>.Allocate(operators!.Size, true);
            UnsafeMemoryBuffer<StackFrame> stackFrames = UnsafeMemoryBuffer<StackFrame>.Allocate(MaximumStackSize, false);

            // Create the interpreter session
            InterpreterSession session = new InterpreterSession(
                operators,
                breakpointsTable,
                jumpTable,
                functions,
                definitions,
                stackFrames,
                stdin,
                memorySize,
                overflowMode,
                CancellationToken.None,
                CancellationToken.None);

            return Option<InterpreterSession>.From(validationResult, session);
        }

        /// <summary>
        /// Loads the current stack trace for a halted execution of a script
        /// </summary>
        /// <param name="operators">The sequence of parsed operators to execute</param>
        /// <param name="stackFrames">The sequence of stack frames for the current execution</param>
        /// <param name="depth">The current stack depth</param>
        /// <returns>An array of <see cref="string"/> instances representing each stack frame, in reverse order</returns>
        [Pure]
        internal static string[] LoadStackTrace(
            UnsafeMemory<byte> operators,
            UnsafeMemory<StackFrame> stackFrames,
            int depth)
        {
            DebugGuard.MustBeTrue(operators.Size > 0, nameof(operators));
            DebugGuard.MustBeEqualTo(stackFrames.Size, MaximumStackSize, nameof(stackFrames));
            DebugGuard.MustBeGreaterThanOrEqualTo(depth, -1, nameof(depth));

            // No stack trace for scripts completed successfully
            if (depth == -1) return Array.Empty<string>();

            int count = depth + 1;
            string[] result = new string[count];
            ref string r0 = ref result[0];

            // Process all the declared functions
            for (int i = 0, j = count - 1; j >= 0; i++, j--)
            {
                StackFrame frame = stackFrames[j];
                UnsafeMemory<byte> memory = operators.Slice(frame.Range.Start, frame.Offset);
                string body = Brainf_ckParser.ExtractSource(memory);

                Unsafe.Add(ref r0, i) = body;
            }

            return result;
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
        internal static FunctionDefinition[] LoadFunctionDefinitions(
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
                            ? Unsafe.Add(ref rootTempIndicesRef, --r)
                            : Unsafe.Add(ref functionTempIndicesRef, --f);
                        jumpTable[start] = i;
                        jumpTable[i] = start;
                        break;

                    /* When a function definition starts, the offset into the
                     * temporary buffer for the function indices is set to 1.
                     * This is because in this case a 1-based indexing is used:
                     * the first location in the temporary buffer is used to store
                     * the index of the open parenthesis for the function definition. */
                    case Operators.FunctionStart:
                        f = 1;
                        functionTempIndicesRef = i;
                        break;
                    case Operators.FunctionEnd:
                        f = -1;
                        jumpTable[functionTempIndicesRef] = i;
                        jumpTable[i] = functionTempIndicesRef;
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
        /// Loads the breakpoints table for a given source code and collection of breakpoints
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="operatorsCount">The precomputed number of operators in the input source code</param>
        /// <param name="breakpoints">The sequence of indices for the breakpoints to apply to the script</param>
        /// <returns>The resulting precomputed breakpoints table for the input executable</returns>
        [Pure]
        private static UnsafeMemoryBuffer<bool> LoadBreakpointsTable(string source, int operatorsCount, ReadOnlySpan<int> breakpoints)
        {
            /* This temporary buffer is used to build a quick lookup table for the
             * valid indices from the input breakpoints collection. This table is
             * built in O(M), and then provides constant time checking for each
             * character from the input script. The result is an algorithm that
             * builds the final breakpoints table in O(M + N) instead of O(M * N). */
            using UnsafeMemoryBuffer<bool> temporaryBuffer = UnsafeMemoryBuffer<bool>.Allocate(source.Length, true);

            // Build the temporary table to store the indirect offsets of the breakpoints
            for (int i = 0; i < breakpoints.Length; i++)
            {
                int index = breakpoints[i];

                Guard.MustBeGreaterThan(index, 0, nameof(breakpoints));
                Guard.MustBeLessThan(index, source.Length, nameof(breakpoints));

                temporaryBuffer[index] = true;
            }

            UnsafeMemoryBuffer<bool> breakpointsBuffer = UnsafeMemoryBuffer<bool>.Allocate(operatorsCount, false);

            // Build the breakpoints table by going through the temporary table with the markers
            for (int i = 0, j = 0; j < source.Length; j++)
            {
                if (!Brainf_ckParser.IsOperator(source[i])) continue;

                breakpointsBuffer[i++] = temporaryBuffer[j];
            }

            return breakpointsBuffer;
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
        /// <returns>The resulting <see cref="ExitCode"/> value for the current execution of the input script</returns>
        internal static ExitCode Run(
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
            StackFrame frame;
            int i;
            do
            {
                frame = stackFrames[depth];

                /* This label is used when a function call is performed: a new stack frame
                 * is pushed in the frames collection and then a goto is used to jump out
                 * of both the switch case and the inner loop. This is faster than using
                 * another variable to manually handle the two consecutive breaks to
                 * reach the start of the inner loop from a switch case. */
                StackFrameLoop:

                // Iterate over the current operators
                for (i = frame.Offset; i < frame.Range.End; i++)
                {
                    // Check if a breakpoint has been reached
                    if (breakpoints[i] && !debugToken.IsCancellationRequested)
                    {
                        /* Disable the current breakpoint so that it won't be
                         * triggered again when the execution resumes from this point */
                        breakpoints[i] = false;
                        goto BreakpointReached;
                    }

                    // Execute the current operator
                    switch (operators[i])
                    {
                        // ptr++
                        case Operators.ForwardPtr:
                            if (state.TryMoveNext()) totalOperations++;
                            else goto UpperBoundExceeded;
                            break;

                        // ptr--
                        case Operators.BackwardPtr:
                            if (state.TryMoveBack()) totalOperations++;
                            else goto LowerBoundExceeded;
                            break;

                        // (*ptr)++
                        case Operators.Plus:
                            if (state.TryIncrement()) totalOperations++;
                            else goto MaxValueExceeded;
                            break;

                        // (*ptr)--
                        case Operators.Minus:
                            if (state.TryDecrement()) totalOperations++;
                            else goto NegativeValue;
                            break;

                        // putch(*ptr)
                        case Operators.PrintChar:
                            if (stdout.TryWrite((char)state.Current)) totalOperations++;
                            else goto StdoutBufferLimitExceeded;
                            break;

                        // *ptr = getch()
                        case Operators.ReadChar:
                            if (stdin.TryRead(out char c))
                            {
                                // Check if the input character can be stored in the current cell
                                if (state.TryInput(c)) totalOperations++;
                                else goto MaxValueExceeded;
                            }
                            else goto StdinBufferExhausted;
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
                                goto ThresholdExceeded;
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
                            if (functions[state.Current].Length != 0) goto DuplicateFunctionDefinition;

                            // Check that the current function has not been defined before
                            if (definitions[i] != 0) goto FunctionAlreadyDefined;

                            // Save the new function definition
                            Range function = new Range(i + 1, jumpTable[i]);
                            functions[state.Current] = function;
                            definitions[i] = state.Current;
                            totalFunctions++;
                            totalOperations++;
                            i += function.Length;
                            break;
                        }

                        // f[*ptr]()
                        case Operators.FunctionCall:
                        {
                            // Try to retrieve the function to invoke
                            Range function = functions[state.Current];
                            if (function.Length == 0) goto UndefinedFunctionCalled;

                            // Ensure the stack has space for the new function invocation
                            if (depth == MaximumStackSize - 1) goto StackLimitExceeded;

                            // Update the current stack frame and exit the inner loop
                            stackFrames[depth++] = frame.WithOffset(i + 1);
                            frame = new StackFrame(function);
                            totalOperations++;
                            goto StackFrameLoop;
                        }
                    }
                }
            } while (--depth >= 0);

            return stdout.IsEmpty ? ExitCode.NoOutput : ExitCode.TextOutput;

            /* Exit paths for all failures or partial executions in the interpreter.
             * Whenever an executable completes its execution and the current stack
             * frame needs to be updated with the current position, it is done from
             * one of these labels: each of them sets the right exit flag and then
             * jumps to the exit label, which updates the current stack frame and
             * returns. Having all these exit paths here makes the code more compact
             * into the inner loop, and the two jumps don't produce overhead since
             * one of them would only be triggered when the inner loop has terminated. */
            BreakpointReached:
            ExitCode exitCode = ExitCode.BreakpointReached;
            goto UpdateStackFrameAndExit;

            UpperBoundExceeded:
            exitCode = ExitCode.UpperBoundExceeded;
            goto UpdateStackFrameAndExit;

            LowerBoundExceeded:
            exitCode = ExitCode.LowerBoundExceeded;
            goto UpdateStackFrameAndExit;

            MaxValueExceeded:
            exitCode = ExitCode.MaxValueExceeded;
            goto UpdateStackFrameAndExit;

            NegativeValue:
            exitCode = ExitCode.NegativeValue;
            goto UpdateStackFrameAndExit;

            StdoutBufferLimitExceeded:
            exitCode = ExitCode.StdoutBufferLimitExceeded;
            goto UpdateStackFrameAndExit;

            StdinBufferExhausted:
            exitCode = ExitCode.StdinBufferExhausted;
            goto UpdateStackFrameAndExit;

            ThresholdExceeded:
            exitCode = ExitCode.ThresholdExceeded;
            goto UpdateStackFrameAndExit;

            DuplicateFunctionDefinition:
            exitCode = ExitCode.DuplicateFunctionDefinition;
            goto UpdateStackFrameAndExit;

            FunctionAlreadyDefined:
            exitCode = ExitCode.FunctionAlreadyDefined;
            goto UpdateStackFrameAndExit;

            UndefinedFunctionCalled:
            exitCode = ExitCode.UndefinedFunctionCalled;
            goto UpdateStackFrameAndExit;

            StackLimitExceeded:
            exitCode = ExitCode.StackLimitExceeded;

            UpdateStackFrameAndExit:
            stackFrames[depth] = frame.WithOffset(i);
            return exitCode;
        }
    }
}
