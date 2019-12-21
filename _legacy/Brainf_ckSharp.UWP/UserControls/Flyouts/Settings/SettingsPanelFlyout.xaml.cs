using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;
using Brainf_ck_sharp.Legacy.UWP.ViewModels.FlyoutsViewModels.Settings;

namespace Brainf_ck_sharp.Legacy.UWP.UserControls.Flyouts.Settings
{
    public sealed partial class SettingsPanelFlyout : UserControl
    {
        public SettingsPanelFlyout()
        {
            this.InitializeComponent();
            DataContext = new SettingsJumpListViewModel();
            Unloaded += (s, e) =>
            {
                this.Bindings.StopTracking();
                DataContext = null;
            };
        }

        public SettingsJumpListViewModel ViewModel => DataContext.To<SettingsJumpListViewModel>();
    }
}
