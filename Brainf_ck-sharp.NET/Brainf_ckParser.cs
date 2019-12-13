using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Brainf_ck_sharp.NET.Buffers;
using Brainf_ck_sharp.NET.Constants;
using Brainf_ck_sharp.NET.Enum;
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
        public const int OperatorsLookupTableMaxIndex = 94;

        /// <summary>
        /// A lookup table to quickly check characters
        /// </summary>
        public static ReadOnlySpan<byte> OperatorsLookupTable => new byte[]
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 1, 1, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // ()+,-.
            0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // :<>
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0  // []
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

            return r1 != 0;
        }

        /// <summary>
        /// Checks whether or not the syntax of the input script is valid
        /// </summary>
        /// <param name="code">The input script to validate</param>
        /// <returns>A <see cref="SyntaxValidationResult"/> instance with the results of the parsing operation</returns>
        [Pure]
        public static SyntaxValidationResult IsSyntaxValid(string code)
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

            for (int i = 0; i < code.Length; i++)
            {
                switch (code[i])
                {
                    case Operators.Plus:
                    case Operators.Minus:
                    case Operators.ForwardPtr:
                    case Operators.BackwardPtr:
                    case Operators.PrintChar:
                    case Operators.ReadChar:
                    case Operators.FunctionCall:

                        /* For action operators, simply increase the counter if the current
                         * parser is inside a function definition. The counter is used to
                         * validate function definition without having to iterate again
                         * over the span of characters contained in the definition */
                        totalOps++;
                        if (functionStart != -1) functionOps++;
                        break;
                    case Operators.LoopStart:

                        // Increase the appropriate depth level
                        totalOps++;
                        if (functionStart == -1)
                        {
                            if (functionLoopStart == -1) functionLoopStart = i;
                            rootDepth++;
                        }
                        else
                        {
                            if (rootDepth == 0) outerLoopStart = i;
                            functionDepth++;
                            functionOps++;
                        }
                        break;
                    case Operators.LoopEnd:

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
                    case Operators.FunctionStart:

                        // Start a function definition, track the index and reset the counter
                        if (functionStart != -1) return new SyntaxValidationResult(SyntaxError.NestedFunctionDeclaration, i);
                        totalOps++;
                        functionStart = i;
                        functionDepth = 0;
                        functionLoopStart = -1;
                        functionOps = 0;
                        break;
                    case Operators.FunctionEnd:

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
            if (rootDepth != 0) return new SyntaxValidationResult(SyntaxError.MismatchedSquareBracket, outerLoopStart);
            if (totalOps == 0) return new SyntaxValidationResult(SyntaxError.MissingOperators, -1, 0);

            return new SyntaxValidationResult(SyntaxError.None, -1, totalOps);
        }

        /// <summary>
        /// Tries to parse the input source script, if possible
        /// </summary>
        /// <param name="code">The input script to validate</param>
        /// <param name="operators">The resulting buffer of <see cref="Brainf_ckBinaryItem"/> instances for the parsed script</param>
        /// <returns>A <see cref="SyntaxValidationResult"/> instance with the results of the parsing operation</returns>
        internal static SyntaxValidationResult TryParse(string code, out UnsafeMemoryBuffer<Brainf_ckBinaryItem>? operators)
        {
            // Check the syntax of the input source code
            SyntaxValidationResult validationResult = IsSyntaxValid(code);

            if (validationResult.IsSuccess)
            {
                // Allocate the buffer of binary items with the input operators
                operators = UnsafeMemoryBuffer<Brainf_ckBinaryItem>.Allocate(validationResult.OperatorsCount);

                // Extract all the operators from the input source code
                for (int i = 0, j = 0; j < code.Length; j++)
                {
                    char c = code[j];
                    if (IsOperator(c))
                    {
                        operators[i] = new Brainf_ckBinaryItem(i++, c);
                    }
                }
            }
            else operators = null;

            return validationResult;
        }
    }
}
