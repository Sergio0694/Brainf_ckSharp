﻿using Brainf_ck_sharp.NET.Enum;
using Brainf_ck_sharp.NET.Helpers;

namespace Brainf_ck_sharp.NET.Models
{
    /// <summary>
    /// A model that represents the result of a parsing operation on a given source file
    /// </summary>
    public readonly struct SyntaxValidationResult
    {
        /// <summary>
        /// Gets whether or not the input source file has been parsed successfully
        /// </summary>
        public bool IsSuccess => ErrorType == SyntaxError.None;

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
        /// Creates a new <see cref="SyntaxValidationResult"/> instaance with the specified parameters
        /// </summary>
        /// <param name="error">The syntax error for the current source file, if any</param>
        /// <param name="offset">The index of the parsing error, if present</param>
        /// <param name="operatorsCount">The total number of Brainf*ck/PBrain operators in the original source file</param>
        internal SyntaxValidationResult(SyntaxError error, int offset, int operatorsCount = -1)
        {
            DebugGuard.MustBeGreaterThanOrEqualTo(offset, -1, nameof(offset));
            DebugGuard.MustBeTrue(offset >= 0 || error == SyntaxError.None, nameof(offset));
            DebugGuard.MustBeGreaterThanOrEqualTo(operatorsCount, -1, nameof(operatorsCount));
            DebugGuard.MustBeTrue(operatorsCount >= 0 || error != SyntaxError.None, nameof(operatorsCount));

            ErrorType = error;
            ErrorOffset = offset;
            OperatorsCount = operatorsCount;
        }
    }
}