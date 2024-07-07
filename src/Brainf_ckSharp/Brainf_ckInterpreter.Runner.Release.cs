using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using Brainf_ckSharp.Buffers;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Memory;
using Brainf_ckSharp.Memory.ExecutionContexts;
using Brainf_ckSharp.Memory.Interfaces;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Internal;
using Brainf_ckSharp.Opcodes;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;
using StackFrame = Brainf_ckSharp.Models.Internal.StackFrame;
using Range = Brainf_ckSharp.Models.Internal.Range;
using static System.Diagnostics.Debug;

#pragma warning disable CS1573

namespace Brainf_ckSharp;

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
        /// <param name="executionOptions">The execution options to use when running the script</param>
        /// <param name="executionToken">A <see cref="CancellationToken"/> that can be used to halt the execution</param>
        /// <returns>An <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        public static InterpreterResult Run(
            Span<Brainf_ckOperation> opcodes,
            ReadOnlySpan<char> stdin,
            IMachineState machineState,
            ExecutionOptions executionOptions,
            CancellationToken executionToken)
        {
            Assert(opcodes.Length >= 0);
            Assert(machineState.Count >= 0);

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

            // Setup the stdin and stdout readers and writers
            StdinBuffer.Reader stdinReader = new(stdin);
            StdoutBuffer.Writer stdoutWriter = stdout.CreateWriter();

            // Prepare the execution parameters for the invocation dispatch
            ExecutionParameters<Brainf_ckOperation> executionParameters = new(
                ref opcodes.DangerousGetReference(),
                ref jumpTable.DangerousGetReference(),
                ref functions.DangerousGetReference(),
                ref definitions.DangerousGetReference(),
                ref stackFrames.DangerousGetReference(),
                ref depth,
                ref totalOperations,
                ref totalFunctions,
                ref stdinReader,
                ref stdoutWriter,
                executionToken);

            Timestamp timestamp = Timestamp.Now;

            // Start the interpreter
            exitCode = machineState.Invoke(executionOptions, in executionParameters);

            elapsed = TimeSpan.FromTicks(timestamp.Ticks);

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

            return new(
                sourceCode,
                exitCode,
                debugInfo,
                machineState,
                functionDefinitions,
                stdinReader.ToString(),
                stdoutWriter.ToString(),
                elapsed,
                totalOperations);
        }

        /// <inheritdoc cref="IMachineState.Invoke"/>
        /// <typeparam name="TValue">The type of values in each memory cell</typeparam>
        /// <param name="machineState">The target machine state to use to run the script</param>
        public static ExitCode Run<TValue>(
            IMachineState machineState,
            ExecutionOptions executionOptions,
            in ExecutionParameters<Brainf_ckOperation> executionParameters)
            where TValue : unmanaged, IBinaryInteger<TValue>, IMinMaxValue<TValue>
        {
            return machineState.Count switch
            {
                32 => Run<TValue, MachineStateSize._32>(machineState, executionOptions, in executionParameters),
                64 => Run<TValue, MachineStateSize._64>(machineState, executionOptions, in executionParameters),
                128 => Run<TValue, MachineStateSize._128>(machineState, executionOptions, in executionParameters),
                256 => Run<TValue, MachineStateSize._256>(machineState, executionOptions, in executionParameters),
                512 => Run<TValue, MachineStateSize._512>(machineState, executionOptions, in executionParameters),
                1024 => Run<TValue, MachineStateSize._1024>(machineState, executionOptions, in executionParameters),
                2048 => Run<TValue, MachineStateSize._2048>(machineState, executionOptions, in executionParameters),
                4096 => Run<TValue, MachineStateSize._4096>(machineState, executionOptions, in executionParameters),
                8192 => Run<TValue, MachineStateSize._8192>(machineState, executionOptions, in executionParameters),
                16384 => Run<TValue, MachineStateSize._16384>(machineState, executionOptions, in executionParameters),
                _ => Run<TValue, MachineStateSize._32768>(machineState, executionOptions, in executionParameters)
            };
        }

        /// <inheritdoc cref="IMachineState.Invoke"/>
        /// <typeparam name="TValue">The type of values in each memory cell</typeparam>
        /// <typeparam name="TSize">The type representing the size of the machine state</typeparam>
        /// <param name="machineState">The target machine state to use to run the script</param>
        private static ExitCode Run<TValue, TSize>(
            IMachineState machineState,
            ExecutionOptions executionOptions,
            in ExecutionParameters<Brainf_ckOperation> executionParameters)
            where TValue : unmanaged, IBinaryInteger<TValue>, IMinMaxValue<TValue>
            where TSize : unmanaged, IMachineStateSize
        {
            return executionOptions.HasFlag(ExecutionOptions.AllowOverflow)
                ? Run<TValue, TSize, MachineStateNumberHandler.Overflow<TValue>>(machineState, in executionParameters)
                : Run<TValue, TSize, MachineStateNumberHandler.NoOverflow<TValue>>(machineState, in executionParameters);
        }

        /// <inheritdoc cref="IMachineState.Invoke"/>
        /// <typeparam name="TValue">The type of values in each memory cell</typeparam>
        /// <typeparam name="TSize">The type representing the size of the machine state</typeparam>
        /// <typeparam name="TNumberHandler">The type handling numeric operations for the machine state</typeparam>
        /// <param name="machineState">The target machine state to use to run the script</param>
        private static ExitCode Run<TValue, TSize, TNumberHandler>(
            IMachineState machineState,
            in ExecutionParameters<Brainf_ckOperation> executionParameters)
            where TValue : unmanaged, IBinaryInteger<TValue>
            where TSize : unmanaged, IMachineStateSize
            where TNumberHandler : unmanaged, IMachineStateNumberHandler<TValue>
        {
            MachineStateExecutionContext<TValue, TSize, TNumberHandler> executionContext = new();

            ExitCode exitCode = Run<TValue, MachineStateExecutionContext<TValue, TSize, TNumberHandler>>(
                ref executionContext,
                in executionParameters);

            machineState.Position = executionContext.Position;

            return exitCode;
        }

        /// <inheritdoc cref="IMachineState.Invoke"/>
        /// <typeparam name="TValue">The type of values in each memory cell</typeparam>
        /// <typeparam name="TExecutionContext">The type of execution context instance to use to run the script.</typeparam>
        /// <param name="executionContext">The execution context instance to use to run the script.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static unsafe ExitCode Run<TValue, TExecutionContext>(
            ref TExecutionContext executionContext,
            in ExecutionParameters<Brainf_ckOperation> executionParameters)
            where TValue : unmanaged
            where TExecutionContext : struct, IMachineStateExecutionContext<TValue>, allows ref struct
        {
            // Outer loop to go through the existing stack frames
            StackFrame frame;
            int i;
            do
            {
                frame = Unsafe.Add(ref executionParameters.StackFrames, executionParameters.Depth);

                // This label is used when a function call is performed: a new stack frame
                // is pushed in the frames collection and then a goto is used to jump out
                // of both the switch case and the inner loop. This is faster than using
                // another variable to manually handle the two consecutive breaks to
                // reach the start of the inner loop from a switch case.
                StackFrameLoop:

                // Iterate over the current opcodes
                for (i = frame.Offset; i < frame.Range.End; i++)
                {
                    Brainf_ckOperation opcode = Unsafe.Add(ref executionParameters.Opcodes, i);

                    // Execute the current operator
                    switch (opcode.Operator)
                    {
                        // ptr++
                        case Operators.ForwardPtr:
                            if (!executionContext.TryMoveNext(opcode.Count, ref executionParameters.TotalOperations))
                            {
                                goto UpperBoundExceeded;
                            }

                            break;

                        // ptr--
                        case Operators.BackwardPtr:
                            if (!executionContext.TryMoveBack(opcode.Count, ref executionParameters.TotalOperations))
                            {
                                goto LowerBoundExceeded;
                            }

                            break;

                        // (*ptr)++
                        case Operators.Plus:
                            if (!executionContext.TryIncrement(opcode.Count, ref executionParameters.TotalOperations))
                            {
                                goto MaxValueExceeded;
                            }

                            break;

                        // (*ptr)--
                        case Operators.Minus:
                            if (!executionContext.TryDecrement(opcode.Count, ref executionParameters.TotalOperations))
                            {
                                goto NegativeValue;
                            }

                            break;

                        // putch(*ptr)
                        case Operators.PrintChar:
                            if (executionParameters.StdoutWriter->TryWrite(executionContext.CurrentCharacter))
                            {
                                executionParameters.TotalOperations++;
                            }
                            else
                            {
                                goto StdoutBufferLimitExceeded;
                            }

                            break;

                        // *ptr = getch()
                        case Operators.ReadChar:
                            if (executionParameters.StdinReader->TryRead(out char c))
                            {
                                // Check if the input character can be stored in the current cell
                                if (executionContext.TryInput(c))
                                {
                                    executionParameters.TotalOperations++;
                                }
                                else
                                {
                                    goto MaxValueExceeded;
                                }
                            }
                            else
                            {
                                goto StdinBufferExhausted;
                            }

                            break;

                        // while (*ptr) {
                        case Operators.LoopStart:

                            // Check whether the loop is active
                            if (!executionContext.IsCurrentValuePositive)
                            {
                                i = Unsafe.Add(ref executionParameters.JumpTable, i);
                                executionParameters.TotalOperations++;
                            }
                            else if (Unsafe.Add(ref executionParameters.JumpTable, i) == i + 2 &&
                                     Unsafe.Add(ref executionParameters.Opcodes, i + 1).Operator == Operators.Minus)
                            {
                                // Fast path for [-] loops
                                executionContext.ResetCell();
                                //executionParameters.TotalOperations += (executionContext.CurrentValue * 2) + 1;
                                i += 2;
                            }

                            break;

                        // {
                        case Operators.LoopEnd:

                            // Check whether the loop is still active and can be repeated
                            if (executionContext.IsCurrentValuePositive)
                            {
                                i = Unsafe.Add(ref executionParameters.JumpTable, i);

                                // Check whether the code can still be executed before starting an active loop
                                if (executionParameters.ExecutionToken.IsCancellationRequested)
                                {
                                    goto ThresholdExceeded;
                                }
                            }

                            executionParameters.TotalOperations++;
                            break;

                        // f[*ptr] = []() {
                        case Operators.FunctionStart:
                        {
                            // Check for duplicate function definitions
                            if (Unsafe.Add(ref executionParameters.Functions, (uint)executionContext.CurrentValue).Length != 0)
                            {
                                goto DuplicateFunctionDefinition;
                            }

                            // Save the new function definition
                            Range function = new(i + 1, Unsafe.Add(ref executionParameters.JumpTable, i));
                            Unsafe.Add(ref executionParameters.Functions, (uint)executionContext.CurrentValue) = function;
                            Unsafe.Add(ref executionParameters.Definitions, executionParameters.TotalFunctions++) = executionContext.CurrentValue;
                            executionParameters.TotalOperations++;
                            i += function.Length;
                            break;
                        }

                        // f[*ptr]()
                        case Operators.FunctionCall:
                        {
                            // Try to retrieve the function to invoke
                            Range function = Unsafe.Add(ref executionParameters.Functions, (uint)executionContext.CurrentValue);
                            if (function.Length == 0)
                            {
                                goto UndefinedFunctionCalled;
                            }

                            // Ensure the stack has space for the new function invocation
                            if (executionParameters.Depth == Specs.MaximumStackSize - 1)
                            {
                                goto StackLimitExceeded;
                            }

                            // Check for remaining time
                            if (executionParameters.ExecutionToken.IsCancellationRequested)
                            {
                                goto ThresholdExceeded;
                            }

                            // Update the current stack frame and exit the inner loop
                            Unsafe.Add(ref executionParameters.StackFrames, executionParameters.Depth++) = frame.WithOffset(i + 1);
                            frame = new StackFrame(function);
                            executionParameters.TotalOperations++;
                            goto StackFrameLoop;
                        }
                    }
                }
            } while (--executionParameters.Depth >= 0);

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
            Unsafe.Add(ref executionParameters.StackFrames, executionParameters.Depth) = frame.WithOffset(i);

            Exit:
            return exitCode;
        }
    }
}
