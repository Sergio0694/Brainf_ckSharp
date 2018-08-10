using System.Text.RegularExpressions;
using Microsoft.HockeyApp;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Brainf_ck_sharp_UWP.DataModels.SQLite;
using Brainf_ck_sharp_UWP.Enums;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.Helpers.Settings;
using Brainf_ck_sharp_UWP.Helpers.WindowsAPIs;
using Brainf_ck_sharp_UWP.Messages;
using Brainf_ck_sharp_UWP.Messages.Actions;
using Brainf_ck_sharp_UWP.Messages.UI;
using Brainf_ck_sharp_UWP.PopupService;
using Brainf_ck_sharp_UWP.Resources;
using Brainf_ck_sharp_UWP.SQLiteDatabase;
using Brainf_ck_sharp_UWP.UserControls;
using GalaSoft.MvvmLight.Messaging;
using UICompositionAnimations.Lights;
#if DEBUG
using System.Diagnostics;
#endif

namespace Brainf_ck_sharp_UWP
{
    /// <summary>
    /// Fornisci un comportamento specifico dell'applicazione in supplemento alla classe Application predefinita.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Inizializza l'oggetto Application singleton. Si tratta della prima riga del codice creato
        /// creato e, come tale, corrisponde all'equivalente logico di main() o WinMain().
        /// </summary>
        public App()
        {
#if DEBUG
            if (!Debugger.IsAttached)
                HockeyClient.Current.Configure("d992b6490330446db870404084b19c39");
#else
            HockeyClient.Current.Configure("d992b6490330446db870404084b19c39");
#endif
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Richiamato quando l'applicazione viene avviata normalmente dall'utente. All'avvio dell'applicazione
        /// verranno usati altri punti di ingresso per aprire un file specifico.
        /// </summary>
        /// <param name="e">Dettagli sulla richiesta e sul processo di avvio.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Language test
#if DEBUG
            Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "en-US";
#endif

            bool startup = InitializeUI();

            // Additional setup steps
            Task.Run(() => SQLiteManager.Instance.TrySyncSharedCodesAsync()).Forget();
            if (AppSettingsManager.Instance.GetValue<bool>(nameof(AppSettingsKeys.EnableTimeline))) TimelineManager.IsEnabled = true;

            // Hide the splash screen
            if (startup) if (AppSettingsManager.Instance.GetValue<int>(nameof(AppSettingsKeys.StartingPage)) == 1) await Task.Delay(250); // Delay to hide the animations
            Window.Current.Activate();
        }

        /// <inheritdoc cref="Application"/>
        protected override async void OnActivated(IActivatedEventArgs e)
        {
            // UI setup
            bool startup = InitializeUI();

            // Handle the requested code
            if (e.Kind == ActivationKind.Protocol && e is ProtocolActivatedEventArgs args && args.Uri.Host.Equals("ide"))
            {
                Match match = Regex.Match(args.Uri.Query, "id=([0-9a-f-]{36})");
                if (match.Success)
                {
                    Messenger.Default.Send(new AppLoadingStatusChangedMessage(true, startup));
                    if (AppSettingsManager.Instance.GetValue<int>(nameof(AppSettingsKeys.StartingPage)) == 0 || !startup) Messenger.Default.Send(new IDEDisplayRequestMessage());
                    if (startup) await Task.Delay(500); // Increased delay to wait for the loading UI to be shown
                    Window.Current.Activate(); // Hide the splash screen
                    await SQLiteManager.Instance.TrySyncSharedCodesAsync();
                    CategorizedSourceCode code = await SQLiteManager.Instance.TryLoadSavedCodeAsync(match.Groups[1].Value);
                    if (code != null) Messenger.Default.Send(new SourceCodeLoadingRequestedMessage(code, ShourceCodeLoadingSource.Timeline));
                    else
                    {
                        Messenger.Default.Send(new AppLoadingStatusChangedMessage(false));
                        NotificationsManager.Instance.ShowDefaultErrorNotification(LocalizationManager.GetResource("CodeNotFoundTitle"), LocalizationManager.GetResource("CodeNotFoundBody"));
                    }
                }
            }
        }

        /// <summary>
        /// Initializes the window content, if necessary
        /// </summary>
        private bool InitializeUI()
        {
            if (Window.Current.Content is Shell) return false;
            
            // Settings
            AppSettingsManager.Instance.InitializeSettings();
            AppSettingsManager.Instance.IncrementStartupsCount();

            // Initialize the UI
            BrushResourcesManager.InitializeOrRefreshInstance();
            LightsSourceHelper.Initialize(
                () => new PointerPositionSpotLight { Shade = 0x60 },
                () => new PointerPositionSpotLight
                {
                    IdAppendage = "[Wide]",
                    Z = 30,
                    Shade = 0x10
                });
            Shell shell = new Shell();
            LightsSourceHelper.SetIsLightsContainer(shell, true);

            // Handle the UI
            TitleBarHelper.StyleAppTitleBar();

            // Setup the view mode
            ApplicationView view = ApplicationView.GetForCurrentView();
            view.SetDesiredBoundsMode(ApplicationViewBoundsMode.UseVisible);

            // Enable the key listener
            KeyEventsListener.IsEnabled = true;
            Window.Current.Content = shell;
            return true;
        }

        /// <summary>
        /// Richiamato quando l'esecuzione dell'applicazione viene sospesa. Lo stato dell'applicazione viene salvato
        /// senza che sia noto se l'applicazione verrà terminata o ripresa con il contenuto
        /// della memoria ancora integro.
        /// </summary>
        /// <param name="sender">Origine della richiesta di sospensione.</param>
        /// <param name="e">Dettagli relativi alla richiesta di sospensione.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
            if (AppSettingsManager.Instance.GetValue<bool>(nameof(AppSettingsKeys.AutosaveDocuments)))
            {
                // Waits for the autosave to be completed
                IDEAutosaveTriggeredMessage message = new IDEAutosaveTriggeredMessage();
                Messenger.Default.Send(message);
                await message.Autosave;
            }
            deferral.Complete();
        }
    }
}
