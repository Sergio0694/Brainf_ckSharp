using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Brainf_ckSharp.Shared.Enums;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages;

namespace Brainf_ckSharp.Uwp.Converters.SubPages
{
    /// <summary>
    /// A <see langword="class"/> with helper functions to format <see cref="SettingsSection"/> values
    /// </summary>
    public static class SettingsSectionConverters
    {
        /// <summary>
        /// Converts a <see cref="SettingsSection"/> value into its representation
        /// </summary>
        /// <param name="section">The input <see cref="SettingsSection"/> value</param>
        /// <returns>A <see cref="string"/> representing the input <see cref="SettingsSection"/> value</returns>
        [Pure]
        public static string ConvertSectionName(SettingsSection section)
        {
            return section switch
            {
                SettingsSection.Ide => "IDE",
                SettingsSection.UI => "UI",
                SettingsSection.Interpreter => "Interpreter",
                _ => throw new ArgumentException($"Invalid settings section: {section}", nameof(section))
            };
        }

        /// <summary>
        /// Converts a <see cref="SettingsSection"/> value into its description representation
        /// </summary>
        /// <param name="section">The input <see cref="SettingsSection"/> value</param>
        /// <returns>A <see cref="string"/> representing the input <see cref="SettingsSection"/> value</returns>
        [Pure]
        public static string ConvertSectionDescription(SettingsSection section)
        {
            int numberOfProperties = (
                from property in typeof(SettingsSubPageViewModel).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                let attribute = property.GetCustomAttribute<SettingsSubPageViewModel.SettingPropertyAttribute>()
                where attribute?.Section == section
                select property).Count();

            return section switch
            {
                SettingsSection.Ide => $"{numberOfProperties} settings available",
                SettingsSection.UI => $"{numberOfProperties} settings available",
                SettingsSection.Interpreter => $"{numberOfProperties} settings available",
                _ => throw new ArgumentException($"Invalid settings section: {section}", nameof(section))
            };
        }
    }
}
