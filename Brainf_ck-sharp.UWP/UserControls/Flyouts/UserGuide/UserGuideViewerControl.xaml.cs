using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.ViewModels.FlyoutsViewModels;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts.UserGuide
{
    public sealed partial class UserGuideViewerControl : UserControl
    {
        public UserGuideViewerControl()
        {
            this.InitializeComponent();
            this.DataContext = new UserGuideViewerControlViewModel();
            this.Unloaded += (s, e) =>
            {

                ViewModel.Cleanup();
                DataContext = null;
            };
        }

        public UserGuideViewerControlViewModel ViewModel => this.DataContext.To<UserGuideViewerControlViewModel>();
    }
}
