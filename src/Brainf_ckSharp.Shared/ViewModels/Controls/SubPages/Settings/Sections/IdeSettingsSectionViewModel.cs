using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Services.Enums;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Enums.Settings;
using Brainf_ckSharp.Shared.Messages.Settings;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections.Abstract;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections;

public sealed class IdeSettingsSectionViewModel : SettingsSectionViewModelBase
{
    /// <summary>
    /// The <see cref="IAnalyticsService"/> instance currently in use
    /// </summary>
    private readonly IAnalyticsService AnalyticsService;

    /// <summary>
    /// The <see cref="IStoreService"/> instance currently in use
    /// </summary>
    private readonly IStoreService StoreService;

    /// <summary>
    /// The <see cref="AppConfiguration"/> instance currently in use
    /// </summary>
    private readonly AppConfiguration Configuration;

    /// <summary>
    /// Creates a new <see cref="IdeSettingsSectionViewModel"/> instance
    /// </summary>
    /// <param name="messenger">The <see cref="IMessenger"/> instance to use</param>
    /// <param name="analyticsService">The <see cref="IAnalyticsService"/> instance to use</param>
    /// <param name="storeService">The <see cref="IStoreService"/> instance to use</param>
    /// <param name="settingsService">The <see cref="ISettingsService"/> instance to use</param>
    /// <param name="configuration">The <see cref="AppConfiguration"/> instance to use</param>
    public IdeSettingsSectionViewModel(IMessenger messenger, IAnalyticsService analyticsService, IStoreService storeService, ISettingsService settingsService, AppConfiguration configuration)
        : base(messenger, settingsService)
    {
        AnalyticsService = analyticsService;
        StoreService = storeService;
        Configuration = configuration;

        _IdeTheme = SettingsService.GetValue<IdeTheme>(SettingsKeys.IdeTheme);
        _BracketsFormattingStyle = SettingsService.GetValue<BracketsFormattingStyle>(SettingsKeys.BracketsFormattingStyle);
        _RenderWhitespaces = SettingsService.GetValue<bool>(SettingsKeys.RenderWhitespaces);

        InitializeCommand = new AsyncRelayCommand(InitializeAsync);
        UnlockThemesSelectorCommand = new AsyncRelayCommand(TryUnlockThemesSelectorAsync);
    }

    /// <summary>
    /// Gets the <see cref="ICommand"/> instance responsible for initializing the view model
    /// </summary>
    public ICommand InitializeCommand { get; }

    /// <summary>
    /// Gets the <see cref="ICommand"/> instance responsible for unlocking the themes selector
    /// </summary>
    public ICommand UnlockThemesSelectorCommand { get; }

    /// <summary>
    /// Gets the available themes.
    /// </summary>
    public IReadOnlyCollection<IdeTheme> IdeThemes { get; } = (IdeTheme[])typeof(IdeTheme).GetEnumValues();

    private IdeTheme _IdeTheme;

    /// <summary>
    /// Exposes the <see cref="SettingsKeys.IdeTheme"/> setting
    /// </summary>
    public IdeTheme IdeTheme
    {
        get => _IdeTheme;
        set
        {
            if (SetProperty<IdeTheme, IdeThemeSettingChangedMessage>(ref _IdeTheme, value))
            {
                AnalyticsService.Log(EventNames.ThemeChanged, (nameof(Enums.Settings.IdeTheme), value.ToString()));
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
        private set => SetProperty(ref _IsThemeSelectorAvailable, value);
    }

    /// <summary>
    /// Gets the available bracket formatting styles.
    /// </summary>
    public IReadOnlyCollection<BracketsFormattingStyle> BracketsFormattingStyles { get; } = (BracketsFormattingStyle[])typeof(BracketsFormattingStyle).GetEnumValues();

    private BracketsFormattingStyle _BracketsFormattingStyle;

    /// <summary>
    /// Exposes the <see cref="SettingsKeys.BracketsFormattingStyle"/> setting
    /// </summary>
    public BracketsFormattingStyle BracketsFormattingStyle
    {
        get => _BracketsFormattingStyle;
        set => SetProperty<BracketsFormattingStyle, BracketsFormattingStyleSettingsChangedMessage>(ref _BracketsFormattingStyle, value);
    }

    private bool _RenderWhitespaces;

    /// <summary>
    /// Exposes the <see cref="SettingsKeys.RenderWhitespaces"/> setting
    /// </summary>
    public bool RenderWhitespaces
    {
        get => _RenderWhitespaces;
        set => SetProperty<bool, RenderWhitespacesSettingChangedMessage>(ref _RenderWhitespaces, value);
    }

    /// <summary>
    /// Initializes the current view model
    /// </summary>
    private async Task InitializeAsync()
    {
        if (!IsThemeSelectorAvailable)
        {
            Guard.IsNotNull(Configuration.UnlockThemesIapId, nameof(AppConfiguration.UnlockThemesIapId));

            IsThemeSelectorAvailable = await StoreService.IsProductPurchasedAsync(Configuration.UnlockThemesIapId);
        }
    }

    /// <summary>
    /// Prompts the user to unlock the themes selector
    /// </summary>
    private async Task TryUnlockThemesSelectorAsync()
    {
        Guard.IsNotNull(Configuration.UnlockThemesIapId, nameof(AppConfiguration.UnlockThemesIapId));

        var result = await StoreService.TryPurchaseProductAsync(Configuration.UnlockThemesIapId);

        IsThemeSelectorAvailable = result == StorePurchaseResult.Success ||
                                   result == StorePurchaseResult.AlreadyPurchased;

        AnalyticsService.Log(EventNames.ThemesUnlockRequest, (nameof(StorePurchaseResult), result.ToString()));
    }
}
