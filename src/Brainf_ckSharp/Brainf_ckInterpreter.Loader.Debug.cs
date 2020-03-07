using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Brainf_ckSharp
{
    public static partial class Brainf_ckInterpreter
    {
        /// <summary>
        /// Loads the function definitions table for a script to execute
        /// </summary>
        /// <param name="functionsCount">The total number of declared functions in the script to execute</param>
        /// <returns>The resulting buffer to store keys for the declared functions</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static PinnedUnmanagedMemoryOwner<ushort> LoadDefinitionsTable(int functionsCount)
        {
            DebugGuard.MustBeGreaterThanOrEqualTo(functionsCount, 0, nameof(functionsCount));

            return functionsCount switch
            {
                0 => PinnedUnmanagedMemoryOwner<ushort>.Empty,
                _ => PinnedUnmanagedMemoryOwner<ushort>.Allocate(functionsCount, false)
            };
        }

        /// <summary>
        /// A <see langword="class"/> implementing interpreter methods for the DEBUG configuration
        /// </summary>
        internal static partial class Debug
        {
            /// <summary>
            /// Loads the breakpoints table for a given source code and collection of breakpoints
            /// </summary>
            /// <param name="source">The source code to parse and execute</param>
            /// <param name="operatorsCount">The precomputed number of operators in the input source code</param>
            /// <param name="breakpoints">The sequence of indices for the breakpoints to apply to the script</param>
            /// <returns>The resulting precomputed breakpoints table for the input executable</returns>
            [Pure]
            public static PinnedUnmanagedMemoryOwner<bool> LoadBreakpointsTable(
                string source,
                int operatorsCount,
                ReadOnlySpan<int> breakpoints)
            {
                /* This temporary buffer is used to build a quick lookup table for the
                 * valid indices from the input breakpoints collection. This table is
                 * built in O(M), and then provides constant time checking for each
                 * character from the input script. The result is an algorithm that
                 * builds the final breakpoints table in O(M + N) instead of O(M * N). */
                using PinnedUnmanagedMemoryOwner<bool> temporaryBuffer = PinnedUnmanagedMemoryOwner<bool>.Allocate(source.Length, true);

                // Build the temporary table to store the indirect offsets of the breakpoints
                foreach (int index in breakpoints)
                {
                    Guard.MustBeGreaterThan(index, 0, nameof(breakpoints));
                    Guard.MustBeLessThan(index, source.Length, nameof(breakpoints));

                    temporaryBuffer[index] = true;
                }

                PinnedUnmanagedMemoryOwner<bool> breakpointsBuffer = PinnedUnmanagedMemoryOwner<bool>.Allocate(operatorsCount, false);

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
}
