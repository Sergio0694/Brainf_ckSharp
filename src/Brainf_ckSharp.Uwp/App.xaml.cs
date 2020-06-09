using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Brainf_ckSharp.Uwp.Controls.Host;
using Brainf_ckSharp.Uwp.Helpers;

#nullable enable

namespace Brainf_ckSharp.Uwp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default <see cref="Application"/> class
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Creates a new <see cref="App"/> instance
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public new static App Current => (App)Application.Current;

        /// <inheritdoc/>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            OnActivated(e.PrelaunchActivated);
        }

        /// <inheritdoc/>
        protected override void OnFileActivated(FileActivatedEventArgs args)
        {
            OnActivated(false);

            base.OnFileActivated(args);
        }

        /// <summary>
        /// Initilizes and activates the app
        /// </summary>
        /// <param name="prelaunchActivated">Whether or not the prelaunch is enabled for the current activation</param>
        private void OnActivated(bool prelaunchActivated)
        {
            // Initialize the UI if needed
            if (!(Window.Current.Content is Shell))
            {
                // Initial UI styling
                TitleBarHelper.ExpandViewIntoTitleBar();
                TitleBarHelper.StyleTitleBar();

                Window.Current.Content = new Shell();
            }

            // Activate the window when launching the app
            if (prelaunchActivated == false)
            {
                CoreApplication.EnablePrelaunch(true);

                Window.Current.Activate();
            }
        }
    }
}
