using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.ViewModels.FlyoutsViewModels;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts
{
    public sealed partial class SettingsPanelFlyout : UserControl
    {
        public SettingsPanelFlyout()
        {
            this.InitializeComponent();
            DataContext = new SettingsPanelFlyoutViewModel();
        }

        public SettingsPanelFlyoutViewModel ViewModel => DataContext.To<SettingsPanelFlyoutViewModel>();
    }
}
