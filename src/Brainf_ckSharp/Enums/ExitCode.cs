using System;

namespace Brainf_ckSharp.Enums;

/// <summary>
/// An <see langword="enum"/> that indicates the exit code for an interpreted Brainf*ck/PBrain script
/// </summary>
[Flags]
public enum ExitCode : uint
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
    /// The source code produced a runtime exception
    /// </summary>
    ExceptionThrown = 1 << 2| Failure,

    /// <summary>
    /// The code run into an infinite loop (according to the desired time threshold)
    /// </summary>
    ThresholdExceeded = 1 << 3 | Failure,

    /// <summary>
    /// The script execution was halted after reaching a breakpoint
    /// </summary>
    BreakpointReached = 1 << 4 | Success,

    /// <summary>
    /// The script tried to move back from the first memory location
    /// </summary>
    LowerBoundExceeded = 1 << 5 | ExceptionThrown,

    /// <summary>
    /// The script tried to move over the last memory location
    /// </summary>
    UpperBoundExceeded = 1 << 6 | ExceptionThrown,

    /// <summary>
    /// The script tried to lower the value of a memory cell set to 0
    /// </summary>
    NegativeValue = 1 << 7 | ExceptionThrown,

    /// <summary>
    /// The script tried to increase the value of a memory cell that had the maximum allowed value
    /// </summary>
    MaxValueExceeded = 1 << 8 | ExceptionThrown,

    /// <summary>
    /// The script requested another input character when the available buffer was empty
    /// </summary>
    StdinBufferExhausted = 1 << 9 | ExceptionThrown,

    /// <summary>
    /// The script tried to print too many characters in the output buffer
    /// </summary>
    StdoutBufferLimitExceeded = 1 << 10 | ExceptionThrown,

    /// <summary>
    /// The script tried to reference an undefined function
    /// </summary>
    UndefinedFunctionCalled = 1 << 11 | ExceptionThrown,

    /// <summary>
    /// The script tried to define a function with a value that was already mapped to another function
    /// </summary>
    DuplicateFunctionDefinition = 1 << 12 | ExceptionThrown,

    /// <summary>
    /// The script executed one or more recursive functions too many times
    /// </summary>
    StackLimitExceeded = 1 << 13 | ExceptionThrown
}
