using System;
using Microsoft.HockeyApp;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.Helpers.Settings;
using Brainf_ck_sharp_UWP.Helpers.WindowsAPIs;
using Brainf_ck_sharp_UWP.Messages.Actions;
using Brainf_ck_sharp_UWP.Resources;
using Brainf_ck_sharp_UWP.SQLiteDatabase;
using Brainf_ck_sharp_UWP.UserControls;
using GalaSoft.MvvmLight.Messaging;
using UICompositionAnimations.Helpers;
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

            // Hide the splash screen
            if (startup) await Task.Delay(AppSettingsManager.Instance.GetValue<int>(nameof(AppSettingsKeys.StartingPage)) == 1 ? 250 : 100); // Delay to hide the animations
            Window.Current.Activate();
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
            StatusBarHelper.HideAsync().Forget();

            // Setup the view mode
            ApplicationView view = ApplicationView.GetForCurrentView();
            view.SetDesiredBoundsMode(ApplicationViewBoundsMode.UseVisible);
            if (ApiInformationHelper.IsMobileDevice)
            {
                view.VisibleBoundsChanged += (s, _) => UpdateVisibleBounds(s);
                Task.Delay(1000).ContinueWith(t => UpdateVisibleBounds(ApplicationView.GetForCurrentView()), TaskScheduler.FromCurrentSynchronizationContext());
            }


            // Enable the key listener
            KeyEventsListener.IsEnabled = true;
            Window.Current.Content = shell;
            return true;
        }

        public static Shell DefaultContent => Window.Current.Content.To<Shell>();

        private void UpdateVisibleBounds(ApplicationView sender)
        {
            // Return if the content hasn't finished loading yet
            if (DefaultContent == null) return;

            // Close the open flyout if the navigation bar has been changed
            if (sender.Orientation == ApplicationViewOrientation.Portrait)
            {
                double navBarHeight = Window.Current.Bounds.Height - sender.VisibleBounds.Bottom;
                if (navBarHeight < 0) navBarHeight = 0;

                // Adjust the app UI
                DefaultContent.Margin = new Thickness(0, 0, 0, navBarHeight);
            }
            else
            {
                // Return if the status bar is still visible
                Rect windowBounds = Window.Current.Bounds;
                if (!StatusBarHelper.OccludedHeight.EqualsWithDelta(0)) return;

                // Set the left margin if the device orientation is left
                if (sender.VisibleBounds.Left.EqualsWithDelta(0) && sender.VisibleBounds.Right < windowBounds.Width)
                {
                    double navBarWidth = windowBounds.Width - sender.VisibleBounds.Right;
                    DefaultContent.Margin = new Thickness(0, 0, navBarWidth, 0);
                }
                else if (sender.VisibleBounds.Left > 0 && sender.VisibleBounds.Width < windowBounds.Width)
                {
                    // Adjust the right margin in the case the orientation is right
                    DefaultContent.Margin = new Thickness(sender.VisibleBounds.Left, 0, 0, 0);
                }
                else DefaultContent.Margin = new Thickness();
            }
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
