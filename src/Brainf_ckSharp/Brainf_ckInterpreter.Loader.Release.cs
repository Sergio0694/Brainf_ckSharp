using System;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Opcodes.Interfaces;
using CommunityToolkit.HighPerformance.Buffers;
using static System.Diagnostics.Debug;

namespace Brainf_ckSharp;

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
        private static SpanOwner<int> LoadJumpTable<TOpcode>(
            Span<TOpcode> opcodes,
            out int functionsCount)
            where TOpcode : unmanaged, IOpcode
        {
            SpanOwner<int> jumpTable = SpanOwner<int>.Allocate(opcodes.Length);

            Brainf_ckInterpreter.LoadJumpTable(opcodes, jumpTable.Span, out functionsCount);

            return jumpTable;
        }

        /// <summary>
        /// Loads the function definitions table for a script to execute
        /// </summary>
        /// <param name="functionsCount">The total number of declared functions in the script to execute</param>
        /// <returns>The resulting buffer to store keys for the declared functions</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static SpanOwner<ushort> LoadDefinitionsTable(int functionsCount)
        {
            Assert(functionsCount >= 0);

            return functionsCount switch
            {
                0 => SpanOwner<ushort>.Empty,
                _ => SpanOwner<ushort>.Allocate(functionsCount)
            };
        }
    }
}
