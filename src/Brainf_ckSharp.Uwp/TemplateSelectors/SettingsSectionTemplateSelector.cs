using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections;

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
            return item switch
            {
                IdeSettingsSectionViewModel _ => IdeSettingsTemplate,
                UISettingsSectionViewModel _ => UISettingsTemplate,
                InterpreterSettingsSectionViewModel _ => InterpreterSettingsTemplate,
                null => throw new ArgumentNullException(nameof(item), "The input item can't be null"),
                _ => throw new ArgumentException($"Invalid section: {item}", nameof(item))
            } ?? throw new ArgumentException($"Missing template for section: {item}", nameof(item));
        }
    }
}
