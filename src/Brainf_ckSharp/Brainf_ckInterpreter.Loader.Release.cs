using System;
using System.Diagnostics.Contracts;
using Brainf_ckSharp.Opcodes.Interfaces;
using Microsoft.Toolkit.HighPerformance.Buffers;

namespace Brainf_ckSharp
{
    public static partial class Brainf_ckInterpreter
    {
        /// <summary>
        /// A <see langword="class"/> implementing interpreter methods for the DEBUG configuration
        /// </summary>
        internal static partial class Release
        {
            /// <summary>
            /// Loads the jump table for loops and functions from a given executable
            /// </summary>
            /// <typeparam name="TOpcode">The type of opcode to process</typeparam>
            /// <param name="opcodes">The sequence of parsed opcodes to inspect</param>
            /// <param name="functionsCount">The total number of declared functions in the input sequence of opcodes</param>
            /// <returns>The resulting precomputed jump table for the input executable</returns>
            [Pure]
            private static SpanOwner<int> LoadJumpTable<TOpcode>(
                Span<TOpcode> opcodes,
                out int functionsCount)
                where TOpcode : unmanaged, IOpcode
            {
                SpanOwner<int> jumpTable = SpanOwner<int>.Allocate(opcodes.Length);

                Brainf_ckInterpreter.LoadJumpTable(opcodes, jumpTable.Span, out functionsCount);

                return jumpTable;
            }
        }
    }
}
