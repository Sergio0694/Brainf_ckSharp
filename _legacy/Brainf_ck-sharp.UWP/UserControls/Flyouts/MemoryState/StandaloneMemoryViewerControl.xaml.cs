using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.DataModels;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ckSharp.Legacy.MemoryState;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts.MemoryState
{
    public sealed partial class StandaloneMemoryViewerControl : UserControl
    {
        public StandaloneMemoryViewerControl()
        {
            SizeChanged += (_, e) =>
            {
                ItemsWidth = e.NewSize.Width / (e.NewSize.Width > 480 ? 5 : 4);
            };
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the memory state to display in the control
        /// </summary>
        public IEnumerable<IndexedModelWithValue<Brainf_ckMemoryCell>> Source
        {
            get => (IEnumerable<IndexedModelWithValue<Brainf_ckMemoryCell>>)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            nameof(Source), typeof(IEnumerable<IndexedModelWithValue<Brainf_ckMemoryCell>>), typeof(StandaloneMemoryViewerControl), 
            new PropertyMetadata(null, OnSourcePropertyChanged));

        private static void OnSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.To<StandaloneMemoryViewerControl>().MemoryList.ItemsSource = e.NewValue.To<IEnumerable<IndexedModelWithValue<Brainf_ckMemoryCell>>>();
        }

        public double ItemsWidth
        {
            get => (double)GetValue(ItemsWidthProperty);
            set => SetValue(ItemsWidthProperty, value);
        }

        public static readonly DependencyProperty ItemsWidthProperty = DependencyProperty.Register(
            nameof(ItemsWidth), typeof(double), typeof(StandaloneMemoryViewerControl), new PropertyMetadata(default(double)));

        public double ItemsHeight
        {
            get => (double)GetValue(ItemsHeightProperty);
            set => SetValue(ItemsHeightProperty, value);
        }

        public static readonly DependencyProperty ItemsHeightProperty = DependencyProperty.Register(
            nameof(ItemsHeight), typeof(double), typeof(StandaloneMemoryViewerControl), new PropertyMetadata(76d));
    }
}
