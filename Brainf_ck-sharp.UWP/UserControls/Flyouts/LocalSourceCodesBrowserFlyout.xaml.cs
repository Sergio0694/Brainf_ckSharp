using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.ViewModels;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts
{
    public sealed partial class LocalSourceCodesBrowserFlyout : UserControl
    {
        public LocalSourceCodesBrowserFlyout()
        {
            this.InitializeComponent();
            DataContext = new LocalSourceCodesBrowserFlyoutViewModel();
        }

        public LocalSourceCodesBrowserFlyoutViewModel ViewModel => DataContext.To<LocalSourceCodesBrowserFlyoutViewModel>();
    }
}
