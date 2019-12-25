using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI.Extensions;

namespace Brainf_ckSharp.UWP.Controls.Windows.UI.Xaml.Controls
{
    /// <summary>
    /// A custom <see cref="GridView"/> with a fixed number of columns, automatically expanded to fit the available space
    /// </summary>
    public sealed class FixedGridView : GridView
    {
        /// <summary>
        /// Creates a new <see cref="FixedGridView"/> instance
        /// </summary>
        public FixedGridView()
        {
            this.SizeChanged += FluentGridView_SizeChanged;
        }

        /// <summary>
        /// Gets or sets the desired number of fixed columns to display
        /// </summary>
        public int NumberOfColumns { get; set; }

        /// <summary>
        /// The <see cref="ItemsWrapGrid"/> used to display items
        /// </summary>
        private ItemsWrapGrid? _WrapGrid;

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            _WrapGrid = this.FindChild<ItemsWrapGrid>() ?? throw new InvalidOperationException("Couldn't find a valid wrap grid");
        }

        // Adjusts the size of each item template
        private void FluentGridView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _WrapGrid.ItemWidth = e.NewSize.Width / NumberOfColumns;
        }
    }
}
