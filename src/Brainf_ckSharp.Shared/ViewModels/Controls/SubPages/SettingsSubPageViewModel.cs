using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Services.Enums;
using Brainf_ckSharp.Shared.Enums;
using Brainf_ckSharp.Shared.Enums.Settings;
using Brainf_ckSharp.Shared.Extensions.Microsoft.Toolkit.Collections;
using Brainf_ckSharp.Shared.Messages.Settings;
using Brainf_ckSharp.Shared.ViewModels.Abstract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Collections;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;

#nullable enable

namespace Brainf_ckSharp.Shared.ViewModels.Controls.SubPages
{
    public sealed class SettingsSubPageViewModel : ViewModelBase<ObservableGroupedCollection<SettingsSection, SettingsSection>>
    {
        /// <summary>
        /// The <see cref="IAnalyticsService"/> instance currently in use
        /// </summary>
        private readonly IAnalyticsService AnalyticsService = Ioc.Default.GetRequiredService<IAnalyticsService>();

        /// <summary>
        /// The <see cref="IStoreService"/> instance currently in use
        /// </summary>
        private readonly IStoreService StoreService = Ioc.Default.GetRequiredService<IStoreService>();

        /// <summary>
        /// The <see cref="ISettingsService"/> instance currently in use
        /// </summary>
        private readonly ISettingsService SettingsService = Ioc.Default.GetRequiredService<ISettingsService>();

        /// <summary>
        /// Creates a new <see cref="SettingsSubPageViewModel"/> instance
        /// </summary>
        public SettingsSubPageViewModel()
        {
            _AutoindentBrackets = SettingsService.GetValue<bool>(SettingsKeys.AutoindentBrackets);
            _IdeTheme = SettingsService.GetValue<IdeTheme>(SettingsKeys.IdeTheme);
            _BracketsFormattingStyle = SettingsService.GetValue<BracketsFormattingStyle>(SettingsKeys.BracketsFormattingStyle);
            _RenderWhitespaces = SettingsService.GetValue<bool>(SettingsKeys.RenderWhitespaces);
            _StartingView = SettingsService.GetValue<ViewType>(SettingsKeys.StartingView);
            _ClearStdinBufferOnRequest = SettingsService.GetValue<bool>(SettingsKeys.ClearStdinBufferOnRequest);
            _ShowPBrainButtons = SettingsService.GetValue<bool>(SettingsKeys.ShowPBrainButtons);
            _OverflowMode = SettingsService.GetValue<OverflowMode>(SettingsKeys.OverflowMode);
            _MemorySize = SettingsService.GetValue<int>(SettingsKeys.MemorySize);

            InitializeCommand = new AsyncRelayCommand(InitializeAsync);
            UnlockThemesSelectorCommand = new AsyncRelayCommand(TryUnlockThemesSelectorAsync);

            Source.Add(SettingsSection.Ide, SettingsSection.Ide);
            Source.Add(SettingsSection.UI, SettingsSection.UI);
            Source.Add(SettingsSection.Interpreter, SettingsSection.Interpreter);
        }

        /// <summary>
        /// Gets the <see cref="ICommand"/> instance responsible for initializing the view model
        /// </summary>
        public ICommand InitializeCommand { get; }

        /// <summary>
        /// Gets the <see cref="ICommand"/> instance responsible for unlocking the themes selector
        /// </summary>
        public ICommand UnlockThemesSelectorCommand { get; }

        private bool _AutoindentBrackets;

        /// <summary>
        /// Exposes the <see cref="SettingsKeys.AutoindentBrackets"/> setting
        /// </summary>
        [SettingProperty(SettingsSection.Ide)]
        public bool AutoindentBrackets
        {
            get => _AutoindentBrackets;
            set => Set(ref _AutoindentBrackets, value);
        }

        private IdeTheme _IdeTheme;

        /// <summary>
        /// Exposes the <see cref="SettingsKeys.IdeTheme"/> setting
        /// </summary>
        [SettingProperty(SettingsSection.Ide)]
        public IdeTheme IdeTheme
        {
            get => _IdeTheme;
            set
            {
                if (Set<IdeTheme, IdeThemeSettingChangedMessage>(ref _IdeTheme, value))
                {
                    AnalyticsService.Log(Constants.Events.ThemeChanged, (nameof(Enums.Settings.IdeTheme), value.ToString()));
                }
            }
        }

        private static bool _IsThemeSelectorAvailable;

        /// <summary>
        /// Gets whether or not the selector for <see cref="IdeTheme"/> can be used
        /// </summary>
        public bool IsThemeSelectorAvailable
        {
            get => _IsThemeSelectorAvailable;
            private set => Set(ref _IsThemeSelectorAvailable, value);
        }

        private BracketsFormattingStyle _BracketsFormattingStyle;

        /// <summary>
        /// Exposes the <see cref="SettingsKeys.BracketsFormattingStyle"/> setting
        /// </summary>
        [SettingProperty(SettingsSection.Ide)]
        public BracketsFormattingStyle BracketsFormattingStyle
        {
            get => _BracketsFormattingStyle;
            set => Set<BracketsFormattingStyle, BracketsFormattingStyleSettingsChangedMessage>(ref _BracketsFormattingStyle, value);
        }

        private bool _RenderWhitespaces;

