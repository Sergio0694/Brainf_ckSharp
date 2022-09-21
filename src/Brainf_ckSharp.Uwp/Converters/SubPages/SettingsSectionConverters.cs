using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using Brainf_ckSharp.Shared.Enums;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections.Abstract;
using Microsoft.Toolkit.Collections;
using Microsoft.Toolkit.Uwp;

namespace Brainf_ckSharp.Uwp.Converters.SubPages;

/// <summary>
/// A <see langword="class"/> with helper functions to format <see cref="SettingsSection"/> values
/// </summary>
public static class SettingsSectionConverters
{
    /// <summary>
    /// Converts a <see cref="IReadOnlyObservableGroup"/> value into its description representation
    /// </summary>
    /// <param name="section">The input <see cref="IReadOnlyObservableGroup"/> value</param>
    /// <returns>A <see cref="string"/> representing the input <see cref="IReadOnlyObservableGroup"/> value</returns>
    [Pure]
    public static string ConvertSectionDescription(IReadOnlyObservableGroup section)
    {
        Type viewModelType = ((ObservableGroup<SettingsSection, SettingsSectionViewModelBase>)section)[0].GetType();

        int numberOfProperties = viewModelType.GetProperties(BindingFlags.Instance | BindingFlags.Public).Length;

        return string.Format("Settings/SettingsAvailable".GetLocalized(), numberOfProperties);
    }
}
