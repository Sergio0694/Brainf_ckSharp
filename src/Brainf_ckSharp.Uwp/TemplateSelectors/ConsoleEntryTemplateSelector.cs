using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Shared.Models.Console;
using Microsoft.Toolkit.Diagnostics;

#nullable enable

namespace Brainf_ckSharp.Uwp.TemplateSelectors
{
    /// <summary>
    /// A template selector for console entries
    /// </summary>
    public sealed class ConsoleEntryTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> for console commands
        /// </summary>
        public DataTemplate? CommandTemplate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> for console textual results
        /// </summary>
        public DataTemplate? ResultTemplate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> for console syntax errors
        /// </summary>
        public DataTemplate? SyntaxErrorTemplate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> for console exceptions
        /// </summary>
        public DataTemplate? ExceptionTemplate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> for console restart requests
        /// </summary>
        public DataTemplate? RestartTemplate { get; set; }

        /// <inheritdoc/>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            Guard.IsNotNull(item, nameof(item));

            DataTemplate? template = item switch
            {
                ConsoleCommand _ => CommandTemplate,
                ConsoleResult _ => ResultTemplate,
                ConsoleSyntaxError _ => SyntaxErrorTemplate,
                ConsoleException _ => ExceptionTemplate,
                ConsoleRestart _ => RestartTemplate,
                _ => ThrowHelper.ThrowArgumentException<DataTemplate>("Invalid input item type")
            };

            if (template is null)
            {
                ThrowHelper.ThrowInvalidOperationException("The requested template is null");
            }

            return template;
        }
    }
}
