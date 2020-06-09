using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp.Legacy.UWP.DataModels.Settings;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;

namespace Brainf_ck_sharp.Legacy.UWP.UserControls.Flyouts.Settings.Sections
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
            DataContext.To<CategorizedSettingsViewModel>()?.ViewModel.TryPurchaseThemesPackAsync();
        }
    }
}
