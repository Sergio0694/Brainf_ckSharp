using Microsoft.HockeyApp;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Brainf_ck_sharp_UWP.Helpers.Settings;
using Brainf_ck_sharp_UWP.Helpers.WindowsAPIs;
using Brainf_ck_sharp_UWP.Messages.Actions;
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
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Language test
#if DEBUG
            Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "en-US";
#endif

            // Initialize the window content
            if (!(Window.Current.Content is Shell))
            {
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

                // Sync the roaming source codes
                Task.Run(() => SQLiteManager.Instance.TrySyncSharedCodesAsync());
            }
            Window.Current.Activate();
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
