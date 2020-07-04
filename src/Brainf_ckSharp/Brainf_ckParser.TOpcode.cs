using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Opcodes;
using Brainf_ckSharp.Opcodes.Interfaces;
using Microsoft.Toolkit.HighPerformance.Buffers;
using Microsoft.Toolkit.HighPerformance.Extensions;
using static System.Diagnostics.Debug;

namespace Brainf_ckSharp
{
    /// <summary>
    /// A <see langword="class"/> responsible for parsing and validating Brainf*ck/PBrain scripts
    /// </summary>
    public static partial class Brainf_ckParser
    {
        /// <summary>
        /// Gets the corresponding <see cref="char"/> value for a given processed Brainf*ck/PBrain opcode
        /// </summary>
        /// <typeparam name="TOpcode">The type of opcode to process</typeparam>
        /// <param name="opcode">The input processed opcode to convert</param>
        /// <returns>The  <see cref="char"/> value representing <paramref name="opcode"/></returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static char GetCharacterFromOpcode<TOpcode>(in TOpcode opcode)
            where TOpcode : unmanaged, IOpcode
        {
            Assert(opcode.Operator < OperatorsInverseLookupTable.Length);

            return (char)OperatorsInverseLookupTable.DangerousGetReferenceAt(opcode.Operator);
        }

        /// <summary>
        /// Tries to parse the input source script, if possible
        /// </summary>
        /// <typeparam name="TOpcode">The type of opcode to process</typeparam>
        /// <param name="source">The input script to validate</param>
        /// <param name="validationResult">The <see cref="SyntaxValidationResult"/> instance with the results of the parsing operation</param>
        /// <returns>The resulting buffer of opcodes for the parsed script</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static MemoryOwner<TOpcode>? TryParse<TOpcode>(ReadOnlySpan<char> source, out SyntaxValidationResult validationResult)
            where TOpcode : unmanaged, IOpcode
        {
            if (typeof(TOpcode) == typeof(Brainf_ckOperator))
            {
                return Debug.TryParse(source, out validationResult) as MemoryOwner<TOpcode>;
            }

            if (typeof(TOpcode) == typeof(Brainf_ckOperation))
            {
                return Release.TryParse(source, out validationResult) as MemoryOwner<TOpcode>;
            }

            throw new ArgumentException($"Invalid opcode type: {typeof(TOpcode)}", nameof(TOpcode));
        }

        /// <summary>
        /// Extracts the compacted source code from a given sequence of operators
        /// </summary>
        /// <typeparam name="TOpcode">The type of opcode to process</typeparam>
        /// <param name="opcodes">The input sequence of parsed opcodes to read</param>
        /// <returns>A <see cref="string"/> representing the input sequence of opcodes</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string ExtractSource<TOpcode>(Span<TOpcode> opcodes)
            where TOpcode : unmanaged, IOpcode
        {
            Assert(opcodes.Length > 0);

            if (typeof(TOpcode) == typeof(Brainf_ckOperator))
            {
                return Debug.ExtractSource(opcodes.Cast<TOpcode, Brainf_ckOperator>());
            }
            
            if (typeof(TOpcode) == typeof(Brainf_ckOperation))
            {
                return Release.ExtractSource(opcodes.Cast<TOpcode, Brainf_ckOperation>());
            }
            
            throw new ArgumentException($"Invalid opcode type: {typeof(TOpcode)}", nameof(TOpcode));
        }
    }
}
