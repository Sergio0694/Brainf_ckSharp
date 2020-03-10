using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Opcodes;
using Brainf_ckSharp.Opcodes.Interfaces;
using Microsoft.Toolkit.HighPerformance.Buffers;

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
            ref byte r0 = ref MemoryMarshal.GetReference(OperatorsInverseLookupTable);
            byte r1 = Unsafe.Add(ref r0, opcode.Operator);

            return (char)r1;
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
        internal static MemoryOwner<TOpcode>? TryParse<TOpcode>(string source, out SyntaxValidationResult validationResult)
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
        internal static string ExtractSource<TOpcode>(ReadOnlySpan<TOpcode> opcodes)
            where TOpcode : unmanaged, IOpcode
        {
            DebugGuard.MustBeGreaterThan(opcodes.Length, 0, nameof(opcodes));

            if (typeof(TOpcode) == typeof(Brainf_ckOperator))
            {
                return Debug.ExtractSource(MemoryMarshal.Cast<TOpcode, Brainf_ckOperator>(opcodes));
            }
            
            if (typeof(TOpcode) == typeof(Brainf_ckOperation))
            {
                return Release.ExtractSource(MemoryMarshal.Cast<TOpcode, Brainf_ckOperation>(opcodes));
            }
            
            throw new ArgumentException($"Invalid opcode type: {typeof(TOpcode)}", nameof(TOpcode));
        }
    }
}
