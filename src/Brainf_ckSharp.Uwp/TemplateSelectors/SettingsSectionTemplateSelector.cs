﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections;
using Microsoft.Toolkit.Diagnostics;

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
            Guard.IsNotNull(item, nameof(item));

            DataTemplate? template = item switch
            {
                IdeSettingsSectionViewModel _ => IdeSettingsTemplate,
                UISettingsSectionViewModel _ => UISettingsTemplate,
                InterpreterSettingsSectionViewModel _ => InterpreterSettingsTemplate,
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
