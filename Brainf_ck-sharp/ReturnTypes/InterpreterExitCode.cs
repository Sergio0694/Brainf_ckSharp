using System;

namespace Brainf_ck_sharp.ReturnTypes
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

        /// <summary>
        /// The script execution was halted after reaching a breakpoint
        /// </summary>
        BreakpointReached = 1 << 9,

        /// <summary>
        /// The script tried to move back from the first memory location
        /// </summary>
        LowerBoundExceeded = 1 << 10,

        /// <summary>
        /// The script tried to move over the last memory location
        /// </summary>
        UpperBoundExceeded = 1 << 11,

        /// <summary>
        /// The script tried to lower the value of a memory cell set to 0
        /// </summary>
        NegativeValue = 1 << 12,

        /// <summary>
        /// The script tried to increase the value of a memory cell that had the maximum allowed value
        /// </summary>
        MaxValueExceeded = 1 << 13,

        /// <summary>
        /// The script requested another input character when the available buffer was empty
        /// </summary>
        StdinBufferExhausted = 1 << 14,

        /// <summary>
        /// The script tried to print too many characters in the output buffer
        /// </summary>
        StdoutBufferLimitExceeded = 1 << 15
    }
}