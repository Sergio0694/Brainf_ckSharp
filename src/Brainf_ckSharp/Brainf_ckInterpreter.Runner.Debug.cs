using System;
using System.Diagnostics;
using System.Threading;
using Brainf_ckSharp.Buffers;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Extensions.Types;
using Brainf_ckSharp.Memory;
using Brainf_ckSharp.Memory.Interfaces;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Base;
using Brainf_ckSharp.Opcodes;
using Microsoft.Toolkit.HighPerformance.Buffers;
using StackFrame = Brainf_ckSharp.Models.Internal.StackFrame;

namespace Brainf_ckSharp
{
    /// <summary>
    /// A <see langword="class"/> responsible for interpreting and debugging Brainf*ck/PBrain scripts
    /// </summary>
    public static partial class Brainf_ckInterpreter
    {
        /// <summary>
        /// A <see langword="class"/> implementing interpreter methods for the DEBUG configuration
        /// </summary>
        internal static partial class Debug
        {
            /// <summary>
            /// Creates a new Brainf*ck/PBrain session with the given parameters
            /// </summary>
            /// <param name="source">The source code to parse and execute</param>
            /// <param name="breakpoints">The sequence of indices for the breakpoints to apply to the script</param>
            /// <param name="stdin">The input buffer to read data from</param>
            /// <param name="machineState">The target machine state to use to run the script</param>
            /// <param name="executionToken">A <see cref="CancellationToken"/> that can be used to halt the execution</param>
            /// <param name="debugToken">A <see cref="CancellationToken"/> that is used to ignore/respect existing breakpoints</param>
            /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterSession"/> instance with the results of the execution</returns>
            public static Option<InterpreterSession> TryCreateSession(
                string source,
                ReadOnlySpan<int> breakpoints,
                string stdin,
                TuringMachineState machineState,
                CancellationToken executionToken,
                CancellationToken debugToken)
            {
                MemoryOwner<Brainf_ckOperator> opcodes = Brainf_ckParser.TryParse<Brainf_ckOperator>(source, out SyntaxValidationResult validationResult)!;

                if (!validationResult.IsSuccess) return Option<InterpreterSession>.From(validationResult);

                // Initialize the temporary buffers
                MemoryOwner<bool> breakpointsTable = LoadBreakpointsTable(source, validationResult.OperatorsCount, breakpoints);
                MemoryOwner<int> jumpTable = LoadJumpTable<Brainf_ckOperator>(opcodes.Span, out int functionsCount);
                MemoryOwner<Range> functions = MemoryOwner<Range>.Allocate(ushort.MaxValue, AllocationMode.Clear);
                MemoryOwner<ushort> definitions = LoadDefinitionsTable(functionsCount);
                MemoryOwner<StackFrame> stackFrames = MemoryOwner<StackFrame>.Allocate(Specs.MaximumStackSize);

                // Initialize the root stack frame
                stackFrames.DangerousGetReference() = new StackFrame(new Range(0, opcodes.Length), 0);

                // Create the interpreter session
                InterpreterSession session = new InterpreterSession(
                    opcodes,
                    breakpointsTable,
                    jumpTable,
                    functions,
                    definitions,
                    stackFrames,
                    stdin,
                    machineState,
                    executionToken,
                    debugToken);

                return Option<InterpreterSession>.From(validationResult, session);
            }

            /// <summary>
            /// Tries to run a given input Brainf*ck/PBrain executable
            /// </summary>
            /// <typeparam name="TExecutionContext">The type implementing <see cref="IMachineStateExecutionContext"/> to use</typeparam>
            /// <param name="executionContext">The target <typeparamref name="TExecutionContext"/>/> instance to execute the code on</param>
            /// <param name="opcodes">The sequence of parsed opcodes to execute</param>
            /// <param name="breakpoints">The table of breakpoints for the current executable</param>
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
            /// <param name="debugToken">A <see cref="CancellationToken"/> that is used to ignore/respect existing breakpoints</param>
            /// <returns>The resulting <see cref="ExitCode"/> value for the current execution of the input script</returns>
            public static ExitCode Run<TExecutionContext>(
                ref TExecutionContext executionContext,
                ReadOnlySpan<Brainf_ckOperator> opcodes,
                Span<bool> breakpoints,
                ReadOnlySpan<int> jumpTable,
                Span<Range> functions,
                Span<ushort> definitions,
                Span<StackFrame> stackFrames,
                ref int depth,
                ref int totalOperations,
                ref int totalFunctions,
                ref StdinBuffer stdin,
                StdoutBuffer stdout,
                CancellationToken executionToken,
                CancellationToken debugToken)
                where TExecutionContext : struct, IMachineStateExecutionContext
            {
                DebugGuard.MustBeTrue(opcodes.Length > 0, nameof(opcodes));
                DebugGuard.MustBeEqualTo(breakpoints.Length, opcodes.Length, nameof(breakpoints));
                DebugGuard.MustBeEqualTo(jumpTable.Length, opcodes.Length, nameof(jumpTable));
                DebugGuard.MustBeEqualTo(functions.Length, ushort.MaxValue, nameof(functions));
                DebugGuard.MustBeGreaterThanOrEqualTo(definitions.Length, 0, nameof(definitions));
                DebugGuard.MustBeLessThanOrEqualTo(definitions.Length, opcodes.Length / 3, nameof(definitions));
                DebugGuard.MustBeEqualTo(stackFrames.Length, Specs.MaximumStackSize, nameof(stackFrames));
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
                        // Check if a breakpoint has been reached
                        if (breakpoints[i] && !debugToken.IsCancellationRequested)
                        {
                            /* Disable the current breakpoint so that it won't be
                             * triggered again when the execution resumes from this point */
                            breakpoints[i] = false;
                            goto BreakpointReached;
                        }

                        // Execute the current operator
                        switch (opcodes[i].Operator)
                        {
                            // ptr++
                            case Operators.ForwardPtr:
                                if (executionContext.TryMoveNext()) totalOperations++;
                                else goto UpperBoundExceeded;
                                break;

                            // ptr--
                            case Operators.BackwardPtr:
                                if (executionContext.TryMoveBack()) totalOperations++;
                                else goto LowerBoundExceeded;
                                break;

                            // (*ptr)++
                            case Operators.Plus:
                                if (executionContext.TryIncrement()) totalOperations++;
                                else goto MaxValueExceeded;
                                break;

                            // (*ptr)--
                            case Operators.Minus:
                                if (executionContext.TryDecrement()) totalOperations++;
                                else goto NegativeValue;
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
