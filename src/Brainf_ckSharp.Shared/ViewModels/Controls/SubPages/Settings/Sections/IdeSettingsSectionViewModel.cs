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
    private readonly IAnalyticsService analyticsService;

    /// <summary>
    /// The <see cref="IStoreService"/> instance currently in use
    /// </summary>
    private readonly IStoreService storeService;

    /// <summary>
    /// The <see cref="AppConfiguration"/> instance currently in use
    /// </summary>
    private readonly AppConfiguration configuration;

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
        this.analyticsService = analyticsService;
        this.storeService = storeService;
        this.configuration = configuration;

        this.ideTheme = this.SettingsService.GetValue<IdeTheme>(SettingsKeys.IdeTheme);
        this.bracketsFormattingStyle = this.SettingsService.GetValue<BracketsFormattingStyle>(SettingsKeys.BracketsFormattingStyle);
        this.renderWhitespaces = this.SettingsService.GetValue<bool>(SettingsKeys.RenderWhitespaces);
    }

    /// <summary>
    /// Gets the available themes.
    /// </summary>
    public IReadOnlyCollection<IdeTheme> IdeThemes { get; } = (IdeTheme[])typeof(IdeTheme).GetEnumValues();

    private IdeTheme ideTheme;

    /// <summary>
    /// Exposes the <see cref="SettingsKeys.IdeTheme"/> setting
    /// </summary>
    public IdeTheme IdeTheme
    {
        get => this.ideTheme;
        set
        {
            if (SetProperty<IdeTheme, IdeThemeSettingChangedMessage>(ref this.ideTheme, value))
            {
                this.analyticsService.Log(EventNames.ThemeChanged, (nameof(Enums.Settings.IdeTheme), value.ToString()));
            }
        }
    }

    private static bool isThemeSelectorAvailable;

    /// <summary>
    /// Gets whether or not the selector for <see cref="IdeTheme"/> can be used
    /// </summary>
    public bool IsThemeSelectorAvailable
    {
        get => isThemeSelectorAvailable;
        private set => SetProperty(ref isThemeSelectorAvailable, value);
    }

    /// <summary>
    /// Gets the available bracket formatting styles.
    /// </summary>
    public IReadOnlyCollection<BracketsFormattingStyle> BracketsFormattingStyles { get; } = (BracketsFormattingStyle[])typeof(BracketsFormattingStyle).GetEnumValues();

    private BracketsFormattingStyle bracketsFormattingStyle;

    /// <summary>
    /// Exposes the <see cref="SettingsKeys.BracketsFormattingStyle"/> setting
    /// </summary>
    public BracketsFormattingStyle BracketsFormattingStyle
    {
        get => this.bracketsFormattingStyle;
        set => SetProperty<BracketsFormattingStyle, BracketsFormattingStyleSettingsChangedMessage>(ref this.bracketsFormattingStyle, value);
    }

    private bool renderWhitespaces;

    /// <summary>
    /// Exposes the <see cref="SettingsKeys.RenderWhitespaces"/> setting
    /// </summary>
    public bool RenderWhitespaces
    {
        get => this.renderWhitespaces;
        set => SetProperty<bool, RenderWhitespacesSettingChangedMessage>(ref this.renderWhitespaces, value);
    }

    /// <summary>
    /// Initializes the current view model
    /// </summary>
    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (!IsThemeSelectorAvailable)
        {
            Guard.IsNotNull(this.configuration.UnlockThemesIapId, nameof(AppConfiguration.UnlockThemesIapId));

            IsThemeSelectorAvailable = await this.storeService.IsProductPurchasedAsync(this.configuration.UnlockThemesIapId);
        }
    }

    /// <summary>
    /// Prompts the user to unlock the themes selector
    /// </summary>
    [RelayCommand]
    private async Task TryUnlockThemesSelectorAsync()
    {
        Guard.IsNotNull(this.configuration.UnlockThemesIapId, nameof(AppConfiguration.UnlockThemesIapId));

        StorePurchaseResult result = await this.storeService.TryPurchaseProductAsync(this.configuration.UnlockThemesIapId);

        IsThemeSelectorAvailable = result == StorePurchaseResult.Success ||
                                   result == StorePurchaseResult.AlreadyPurchased;

        this.analyticsService.Log(EventNames.ThemesUnlockRequest, (nameof(StorePurchaseResult), result.ToString()));
    }
}
