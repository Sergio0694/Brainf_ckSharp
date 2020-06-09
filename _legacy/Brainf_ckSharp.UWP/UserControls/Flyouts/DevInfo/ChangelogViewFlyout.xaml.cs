using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;
using Brainf_ck_sharp.Legacy.UWP.ViewModels.FlyoutsViewModels;

namespace Brainf_ck_sharp.Legacy.UWP.UserControls.Flyouts.DevInfo
{
    public sealed partial class ChangelogViewFlyout : UserControl
    {
        public ChangelogViewFlyout()
        {
            this.InitializeComponent();
            DataContext = new ChangelogViewFlyoutViewModel();
            Unloaded += (s, e) =>
            {
                this.Bindings.StopTracking();
                DataContext = null;
            };
        }

        public ChangelogViewFlyoutViewModel ViewModel => DataContext.To<ChangelogViewFlyoutViewModel>();
    }
}
