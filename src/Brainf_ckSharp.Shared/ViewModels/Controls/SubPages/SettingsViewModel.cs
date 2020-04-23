using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Enums;
using Brainf_ckSharp.Shared.Extensions.Microsoft.Toolkit.Collections;
using Brainf_ckSharp.Shared.Models;
using Brainf_ckSharp.Shared.ViewModels.Abstract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Collections;
using Microsoft.Toolkit.Mvvm.DependencyInjection;

namespace Brainf_ckSharp.Shared.ViewModels.Controls.SubPages
{
    public sealed class SettingsViewModel : ViewModelBase<ObservableGroupedCollection<SettingsSection, SettingsViewModelWithSectionInfo>>
    {
        /// <summary>
        /// Creates a new <see cref="SettingsViewModel"/> instance
        /// </summary>
        public SettingsViewModel()
        {
            Source.Add(SettingsSection.Ide, new SettingsViewModelWithSectionInfo(SettingsSection.Ide, this));
            Source.Add(SettingsSection.UI, new SettingsViewModelWithSectionInfo(SettingsSection.UI, this));
            Source.Add(SettingsSection.Interpreter, new SettingsViewModelWithSectionInfo(SettingsSection.Interpreter, this));
        }

        /// <summary>
        /// Gets a setting with the specified name
        /// </summary>
        /// <typeparam name="T">The type of setting value to retrieve</typeparam>
        /// <param name="name">The setting key to retrieve</param>
        /// <returns>The value of the setting with the specified name</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T Get<T>(string name)
        {
            return Ioc.Default.GetRequiredService<ISettingsService>().GetValue<T>(name);
        }

        /// <summary>
        /// A proxy for <see cref="Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject.Set{T}"/> that
        /// also overwrites the value stored in the local settings when a property changes
        /// </summary>
        /// <typeparam name="T">The type of setting to set</typeparam>
        /// <param name="oldValue">The previous setting value</param>
        /// <param name="value">The new value to set</param>
        /// <param name="name">The name of the setting that changed</param>
        private new void Set<T>(ref T oldValue, T value, [CallerMemberName] string name = null!)
        {
            if (base.Set(ref oldValue, value, name))
            {
                Ioc.Default.GetRequiredService<ISettingsService>().SetValue(name, value);
            }
        }

        private bool _AutosaveDocuments = Get<bool>(nameof(AutosaveDocuments));

        /// <summary>
        /// Exposes the <see cref="SettingsKeys.AutosaveDocuments"/> setting
        /// </summary>
        public bool AutosaveDocuments
        {
            get => _AutosaveDocuments;
            set => Set(ref _AutosaveDocuments, value);
        }

        private bool _ProtectUnsavedChanges = Get<bool>(nameof(ProtectUnsavedChanges));

        /// <summary>
        /// Exposes the <see cref="SettingsKeys.ProtectUnsavedChanges"/> setting
        /// </summary>
        public bool ProtectUnsavedChanges
        {
            get => _ProtectUnsavedChanges;
            set => Set(ref _ProtectUnsavedChanges, value);
        }

        private bool _AutoindentBrackets = Get<bool>(nameof(AutoindentBrackets));

        /// <summary>
        /// Exposes the <see cref="SettingsKeys.AutoindentBrackets"/> setting
        /// </summary>
        public bool AutoindentBrackets
        {
            get => _AutoindentBrackets;
            set => Set(ref _AutoindentBrackets, value);
        }

        private bool _BracketsOnNewLine = Get<bool>(nameof(BracketsOnNewLine));

        /// <summary>
        /// Exposes the <see cref="SettingsKeys.BracketsOnNewLine"/> setting
        /// </summary>
        public bool BracketsOnNewLine
        {
            get => _BracketsOnNewLine;
            set => Set(ref _BracketsOnNewLine, value);
        }

        /// <summary>
        /// Gets the collection of the available tab lengths
        /// </summary>
        public IReadOnlyCollection<int> TabLengthOptions { get; } = new[] { 4, 6, 8, 10, 12 };

        private int _TabLength = Get<int>(nameof(TabLength));

        /// <summary>
        /// Exposes the <see cref="SettingsKeys.TabLength"/> setting
        /// </summary>
        public int TabLength
        {
            get => _TabLength;
            set => Set(ref _TabLength, value);
        }

        private bool _RenderWhitespaces = Get<bool>(nameof(RenderWhitespaces));

        /// <summary>
        /// Exposes the <see cref="SettingsKeys.RenderWhitespaces"/> setting
        /// </summary>
        public bool RenderWhitespaces
        {
            get => _RenderWhitespaces;
            set => Set(ref _RenderWhitespaces, value);
        }

        private bool _EnableTimeline = Get<bool>(nameof(EnableTimeline));

        /// <summary>
        /// Exposes the <see cref="SettingsKeys.EnableTimeline"/> setting
        /// </summary>
        public bool EnableTimeline
        {
            get => _EnableTimeline;
            set => Set(ref _EnableTimeline, value);
        }

        private int _StartingPage = Get<int>(nameof(StartingPage));

        /// <summary>
        /// Exposes the <see cref="SettingsKeys.StartingPage"/> setting
        /// </summary>
        public int StartingPage
        {
            get => _StartingPage;
            set => Set(ref _StartingPage, value);
        }

        private bool _ClearStdinBufferOnRequest = Get<bool>(nameof(ClearStdinBufferOnRequest));

        /// <summary>
        /// Exposes the <see cref="SettingsKeys.ClearStdinBufferOnRequest"/> setting
        /// </summary>
        public bool ClearStdinBufferOnRequest
        {
            get => _ClearStdinBufferOnRequest;
            set => Set(ref _ClearStdinBufferOnRequest, value);
        }

        private bool _ShowPBrainButtons = Get<bool>(nameof(ShowPBrainButtons));

        /// <summary>
        /// Exposes the <see cref="SettingsKeys.ShowPBrainButtons"/> setting
        /// </summary>
        public bool ShowPBrainButtons
        {
            get => _ShowPBrainButtons;
            set => Set(ref _ShowPBrainButtons, value);
        }

        private OverflowMode _OverflowMode = Get<OverflowMode>(nameof(OverflowMode));

        /// <summary>
        /// Exposes the <see cref="SettingsKeys.OverflowMode"/> setting
        /// </summary>
        public OverflowMode OverflowMode
        {
            get => _OverflowMode;
            set => Set(ref _OverflowMode, value);
        }

        /// <summary>
        /// Gets the collection of the available tab lengths
        /// </summary>
        public IReadOnlyCollection<int> MemorySizeOptions { get; } = new[] { 32, 64, 128, 256 };

        private int _MemorySize = Get<int>(nameof(MemorySize));

        /// <summary>
        /// Exposes the <see cref="SettingsKeys.MemorySize"/> setting
        /// </summary>
        public int MemorySize
        {
            get => _MemorySize;
            set => Set(ref _MemorySize, value);
        }
    }
}
