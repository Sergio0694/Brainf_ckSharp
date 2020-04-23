using System;
using System.Diagnostics.Contracts;
using Brainf_ckSharp.Shared.Enums;

namespace Brainf_ckSharp.Uwp.Converters.SubPages
{
    /// <summary>
    /// A <see langword="class"/> with helper functions to format <see cref="IdeResultSection"/> values
    /// </summary>
    public static class IdeResultSectionConverter
    {
        /// <summary>
        /// Converts an <see cref="IdeResultSection"/> value into its representation
        /// </summary>
        /// <param name="section">The input <see cref="IdeResultSection"/> value</param>
        /// <returns>A <see cref="string"/> representing the input <see cref="IdeResultSection"/> value</returns>
        [Pure]
        public static string Convert(IdeResultSection section)
        {
            return section switch
            {
                IdeResultSection.ExceptionType => "Exception type",
                IdeResultSection.ErrorLocation => "Error location",
                IdeResultSection.BreakpointReached => "Breakpoint reached",
                IdeResultSection.StackTrace => "Stack trace",
                IdeResultSection.Stdout => "Stdout",
                IdeResultSection.SourceCode => "Source code",
                IdeResultSection.FunctionDefinitions => "Function definitions",
                IdeResultSection.MemoryState => "Memory state",
                IdeResultSection.Statistics => "Statistics",
                _ => throw new ArgumentException($"Invalid input section: {section}", nameof(section))
            };
        }
    }
}
