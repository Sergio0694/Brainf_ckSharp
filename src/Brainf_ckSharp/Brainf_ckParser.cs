using System;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Models;
using CommunityToolkit.HighPerformance;

namespace Brainf_ckSharp;

/// <summary>
/// A <see langword="class"/> responsible for parsing and validating Brainf*ck/PBrain scripts
/// </summary>
public static partial class Brainf_ckParser
{
    /// <summary>
    /// A lookup table to quickly check characters
    /// </summary>
    private static ReadOnlySpan<byte> OperatorsLookupTable => new byte[]
    {
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        Operators.FunctionStart, 
        Operators.FunctionEnd,
        0xFF,
        Operators.Plus,
        Operators.ReadChar,
        Operators.Minus,
        Operators.PrintChar,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        Operators.FunctionCall,
        0xFF,
        Operators.BackwardPtr,
        0xFF,
        Operators.ForwardPtr,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        Operators.LoopStart,
        0xFF,
        Operators.LoopEnd
    };

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
    /// Checks whether or not an input character is a Brainf*ck/PBrain operator
    /// </summary>
    /// <param name="c">The input character to check</param>
    /// <returns><see langword="true"/> if the input character is a Brainf*ck/PBrain operator, <see langword="false"/> otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsOperator(char c)
    {
        return OperatorsLookupTable.DangerousGetLookupReferenceAt(c) != 0xFF;
    }

    /// <summary>
    /// Checks whether or not an input character is a Brainf*ck/PBrain operator
    /// </summary>
    /// <param name="c">The input character to check</param>
    /// <param name="op">The resulting operator, if <paramref name="c"/> was valid</param>
    /// <returns><see langword="true"/> if the input character is a Brainf*ck/PBrain operator, <see langword="false"/> otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool TryParseOperator(char c, out byte op)
    {
        op = OperatorsLookupTable.DangerousGetLookupReferenceAt(c);

        return op != 0xFF;
    }

    /// <summary>
    /// Checks whether or not the syntax of the input script is valid
    /// </summary>
    /// <param name="source">The input script to validate</param>
    /// <returns>A <see cref="SyntaxValidationResult"/> instance with the results of the parsing operation</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SyntaxValidationResult ValidateSyntax(string source) => ValidateSyntax(source.AsSpan());

    /// <summary>
    /// Checks whether or not the syntax of the input script is valid
    /// </summary>
    /// <param name="span">A <see cref="ReadOnlySpan{T}"/> instance with the input script to validate</param>
    /// <returns>A <see cref="SyntaxValidationResult"/> instance with the results of the parsing operation</returns>
    public static SyntaxValidationResult ValidateSyntax(ReadOnlySpan<char> span)
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

        for (int i = 0; i < span.Length; i++)
        {
            switch (span[i])
            {
                case Characters.Plus:
                case Characters.Minus:
                case Characters.ForwardPtr:
                case Characters.BackwardPtr:
                case Characters.PrintChar:
                case Characters.ReadChar:
                case Characters.FunctionCall:

                    // For action operators, simply increase the counter if the current
                    // parser is inside a function definition. The counter is used to
                    // validate function definition without having to iterate again
                    // over the span of characters contained in the definition
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

                    // Decrease the current depth level, either in the standard
                    // code flow or inside a function definition. If the current
                    // depth level is already 0, the source code is invalid
                    if (functionStart == -1)
                    {
                        if (rootDepth == 0) return new(SyntaxError.MismatchedSquareBracket, i);
                        totalOps++;
                        rootDepth--;
                    }
                    else
                    {
                        if (functionDepth == 0) return new(SyntaxError.MismatchedSquareBracket, i);
                        totalOps++;
                        functionDepth--;
                        functionOps++;
                    }
                    break;
                case Characters.FunctionStart:

                    // Start a function definition, track the index and reset the counter
                    if (rootDepth != 0) return new(SyntaxError.InvalidFunctionDeclaration, i);
                    if (functionStart != -1) return new(SyntaxError.NestedFunctionDeclaration, i);
                    totalOps++;
                    functionStart = i;
                    functionDepth = 0;
                    functionLoopStart = -1;
                    functionOps = 0;
                    break;
                case Characters.FunctionEnd:

                    // Validate the function definition and reset the index
                    if (functionStart == -1) return new(SyntaxError.MismatchedParenthesis, i);
                    if (functionDepth != 0) return new(SyntaxError.MismatchedSquareBracket, functionLoopStart);
                    if (functionOps == 0) return new(SyntaxError.EmptyFunctionDeclaration, i);
                    totalOps++;
                    functionStart = -1;
                    break;
            }
        }

        // Handle the remaining failure cases:
        //   - An incomplete function declaration, when the user missed the closing parenthesis
        //   - A missing square bracket for one of the loops in the main script
        //   - No operators present in the source file
        if (functionStart != -1) return new(SyntaxError.IncompleteFunctionDeclaration, functionStart);
        if (rootDepth != 0) return new(SyntaxError.IncompleteLoop, outerLoopStart);
        if (totalOps == 0) return new(SyntaxError.MissingOperators, -1, 0);

        return new(SyntaxError.None, -1, totalOps);
    }
}
