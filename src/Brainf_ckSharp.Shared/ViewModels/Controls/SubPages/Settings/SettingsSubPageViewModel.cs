using Brainf_ckSharp.Shared.Enums;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections.Abstract;
using Microsoft.Toolkit.Collections;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings
{
    public sealed class SettingsSubPageViewModel : ObservableRecipient
    {
        /// <summary>
        /// Creates a new <see cref="SettingsSubPageViewModel"/> instance
        /// </summary>
        public SettingsSubPageViewModel()
        {
            Source.AddGroup(SettingsSection.Ide, new IdeSettingsSectionViewModel());
            Source.AddGroup(SettingsSection.UI, new UISettingsSectionViewModel());
            Source.AddGroup(SettingsSection.Interpreter, new InterpreterSettingsSectionViewModel());
        }

        /// <summary>
        /// Gets the current collection of sections to display
        /// </summary>
        public ObservableGroupedCollection<SettingsSection, SettingsSectionViewModelBase> Source { get; } = new ObservableGroupedCollection<SettingsSection, SettingsSectionViewModelBase>();
    }
}
