using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.DataModels.Misc;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.UserControls.Flyouts.UserGuide.Sections;
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
                this.Bindings.StopTracking();
                ViewModel.Cleanup();
                DataContext = null;
            };
        }

        public UserGuideViewerControlViewModel ViewModel => this.DataContext.To<UserGuideViewerControlViewModel>();

        // Scrolls to the bottom of the list
        public async void TryScrollToPBrainSection()
        {
            ScrollViewer scroller = SectionsList.FindChild<ScrollViewer>();
            if (scroller == null) throw new NullReferenceException("This can't really happen");
            int index = ViewModel.Source.Count - 1;
            if (index != 2) throw new InvalidOperationException("Invalid data source");
            DependencyObject container = SectionsList.ContainerFromIndex(index);
            if (container == null)
            {
                TaskCompletionSource<Unit> tcs = new TaskCompletionSource<Unit>();
                Stopwatch timer = new Stopwatch();
                timer.Start();
                void LayoutHandler(object sender, object e)
                {
                    container = SectionsList.ContainerFromIndex(index);
                    if (container != null || timer.ElapsedMilliseconds > 1000)
                    {
                        tcs.TrySetResult(Unit.Instance);
                        SectionsList.LayoutUpdated -= LayoutHandler;
                    }
                }
                SectionsList.LayoutUpdated += LayoutHandler;
                await tcs.Task;
                timer.Stop();
            }
            if (container is ListViewItem item && item.ContentTemplateRoot is PBrainGuideControl pbrain)
            {
                double offset = scroller.ExtentHeight - pbrain.ActualHeight;
                scroller.ChangeView(null, offset, null);
            }
            else scroller.ChangeView(null, scroller.ScrollableHeight, null);
        }
    }
}
