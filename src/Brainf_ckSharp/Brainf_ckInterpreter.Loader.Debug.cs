using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Opcodes.Interfaces;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance.Buffers;
using static System.Diagnostics.Debug;

namespace Brainf_ckSharp;

public static partial class Brainf_ckInterpreter
{
    /// <summary>
    /// A <see langword="class"/> implementing interpreter methods for the DEBUG configuration
    /// </summary>
    internal static partial class Debug
    {
        /// <summary>
        /// Loads the jump table for loops and functions from a given executable
        /// </summary>
        /// <typeparam name="TOpcode">The type of opcode to process</typeparam>
        /// <param name="opcodes">The sequence of parsed opcodes to inspect</param>
        /// <param name="functionsCount">The total number of declared functions in the input sequence of opcodes</param>
        /// <returns>The resulting precomputed jump table for the input executable</returns>
        [Pure]
        private static MemoryOwner<int> LoadJumpTable<TOpcode>(
            Span<TOpcode> opcodes,
            out int functionsCount)
            where TOpcode : unmanaged, IOpcode
        {
            MemoryOwner<int> jumpTable = MemoryOwner<int>.Allocate(opcodes.Length);

            Brainf_ckInterpreter.LoadJumpTable(opcodes, jumpTable.Span, out functionsCount);

            return jumpTable;
        }

        /// <summary>
        /// Loads the function definitions table for a script to execute
        /// </summary>
        /// <param name="functionsCount">The total number of declared functions in the script to execute</param>
        /// <returns>The resulting buffer to store keys for the declared functions</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static MemoryOwner<ushort> LoadDefinitionsTable(int functionsCount)
        {
            Assert(functionsCount >= 0);

            return functionsCount switch
            {
                0 => MemoryOwner<ushort>.Empty,
                _ => MemoryOwner<ushort>.Allocate(functionsCount)
            };
        }

        /// <summary>
        /// Loads the breakpoints table for a given source code and collection of breakpoints
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="operatorsCount">The precomputed number of operators in the input source code</param>
        /// <param name="breakpoints">The sequence of indices for the breakpoints to apply to the script</param>
        /// <returns>The resulting precomputed breakpoints table for the input executable</returns>
        [Pure]
        public static MemoryOwner<bool> LoadBreakpointsTable(
            ReadOnlySpan<char> source,
            int operatorsCount,
            ReadOnlySpan<int> breakpoints)
        {
            // Fast path if there are no breakpoints to process
            if (breakpoints.IsEmpty)
            {
                return MemoryOwner<bool>.Allocate(operatorsCount, AllocationMode.Clear);
            }

            // This temporary buffer is used to build a quick lookup table for the
            // valid indices from the input breakpoints collection. This table is
            // built in O(M), and then provides constant time checking for each
            // character from the input script. The result is an algorithm that
            // builds the final breakpoints table in O(M + N) instead of O(M * N).
            using SpanOwner<bool> temporaryBuffer = SpanOwner<bool>.Allocate(source.Length, AllocationMode.Clear);
            ref bool temporaryBufferRef = ref temporaryBuffer.DangerousGetReference();

            // Build the temporary table to store the indirect offsets of the breakpoints
            foreach (int index in breakpoints)
            {
                Guard.IsInRangeFor(index, source, nameof(breakpoints));

                Unsafe.Add(ref temporaryBufferRef, index) = true;
            }

            MemoryOwner<bool> breakpointsBuffer = MemoryOwner<bool>.Allocate(operatorsCount);
            ref bool breakpointsBufferRef = ref breakpointsBuffer.DangerousGetReference();

            // Build the breakpoints table by going through the temporary table with the markers
            for (int i = 0, j = 0; j < source.Length; j++)
            {
                if (!Brainf_ckParser.IsOperator(source[j])) continue;

                Unsafe.Add(ref breakpointsBufferRef, i++) = Unsafe.Add(ref temporaryBufferRef, j);
            }

            return breakpointsBuffer;
        }
    }
}
