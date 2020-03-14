using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Uwp.Enums;

#nullable enable

namespace Brainf_ckSharp.Uwp.TemplateSelectors
{
    /// <summary>
    /// A template selector for IDE result sections
    /// </summary>
    public sealed class IdeResultSectionTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> for the exception type section
        /// </summary>
        public DataTemplate? ExceptionTypeTemplate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> for the error location section
        /// </summary>
        public DataTemplate? ErrorLocationTemplate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> for the breakpoint reached section
        /// </summary>
        public DataTemplate? BreakpointReachedTemplate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> for the stack trace section
        /// </summary>
        public DataTemplate? StackTraceTemplate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> for the stdout section
        /// </summary>
        public DataTemplate? StdoutTemplate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> for the source code section
        /// </summary>
        public DataTemplate? SourceCodeTemplate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> for the function definitions section
        /// </summary>
        public DataTemplate? FunctionDefinitionsTemplate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> for the memory state section
        /// </summary>
        public DataTemplate? MemoryStateTemplate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> for the statistics section
        /// </summary>
        public DataTemplate? StatisticsTemplate { get; set; }

        /// <inheritdoc/>
        protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container)
        {
            var section = item as IdeResultSection ?? throw new ArgumentException($"Invalid item: {item}");

            return section.Value switch
            {
                IdeResultSection.Entry.ExceptionType => ExceptionTypeTemplate,
                IdeResultSection.Entry.ErrorLocation => ErrorLocationTemplate,
                IdeResultSection.Entry.BreakpointReached => BreakpointReachedTemplate,
                IdeResultSection.Entry.StackTrace => StackTraceTemplate,
                IdeResultSection.Entry.Stdout => StdoutTemplate,
                IdeResultSection.Entry.SourceCode => SourceCodeTemplate,
                IdeResultSection.Entry.FunctionDefinitions => FunctionDefinitionsTemplate,
                IdeResultSection.Entry.MemoryState => MemoryStateTemplate,
                IdeResultSection.Entry.Statistics => StatisticsTemplate,
                _ => throw new ArgumentOutOfRangeException($"Invalid section entry: {section.Value}")
            };
        }
    }
}
