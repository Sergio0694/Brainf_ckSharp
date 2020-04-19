using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Brainf_ckSharp.Uwp.Controls.Host;
using Brainf_ckSharp.Uwp.Helpers.UI;
using Brainf_ckSharp.Uwp.Services;
using Brainf_ckSharp.Uwp.Services.Settings;
using Microsoft.Toolkit.Mvvm.DependencyInjection;

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

        /// <inheritdoc/>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Initialize the UI if needed
            if (!(Window.Current.Content is Shell))
            {
                // Initialize the necessary services
                ServicesManager.InitializeServices();
                Ioc.Default.GetInstance<ISettingsService>().EnsureDefaults();

                // Initial UI styling
                TitleBarHelper.ExpandViewIntoTitleBar();
                TitleBarHelper.StyleTitleBar();

                Window.Current.Content = new Shell();
            }

            // Activate the window when launching the app
            if (e.PrelaunchActivated == false)
            {
                CoreApplication.EnablePrelaunch(true);

                Window.Current.Activate();
            }
        }
    }
}