        /// <summary>
        /// Exposes the <see cref="SettingsKeys.RenderWhitespaces"/> setting
        /// </summary>
        [SettingProperty(SettingsSection.Ide)]
        public bool RenderWhitespaces
        {
            get => _RenderWhitespaces;
            set => Set<bool, RenderWhitespacesSettingChangedMessage>(ref _RenderWhitespaces, value);
        }

        private ViewType _StartingView;

        /// <summary>
        /// Exposes the <see cref="SettingsKeys.StartingView"/> setting
        /// </summary>
        [SettingProperty(SettingsSection.UI)]
        public ViewType StartingView
        {
            get => _StartingView;
            set => Set(ref _StartingView, value);
        }

        private bool _ClearStdinBufferOnRequest;

        /// <summary>
        /// Exposes the <see cref="SettingsKeys.ClearStdinBufferOnRequest"/> setting
        /// </summary>
        [SettingProperty(SettingsSection.UI)]
        public bool ClearStdinBufferOnRequest
        {
            get => _ClearStdinBufferOnRequest;
            set => Set(ref _ClearStdinBufferOnRequest, value);
        }

        private bool _ShowPBrainButtons;

        /// <summary>
        /// Exposes the <see cref="SettingsKeys.ShowPBrainButtons"/> setting
        /// </summary>
        [SettingProperty(SettingsSection.UI)]
        public bool ShowPBrainButtons
        {
            get => _ShowPBrainButtons;
            set => Set<bool, ShowPBrainButtonsSettingsChangedMessage>(ref _ShowPBrainButtons, value);
        }

        private OverflowMode _OverflowMode;

        /// <summary>
        /// Exposes the <see cref="SettingsKeys.OverflowMode"/> setting
        /// </summary>
        [SettingProperty(SettingsSection.Interpreter)]
        public OverflowMode OverflowMode
        {
            get => _OverflowMode;
            set => Set<OverflowMode, OverflowModeSettingChangedMessage>(ref _OverflowMode, value);
        }

        /// <summary>
        /// Gets the collection of the available tab lengths
        /// </summary>
        public IReadOnlyCollection<int> MemorySizeOptions { get; } = new[] { 32, 64, 128, 256 };

        private int _MemorySize;

        /// <summary>
        /// Exposes the <see cref="SettingsKeys.MemorySize"/> setting
        /// </summary>
        [SettingProperty(SettingsSection.Interpreter)]
        public int MemorySize
        {
            get => _MemorySize;
            set => Set<int, MemorySizeSettingChangedMessage>(ref _MemorySize, value);
        }

        /// <summary>
        /// Initializes the current view model
        /// </summary>
        private async Task InitializeAsync()
        {
            if (!IsThemeSelectorAvailable)
            {
                const string id = Constants.Store.StoreIds.IAPs.UnlockThemes;

                IsThemeSelectorAvailable = await StoreService.IsProductPurchasedAsync(id);
            }
        }

        /// <summary>
        /// Prompts the user to unlock the themes selector
        /// </summary>
        private async Task TryUnlockThemesSelectorAsync()
        {
            const string id = Constants.Store.StoreIds.IAPs.UnlockThemes;

            var result = await StoreService.TryPurchaseProductAsync(id);

            IsThemeSelectorAvailable = result == StorePurchaseResult.Success ||
                                       result == StorePurchaseResult.AlreadyPurchased;

            AnalyticsService.Log(Constants.Events.ThemesUnlockRequest, (nameof(StorePurchaseResult), result.ToString()));
        }

        /// <summary>
        /// A proxy for <see cref="Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject.Set{T}(ref T, T, string)"/> that
        /// also overwrites the value stored in the local settings when a property changes
        /// </summary>
        /// <typeparam name="T">The type of setting to set</typeparam>
        /// <param name="field">The previous setting value</param>
        /// <param name="value">The new value to set</param>
        /// <param name="name">The name of the setting that changed</param>
        private new void Set<T>(ref T field, T value, [CallerMemberName] string name = null!)
        {
            if (base.Set(ref field, value, name))
            {
                SettingsService.SetValue(name, value);
            }
        }

        /// <summary>
        /// A proxy for <see cref="Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject.Set{T}(ref T, T, string)"/> that
        /// also overwrites the value stored in the local settings when a property changes and broadcasts a message
        /// </summary>
        /// <typeparam name="T">The type of setting to set</typeparam>
        /// <typeparam name="TMessage">The type of message to broadcast</typeparam>
        /// <param name="field">The previous setting value</param>
        /// <param name="value">The new value to set</param>
        /// <param name="name">The name of the setting that changed</param>
        private bool Set<T, TMessage>(ref T field, T value, [CallerMemberName] string name = null!)
            where TMessage : ValueChangedMessage<T>
        {
            if (base.Set(ref field, value, name))
            {
                SettingsService.SetValue(name, value);

                TMessage message = (TMessage)Activator.CreateInstance(typeof(TMessage), value);

                Messenger.Send(message);

                return true;
            }

            return false;
        }

        /// <summary>
        /// A custom attribute that indicates a setting property
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public sealed class SettingPropertyAttribute : Attribute
        {
            /// <summary>
            /// Creates a new <see cref="SettingPropertyAttribute"/> instance with the specified parameters
            /// </summary>
            /// <param name="section">The setting section this setting belongs to</param>
            public SettingPropertyAttribute(SettingsSection section)
            {
                Section = section;
            }

            /// <summary>
            /// Gets the current setting section in use
            /// </summary>
            public SettingsSection Section { get; }
        }
    }
}
