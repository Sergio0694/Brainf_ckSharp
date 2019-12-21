using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using UICompositionAnimations;

namespace Brainf_ck_sharp_UWP.UserControls.InheritedControls
{
    /// <summary>
    /// A custom <see cref="Grid"/> thaat hosts a <see cref="Popup"/> with a shadow behind it
    /// </summary>
    public sealed class PopupDropShadowGrid : Grid
    {
        // Inner border with the bottom shadow
        private readonly Border ShadowBottomBorder = new Border();

        // Inner border with the left shadow
        private readonly Border ShadowLeftBorder = new Border();

        // Inner border with the right shadow
        private readonly Border ShadowRightBorder = new Border();

        public PopupDropShadowGrid()
        {
            this.Loaded += (s, e) =>
            {
                // Input check
                if (ContainedGrid == null) return;

                // Apply the shadow
                OnLoaded();
            };

            this.Children.Add(ShadowBottomBorder);
            this.Children.Add(ShadowRightBorder);
            this.Children.Add(ShadowLeftBorder);
        }

        private FrameworkElement _ContainedGrid;

        /// <summary>
        /// Gets or sets the contained element that will cast the shadow
        /// </summary>
        public FrameworkElement ContainedGrid
        {
            get => _ContainedGrid;
            set
            {
                if (ContainedGrid != value)
                {
                    if (ContainedGrid != null) this.Children.Remove(ContainedGrid);
                    this.Children.Add(value);
                    _ContainedGrid = value;
                }
            }
        }

        // Prepares the shadow for the hosted content
        private void OnLoaded()
        {
            // Adjust the popup
            Popup popup = this.FindParent<Popup>();
            if (popup != null) popup.HorizontalOffset -= 12;

            // Shadows setup
            ContainedGrid.AttachVisualShadow(ShadowLeftBorder, true, (float)ContainedGrid.ActualWidth, (float)ContainedGrid.ActualHeight - 4, Colors.Black, 1, 12, 6, new Thickness(0, -8, ContainedGrid.ActualWidth - 12, -8), 0, 6, 12);
            ContainedGrid.AttachVisualShadow(ShadowRightBorder, true, (float)ContainedGrid.ActualWidth, (float)ContainedGrid.ActualHeight - 4, Colors.Black, 1, 12, 6, new Thickness(ContainedGrid.ActualWidth - 12, -8, 0, -8), 24, 6, 12);
            ContainedGrid.AttachVisualShadow(ShadowBottomBorder, true, (float)ContainedGrid.ActualWidth, (float)ContainedGrid.ActualHeight - 4, Colors.Black, 1, 12, 6, new Thickness(0, ContainedGrid.ActualHeight - 8, 0, -8), 12, 8, 12);

            // Setup the grid size
            double width = ContainedGrid.ActualWidth, height = ContainedGrid.ActualHeight;
            ContainedGrid.Width = width;
            this.Width = width + 32;
            ContainedGrid.Height = height;
            this.Height = height + 20;
            ContainedGrid.VerticalAlignment = VerticalAlignment.Top;
            ContainedGrid.HorizontalAlignment = HorizontalAlignment.Left;
            ContainedGrid.SetVisualOffset(12, 0);
        }
    }
}
