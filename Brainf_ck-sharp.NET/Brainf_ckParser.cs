using System;
using System.Buffers;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Brainf_ck_sharp.NET.Buffers;
using Brainf_ck_sharp.NET.Constants;
using Brainf_ck_sharp.NET.Enums;
using Brainf_ck_sharp.NET.Models;

namespace Brainf_ck_sharp.NET
{
    /// <summary>
    /// A <see langword="class"/> responsible for parsing and validating Brainf*ck/PBrain scripts
    /// </summary>
    public static class Brainf_ckParser
    {
        /// <summary>
        /// The maximum valid index in <see cref="OperatorsLookupTable"/>
        /// </summary>
        private const int OperatorsLookupTableMaxIndex = 93;

        /// <summary>
        /// A lookup table to quickly check characters
        /// </summary>
        private static ReadOnlySpan<byte> OperatorsLookupTable => new byte[]
        {
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            Operators.FunctionStart, 
            Operators.FunctionEnd,
            0xFF,
            Operators.Plus,
            Operators.ReadChar,
            Operators.Minus,
            Operators.PrintChar,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF,
            Operators.FunctionCall,
            0xFF,
            Operators.BackwardPtr,
            0xFF,
            Operators.ForwardPtr,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            Operators.LoopStart, 0xFF,
            Operators.LoopEnd
        };

        /// <summary>
        /// Checks whether or not an input character is a Brainf*ck/PBrain operator
        /// </summary>
        /// <param name="c">The input character to check</param>
        /// <returns><see langword="true"/> if the input character is a Brainf*ck/PBrain operator, <see langword="false"/> otherwise</returns>
        [Pure]
        public static bool IsOperator(char c)
        {
            int
                diff = OperatorsLookupTableMaxIndex - c,
                sign = diff & (1 << 31),
                mask = ~(sign >> 31),
                offset = c & mask;
            ref byte r0 = ref MemoryMarshal.GetReference(OperatorsLookupTable);
            byte r1 = Unsafe.Add(ref r0, offset);

            return r1 != 0xFF;
        }

