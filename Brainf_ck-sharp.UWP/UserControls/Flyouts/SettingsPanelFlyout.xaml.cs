using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts
{
    public sealed partial class SettingsPanelFlyout : UserControl
    {
        public SettingsPanelFlyout()
        {
            this.InitializeComponent();
            ComboBox.ItemsSource = new[] {"Hello!", "How", "are", "you?"};
        }
    }
}
