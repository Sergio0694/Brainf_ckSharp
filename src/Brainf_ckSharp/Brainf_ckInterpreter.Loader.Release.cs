using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Extensions.Types;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Internal;

namespace Brainf_ckSharp
{
    public static partial class Brainf_ckInterpreter
    {
        /// <summary>
        /// Loads the function definitions with the given executable and parameters
        /// </summary>
        /// <param name="operations">The sequence of parsed operations to execute</param>
        /// <param name="functions">The mapping of functions for the current execution</param>
        /// <param name="definitions">The lookup table to check which functions are defined</param>
        /// <param name="totalFunctions">The total number of defined functions</param>
        /// <returns>An array of <see cref="FunctionDefinition"/> instance with the defined functions</returns>
        /// <remarks>This method mirrors <see cref="LoadFunctionDefinitions(UnmanagedSpan{byte},UnmanagedSpan{Range},UnmanagedSpan{ushort},int)"/></remarks>
        [Pure]
        internal static FunctionDefinition[] LoadFunctionDefinitions(
            UnmanagedSpan<Brainf_ckOperation> operations,
            UnmanagedSpan<Range> functions,
            UnmanagedSpan<ushort> definitions,
            int totalFunctions)
        {
            DebugGuard.MustBeGreaterThanOrEqualTo(operations.Size, 0, nameof(operations));
            DebugGuard.MustBeEqualTo(functions.Size, ushort.MaxValue, nameof(functions));
            DebugGuard.MustBeGreaterThanOrEqualTo(definitions.Size, 0, nameof(definitions));
            DebugGuard.MustBeLessThanOrEqualTo(definitions.Size, operations.Size / 3, nameof(definitions));
            DebugGuard.MustBeGreaterThanOrEqualTo(totalFunctions, 0, nameof(totalFunctions));

            // No declared functions
            if (totalFunctions == 0) return Array.Empty<FunctionDefinition>();

            FunctionDefinition[] result = new FunctionDefinition[totalFunctions];
            ref FunctionDefinition r0 = ref result[0];

            // Process all the declared functions
            for (int i = 0; i < totalFunctions; i++)
            {
                ushort key = definitions[i];
                Range range = functions[key];
                int offset = range.Start - 1;

                UnmanagedSpan<Brainf_ckOperation> memory = operations.Slice(in range);
                string body = Brainf_ckParser.ExtractSource(memory);

                Unsafe.Add(ref r0, i) = new FunctionDefinition(key, i, offset, body);
            }

            return result;
        }

        /// <summary>
        /// Loads the jump table for loops and functions from a given executable
        /// </summary>
        /// <param name="operations">The sequence of parsed operations to inspect</param>
        /// <param name="functionsCount">The total number of declared functions in the input sequence of operations</param>
        /// <returns>The resulting precomputed jump table for the input executable</returns>
        /// <remarks>This method mirrors <see cref="LoadJumpTable(PinnedUnmanagedMemoryOwner{byte},out int)"/></remarks>
        [Pure]
        private static PinnedUnmanagedMemoryOwner<int> LoadJumpTable(PinnedUnmanagedMemoryOwner<Brainf_ckOperation> operations, out int functionsCount)
        {
            DebugGuard.MustBeGreaterThanOrEqualTo(operations.Size, 0, nameof(operations));

            PinnedUnmanagedMemoryOwner<int> jumpTable = PinnedUnmanagedMemoryOwner<int>.Allocate(operations.Size, false);

            /* Temporarily allocate two buffers to store the indirect indices to build the jump table.
             * This method perfectly mirrors the behavior of the one working directly on operators. */
            int tempBuffersLength = operations.Size / 2 + 1;
            using StackOnlyUnmanagedMemoryOwner<int> rootTempIndices = StackOnlyUnmanagedMemoryOwner<int>.Allocate(tempBuffersLength);
            using StackOnlyUnmanagedMemoryOwner<int> functionTempIndices = StackOnlyUnmanagedMemoryOwner<int>.Allocate(tempBuffersLength);
            ref int rootTempIndicesRef = ref rootTempIndices.GetReference();
            ref int functionTempIndicesRef = ref functionTempIndices.GetReference();
            functionsCount = 0;

            // Go through the executable to build the jump table
            for (int r = 0, f = -1, i = 0; i < operations.Size; i++)
            {
                switch (operations[i].Operator)
                {
                    // while (*ptr) {
                    case Operators.LoopStart:
                        if (f == -1) Unsafe.Add(ref rootTempIndicesRef, r++) = i;
                        else Unsafe.Add(ref functionTempIndicesRef, f++) = i;
                        break;

                    // {
                    case Operators.LoopEnd:
                        int start = f == -1
                            ? Unsafe.Add(ref rootTempIndicesRef, --r)
                            : Unsafe.Add(ref functionTempIndicesRef, --f);
                        jumpTable[start] = i;
                        jumpTable[i] = start;
                        break;

                    // Function definition
                    case Operators.FunctionStart:
                        f = 1;
                        functionTempIndicesRef = i;
                        functionsCount++;
                        break;
                    case Operators.FunctionEnd:
                        f = -1;
                        jumpTable[functionTempIndicesRef] = i;
                        jumpTable[i] = functionTempIndicesRef;
                        break;
                }
            }

            return jumpTable;
        }
    }
}
