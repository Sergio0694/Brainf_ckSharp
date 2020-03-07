using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Opcodes;
using Brainf_ckSharp.Models.Opcodes.Interfaces;

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
        internal static PinnedUnmanagedMemoryOwner<TOpcode>? TryParse<TOpcode>(string source, out SyntaxValidationResult validationResult)
            where TOpcode : unmanaged, IOpcode
        {
            if (typeof(TOpcode) == typeof(Brainf_ckOperator))
            {
                return Debug.TryParse(source, out validationResult) as PinnedUnmanagedMemoryOwner<TOpcode>;
            }

            if (typeof(TOpcode) == typeof(Brainf_ckOperation))
            {
                return Release.TryParse(source, out validationResult) as PinnedUnmanagedMemoryOwner<TOpcode>;
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
        internal static unsafe string ExtractSource<TOpcode>(UnmanagedSpan<TOpcode> opcodes)
            where TOpcode : unmanaged, IOpcode
        {
            DebugGuard.MustBeGreaterThan(opcodes.Size, 0, nameof(opcodes));

            if (typeof(TOpcode) == typeof(Brainf_ckOperator))
            {
                ref TOpcode r1 = ref opcodes[0];
                ref Brainf_ckOperator r2 = ref Unsafe.As<TOpcode, Brainf_ckOperator>(ref r1);
                Brainf_ckOperator* p = (Brainf_ckOperator*)Unsafe.AsPointer(ref r2);
                UnmanagedSpan<Brainf_ckOperator> span = new UnmanagedSpan<Brainf_ckOperator>(opcodes.Size, p);

                return Debug.ExtractSource(span);
            }
            
            if (typeof(TOpcode) == typeof(Brainf_ckOperation))
            {
                ref TOpcode r1 = ref opcodes[0];
                ref Brainf_ckOperation r2 = ref Unsafe.As<TOpcode, Brainf_ckOperation>(ref r1);
                Brainf_ckOperation* p = (Brainf_ckOperation*)Unsafe.AsPointer(ref r2);
                UnmanagedSpan<Brainf_ckOperation> span = new UnmanagedSpan<Brainf_ckOperation>(opcodes.Size, p);

                return Release.ExtractSource(span);
            }
            
            throw new ArgumentException($"Invalid opcode type: {typeof(TOpcode)}", nameof(TOpcode));
        }
    }
}
