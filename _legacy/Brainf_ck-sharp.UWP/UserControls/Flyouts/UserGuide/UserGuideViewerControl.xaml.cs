using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.DataModels.Misc;
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

            // Wait for the scroller to load the content
            DependencyObject container = SectionsList.ContainerFromIndex(1); // Wait for the 2nd item
            if (container == null)
            {
                TaskCompletionSource<Unit> tcs = new TaskCompletionSource<Unit>();
                Stopwatch timer = new Stopwatch();
                timer.Start();
                void LayoutHandler(object sender, object e)
                {
                    container = SectionsList.ContainerFromIndex(ViewModel.Source.Count - 1);
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

            // Scroll to offset
            DependencyObject[] trailing = Enumerable.Range(0, 2).Select(SectionsList.ContainerFromIndex).ToArray();
            if (trailing.All(section => section is ListViewItem item && item.ContentTemplateRoot is FrameworkElement))
            {
                double offset = trailing.Sum(section => ((section as ListViewItem)?.ContentTemplateRoot as FrameworkElement)?.ActualHeight ?? 0) + 88;
                scroller.ChangeView(null, offset, null);
            }
            else scroller.ChangeView(null, scroller.ScrollableHeight, null);
        }
    }
}
