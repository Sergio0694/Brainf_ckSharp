using Brainf_ckSharp.Uwp.Enums.Abstract;

namespace Brainf_ckSharp.Uwp.Enums
{
    /// <summary>
    /// An <see cref="object"/> based <see langword="enum"/> that indicates the kind of info to show about an interpreter session
    /// </summary>
    /// <remarks>This type is not a value type to avoid repeated boxing</remarks>
    public sealed class IdeResultSection : Box<IdeResultSection.Entry>
    {
        /// <inheritdoc/>
        private IdeResultSection(Entry entry) : base(entry) { }

        /// <summary>
        /// The type of exception raised
        /// </summary>
        public static readonly IdeResultSection ExceptionType = Entry.ExceptionType;

        /// <summary>
        /// The error location
        /// </summary>
        public static readonly IdeResultSection ErrorLocation = Entry.ErrorLocation;

        /// <summary>
        /// The currently hit breakpoint
        /// </summary>
        public static readonly IdeResultSection BreakpointReached = Entry.BreakpointReached;

        /// <summary>
        /// The stack trace currently available
        /// </summary>
        public static readonly IdeResultSection StackTrace = Entry.StackTrace;

        /// <summary>
        /// The stdout buffer
        /// </summary>
        public static readonly IdeResultSection Stdout = Entry.Stdout;

        /// <summary>
        /// The executed source code
        /// </summary>
        public static readonly IdeResultSection SourceCode = Entry.SourceCode;

        /// <summary>
        /// The defined functions in the current script
        /// </summary>
        public static readonly IdeResultSection FunctionDefinitions = Entry.FunctionDefinitions;

        /// <summary>
        /// The current memory state
        /// </summary>
        public static readonly IdeResultSection MemoryState = Entry.MemoryState;

        /// <summary>
        /// The statistics for the current session
        /// </summary>
        public static readonly IdeResultSection Statistics = Entry.Statistics;

        /// <inheritdoc cref="IdeResultSection(Entry)"/>
        public static implicit operator IdeResultSection(Entry entry)
        {
            return new IdeResultSection(entry);
        }

        /// <summary>
        /// The actual <see langword="enum"/> indicating an option for <see cref="IdeResultSection"/>
        /// </summary>
        public enum Entry
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
            Statistics,
        }
    }
}
