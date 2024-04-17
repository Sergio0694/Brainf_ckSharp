using System.Collections.Generic;
using System.Threading.Tasks;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Services.Enums;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Enums.Settings;
using Brainf_ckSharp.Shared.Messages.Settings;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections.Abstract;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections;

/// <summary>
/// A viewmodel for the IDE settings section.
/// </summary>
public sealed partial class IdeSettingsSectionViewModel : SettingsSectionViewModelBase
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
        this.AnalyticsService = analyticsService;
        this.StoreService = storeService;
        this.Configuration = configuration;

        this._IdeTheme = this.SettingsService.GetValue<IdeTheme>(SettingsKeys.IdeTheme);
        this._BracketsFormattingStyle = this.SettingsService.GetValue<BracketsFormattingStyle>(SettingsKeys.BracketsFormattingStyle);
        this._RenderWhitespaces = this.SettingsService.GetValue<bool>(SettingsKeys.RenderWhitespaces);
    }

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
        get => this._IdeTheme;
        set
        {
            if (SetProperty<IdeTheme, IdeThemeSettingChangedMessage>(ref this._IdeTheme, value))
            {
                this.AnalyticsService.Log(EventNames.ThemeChanged, (nameof(Enums.Settings.IdeTheme), value.ToString()));
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
        get => this._BracketsFormattingStyle;
        set => SetProperty<BracketsFormattingStyle, BracketsFormattingStyleSettingsChangedMessage>(ref this._BracketsFormattingStyle, value);
    }

    private bool _RenderWhitespaces;

    /// <summary>
    /// Exposes the <see cref="SettingsKeys.RenderWhitespaces"/> setting
    /// </summary>
    public bool RenderWhitespaces
    {
        get => this._RenderWhitespaces;
        set => SetProperty<bool, RenderWhitespacesSettingChangedMessage>(ref this._RenderWhitespaces, value);
    }

    /// <summary>
    /// Initializes the current view model
    /// </summary>
    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (!IsThemeSelectorAvailable)
        {
            Guard.IsNotNull(this.Configuration.UnlockThemesIapId, nameof(AppConfiguration.UnlockThemesIapId));

            IsThemeSelectorAvailable = await this.StoreService.IsProductPurchasedAsync(this.Configuration.UnlockThemesIapId);
        }
    }

    /// <summary>
    /// Prompts the user to unlock the themes selector
    /// </summary>
    [RelayCommand]
    private async Task TryUnlockThemesSelectorAsync()
    {
        Guard.IsNotNull(this.Configuration.UnlockThemesIapId, nameof(AppConfiguration.UnlockThemesIapId));

        StorePurchaseResult result = await this.StoreService.TryPurchaseProductAsync(this.Configuration.UnlockThemesIapId);

        IsThemeSelectorAvailable = result == StorePurchaseResult.Success ||
                                   result == StorePurchaseResult.AlreadyPurchased;

        this.AnalyticsService.Log(EventNames.ThemesUnlockRequest, (nameof(StorePurchaseResult), result.ToString()));
    }
}
