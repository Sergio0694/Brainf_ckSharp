using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Brainf_ck_sharp.NET.Buffers;
using Brainf_ck_sharp.NET.Buffers.IO;
using Brainf_ck_sharp.NET.Enums;
using Brainf_ck_sharp.NET.Extensions.Types;
using Brainf_ck_sharp.NET.Helpers;
using Brainf_ck_sharp.NET.Models;

namespace Brainf_ck_sharp.NET
{
    /// <summary>
    /// A <see langword="class"/> responsible for interpreting and debugging Brainf*ck/PBrain scripts
    /// </summary>
    public static class Brainf_ckInterpreter
    {
        /// <summary>
        /// Gets the maximum number of functions that can be defined in a single script
        /// </summary>
        public const int FunctionDefinitionsLimit = 128;

        /// <summary>
        /// Gets the maximum number of recursive calls that can be performed by a script
        /// </summary>
        /// <remarks>The frame index is 0-based, so there are effectively 512 frames in total</remarks>
        public const int MaximumStackSize = 511;

        /// <summary>
        /// Loads the jump table for loops and functions from a given executable
        /// </summary>
        /// <param name="operators">The sequence of parsed operators to inspect</param>
        /// <param name="jumpTable">The resulting precomputed jump table for the input executable</param>
        private static void LoadJumpTable(UnsafeMemoryBuffer<Operator> operators, out UnsafeMemoryBuffer<int> jumpTable)
        {
            jumpTable = UnsafeMemoryBuffer<int>.Allocate(operators.Size);

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
                    case Operator.LoopStart:
                        if (f == -1) Unsafe.Add(ref rootTempIndicesRef, r++) = i;
                        else Unsafe.Add(ref functionTempIndicesRef, f++) = i;
                        break;

                    /* When a loop ends, the index of the corresponding open
                     * square bracket is retrieved from the right temporary
                     * buffer, and the current index is stored at that location
                     * in the final jump table being built. The inverse mapping is
                     * stored too, so that each closing square bracket can reference the
                     * corresponding open bracket at the start of the loop. */
                    case Operator.LoopEnd:
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
                    case Operator.FunctionStart:
                        f = 1;
                        functionTempIndicesRef = i;
                        break;
                    case Operator.FunctionEnd:
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
        }

        /// <summary>
        /// Tries to run a given input Brainf*ck/PBrain executable
        /// </summary>
        /// <param name="operators">The sequence of parsed operators to execute</param>
        /// <param name="breakpoints">The table of breakpoints for the current executable</param>
        /// <param name="jumpTable">The jump table for loops and function declarations</param>
        /// <param name="functions">A <see cref="Dictionary{TKey,TValue}"/> to keep track of declared functions</param>
        /// <param name="state">The target <see cref="TuringMachineState"/> instance to execute the code on</param>
        /// <param name="stdin">The input buffer to read characters from</param>
        /// <param name="stdout">The output buffer to write characters to</param>
        /// <param name="stackFrames">The sequence of stack frames for the current execution</param>
        /// <param name="depth">The current stack depth</param>
        /// <param name="operations">The total number of executed operators</param>
        /// <param name="executionToken">A <see cref="CancellationToken"/> that can be used to halt the execution</param>
        /// <param name="debugToken">A <see cref="CancellationToken"/> that is used to ignore/respect existing breakpoints</param>
        /// <returns>An <see cref="IEnumerator{T}"/> that produces <see cref="InterpreterWorkingData"/> instances for the execution results</returns>
        private static InterpreterWorkingData TryRun(
            UnsafeMemory<Operator> operators,
            UnsafeMemory<bool> breakpoints,
            UnsafeMemory<int> jumpTable,
            Dictionary<ushort, Range> functions,
            TuringMachineState state,
            StdinBuffer stdin,
            StdoutBuffer stdout,
            UnsafeMemory<StackFrame> stackFrames,
            Int depth,
            Int operations,
            CancellationToken executionToken,
            CancellationToken debugToken)
        {
            DebugGuard.MustBeTrue(operators.Size > 0, nameof(operators));
            DebugGuard.MustBeEqualTo(breakpoints.Size, operators.Size, nameof(breakpoints));
            DebugGuard.MustBeEqualTo(jumpTable.Size, operators.Size, nameof(jumpTable));
            DebugGuard.MustBeEqualTo(stackFrames.Size, MaximumStackSize, nameof(stackFrames));
            DebugGuard.MustBeGreaterThanOrEqualTo((int)depth, 0, nameof(depth));
            DebugGuard.MustBeGreaterThanOrEqualTo((int)operations, 0, nameof(operations));

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
                        return new InterpreterWorkingData(InterpreterExitCode.BreakpointReached, operators.Slice(frame.Range.Start, i + 1), i, operations);
                    }

