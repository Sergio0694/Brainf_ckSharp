using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Brainf_ckSharp.Buffers;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Memory;
using Brainf_ckSharp.Memory.Interfaces;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Internal;
using Brainf_ckSharp.Opcodes;
using Microsoft.Toolkit.HighPerformance.Buffers;
using Microsoft.Toolkit.HighPerformance.Extensions;
using StackFrame = Brainf_ckSharp.Models.Internal.StackFrame;

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
        internal static partial class Release
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
                Span<Brainf_ckOperation> opcodes,
                string stdin,
                TuringMachineState machineState,
                CancellationToken executionToken)
            {
                System.Diagnostics.Debug.Assert(opcodes.Length >= 0);
                System.Diagnostics.Debug.Assert(machineState.Size >= 0);

                return machineState.Mode switch
                {
                    OverflowMode.ByteWithOverflow => Run<TuringMachineState.ByteWithOverflowExecutionContext>(opcodes, stdin, machineState, executionToken),
                    OverflowMode.ByteWithNoOverflow => Run<TuringMachineState.ByteWithNoOverflowExecutionContext>(opcodes, stdin, machineState, executionToken),
                    OverflowMode.UshortWithOverflow => Run<TuringMachineState.UshortWithOverflowExecutionContext>(opcodes, stdin, machineState, executionToken),
                    OverflowMode.UshortWithNoOverflow => Run<TuringMachineState.UshortWithNoOverflowExecutionContext>(opcodes, stdin, machineState, executionToken),
                    _ => ThrowArgumentOutOfRangeForOverflowMode(machineState)
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
            private static InterpreterResult Run<TExecutionContext>(
                Span<Brainf_ckOperation> opcodes,
                string stdin,
                TuringMachineState machineState,
                CancellationToken executionToken)
                where TExecutionContext : struct, IMachineStateExecutionContext
            {
                // Initialize the temporary buffers, using the SpanOwner<T> to save the extra allocations.
                // This is possible because all these buffers are only used within the scope of this method.
                using SpanOwner<int> jumpTable = LoadJumpTable(opcodes, out int functionsCount);
                using SpanOwner<Range> functions = SpanOwner<Range>.Allocate(ushort.MaxValue, AllocationMode.Clear);
                using SpanOwner<ushort> definitions = LoadDefinitionsTable(functionsCount);
                using SpanOwner<StackFrame> stackFrames = SpanOwner<StackFrame>.Allocate(Specs.MaximumStackSize);
                using StdoutBuffer stdout = StdoutBuffer.Allocate();

                // Shared counters
                int
                    depth = 0,
                    totalOperations = 0,
                    totalFunctions = 0;
                ExitCode exitCode;
                TimeSpan elapsed;

                // Manually set the initial stack frame to the entire script
                stackFrames.DangerousGetReference() = new StackFrame(new Range(0, opcodes.Length), 0);

                // Wrap the stdin buffer
                StdinBuffer stdinBuffer = new StdinBuffer(stdin);

                // Create the execution session
                using (TuringMachineState.ExecutionSession<TExecutionContext> session = machineState.CreateExecutionSession<TExecutionContext>())
                {
                    Timestamp timestamp = Timestamp.Now;

                    // Start the interpreter
                    exitCode = Run(
                        ref Unsafe.AsRef(session.ExecutionContext),
                        ref opcodes.DangerousGetReference(),
                        ref jumpTable.DangerousGetReference(),
                        ref functions.DangerousGetReference(),
                        ref definitions.DangerousGetReference(),
                        ref stackFrames.DangerousGetReference(),
                        ref depth,
                        ref totalOperations,
                        ref totalFunctions,
                        ref stdinBuffer,
                        ref Unsafe.AsRef(stdout),
                        executionToken);

                    elapsed = TimeSpan.FromTicks(timestamp.Ticks);
                }

                // Rebuild the compacted source code
                string sourceCode = Brainf_ckParser.ExtractSource(opcodes);

                // Prepare the debug info
                HaltedExecutionInfo? debugInfo = LoadDebugInfo(
                    opcodes,
                    stackFrames.Span,
                    depth);

                // Build the collection of defined functions
                FunctionDefinition[] functionDefinitions = LoadFunctionDefinitions(
                    opcodes,
                    functions.Span,
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
            private static ExitCode Run<TExecutionContext>(
                ref TExecutionContext executionContext,
                ref Brainf_ckOperation opcodes,
                ref int jumpTable,
                ref Range functions,
                ref ushort definitions,
                ref StackFrame stackFrames,
                ref int depth,
                ref int totalOperations,
                ref int totalFunctions,
                ref StdinBuffer stdin,
                ref StdoutBuffer stdout,
                CancellationToken executionToken)
                where TExecutionContext : struct, IMachineStateExecutionContext
            {
                System.Diagnostics.Debug.Assert(depth >= 0);
                System.Diagnostics.Debug.Assert(totalOperations >= 0);
                System.Diagnostics.Debug.Assert(totalFunctions >= 0);

                // Outer loop to go through the existing stack frames
                StackFrame frame;
                int i;
                do
                {
                    frame = Unsafe.Add(ref stackFrames, depth);

                    // This label is used when a function call is performed: a new stack frame
                    // is pushed in the frames collection and then a goto is used to jump out
                    // of both the switch case and the inner loop. This is faster than using
                    // another variable to manually handle the two consecutive breaks to
                    // reach the start of the inner loop from a switch case.
                    StackFrameLoop:

                    // Iterate over the current opcodes
                    for (i = frame.Offset; i < frame.Range.End; i++)
                    {
                        Brainf_ckOperation opcode = Unsafe.Add(ref opcodes, i);

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
                                    i = Unsafe.Add(ref jumpTable, i);
                                    totalOperations++;
                                }
                                else if (Unsafe.Add(ref jumpTable, i) == i + 2 &&
                                         Unsafe.Add(ref opcodes, i + 1).Operator == Operators.Minus)
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
                                    i = Unsafe.Add(ref jumpTable, i);

                                    // Check whether the code can still be executed before starting an active loop
                                    if (executionToken.IsCancellationRequested) goto ThresholdExceeded;
                                }
                                totalOperations++;
                                break;

                            // f[*ptr] = []() {
                            case Operators.FunctionStart:
                            {
                                // Check for duplicate function definitions
                                if (Unsafe.Add(ref functions, executionContext.Current).Length != 0) goto DuplicateFunctionDefinition;

                                // Save the new function definition
                                Range function = new Range(i + 1, Unsafe.Add(ref jumpTable, i));
                                Unsafe.Add(ref functions, executionContext.Current) = function;
                                Unsafe.Add(ref definitions, totalFunctions++) = executionContext.Current;
                                totalOperations++;
                                i += function.Length;
                                break;
                            }

                            // f[*ptr]()
                            case Operators.FunctionCall:
                            {
                                // Try to retrieve the function to invoke
                                Range function = Unsafe.Add(ref functions, executionContext.Current);
                                if (function.Length == 0) goto UndefinedFunctionCalled;

                                // Ensure the stack has space for the new function invocation
                                if (depth == Specs.MaximumStackSize - 1) goto StackLimitExceeded;

                                // Check for remaining time
                                if (executionToken.IsCancellationRequested) goto ThresholdExceeded;

                                // Update the current stack frame and exit the inner loop
                                Unsafe.Add(ref stackFrames, depth++) = frame.WithOffset(i + 1);
                                frame = new StackFrame(function);
                                totalOperations++;
                                goto StackFrameLoop;
                            }
                        }
                    }
                } while (--depth >= 0);

                // We still use a goto in the success path to be able to reuse
                // the same epilogue in the generated code for all exit conditions.
                // This is because this method needs to push quite a few registers to
                // the stack, so having a single exit point helps to reduce the code size.
                ExitCode exitCode = ExitCode.Success;
                goto Exit;

                // Exit paths for all failures or partial executions in the interpreter.
                // Whenever an executable completes its execution and the current stack
                // frame needs to be updated with the current position, it is done from
                // one of these labels: each of them sets the right exit flag and then
                // jumps to the exit label, which updates the current stack frame and
                // returns. Having all these exit paths here makes the code more compact
                // into the inner loop, and the two jumps don't produce overhead since
                // one of them would only be triggered when the inner loop has terminated.
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

                UndefinedFunctionCalled:
                exitCode = ExitCode.UndefinedFunctionCalled;
                goto UpdateStackFrameAndExit;

                StackLimitExceeded:
                exitCode = ExitCode.StackLimitExceeded;

                UpdateStackFrameAndExit:
                Unsafe.Add(ref stackFrames, depth) = frame.WithOffset(i);

                Exit:
                return exitCode;
            }

            /// <summary>
            /// Throws an <see cref="ArgumentOutOfRangeException"/> when the current <see cref="OverflowMode"/> setting is invalid
            /// </summary>
            [MethodImpl(MethodImplOptions.NoInlining)]
            private static InterpreterResult ThrowArgumentOutOfRangeForOverflowMode(TuringMachineState machineState)
            {
                throw new ArgumentOutOfRangeException(nameof(TuringMachineState.Mode), $"Invalid execution mode: {machineState.Mode}");
            }
        }
    }
}
