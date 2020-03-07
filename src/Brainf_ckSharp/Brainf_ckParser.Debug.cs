using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Models;

namespace Brainf_ckSharp
{
    /// <summary>
    /// A <see langword="class"/> responsible for parsing and validating Brainf*ck/PBrain scripts
    /// </summary>
    public static partial class Brainf_ckParser
    {
        /// <summary>
        /// A lookup table to quickly check characters
        /// </summary>
        private static ReadOnlySpan<byte> OperatorsInverseLookupTable => new[]
        {
            (byte)Characters.LoopStart,
            (byte)Characters.LoopEnd,
            (byte)Characters.FunctionStart,
            (byte)Characters.FunctionEnd,
            (byte)Characters.Plus,
            (byte)Characters.Minus,
            (byte)Characters.ForwardPtr,
            (byte)Characters.BackwardPtr,
            (byte)Characters.PrintChar,
            (byte)Characters.ReadChar,
            (byte)Characters.FunctionCall
        };

        /// <summary>
        /// Tries to parse the input source script, if possible
        /// </summary>
        /// <param name="source">The input script to validate</param>
        /// <param name="validationResult">The <see cref="SyntaxValidationResult"/> instance with the results of the parsing operation</param>
        /// <returns>The resulting buffer of operators for the parsed script</returns>
        [Pure]
        internal static PinnedUnmanagedMemoryOwner<byte>? TryParseInDebugMode(string source, out SyntaxValidationResult validationResult)
        {
            // Check the syntax of the input source code
            validationResult = ValidateSyntax(source);

            if (!validationResult.IsSuccess) return null;

            // Allocate the buffer of binary items with the input operators
            PinnedUnmanagedMemoryOwner<byte> operators = PinnedUnmanagedMemoryOwner<byte>.Allocate(validationResult.OperatorsCount, false);

            // Extract all the operators from the input source code
            ref byte r0 = ref MemoryMarshal.GetReference(OperatorsLookupTable);
            for (int i = 0, j = 0; j < source.Length; j++)
            {
                // Explicitly get the lookup value to avoid a repeated memory access
                char c = source[j];
                int
                    diff = OperatorsLookupTableMaxIndex - c,
                    sign = diff & (1 << 31),
                    mask = ~(sign >> 31),
                    offset = c & mask;
                byte r1 = Unsafe.Add(ref r0, offset);

                // If the current character is an operator, convert and store it
                if (r1 != 0xFF) operators[i++] = r1;
            }

            return operators;
        }

        /// <summary>
        /// Gets the corresponding <see cref="char"/> value for a given processed Brainf*ck/PBrain operator
        /// </summary>
        /// <param name="opcode">The input processed operator to convert</param>
        /// <returns>The character representing the input operator</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static char GetCharacterFromOperator(byte opcode)
        {
            DebugGuard.MustBeGreaterThanOrEqualTo(opcode, 0, nameof(opcode));
            DebugGuard.MustBeLessThan(opcode, OperatorsLookupTable.Length, nameof(opcode));

            ref byte r0 = ref MemoryMarshal.GetReference(OperatorsInverseLookupTable);
            byte r1 = Unsafe.Add(ref r0, opcode);

            return (char) r1;
        }

        /// <summary>
        /// Extracts the compacted source code from a given sequence of operators
        /// </summary>
        /// <param name="operators">The input sequence of parsed operators to read</param>
        /// <returns>A <see cref="string"/> representing the input sequence of operators</returns>
        [Pure]
        internal static unsafe string ExtractSource(UnmanagedSpan<byte> operators)
        {
            // Rent a buffer to use to build the final string
            using StackOnlyUnmanagedMemoryOwner<char> characters = StackOnlyUnmanagedMemoryOwner<char>.Allocate(operators.Size);

            ref char targetRef = ref characters.GetReference();
            ref byte lookupRef = ref MemoryMarshal.GetReference(OperatorsInverseLookupTable);

            // Build the source string with the inverse operators lookup table
            for (int i = 0; i < operators.Size; i++)
            {
                byte code = Unsafe.Add(ref lookupRef, operators[i]);
                Unsafe.Add(ref targetRef, i) = (char)code;
            }

            // Allocate the new string from the rented buffer
            fixed (char* p = &targetRef)
            {
                return new string(p, 0, operators.Size);
            }
        }
    }
}
