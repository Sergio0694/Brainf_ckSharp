using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Brainf_ck_sharp.NET.Buffers;
using Brainf_ck_sharp.NET.Buffers.IO;
using Brainf_ck_sharp.NET.Constants;
using Brainf_ck_sharp.NET.Enums;
using Brainf_ck_sharp.NET.Extensions.Types;
using Brainf_ck_sharp.NET.Helpers;
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
        /// Gets the maximum number of recursive calls that can be performed by a script
        /// </summary>
        public const int MaximumStackSize = 512;

        /// <summary>
        /// Loads the jump table for loops and functions from a given executable
        /// </summary>
        /// <param name="operators">The sequence of parsed operators to inspect</param>
        /// <returns>The resulting precomputed jump table for the input executable</returns>
        private static UnsafeMemoryBuffer<int> LoadJumpTable(UnsafeMemoryBuffer<byte> operators)
        {
            UnsafeMemoryBuffer<int> jumpTable = UnsafeMemoryBuffer<int>.Allocate(operators.Size);

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
            UnsafeMemory<bool> definitions,
            TuringMachineState state,
            StdinBuffer stdin,
            StdoutBuffer stdout,
            UnsafeMemory<StackFrame> stackFrames,
            Int depth,
            Int totalOperations,
            Int totalFunctions,
            CancellationToken executionToken,
            CancellationToken debugToken)
        {
            DebugGuard.MustBeTrue(operators.Size > 0, nameof(operators));
            DebugGuard.MustBeEqualTo(breakpoints.Size, operators.Size, nameof(breakpoints));
            DebugGuard.MustBeEqualTo(jumpTable.Size, operators.Size, nameof(jumpTable));
            DebugGuard.MustBeEqualTo(functions.Size, ushort.MaxValue, nameof(functions));
            DebugGuard.MustBeEqualTo(definitions.Size, operators.Size, nameof(definitions));
            DebugGuard.MustBeEqualTo(stackFrames.Size, MaximumStackSize, nameof(stackFrames));
            DebugGuard.MustBeGreaterThanOrEqualTo((int)depth, 0, nameof(depth));
            DebugGuard.MustBeGreaterThanOrEqualTo((int)totalOperations, 0, nameof(totalOperations));
            DebugGuard.MustBeGreaterThanOrEqualTo((int)totalFunctions, 0, nameof(totalFunctions));

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
                        return new InterpreterWorkingData(InterpreterExitCode.BreakpointReached, operators.Slice(frame.Range.Start, i + 1), i, totalOperations);
                    }

                    // Execute the current operator
                    switch (operators[i])
                    {
                        // ptr++
                        case Operators.ForwardPtr:
                            if (state.TryMoveNext()) totalOperations++;
                            else
                            {
                                return new InterpreterWorkingData(InterpreterExitCode.UpperBoundExceeded, operators.Slice(frame.Range.Start, i + 1), i, totalOperations + 1);
                            }
                            break;

                        // ptr--
                        case Operators.BackwardPtr:
                            if (state.TryMoveBack()) totalOperations++;
                            else
                            {
                                return new InterpreterWorkingData(InterpreterExitCode.LowerBoundExceeded, operators.Slice(frame.Range.Start, i + 1), i, totalOperations + 1);
                            }
                            break;

                        // (*ptr)++
                        case Operators.Plus:
                            if (state.TryIncrement()) totalOperations++;
                            else
                            {
                                return new InterpreterWorkingData(InterpreterExitCode.MaxValueExceeded, operators.Slice(frame.Range.Start, i + 1), i, totalOperations + 1);
                            }
                            break;

                        // (*ptr)--
                        case Operators.Minus:
                            if (state.TryDecrement()) totalOperations++;
                            else
                            {
                                return new InterpreterWorkingData(InterpreterExitCode.NegativeValue, operators.Slice(frame.Range.Start, i + 1), i, totalOperations + 1);
                            }
                            break;

                        // putch(*ptr)
                        case Operators.PrintChar:
                            if (stdout.TryWrite((char)state.Current)) totalOperations++;
                            else
                            {
                                return new InterpreterWorkingData(InterpreterExitCode.StdoutBufferLimitExceeded, operators.Slice(frame.Range.Start, i + 1), i, totalOperations + 1);
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
                                    return new InterpreterWorkingData(InterpreterExitCode.MaxValueExceeded, operators.Slice(frame.Range.Start, i + 1), i, totalOperations + 1);
                                }
                            }
                            else
                            {
                                return new InterpreterWorkingData(InterpreterExitCode.StdinBufferExhausted, operators.Slice(frame.Range.Start, i + 1), i, totalOperations + 1);
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
                                return new InterpreterWorkingData(InterpreterExitCode.ThresholdExceeded, operators.Slice(frame.Range.Start, i + 1), i, totalOperations);
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
                                return new InterpreterWorkingData(InterpreterExitCode.DuplicateFunctionDefinition, operators.Slice(frame.Range.Start, i + 1), i, totalOperations + 1);
                            }

                            // Check that the current function has not been defined before
                            if (definitions[i])
                            {
                                return new InterpreterWorkingData(InterpreterExitCode.FunctionAlreadyDefined, operators.Slice(frame.Range.Start, i + 1), i, totalOperations + 1);
                            }

                            // Save the new function definition
                            Range function = new Range(i + 1, jumpTable[i]);
                            functions[state.Current] = function;
                            definitions[i] = true;
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
                                return new InterpreterWorkingData(InterpreterExitCode.UndefinedFunctionCalled, operators.Slice(frame.Range.Start, i + 1), i, totalOperations + 1);
                            }

                            // Ensure the stack has space for the new function invocation
                            if (depth == MaximumStackSize - 1)
                            {
                                return new InterpreterWorkingData(InterpreterExitCode.StackLimitExceeded, operators.Slice(frame.Range.Start, i + 1), i, totalOperations + 1);
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
            InterpreterExitCode code = hasOutput ? InterpreterExitCode.TextOutput : InterpreterExitCode.NoOutput;
            return new InterpreterWorkingData(code, operators, operators.Size - 1, totalOperations);
        }
    }
}
