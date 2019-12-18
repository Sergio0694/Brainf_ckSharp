﻿using System;
using System.Buffers;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Brainf_ck_sharp.NET.Buffers;
using Brainf_ck_sharp.NET.Constants;
using Brainf_ck_sharp.NET.Extensions.Types;
using Brainf_ck_sharp.NET.Helpers;
using Brainf_ck_sharp.NET.Models;
using Brainf_ck_sharp.NET.Models.Internal;

namespace Brainf_ck_sharp.NET
{
    public static partial class Brainf_ckInterpreter
    {
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
            DebugGuard.MustBeEqualTo(stackFrames.Size, Specs.MaximumStackSize, nameof(stackFrames));
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

                /* Adjust the offset and process the current range.
                 * This is needed because in case of a partial execution, no matter
                 * if it's a breakpoint or a crash, the stored offset in the top stack
                 * frame will be the operator currently being executed, which needs to
                 * be included in the processed string. For stack frames below that
                 * instead, the offset already refers to the operator immediately after
                 * the function call operator, so the offset doesn't need to be shifted
                 * ahead before extracting the processed string. Doing this with a
                 * reinterpret cast saves a conditional jump in the asm code. */
                bool zero = i == 0;
                int offset = frame.Offset + Unsafe.As<bool, byte>(ref zero);
                UnsafeMemory<byte> memory = operators.Slice(frame.Range.Start, offset);
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
        private static UnsafeMemoryBuffer<bool> LoadBreakpointsTable(
            string source,
            int operatorsCount,
            ReadOnlySpan<int> breakpoints)
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
    }
}