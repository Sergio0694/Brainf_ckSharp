using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Opcodes;
using Microsoft.Toolkit.HighPerformance;
using Microsoft.Toolkit.HighPerformance.Buffers;

namespace Brainf_ckSharp;

/// <summary>
/// A <see langword="class"/> responsible for parsing and validating Brainf*ck/PBrain scripts
/// </summary>
public static partial class Brainf_ckParser
{
    /// <summary>
    /// A <see langword="class"/> implementing parsing methods for the RELEASE configuration
    /// </summary>
    private static class Release
    {
        /// <summary>
        /// Tries to parse the input source script, if possible
        /// </summary>
        /// <param name="source">The input script to validate</param>
        /// <param name="validationResult">The <see cref="SyntaxValidationResult"/> instance with the results of the parsing operation</param>
        /// <returns>The resulting buffer of operators for the parsed script</returns>
        [Pure]
        public static MemoryOwner<Brainf_ckOperation>? TryParse(ReadOnlySpan<char> source, out SyntaxValidationResult validationResult)
        {
            // Check the syntax of the input source code
            validationResult = ValidateSyntax(source);

            if (!validationResult.IsSuccess) return null;

            // Allocate the buffer of binary items with the input operations
            using SpanOwner<Brainf_ckOperation> buffer = SpanOwner<Brainf_ckOperation>.Allocate(validationResult.OperatorsCount);
            ref Brainf_ckOperation bufferRef = ref buffer.DangerousGetReference();

            ref char sourceRef = ref source.DangerousGetReference();
            int i = 0, j = 0;

            // Find the index of the first operator
            while (!IsOperator(Unsafe.Add(ref sourceRef, j))) j++;

            // Initialize the first found operator to optimize the second loop.
            // This makes it so that it's possible to skip checks on the running
            // variable to see whether a past operator has been found already.
            // The access to the operators table is safe at this point because
            // the previous while loop guarantees that the current character
            // is an operator, and therefore also a valid lookup index.
            byte currentOperator = OperatorsLookupTable.DangerousGetReferenceAt(Unsafe.Add(ref sourceRef, j));
            ushort currentCount = 1;

            // Extract all the operators from the input source code.
            // We increment j when the loop starts because that index will
            // be pointing to the first operator, which has already been tracked.
            // The search needs to start from the character right after that.
            for (j++; j < source.Length; j++)
            {
                char c = Unsafe.Add(ref sourceRef, j);

                // Check if the character is an operator
                if (!TryParseOperator(c, out byte op)) continue;

                // Accumulate the current operator or finalize the operation.
                // To check whether an operator is compressable, it is possible to
                // use a bitmap and just perform a bit shift with the current operator
                // value. Each bit in the bitmap represents one of the 11 operators, in
                // ascending order. The only compressable operators are "><+-", which
                // have a value in the [4, 7] range after the conversion to byte format.
                if (op == currentOperator &&
                    ((0b000_1111_0000 >> op) & 1) == 1 &&
                    currentCount < ushort.MaxValue)
                {
                    // This is only allowed for ><+-
                    currentCount++;
                }
                else
                {
                    Unsafe.Add(ref bufferRef, i++) = new Brainf_ckOperation(currentOperator, currentCount);

                    // Start the new sequence compression
                    currentOperator = op;
                    currentCount = 1;
                }
            }

            // Insert the last operator
            Unsafe.Add(ref bufferRef, i++) = new Brainf_ckOperation(currentOperator, currentCount);

            MemoryOwner<Brainf_ckOperation> operations = MemoryOwner<Brainf_ckOperation>.Allocate(i);

            // Copy the compressed operators to the trimmed buffer.
            // This is necessary because the source buffer was sized for
            // the worst case scenario where the source contained just operators
            // with no repeated pairs, and without any comments at all.
            buffer.Span.Slice(0, i).CopyTo(operations.Span);

            return operations;
        }

        /// <summary>
        /// Extracts the compacted source code from a given sequence of operations
        /// </summary>
        /// <param name="operations">The input sequence of parsed operations to read</param>
        /// <returns>A <see cref="string"/> representing the input sequence of operations</returns>
        [Pure]
        public static string ExtractSource(Span<Brainf_ckOperation> operations)
        {
            int size = 0;

            // Count the number of original operators
            foreach (var opcode in operations)
            {
                size += opcode.Count;
            }

            // Rent a buffer to use to build the final string
            using SpanOwner<char> characters = SpanOwner<char>.Allocate(size);

            ref char targetRef = ref characters.DangerousGetReference();
            ref byte lookupRef = ref OperatorsInverseLookupTable.DangerousGetReference();
            ref Brainf_ckOperation operationRef = ref operations.DangerousGetReference();

            // Build the source string with the inverse operators lookup table
            for (int i = 0, j = 0; i < operations.Length; i++)
            {
                Brainf_ckOperation operation = Unsafe.Add(ref operationRef, i);
                char op = (char)Unsafe.Add(ref lookupRef, operation.Operator);

                // Copy the repeated operator
                int k = 0;
                do
                {
                    Unsafe.Add(ref targetRef, j + k) = op;
                } while (++k < operation.Count);

                j += operation.Count;
            }

            return StringPool.Shared.GetOrAdd(characters.Span);
        }
    }
}
