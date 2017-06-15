using System;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.DataModels.SQLite;
using Brainf_ck_sharp_UWP.FlyoutService;
using Brainf_ck_sharp_UWP.FlyoutService.Interfaces;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.ViewModels;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts
{
    public sealed partial class LocalSourceCodesBrowserFlyout : UserControl, IEventConfirmedContent<SourceCode>
    {
        public LocalSourceCodesBrowserFlyout()
        {
            this.InitializeComponent();
            DataContext = new LocalSourceCodesBrowserFlyoutViewModel();
        }

        public LocalSourceCodesBrowserFlyoutViewModel ViewModel => DataContext.To<LocalSourceCodesBrowserFlyoutViewModel>();

        public event EventHandler<SourceCode> ContentConfirmed;

        public SourceCode Result { get; private set; }

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is SourceCode code)
            {
                Result = code;
                ContentConfirmed?.Invoke(this, code);
            }
        }
    }
}