                    // Execute the current operator
                    switch (operators[i])
                    {
                        // ptr++
                        case Operator.ForwardPtr:
                            if (state.TryMoveNext()) operations++;
                            else
                            {
                                return new InterpreterWorkingData(InterpreterExitCode.UpperBoundExceeded, operators.Slice(frame.Range.Start, i + 1), i, operations + 1);
                            }
                            break;

                        // ptr--
                        case Operator.BackwardPtr:
                            if (state.TryMoveBack()) operations++;
                            else
                            {
                                return new InterpreterWorkingData(InterpreterExitCode.LowerBoundExceeded, operators.Slice(frame.Range.Start, i + 1), i, operations + 1);
                            }
                            break;

                        // (*ptr)++
                        case Operator.Plus:
                            if (state.TryIncrement()) operations++;
                            else
                            {
                                return new InterpreterWorkingData(InterpreterExitCode.MaxValueExceeded, operators.Slice(frame.Range.Start, i + 1), i, operations + 1);
                            }
                            break;

                        // (*ptr)--
                        case Operator.Minus:
                            if (state.TryDecrement()) operations++;
                            else
                            {
                                return new InterpreterWorkingData(InterpreterExitCode.NegativeValue, operators.Slice(frame.Range.Start, i + 1), i, operations + 1);
                            }
                            break;

                        // putch(*ptr)
                        case Operator.PrintChar:
                            if (stdout.TryWrite((char)state.Current)) operations++;
                            else
                            {
                                return new InterpreterWorkingData(InterpreterExitCode.StdoutBufferLimitExceeded, operators.Slice(frame.Range.Start, i + 1), i, operations + 1);
                            }
                            break;

                        // *ptr = getch()
                        case Operator.ReadChar:
                            if (stdin.TryRead(out char c))
                            {
                                // Check if the input character can be stored in the current cell
                                if (state.TryInput(c)) operations++;
                                else
                                {
                                    return new InterpreterWorkingData(InterpreterExitCode.MaxValueExceeded, operators.Slice(frame.Range.Start, i + 1), i, operations + 1);
                                }
                            }
                            else
                            {
                                return new InterpreterWorkingData(InterpreterExitCode.StdinBufferExhausted, operators.Slice(frame.Range.Start, i + 1), i, operations + 1);
                            }
                            break;

                        // while (*ptr) {
                        case Operator.LoopStart:

                            // Check whether the loop is active
                            if (state.Current == 0)
                            {
                                i = jumpTable[i];
                                operations++;
                            }
                            else if (jumpTable[i] == i + 2 &&
                                     operators[i + 1] == Operator.Minus &&
                                     (!breakpoints[i + 1] &&
                                      !breakpoints[i + 2] ||
                                      debugToken.IsCancellationRequested))
                            {
                                // Fast path for [-] loops
                                state.ResetCell();
                                operations += state.Current * 2 + 1;
                                i += 2;
                            }
                            else if (executionToken.IsCancellationRequested)
                            {
                                // Check whether the code can still be executed before starting an active loop
                                return new InterpreterWorkingData(InterpreterExitCode.ThresholdExceeded, operators.Slice(frame.Range.Start, i + 1), i, operations);
                            }
                            break;

                        // {
                        case Operator.LoopEnd:
                            if (state.Current > 0) i = jumpTable[i] - 1;
                            operations++;
                            break;

                        // f[*ptr] = []() {
                        case Operator.FunctionStart:
                        {
                            // Check for duplicate function definitions
                            if (functions.ContainsKey(state.Current))
                            {
                                return new InterpreterWorkingData(InterpreterExitCode.DuplicateFunctionDefinition, operators.Slice(frame.Range.Start, i + 1), i, operations + 1);
                            }

                            // Ensure that new functions can stil be defined
                            if (functions.Count == FunctionDefinitionsLimit)
                            {
                                return new InterpreterWorkingData(InterpreterExitCode.FunctionsLimitExceeded, operators.Slice(frame.Range.Start, i + 1), i, operations + 1);
                            }

                            // Save the new function definition
                            Range function = new Range(i + 1, jumpTable[i]);
                            functions.Add(state.Current, function);
                            operations++;
                            i += function.Length;
                            break;
                        }

                        // }
                        case Operator.FunctionEnd:
                            operations++;
                            break;

                        // f[*ptr]()
                        case Operator.FunctionCall:
                        {
                            // Try to retrieve the function to invoke
                            if (!functions.TryGetValue(state.Current, out Range function))
                            {
                                return new InterpreterWorkingData(InterpreterExitCode.UndefinedFunctionCalled, operators.Slice(frame.Range.Start, i + 1), i, operations + 1);
                            }

                            // Ensure the stack has space for the new function invocation
                            if (depth == MaximumStackSize)
                            {
                                return new InterpreterWorkingData(InterpreterExitCode.StackLimitExceeded, operators.Slice(frame.Range.Start, i + 1), i, operations + 1);
                            }

                            // Add the new stack fraame for the function call
                            stackFrames[++depth] = new StackFrame(function, i);
                            operations++;
                            goto StackFrameLoop;
                        }
                    }
                }
            }

            // Return a new state for a successful execution
            bool hasOutput = stdout.Length > 0;
            InterpreterExitCode code = hasOutput ? InterpreterExitCode.TextOutput : InterpreterExitCode.NoOutput;
            return new InterpreterWorkingData(code, operators, operators.Size - 1, operations);
        }
    }
}
