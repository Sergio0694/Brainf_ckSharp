namespace Brainf_ckSharp.Uwp.Enums
{
    /// <summary>
    /// An <see cref="object"/> based <see langword="enum"/> that indicates the kind of info to show about an interpreter session
    /// </summary>
    /// <remarks>This type is not a value type to avoid repeated boxing</remarks>
    public sealed class InterpreterSessionSection
    {
        /// <summary>
        /// The type of exception raised
        /// </summary>
        public static readonly InterpreterSessionSection ExceptionType = new InterpreterSessionSection();

        /// <summary>
        /// The error location
        /// </summary>
        public static readonly InterpreterSessionSection ErrorLocation = new InterpreterSessionSection();

        /// <summary>
        /// The currently hit breakpoint
        /// </summary>
        public static readonly InterpreterSessionSection BreakpointReached = new InterpreterSessionSection();

        /// <summary>
        /// The stack trace currently available
        /// </summary>
        public static readonly InterpreterSessionSection StackTrace = new InterpreterSessionSection();

        /// <summary>
        /// The stdout buffer
        /// </summary>
        public static readonly InterpreterSessionSection Stdout = new InterpreterSessionSection();

        /// <summary>
        /// The executed source code
        /// </summary>
        public static readonly InterpreterSessionSection SourceCode = new InterpreterSessionSection();

        /// <summary>
        /// The defined functions in the current script
        /// </summary>
        public static readonly InterpreterSessionSection FunctionDefinitions = new InterpreterSessionSection();

        /// <summary>
        /// The current memory state
        /// </summary>
        public static readonly InterpreterSessionSection MemoryState = new InterpreterSessionSection();

        /// <summary>
        /// The statistics for the current session
        /// </summary>
        public static readonly InterpreterSessionSection Statistics = new InterpreterSessionSection();
    }
}
