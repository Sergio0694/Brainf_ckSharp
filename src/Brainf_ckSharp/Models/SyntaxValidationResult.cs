using System;
using Brainf_ckSharp.Enums;
using static System.Diagnostics.Debug;

namespace Brainf_ckSharp.Models;

/// <summary>
/// A model that represents the result of a parsing operation on a given source file
/// </summary>
public readonly struct SyntaxValidationResult : IEquatable<SyntaxValidationResult>
{
    /// <summary>
    /// Creates a new <see cref="SyntaxValidationResult"/> instance with the specified parameters
    /// </summary>
    /// <param name="error">The syntax error for the current source file, if any</param>
    /// <param name="offset">The index of the parsing error, if present</param>
    /// <param name="operatorsCount">The total number of Brainf*ck/PBrain operators in the original source file</param>
    internal SyntaxValidationResult(SyntaxError error, int offset, int operatorsCount = -1)
    {
        Assert(offset >= -1);
        Assert(operatorsCount >= -1);
        Assert(operatorsCount >= 0 || error != SyntaxError.None);

        ErrorType = error;
        ErrorOffset = offset;
        OperatorsCount = operatorsCount;
    }

    /// <summary>
    /// Gets whether or not the input source file has been parsed successfully
    /// </summary>
    public bool IsSuccess => ErrorType == SyntaxError.None;

    /// <summary>
    /// Gets whether or not the input source file has been parsed successfully
    /// </summary>
    public bool IsEmptyScript => ErrorType == SyntaxError.MissingOperators;

    /// <summary>
    /// Gets whether the input source file was either valid, or with no operators to execute
    /// </summary>
    public bool IsSuccessOrEmptyScript => ErrorType == SyntaxError.None ||
                                          ErrorType == SyntaxError.MissingOperators;

    /// <summary>
    /// Gets whether or not the input source file has not been parsed successfully
    /// </summary>
    public bool IsError => ErrorType != SyntaxError.None &&
                           ErrorType != SyntaxError.MissingOperators;

    /// <summary>
    /// Gets the specific syntax error that caused the source file not to be parsed correctly, if any
    /// </summary>
    public SyntaxError ErrorType { get; }

    /// <summary>
    /// Gets the position of the parsing error if present, otherwise -1
    /// </summary>
    public int ErrorOffset { get; }

    /// <summary>
    /// Gets the total number of Brainf*ck/PBrain operators in the original source file
    /// </summary>
    /// <remarks>This property is set to -1 when the syntax parsing does not complete successfully</remarks>
    public int OperatorsCount { get; }

    /// <summary>
    /// Checks whether or not two <see cref="SyntaxValidationResult"/> instances are equal
    /// </summary>
    /// <param name="a">The first <see cref="SyntaxValidationResult"/> instance to compare</param>
    /// <param name="b">The second <see cref="SyntaxValidationResult"/> instance to compare</param>
    /// <returns><see langword="true"/> if the two input <see cref="SyntaxValidationResult"/> are equal, <see langword="false"/> otherwise</returns>
    public static bool operator ==(SyntaxValidationResult a, SyntaxValidationResult b) => a.Equals(b);

    /// <summary>
    /// Checks whether or not two <see cref="SyntaxValidationResult"/> instances are not equal
    /// </summary>
    /// <param name="a">The first <see cref="SyntaxValidationResult"/> instance to compare</param>
    /// <param name="b">The second <see cref="SyntaxValidationResult"/> instance to compare</param>
    /// <returns><see langword="true"/> if the two input <see cref="SyntaxValidationResult"/> are not equal, <see langword="false"/> otherwise</returns>
    public static bool operator !=(SyntaxValidationResult a, SyntaxValidationResult b) => !a.Equals(b);

    /// <inheritdoc/>
    public bool Equals(SyntaxValidationResult other)
    {
        return
            ErrorType == other.ErrorType &&
            ErrorOffset == other.ErrorOffset &&
            OperatorsCount == other.OperatorsCount;
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
        return obj is SyntaxValidationResult result && Equals(result);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(ErrorType, ErrorOffset, OperatorsCount);
    }
}