        /// <summary>
        /// Checks whether or not the syntax of the input script is valid
        /// </summary>
        /// <param name="source">The input script to validate</param>
        /// <returns>A <see cref="SyntaxValidationResult"/> instance with the results of the parsing operation</returns>
        [Pure]
        public static SyntaxValidationResult IsSyntaxValid(string source)
        {
            // Local variables to track the depth and the function definitions
            int
                rootDepth = 0,
                outerLoopStart = -1,
                functionStart = -1,
                functionDepth = 0,
                functionLoopStart = -1,
                functionOps = 0,
                totalOps = 0;

            for (int i = 0; i < source.Length; i++)
            {
                switch (source[i])
                {
                    case Characters.Plus:
                    case Characters.Minus:
                    case Characters.ForwardPtr:
                    case Characters.BackwardPtr:
                    case Characters.PrintChar:
                    case Characters.ReadChar:
                    case Characters.FunctionCall:

                        /* For action operators, simply increase the counter if the current
                         * parser is inside a function definition. The counter is used to
                         * validate function definition without having to iterate again
                         * over the span of characters contained in the definition */
                        totalOps++;
                        if (functionStart != -1) functionOps++;
                        break;
                    case Characters.LoopStart:

                        // Increase the appropriate depth level
                        totalOps++;
                        if (functionStart == -1)
                        {
                            if (rootDepth == 0) outerLoopStart = i;
                            rootDepth++;
                        }
                        else
                        {
                            if (functionLoopStart == -1) functionLoopStart = i;
                            functionDepth++;
                            functionOps++;
                        }
                        break;
                    case Characters.LoopEnd:

                        /* Decrease the current depth level, either in the standard
                         * code flow or inside a function definition. If the current
                         * depth level is already 0, the source code is invalid */
                        if (functionStart == -1)
                        {
                            if (rootDepth == 0) return new SyntaxValidationResult(SyntaxError.MismatchedSquareBracket, i);
                            totalOps++;
                            rootDepth--;
                        }
                        else
                        {
                            if (functionDepth == 0) return new SyntaxValidationResult(SyntaxError.MismatchedSquareBracket, i);
                            totalOps++;
                            functionDepth--;
                            functionOps++;
                        }
                        break;
                    case Characters.FunctionStart:

                        // Start a function definition, track the index and reset the counter
                        if (rootDepth != 0) return new SyntaxValidationResult(SyntaxError.InvalidFunctionDeclaration, i);
                        if (functionStart != -1) return new SyntaxValidationResult(SyntaxError.NestedFunctionDeclaration, i);
                        totalOps++;
                        functionStart = i;
                        functionDepth = 0;
                        functionLoopStart = -1;
                        functionOps = 0;
                        break;
                    case Characters.FunctionEnd:

                        // Validate the function definition and reset the index
                        if (functionStart == -1) return new SyntaxValidationResult(SyntaxError.MismatchedParenthesis, i);
                        if (functionDepth != 0) return new SyntaxValidationResult(SyntaxError.MismatchedSquareBracket, functionLoopStart);
                        if (functionOps == 0) return new SyntaxValidationResult(SyntaxError.EmptyFunctionDeclaration, i);
                        totalOps++;
                        functionStart = -1;
                        break;
                }
            }

            /* Handle the remaining failure cases:
             *   - An incomplete function declaration, when the user missed the closing parenthesis
             *   - A missing square bracket for one of the loops in the main script
             *   - No operators present in the source file */
            if (functionStart != -1) return new SyntaxValidationResult(SyntaxError.IncompleteFunctionDeclaration, functionStart);
            if (rootDepth != 0) return new SyntaxValidationResult(SyntaxError.IncompleteLoop, outerLoopStart);
            if (totalOps == 0) return new SyntaxValidationResult(SyntaxError.MissingOperators, -1, 0);

            return new SyntaxValidationResult(SyntaxError.None, -1, totalOps);
        }

        /// <summary>
        /// Tries to parse the input source script, if possible
        /// </summary>
        /// <param name="source">The input script to validate</param>
        /// <param name="validationResult">The <see cref="SyntaxValidationResult"/> instance with the results of the parsing operation</param>
        /// <returns>The resulting buffer of operators for the parsed script</returns>
        [Pure]
        internal static UnsafeMemoryBuffer<byte>? TryParse(string source, out SyntaxValidationResult validationResult)
        {
            // Check the syntax of the input source code
            validationResult = IsSyntaxValid(source);

            if (!validationResult.IsSuccess) return null;

            // Allocate the buffer of binary items with the input operators
            UnsafeMemoryBuffer<byte> operators = UnsafeMemoryBuffer<byte>.Allocate(validationResult.OperatorsCount, false);

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
        /// Extracts the compacted source code from a given sequence of operators
        /// </summary>
        /// <param name="operators">The input sequence of parsed operators to read</param>
        /// <returns>A <see cref="string"/> representing the input sequence of operators</returns>
        [Pure]
        internal static string ExtractSource(UnsafeMemory<byte> operators)
        {
            // Rent a buffer to use to build the final string
            char[] characters = ArrayPool<char>.Shared.Rent(operators.Size);

            ref char targetRef = ref characters[0];
            ref byte lookupRef = ref MemoryMarshal.GetReference(OperatorsInverseLookupTable);

            // Build the source string with the inverse operators lookup table
            for (int i = 0; i < operators.Size; i++)
            {
                byte code = Unsafe.Add(ref lookupRef, operators[i]);
                Unsafe.Add(ref targetRef, i) = (char)code;
            }

            // Allocate the new string from the rented buffer
            string source = new string(characters, 0, operators.Size);

            ArrayPool<char>.Shared.Return(characters);

            return source;
        }
    }
}
