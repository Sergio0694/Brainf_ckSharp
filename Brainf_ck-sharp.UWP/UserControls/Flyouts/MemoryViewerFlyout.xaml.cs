using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp.MemoryState;
using Brainf_ck_sharp_UWP.Helpers;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts
{
    public sealed partial class MemoryViewerFlyout : UserControl
    {
        public MemoryViewerFlyout()
        {
            SizeChanged += (_, e) =>
            {
                ItemsWidth = (e.NewSize.Width - 12) / (e.NewSize.Width > 480 ? 5 : 4);
            };
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the memory state to display in the control
        /// </summary>
        public IEnumerable<IndexedModelWithValue<Brainf_ckMemoryCell>> Source
        {
            get => (IEnumerable<IndexedModelWithValue<Brainf_ckMemoryCell>>)GetValue(PropertyTypeProperty);
            set => SetValue(PropertyTypeProperty, value);
        }

        public static readonly DependencyProperty PropertyTypeProperty = DependencyProperty.Register(
            nameof(Source), typeof(IEnumerable<IndexedModelWithValue<Brainf_ckMemoryCell>>), typeof(MemoryViewerFlyout), new PropertyMetadata(null));

        public double ItemsWidth
        {
            get => (double)GetValue(ItemsWidthProperty);
            set => SetValue(ItemsWidthProperty, value);
        }

        public static readonly DependencyProperty ItemsWidthProperty = DependencyProperty.Register(
            nameof(ItemsWidth), typeof(double), typeof(MemoryViewerFlyout), new PropertyMetadata(default(double)));

        public double ItemsHeight
        {
            get => (double)GetValue(ItemsHeightProperty);
            set => SetValue(ItemsHeightProperty, value);
        }

        public static readonly DependencyProperty ItemsHeightProperty = DependencyProperty.Register(
            nameof(ItemsHeight), typeof(double), typeof(MemoryViewerFlyout), new PropertyMetadata(76));
    }
}
