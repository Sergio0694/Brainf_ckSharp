using System;
using System.Buffers;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Internal;

namespace Brainf_ckSharp
{
    /// <summary>
    /// A <see langword="class"/> responsible for parsing and validating Brainf*ck/PBrain scripts
    /// </summary>
    public static partial class Brainf_ckParser
    {
        /// <summary>
        /// Tries to parse the input source script, if possible
        /// </summary>
        /// <param name="source">The input script to validate</param>
        /// <param name="validationResult">The <see cref="SyntaxValidationResult"/> instance with the results of the parsing operation</param>
        /// <returns>The resulting buffer of operators for the parsed script</returns>
        [Pure]
        internal static unsafe PinnedUnmanagedMemoryOwner<Brainf_ckOperation>? TryParseInReleaseMode(string source, out SyntaxValidationResult validationResult)
        {
            // Check the syntax of the input source code
            validationResult = ValidateSyntax(source);

            if (!validationResult.IsSuccess) return null;

            // Allocate the buffer of binary items with the input operations
            using PinnedUnmanagedMemoryOwner<Brainf_ckOperation> buffer = PinnedUnmanagedMemoryOwner<Brainf_ckOperation>.Allocate(validationResult.OperatorsCount, false);

            ref char sourceRef = ref MemoryMarshal.GetReference(source.AsSpan());
            int i = 0, j = 0;

            // Find the index of the first operator
            while (!IsOperator(Unsafe.Add(ref sourceRef, j))) j++;

            /* Initialize the first found operator to optimize the second loop.
             * This makes it so that it's possible to skip checks on the running
             * variable to see whether a past operator has been found already.
             * The access to the operators table is safe at this point because
             * the previous while loop guarantees that the current character
             * is an operator, and therefore also a valid lookup index. */
            ref byte r0 = ref MemoryMarshal.GetReference(OperatorsLookupTable);
            byte currentOperator = Unsafe.Add(ref r0, Unsafe.Add(ref sourceRef, j));
            int currentCount = 1;

            // Extract all the operators from the input source code
            for (; j < source.Length; j++)
            {
                char c = Unsafe.Add(ref sourceRef, j);
                int
                    diff = OperatorsLookupTableMaxIndex - c,
                    sign = diff & (1 << 31),
                    mask = ~(sign >> 31),
                    offset = c & mask;
                byte r1 = Unsafe.Add(ref r0, offset);

                // Check if the character is an operator
                if (r1 == 0xFF) continue;

                // Accumulate the current operator or finalize the operation
                if (r1 == currentOperator) currentCount++;
                else
                {
                    buffer[i++] = new Brainf_ckOperation(currentOperator, currentCount);

                    // Start the new sequence compression
                    currentOperator = r1;
                    currentCount = 1;
                }
            }

            PinnedUnmanagedMemoryOwner<Brainf_ckOperation> operations = PinnedUnmanagedMemoryOwner<Brainf_ckOperation>.Allocate(i, false);

            /* Copy the compressed operators to the trimmed buffer.
             * This is necessary because the source buffer was sized for
             * the worst case scenario where the source contained just operators
             * with no repeated pairs, and without any comments at all. */
            new Span<Brainf_ckOperation>(buffer.GetPointer(), i).CopyTo(new Span<Brainf_ckOperation>(operations.GetPointer(), operations.Size));

            return operations;
        }
    }
}
