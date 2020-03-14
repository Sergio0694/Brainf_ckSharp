using System;
using System.Diagnostics.Contracts;
using Brainf_ckSharp.Uwp.Enums;

namespace Brainf_ckSharp.Uwp.Converters.SubPages
{
    /// <summary>
    /// A <see langword="class"/> with helper functions to format <see cref="IdeResultSection.Entry"/> instances
    /// </summary>
    public static class IdeResultSectionConverter
    {
        /// <summary>
        /// Converts an <see cref="IdeResultSection.Entry"/> instance into its representation
        /// </summary>
        /// <param name="section">The input <see cref="IdeResultSection.Entry"/> instance</param>
        /// <returns>A <see cref="string"/> representing the input <see cref="IdeResultSection.Entry"/> instance</returns>
        [Pure]
        public static string Convert(IdeResultSection.Entry section)
        {
            return section switch
            {
                IdeResultSection.Entry.ExceptionType => "Exception type",
                IdeResultSection.Entry.ErrorLocation => "Error location",
                IdeResultSection.Entry.BreakpointReached => "Breakpoint reached",
                IdeResultSection.Entry.StackTrace => "Stack trace",
                IdeResultSection.Entry.Stdout => "Stdout",
                IdeResultSection.Entry.SourceCode => "Source code",
                IdeResultSection.Entry.FunctionDefinitions => "Function definitions",
                IdeResultSection.Entry.MemoryState => "Memory state",
                IdeResultSection.Entry.Statistics => "Statistics",
                _ => throw new ArgumentException($"Invalid input section: {section}", nameof(section))
            };
        }
    }
}
