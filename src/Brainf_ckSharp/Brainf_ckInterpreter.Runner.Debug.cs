using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Memory;
using Brainf_ckSharp.Memory.ExecutionContexts;
using Brainf_ckSharp.Memory.Interfaces;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Base;
using Brainf_ckSharp.Opcodes;
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
        /// <param name="executionOptions">The execution options to use when running the script</param>
        /// <param name="executionToken">A <see cref="CancellationToken"/> that can be used to halt the execution</param>
        /// <param name="debugToken">A <see cref="CancellationToken"/> that is used to ignore/respect existing breakpoints</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterSession"/> instance with the results of the execution</returns>
        public static Option<InterpreterSession> TryCreateSession(
            ReadOnlySpan<char> source,
            ReadOnlySpan<int> breakpoints,
            ReadOnlyMemory<char> stdin,
            IMachineState machineState,
            ExecutionOptions executionOptions,
            CancellationToken executionToken,
            CancellationToken debugToken)
        {
            MemoryOwner<Brainf_ckOperator> opcodes = Brainf_ckParser.TryParse<Brainf_ckOperator>(source, out SyntaxValidationResult validationResult)!;

            if (!validationResult.IsSuccess)
            {
                return Option<InterpreterSession>.From(validationResult);
            }

            // Initialize the temporary buffers
            MemoryOwner<bool> breakpointsTable = LoadBreakpointsTable(source, validationResult.OperatorsCount, breakpoints);
            MemoryOwner<int> jumpTable = LoadJumpTable(opcodes.Span, out int functionsCount);
            MemoryOwner<Range> functions = MemoryOwner<Range>.Allocate(ushort.MaxValue, AllocationMode.Clear);
            MemoryOwner<ushort> definitions = LoadDefinitionsTable(functionsCount);
            MemoryOwner<StackFrame> stackFrames = MemoryOwner<StackFrame>.Allocate(Specs.MaximumStackSize);

            // Initialize the root stack frame
            stackFrames.DangerousGetReference() = new StackFrame(new Range(0, opcodes.Length), 0);

            // Create the interpreter session
            InterpreterSession session = new(
                opcodes,
                breakpointsTable,
                jumpTable,
                functions,
                definitions,
                stackFrames,
                stdin,
                machineState,
                executionOptions,
                executionToken,
                debugToken);

            return Option<InterpreterSession>.From(validationResult, session);
        }

        /// <inheritdoc cref="IMachineState.Invoke(ExecutionOptions, in ExecutionParameters{Brainf_ckOperator}, in DebugParameters)"/>
        /// <typeparam name="TValue">The type of values in each memory cell</typeparam>
        /// <param name="machineState">The target machine state to use to run the script</param>
        public static ExitCode Run<TValue>(
            IMachineState<TValue> machineState,
            ExecutionOptions executionOptions,
            in ExecutionParameters<Brainf_ckOperator> executionParameters,
            in DebugParameters debugParameters)
            where TValue : unmanaged, IBinaryInteger<TValue>, IMinMaxValue<TValue>
        {
            return machineState.Count switch
            {
                32 => Run<TValue, MachineStateSize._32>(machineState, executionOptions, in executionParameters, in debugParameters),
                64 => Run<TValue, MachineStateSize._64>(machineState, executionOptions, in executionParameters, in debugParameters),
                128 => Run<TValue, MachineStateSize._128>(machineState, executionOptions, in executionParameters, in debugParameters),
                256 => Run<TValue, MachineStateSize._256>(machineState, executionOptions, in executionParameters, in debugParameters),
                512 => Run<TValue, MachineStateSize._512>(machineState, executionOptions, in executionParameters, in debugParameters),
                1024 => Run<TValue, MachineStateSize._1024>(machineState, executionOptions, in executionParameters, in debugParameters),
                2048 => Run<TValue, MachineStateSize._2048>(machineState, executionOptions, in executionParameters, in debugParameters),
                4096 => Run<TValue, MachineStateSize._4096>(machineState, executionOptions, in executionParameters, in debugParameters),
                8192 => Run<TValue, MachineStateSize._8192>(machineState, executionOptions, in executionParameters, in debugParameters),
                16384 => Run<TValue, MachineStateSize._16384>(machineState, executionOptions, in executionParameters, in debugParameters),
                _ => Run<TValue, MachineStateSize._32768>(machineState, executionOptions, in executionParameters, in debugParameters)
            };
        }

        /// <inheritdoc cref="IMachineState.Invoke(ExecutionOptions, in ExecutionParameters{Brainf_ckOperator}, in DebugParameters)"/>
        /// <typeparam name="TValue">The type of values in each memory cell</typeparam>
        /// <typeparam name="TSize">The type representing the size of the machine state</typeparam>
        /// <param name="machineState">The target machine state to use to run the script</param>
        private static ExitCode Run<TValue, TSize>(
            IMachineState<TValue> machineState,
            ExecutionOptions executionOptions,
            in ExecutionParameters<Brainf_ckOperator> executionParameters,
            in DebugParameters debugParameters)
            where TValue : unmanaged, IBinaryInteger<TValue>, IMinMaxValue<TValue>
            where TSize : unmanaged, IMachineStateSize
        {
            return executionOptions.HasFlag(ExecutionOptions.AllowOverflow)
                ? Run<TValue, TSize, MachineStateNumberHandler.Overflow<TValue>>(machineState, in executionParameters, in debugParameters)
                : Run<TValue, TSize, MachineStateNumberHandler.NoOverflow<TValue>>(machineState, in executionParameters, in debugParameters);
        }

        /// <inheritdoc cref="IMachineState.Invoke(ExecutionOptions, in ExecutionParameters{Brainf_ckOperator}, in DebugParameters)"/>
        /// <typeparam name="TValue">The type of values in each memory cell</typeparam>
        /// <typeparam name="TSize">The type representing the size of the machine state</typeparam>
        /// <typeparam name="TNumberHandler">The type handling numeric operations for the machine state</typeparam>
        /// <param name="machineState">The target machine state to use to run the script</param>
        private static ExitCode Run<TValue, TSize, TNumberHandler>(
            IMachineState<TValue> machineState,
            in ExecutionParameters<Brainf_ckOperator> executionParameters,
            in DebugParameters debugParameters)
            where TValue : unmanaged, IBinaryInteger<TValue>
            where TSize : unmanaged, IMachineStateSize
            where TNumberHandler : unmanaged, IMachineStateNumberHandler<TValue>
        {
            ExecutionContext<TValue, TSize, TNumberHandler> executionContext = machineState.CreateExecutionContext<TSize, TNumberHandler>();

            ExitCode exitCode = Run<TValue, ExecutionContext<TValue, TSize, TNumberHandler>>(
                ref executionContext,
                in executionParameters,
                in debugParameters);

            machineState.FinalizeExecution(in executionContext);

            return exitCode;
        }

        /// <inheritdoc cref="IMachineState.Invoke(ExecutionOptions, in ExecutionParameters{Brainf_ckOperator}, in DebugParameters)"/>
        /// <typeparam name="TValue">The type of values in each memory cell</typeparam>
        /// <typeparam name="TExecutionContext">The type of execution context instance to use to run the script.</typeparam>
        /// <param name="executionContext">The execution context instance to use to run the script.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static unsafe ExitCode Run<TValue, TExecutionContext>(
            ref TExecutionContext executionContext,
            in ExecutionParameters<Brainf_ckOperator> executionParameters,
            in DebugParameters debugParameters)
            where TValue : unmanaged
            where TExecutionContext : struct, IMachineStateExecutionContext, allows ref struct
        {
            Assert(executionParameters.Depth >= 0);
            Assert(executionParameters.TotalFunctions >= 0);
            Assert(debugParameters.TotalOperations >= 0);

            // Outer loop to go through the existing stack frames
            StackFrame frame;
            int i;
            do
            {
                frame = Unsafe.Add(ref executionParameters.StackFrames, executionParameters.Depth);

                StackFrameLoop:

                // Iterate over the current opcodes
                for (i = frame.Offset; i < frame.Range.End; i++)
                {
                    // Check if a breakpoint has been reached
                    if (Unsafe.Add(ref debugParameters.Breakpoints, i) && !debugParameters.DebugToken.IsCancellationRequested)
                    {
                        // Disable the current breakpoint so that it won't be
                        // triggered again when the execution resumes from this point
                        Unsafe.Add(ref debugParameters.Breakpoints, i) = false;

                        goto BreakpointReached;
                    }

                    // Execute the current operator
                    switch (Unsafe.Add(ref executionParameters.Opcodes, i).Operator)
                    {
                        // ptr++
                        case Operators.ForwardPtr:
                            if (executionContext.TryMoveNext())
                            {
                                debugParameters.TotalOperations++;
                            }
                            else
                            {
                                goto UpperBoundExceeded;
                            }

                            break;

                        // ptr--
                        case Operators.BackwardPtr:
                            if (executionContext.TryMoveBack())
                            {
                                debugParameters.TotalOperations++;
                            }
                            else
                            {
                                goto LowerBoundExceeded;
                            }

                            break;

                        // (*ptr)++
                        case Operators.Plus:
                            if (executionContext.TryIncrement())
                            {
                                debugParameters.TotalOperations++;
                            }
                            else
                            {
                                goto MaxValueExceeded;
                            }

                            break;

                        // (*ptr)--
                        case Operators.Minus:
                            if (executionContext.TryDecrement())
                            {
                                debugParameters.TotalOperations++;
                            }
                            else
                            {
                                goto NegativeValue;
                            }

                            break;

                        // putch(*ptr)
                        case Operators.PrintChar:
                            if (executionParameters.StdoutWriter->TryWrite(executionContext.CurrentCharacter))
                            {
                                debugParameters.TotalOperations++;
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
                                    debugParameters.TotalOperations++;
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
                                debugParameters.TotalOperations++;
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

                            debugParameters.TotalOperations++;
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
                            debugParameters.TotalOperations++;
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
                            debugParameters.TotalOperations++;
                            goto StackFrameLoop;
                        }
                    }
                }
            } while (--executionParameters.Depth >= 0);

            ExitCode exitCode = ExitCode.Success;
            goto Exit;

            BreakpointReached:
            exitCode = ExitCode.BreakpointReached;
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
            Unsafe.Add(ref executionParameters.StackFrames, executionParameters.Depth) = frame.WithOffset(i);

            Exit:
            return exitCode;
        }
    }
}
