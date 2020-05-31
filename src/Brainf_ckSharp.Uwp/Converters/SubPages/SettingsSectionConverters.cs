using System;
using System.Diagnostics.Contracts;
using Brainf_ckSharp.Shared.Enums;

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
            return section switch
            {
                SettingsSection.Ide => "7 settings available",
                SettingsSection.UI => "2 settings available",
                SettingsSection.Interpreter => "2 settings available",
                _ => throw new ArgumentException($"Invalid settings section: {section}", nameof(section))
            };
        }
    }
}
