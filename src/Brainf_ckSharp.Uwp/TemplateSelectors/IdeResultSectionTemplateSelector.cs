using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Shared.Enums;
using Brainf_ckSharp.Shared.Models.Ide.Views;
using Microsoft.Toolkit.Diagnostics;

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
        /// Gets or sets the <see cref="DataTemplate"/> for the halting position section
        /// </summary>
        public DataTemplate? HaltingPositionTemplate { get; set; }

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
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            Guard.IsNotNull(item, nameof(item));
            Guard.IsOfType<IdeResultWithSectionInfo>(item, nameof(item));

            var model = (IdeResultWithSectionInfo)item;

            DataTemplate? template = model.Section switch
            {
                IdeResultSection.ExceptionType => ExceptionTypeTemplate,
                IdeResultSection.FaultingOperator => HaltingPositionTemplate,
                IdeResultSection.BreakpointReached => HaltingPositionTemplate,
                IdeResultSection.StackTrace => StackTraceTemplate,
                IdeResultSection.Stdout => StdoutTemplate,
                IdeResultSection.SourceCode => SourceCodeTemplate,
                IdeResultSection.FunctionDefinitions => FunctionDefinitionsTemplate,
                IdeResultSection.MemoryState => MemoryStateTemplate,
                IdeResultSection.Statistics => StatisticsTemplate,
                _ => ThrowHelper.ThrowArgumentException<DataTemplate>("Invalid requested section")
            };

            if (template is null)
            {
                ThrowHelper.ThrowInvalidOperationException("The requested template is null");
            }

            return template;
        }
    }
}
