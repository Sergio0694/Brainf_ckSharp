using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Shared.Enums;
using Microsoft.Toolkit.HighPerformance.Extensions;

#nullable enable

namespace Brainf_ckSharp.Uwp.TemplateSelectors
{
    /// <summary>
    /// A template selector for settings sections
    /// </summary>
    public sealed class SettingsSectionTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> for the IDE settings
        /// </summary>
        public DataTemplate? IdeSettingsTemplate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> for the UI settings
        /// </summary>
        public DataTemplate? UISettingsTemplate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> for the interpreter settings
        /// </summary>
        public DataTemplate? InterpreterSettingsTemplate { get; set; }

        /// <inheritdoc/>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item.TryUnbox(out SettingsSection section))
            {
                return section switch
                {
                    SettingsSection.Ide => IdeSettingsTemplate,
                    SettingsSection.UI => UISettingsTemplate,
                    SettingsSection.Interpreter => InterpreterSettingsTemplate,
                    _ => throw new ArgumentException($"Invalid section: {section}")
                } ?? throw new ArgumentException($"Missing template for section: {section}");
            }
            else throw new ArgumentException("The input item is null or of an invalid type");
        }
    }
}
