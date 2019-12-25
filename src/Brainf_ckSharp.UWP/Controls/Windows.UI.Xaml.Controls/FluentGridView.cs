using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Extensions;

#nullable enable

namespace Brainf_ckSharp.UWP.Controls.Windows.UI.Xaml.Controls
{
    /// <summary>
    /// A custom <see cref="GridView"/> with animations whenever the items are rearranged
    /// </summary>
    public sealed class FluentGridView : GridView
    {
        /// <summary>
        /// The <see cref="ItemsWrapGrid"/> instance used to display items in the current control
        /// </summary>
        private ItemsWrapGrid? _WrapGrid;

        /// <summary>
        /// The offset animation for each item thumbnails
        /// </summary>
        private AnimationCollection? _ThumbnailAnimationCollection;

        /// <summary>
        /// Creates a new <see cref="FluentGridView"/> instance
        /// </summary>
        public FluentGridView()
        {
            this.SizeChanged += FluentGridView_SizeChanged;
            this.ContainerContentChanging += FluentGridView_ContainerContentChanging;
        }

        /// <summary>
        /// Gets or sets the height of each item
        /// </summary>
        public double ItemsHeight { get; set; }

        /// <summary>
        /// Gets or sets the desired size for each item template
        /// </summary>
        public double PreferredItemsWidth { get; set; }

        // Adjusts the size of each item template
        private void FluentGridView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Get the target panel if necessary
            if (_WrapGrid == null)
            {
                //_WrapGrid = this.FindChild<ItemsWrapGrid>() ?? throw new InvalidOperationException("Couldn't find a valid wrap grid");
                _WrapGrid = this.FindChild<ItemsWrapGrid>();
                if (_WrapGrid == null) return; // TODO: fix this crash

                _WrapGrid.ItemHeight = ItemsHeight;
            }

            // Adjust the size of each image
            double
                round = Math.Ceiling(e.NewSize.Width / PreferredItemsWidth),
                size = (e.NewSize.Width - 4) / round;
            _WrapGrid.ItemHeight = _WrapGrid.ItemWidth = size;
        }

        // Set the implicit offset animation when resizing the grid
        private void FluentGridView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (_ThumbnailAnimationCollection == null)
            {
                Vector3Animation animation = new Vector3Animation { Target = "Offset", Duration = TimeSpan.FromSeconds(0.6) };
                _ThumbnailAnimationCollection = new AnimationCollection { animation };
            }
            Implicit.SetAnimations(args.ItemContainer, _ThumbnailAnimationCollection);
        }
    }
}
