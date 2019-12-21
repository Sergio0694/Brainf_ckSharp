using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;
using Brainf_ck_sharp.Legacy.UWP.Helpers.UI;
using Brainf_ck_sharp.Legacy.UWP.PopupService;
using Brainf_ck_sharp.Legacy.UWP.UserControls.Flyouts.UserGuide;

namespace Brainf_ck_sharp.Legacy.UWP.UserControls.Flyouts.Settings.Sections
{
    public sealed partial class UISettingsSection : UserControl
    {
        public UISettingsSection()
        {
            this.InitializeComponent();
        }

        // Shows the PBrain guide section
        private async void PBrainLinkButton_Click(object sender, RoutedEventArgs e)
        {
            UserGuideViewerControl guide = new UserGuideViewerControl();
            FlyoutManager.Instance.ShowAsync(LocalizationManager.GetResource("UserGuide"), guide, null, new Thickness()).Forget();
            await Task.Delay(600);
            guide.TryScrollToPBrainSection();
        }
    }
}
