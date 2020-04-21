using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Shared.Enums;
using Brainf_ckSharp.Uwp.Models.Ide.Views;

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
            var model = item as IdeResultWithSectionInfo ?? throw new ArgumentException($"Invalid item: {item}");

            return model.Section switch
            {
                IdeResultSection.ExceptionType => ExceptionTypeTemplate,
                IdeResultSection.ErrorLocation => ErrorLocationTemplate,
                IdeResultSection.BreakpointReached => BreakpointReachedTemplate,
                IdeResultSection.StackTrace => StackTraceTemplate,
                IdeResultSection.Stdout => StdoutTemplate,
                IdeResultSection.SourceCode => SourceCodeTemplate,
                IdeResultSection.FunctionDefinitions => FunctionDefinitionsTemplate,
                IdeResultSection.MemoryState => MemoryStateTemplate,
                IdeResultSection.Statistics => StatisticsTemplate,
                _ => throw new ArgumentOutOfRangeException($"Invalid section entry: {model.Section}")
            } ?? throw new ArgumentException($"Missing template for item of type {model.Section}");
        }
    }
}
