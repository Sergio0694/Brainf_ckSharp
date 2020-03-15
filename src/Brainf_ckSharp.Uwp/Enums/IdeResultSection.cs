namespace Brainf_ckSharp.Uwp.Enums
{
    /// <summary>
    /// An <see langword="enum"/> that indicates the kind of info to show about an interpreter session
    /// </summary>
    public enum IdeResultSection
    {
        /// <summary>
        /// The type of exception raised
        /// </summary>
        ExceptionType,

        /// <summary>
        /// The error location
        /// </summary>
        ErrorLocation,

        /// <summary>
        /// The currently hit breakpoint
        /// </summary>
        BreakpointReached,

        /// <summary>
        /// The stack trace currently available
        /// </summary>
        StackTrace,

        /// <summary>
        /// The stdout buffer
        /// </summary>
        Stdout,

        /// <summary>
        /// The executed source code
        /// </summary>
        SourceCode,

        /// <summary>
        /// The defined functions in the current script
        /// </summary>
        FunctionDefinitions,

        /// <summary>
        /// The current memory state
        /// </summary>
        MemoryState,

        /// <summary>
        /// The statistics for the current session
        /// </summary>
        Statistics
    }
}
