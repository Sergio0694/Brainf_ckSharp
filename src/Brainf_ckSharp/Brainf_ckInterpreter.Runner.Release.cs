using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Brainf_ckSharp.Buffers;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Extensions.Types;
using Brainf_ckSharp.Memory;
using Brainf_ckSharp.Memory.Interfaces;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Opcodes;
using StackFrame = Brainf_ckSharp.Models.Internal.StackFrame;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Brainf_ckSharp
{
    /// <summary>
    /// A <see langword="class"/> responsible for interpreting and debugging Brainf*ck/PBrain scripts
    /// </summary>
    public static partial class Brainf_ckInterpreter
    {
        /// <summary>
        /// A <see langword="class"/> implementing interpreter methods for the RELEASE configuration
        /// </summary>
        internal static class Release
        {
            /// <summary>
            /// Runs a given Brainf*ck/PBrain executable with the given parameters
            /// </summary>
            /// <param name="opcodes">The executable to run</param>
            /// <param name="stdin">The input buffer to read data from</param>
            /// <param name="machineState">The target machine state to use to run the script</param>
            /// <param name="executionToken">A <see cref="CancellationToken"/> that can be used to halt the execution</param>
            /// <returns>An <see cref="InterpreterResult"/> instance with the results of the execution</returns>
            public static InterpreterResult Run(
                PinnedUnmanagedMemoryOwner<Brainf_ckOperation> opcodes,
                string stdin,
                TuringMachineState machineState,
                CancellationToken executionToken)
            {
                DebugGuard.MustBeGreaterThanOrEqualTo(opcodes.Size, 0, nameof(opcodes));
                DebugGuard.MustBeGreaterThanOrEqualTo(machineState.Size, 0, nameof(machineState));

                return machineState.Mode switch
                {
                    OverflowMode.UshortWithNoOverflow => Run<TuringMachineState.UshortWithNoOverflowExecutionContext>(opcodes, stdin, machineState, executionToken),
                    OverflowMode.UshortWithOverflow => Run<TuringMachineState.UshortWithOverflowExecutionContext>(opcodes, stdin, machineState, executionToken),
                    OverflowMode.ByteWithNoOverflow => Run<TuringMachineState.ByteWithNoOverflowExecutionContext>(opcodes, stdin, machineState, executionToken),
                    OverflowMode.ByteWithOverflow => Run<TuringMachineState.ByteWithOverflowExecutionContext>(opcodes, stdin, machineState, executionToken),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            /// <summary>
            /// Runs a given Brainf*ck/PBrain executable with the given parameters
            /// </summary>
            /// <typeparam name="TExecutionContext">The type implementing <see cref="IMachineStateExecutionContext"/> to use</typeparam>
            /// <param name="opcodes">The executable to run</param>
            /// <param name="stdin">The input buffer to read data from</param>
            /// <param name="machineState">The target machine state to use to run the script</param>
            /// <param name="executionToken">A <see cref="CancellationToken"/> that can be used to halt the execution</param>
            /// <returns>An <see cref="InterpreterResult"/> instance with the results of the execution</returns>
            public static unsafe InterpreterResult Run<TExecutionContext>(
                PinnedUnmanagedMemoryOwner<Brainf_ckOperation> opcodes,
                string stdin,
                TuringMachineState machineState,
                CancellationToken executionToken)
                where TExecutionContext : struct, IMachineStateExecutionContext
            {
                DebugGuard.MustBeGreaterThanOrEqualTo(opcodes.Size, 0, nameof(opcodes));
                DebugGuard.MustBeGreaterThanOrEqualTo(machineState.Size, 0, nameof(machineState));

                /* Initialize the temporary buffers, using the StackOnlyUnmanagedMemoryOwner<T> type when possible
                 * to save the extra allocations here. This is possible because all these buffers are
                 * only used within the scope of this method, and disposed as soon as the method completes.
                 * Additionally, when this type is used, memory is pinned using a fixed statement instead
                 * of the GCHandle, which has slightly less overhead for the runtime. */
                using PinnedUnmanagedMemoryOwner<int> jumpTable = LoadJumpTable(opcodes, out int functionsCount);
                using StackOnlyUnmanagedMemoryOwner<Range> functions = StackOnlyUnmanagedMemoryOwner<Range>.Allocate(ushort.MaxValue, true);
                using PinnedUnmanagedMemoryOwner<ushort> definitions = LoadDefinitionsTable(functionsCount);
                using StackOnlyUnmanagedMemoryOwner<StackFrame> stackFrames = StackOnlyUnmanagedMemoryOwner<StackFrame>.Allocate(Specs.MaximumStackSize, false);
                using StdoutBuffer stdout = new StdoutBuffer();

                fixed (Range* functionsPtr = &functions.GetReference())
                fixed (StackFrame* stackFramesPtr = &stackFrames.GetReference())
                {
                    // Shared counters
                    int
                        depth = 0,
                        totalOperations = 0,
                        totalFunctions = 0;
                    ExitCode exitCode;
                    TimeSpan elapsed;

                    // Manually set the initial stack frame to the entire script
                    stackFramesPtr[0] = new StackFrame(new Range(0, opcodes.Size), 0);

                    // Wrap the stdin buffer
                    StdinBuffer stdinBuffer = new StdinBuffer(stdin);

                    // Create the execution session
                    using (TuringMachineState.ExecutionSession<TExecutionContext> session = machineState.CreateExecutionSession<TExecutionContext>())
                    {
                        Stopwatch stopwatch = Stopwatch.StartNew();

                        // Start the interpreter
                        exitCode = Run(
                            ref Unsafe.AsRef(session.ExecutionContext),
                            opcodes.Span,
                            jumpTable.Span,
                            new UnmanagedSpan<Range>(ushort.MaxValue, functionsPtr),
                            definitions.Span,
                            new UnmanagedSpan<StackFrame>(Specs.MaximumStackSize, stackFramesPtr),
                            ref depth,
                            ref totalOperations,
                            ref totalFunctions,
                            ref stdinBuffer,
                            stdout,
                            executionToken);

                        stopwatch.Stop();
                        elapsed = stopwatch.Elapsed;
                    }

                    // Rebuild the compacted source code
                    string sourceCode = Brainf_ckParser.ExtractSource(opcodes.CoreCLRReadOnlySpan);

                    // Prepare the debug info
                    HaltedExecutionInfo? debugInfo = LoadDebugInfo(
                        opcodes.CoreCLRReadOnlySpan,
                        new UnmanagedSpan<StackFrame>(Specs.MaximumStackSize, stackFramesPtr),
                        depth);

                    // Build the collection of defined functions
                    FunctionDefinition[] functionDefinitions = LoadFunctionDefinitions(
                        opcodes.CoreCLRReadOnlySpan,
                        new UnmanagedSpan<Range>(ushort.MaxValue, functionsPtr),
                        definitions.Span,
                        totalFunctions);

                    return new InterpreterResult(
                        sourceCode,
                        exitCode,
                        debugInfo,
                        machineState,
                        functionDefinitions,
                        stdin,
                        stdout.ToString(),
                        elapsed,
                        totalOperations);
                }
            }

            /// <summary>
            /// Tries to run a given input Brainf*ck/PBrain executable
            /// </summary>
            /// <typeparam name="TExecutionContext">The type implementing <see cref="IMachineStateExecutionContext"/> to use</typeparam>
            /// <param name="executionContext">The target <typeparamref name="TExecutionContext"/>/> instance to execute the code on</param>
            /// <param name="opcodes">The sequence of parsed opcodes to execute</param>
            /// <param name="jumpTable">The jump table for loops and function declarations</param>
            /// <param name="functions">The mapping of functions for the current execution</param>
            /// <param name="definitions">The lookup table to check which functions are defined</param>
            /// <param name="stackFrames">The sequence of stack frames for the current execution</param>
            /// <param name="depth">The current stack depth</param>
            /// <param name="totalOperations">The total number of executed opcodes</param>
            /// <param name="totalFunctions">The total number of defined functions</param>
            /// <param name="stdin">The input buffer to read characters from</param>
            /// <param name="stdout">The output buffer to write characters to</param>
            /// <param name="executionToken">A <see cref="CancellationToken"/> that can be used to halt the execution</param>
            /// <returns>The resulting <see cref="ExitCode"/> value for the current execution of the input script</returns>
            /// <remarks>This method mirrors the one from the <see cref="Debug"/> class, but with more optimizations</remarks>
            public static ExitCode Run<TExecutionContext>(
                ref TExecutionContext executionContext,
                UnmanagedSpan<Brainf_ckOperation> opcodes,
                UnmanagedSpan<int> jumpTable,
                UnmanagedSpan<Range> functions,
                UnmanagedSpan<ushort> definitions,
                UnmanagedSpan<StackFrame> stackFrames,
                ref int depth,
                ref int totalOperations,
                ref int totalFunctions,
                ref StdinBuffer stdin,
                StdoutBuffer stdout,
                CancellationToken executionToken)
                where TExecutionContext : struct, IMachineStateExecutionContext
            {
                DebugGuard.MustBeTrue(opcodes.Size > 0, nameof(opcodes));
                DebugGuard.MustBeEqualTo(jumpTable.Size, opcodes.Size, nameof(jumpTable));
                DebugGuard.MustBeEqualTo(functions.Size, ushort.MaxValue, nameof(functions));
                DebugGuard.MustBeGreaterThanOrEqualTo(definitions.Size, 0, nameof(definitions));
                DebugGuard.MustBeLessThanOrEqualTo(definitions.Size, opcodes.Size / 3, nameof(definitions));
                DebugGuard.MustBeEqualTo(stackFrames.Size, Specs.MaximumStackSize, nameof(stackFrames));
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

                    // Iterate over the current opcodes
                    for (i = frame.Offset; i < frame.Range.End; i++)
                    {
                        Brainf_ckOperation opcode = opcodes[i];

                        // Execute the current operator
                        switch (opcode.Operator)
                        {
                            // ptr++
                            case Operators.ForwardPtr:
                                if (!executionContext.TryMoveNext(opcode.Count, ref totalOperations))
                                    goto UpperBoundExceeded;
                                break;

                            // ptr--
                            case Operators.BackwardPtr:
                                if (!executionContext.TryMoveBack(opcode.Count, ref totalOperations)) 
                                    goto LowerBoundExceeded;
                                break;

                            // (*ptr)++
                            case Operators.Plus:
                                if (!executionContext.TryIncrement(opcode.Count, ref totalOperations))
                                    goto MaxValueExceeded;
                                break;

                            // (*ptr)--
                            case Operators.Minus:
                                if (!executionContext.TryDecrement(opcode.Count, ref totalOperations))
                                    goto NegativeValue;
                                break;

                            // putch(*ptr)
                            case Operators.PrintChar:
                                if (stdout.TryWrite((char)executionContext.Current)) totalOperations++;
                                else goto StdoutBufferLimitExceeded;
                                break;

                            // *ptr = getch()
                            case Operators.ReadChar:
                                if (stdin.TryRead(out char c))
                                {
                                    // Check if the input character can be stored in the current cell
                                    if (executionContext.TryInput(c)) totalOperations++;
                                    else goto MaxValueExceeded;
                                }
                                else goto StdinBufferExhausted;
                                break;

                            // while (*ptr) {
                            case Operators.LoopStart:

                                // Check whether the loop is active
                                if (executionContext.Current == 0)
                                {
                                    i = jumpTable[i];
                                    totalOperations++;
                                }
                                else if (jumpTable[i] == i + 2 &&
                                         opcodes[i + 1].Operator == Operators.Minus)
                                {
                                    // Fast path for [-] loops
                                    executionContext.ResetCell();
                                    totalOperations += executionContext.Current * 2 + 1;
                                    i += 2;
                                }
                                break;

                            // {
                            case Operators.LoopEnd:

                                // Check whether the loop is still active and can be repeated
                                if (executionContext.Current > 0)
                                {
                                    i = jumpTable[i];

                                    // Check whether the code can still be executed before starting an active loop
                                    if (executionToken.IsCancellationRequested) goto ThresholdExceeded;
                                }
                                totalOperations++;
                                break;

                            // f[*ptr] = []() {
                            case Operators.FunctionStart:
                            {
                                // Check for duplicate function definitions
                                if (functions[executionContext.Current].Length != 0) goto DuplicateFunctionDefinition;

                                // Save the new function definition
                                Range function = new Range(i + 1, jumpTable[i]);
                                functions[executionContext.Current] = function;
                                definitions[totalFunctions++] = executionContext.Current;
                                totalOperations++;
                                i += function.Length;
                                break;
                            }

                            // f[*ptr]()
                            case Operators.FunctionCall:
                            {
                                // Try to retrieve the function to invoke
                                Range function = functions[executionContext.Current];
                                if (function.Length == 0) goto UndefinedFunctionCalled;

                                // Ensure the stack has space for the new function invocation
                                if (depth == Specs.MaximumStackSize - 1) goto StackLimitExceeded;

                                // Check for remaining time
                                if (executionToken.IsCancellationRequested) goto ThresholdExceeded;

                                // Update the current stack frame and exit the inner loop
                                stackFrames[depth++] = frame.WithOffset(i + 1);
                                frame = new StackFrame(function);
                                totalOperations++;
                                goto StackFrameLoop;
                            }
                        }
                    }
                } while (--depth >= 0);

                return ExitCode.Success;

                // Same exit handling of the DEBUG configuration, minus breakpoints
                UpperBoundExceeded:
                ExitCode exitCode = ExitCode.UpperBoundExceeded;
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
}
