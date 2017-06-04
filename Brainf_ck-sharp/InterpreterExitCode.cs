﻿using System;

namespace Brainf_ck_sharp
{
    /// <summary>
    /// Tipo enumerato che indica il risultato dell'interpretazione del codice
    /// </summary>
    [Flags]
    public enum InterpreterExitCode : ushort
    {
        /// <summary>
        /// The code was interpreted successfully
        /// </summary>
        Success = 1,

        /// <summary>
        /// There were issues in the code that prevented it from being run successfully
        /// </summary>
        Failure = 1 << 1,

        /// <summary>
        /// The code didn't produce any output
        /// </summary>
        NoOutput = 1 << 2,

        /// <summary>
        /// The code produced at least an output character
        /// </summary>
        TextOutput = 1 << 3,

        /// <summary>
        /// The input source code didn't contain any valid Branf_ck operators to interpret
        /// </summary>
        NoCodeInterpreted = 1 <<4,

        /// <summary>
        /// The source code produced a runtime exception
        /// </summary>
        ExceptionThrown = 1 << 5,

        /// <summary>
        /// The source code contained a syntax error and couldn't be interpreted
        /// </summary>
        MismatchedParentheses = 1 << 6,

        /// <summary>
        /// The code run into an infinite loop (according to the desired time threshold)
        /// </summary>
        ThresholdExceeded = 1 << 7,

        /// <summary>
        /// An internal interpreter exception has been thrown and automatically handled
        /// </summary>
        InternalException = 1 << 8,

        BreakpointReached = 1 << 9,

        LowerBoundExceeded = 1 << 10,
        UpperBoundExceeded = 1 << 11,
        NegativeValue = 1 << 12,
        MaxValueExceeded = 1 << 13,
        StrinBufferExhausted = 1 << 14
    }
}