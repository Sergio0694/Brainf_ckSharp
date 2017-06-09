using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.WindowsAPIs;
using Brainf_ck_sharp_UWP.UserControls;

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
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        private static Shell _DefaultContent;

        /// <summary>
        /// Richiamato quando l'applicazione viene avviata normalmente dall'utente. All'avvio dell'applicazione
        /// verranno usati altri punti di ingresso per aprire un file specifico.
        /// </summary>
        /// <param name="e">Dettagli sulla richiesta e sul processo di avvio.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Shell shell = Window.Current.Content as Shell;
            if (shell == null)
            {
                // Creare un frame che agisca da contesto di navigazione e passare alla prima pagina
                shell = new Shell();

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: caricare lo stato dall'applicazione sospesa in precedenza
                }

                // Handle the UI
                if (UniversalAPIsHelper.IsMobileDevice) StatusBarHelper.HideAsync().Forget();
                else TitleBarHelper.StyleAppTitleBar();

                // Setup the view mode
                ApplicationView view = ApplicationView.GetForCurrentView();
                view.SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
                view.VisibleBoundsChanged += App_VisibleBoundsChanged;

                // Posizionare il frame nella finestra corrente
                Window.Current.Content = shell;
            }
            _DefaultContent = shell;

            Window.Current.Activate();
        }

        /// <summary>
        /// Gets or sets a value that indicates whether or not the navigation bar is visible
        /// </summary>
        private bool _NavBarVisible;

        private void App_VisibleBoundsChanged(ApplicationView sender, object args)
        {
            // Return if the content hasn't finished loading yet
            if (_DefaultContent == null) return;

            // Close the open flyout if the navigation bar has been changed
            if (sender.Orientation == ApplicationViewOrientation.Portrait)
            {
                double navBarHeight = Window.Current.Bounds.Height - sender.VisibleBounds.Bottom;
                _NavBarVisible = navBarHeight > 0;

                // Adjust the app UI
                _DefaultContent.Margin = new Thickness(0, 0, 0, navBarHeight);
            }
            else
            {
                // Return if the StatusBar is still visible
                Rect windowBounds = Window.Current.Bounds;
                if (!StatusBarHelper.OccludedHeight.EqualsWithDelta(0)) return;

                // Set the left margin if the device orientation is left
                if (sender.VisibleBounds.Left.EqualsWithDelta(0) && sender.VisibleBounds.Right < windowBounds.Width)
                {
                    double navBarWidth = windowBounds.Width - sender.VisibleBounds.Right;
                    _DefaultContent.Margin = new Thickness(0, 0, navBarWidth, 0);
                }
                else if (sender.VisibleBounds.Left > 0 && sender.VisibleBounds.Width < windowBounds.Width)
                {
                    // Adjust the right margin in the case the orientation is right
                    _DefaultContent.Margin = new Thickness(sender.VisibleBounds.Left, 0, 0, 0);
                }
                else _DefaultContent.Margin = new Thickness();
            }
        }

        /// <summary>
        /// Richiamato quando l'esecuzione dell'applicazione viene sospesa. Lo stato dell'applicazione viene salvato
        /// senza che sia noto se l'applicazione verrà terminata o ripresa con il contenuto
        /// della memoria ancora integro.
        /// </summary>
        /// <param name="sender">Origine della richiesta di sospensione.</param>
        /// <param name="e">Dettagli relativi alla richiesta di sospensione.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: salvare lo stato dell'applicazione e arrestare eventuali attività eseguite in background
            deferral.Complete();
        }
    }
}
