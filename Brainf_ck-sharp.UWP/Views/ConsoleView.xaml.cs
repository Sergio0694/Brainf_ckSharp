using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.ViewModels;

namespace Brainf_ck_sharp_UWP.Views
{
    public sealed partial class ConsoleView : UserControl
    {
        public ConsoleView()
        {
            this.InitializeComponent();
            this.DataContext = new ConsoleViewModel();
            ViewModel.ConsoleLineAddedOrModified += async (s, e) =>
            {
                await Task.Delay(500);
                ScrollViewer scroller = ConsoleListView.FindChild<ScrollViewer>();
                scroller?.ChangeView(null, scroller.ScrollableHeight, null, false);
            };
        }

        public ConsoleViewModel ViewModel => DataContext.To<ConsoleViewModel>();

        /// <summary>
        /// Adjusts the top margin of the content in the list
        /// </summary>
        /// <param name="height">The desired height</param>
        public void AdjustTopMargin(double height) => TopMarginGrid.Height = height;
    }
}
