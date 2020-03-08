using System;
using System.Buffers;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Opcodes;

namespace Brainf_ckSharp
{
    /// <summary>
    /// A <see langword="class"/> responsible for parsing and validating Brainf*ck/PBrain scripts
    /// </summary>
    public static partial class Brainf_ckParser
    {
        /// <summary>
        /// A <see langword="class"/> implementing parsing methods for the DEBUG configuration
        /// </summary>
        private static class Debug
        {
            /// <summary>
            /// Tries to parse the input source script, if possible
            /// </summary>
            /// <param name="source">The input script to validate</param>
            /// <param name="validationResult">The <see cref="SyntaxValidationResult"/> instance with the results of the parsing operation</param>
            /// <returns>The resulting buffer of operators for the parsed script</returns>
            [Pure]
            public static PinnedUnmanagedMemoryOwner<Brainf_ckOperator>? TryParse(string source, out SyntaxValidationResult validationResult)
            {
                // Check the syntax of the input source code
                validationResult = ValidateSyntax(source);

                if (!validationResult.IsSuccess) return null;

                // Allocate the buffer of binary items with the input operators
                PinnedUnmanagedMemoryOwner<Brainf_ckOperator> operators = PinnedUnmanagedMemoryOwner<Brainf_ckOperator>.Allocate(validationResult.OperatorsCount, false);

                // Extract all the operators from the input source code
                ref char sourceRef = ref MemoryMarshal.GetReference(source.AsSpan());
                ref byte opsRef = ref Unsafe.As<Brainf_ckOperator, byte>(ref operators.GetReference());
                for (int i = 0, j = 0; j < source.Length; j++)
                {
                    char c = Unsafe.Add(ref sourceRef, j);
                    ref byte op = ref Unsafe.Add(ref opsRef, i);

                    if (TryParseOperator(c, out op)) i++;
                }

                return operators;
            }

            /// <summary>
            /// Extracts the compacted source code from a given sequence of operators
            /// </summary>
            /// <param name="operators">The input sequence of parsed operators to read</param>
            /// <returns>A <see cref="string"/> representing the input sequence of operators</returns>
            [Pure]
            public static unsafe string ExtractSource(UnmanagedSpan<Brainf_ckOperator> operators)
            {
                // Rent a buffer to use to build the final string
                using StackOnlyUnmanagedMemoryOwner<char> characters = StackOnlyUnmanagedMemoryOwner<char>.Allocate(operators.Size);

                ref char targetRef = ref characters.GetReference();
                ref byte lookupRef = ref MemoryMarshal.GetReference(OperatorsInverseLookupTable);

                // Build the source string with the inverse operators lookup table
                for (int i = 0; i < operators.Size; i++)
                {
                    byte code = Unsafe.Add(ref lookupRef, operators[i].Operator);
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
}
