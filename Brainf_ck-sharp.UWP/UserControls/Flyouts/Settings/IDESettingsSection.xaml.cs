using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.ViewModels.FlyoutsViewModels.Settings;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts.Settings
{
    public sealed partial class IDESettingsSection : UserControl
    {
        public IDESettingsSection()
        {
            this.InitializeComponent();
        }

        // Invokes the method to try to purchase the IDE themes pack
        private void PurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            DataContext.To<SettingsViewModel>()?.TryPurchaseThemesPackAsync();
        }
    }
}
