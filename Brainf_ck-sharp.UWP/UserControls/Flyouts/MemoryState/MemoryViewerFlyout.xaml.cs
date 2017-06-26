using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.PopupService.Interfaces;
using Brainf_ck_sharp_UWP.ViewModels;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts.MemoryState
{
    public sealed partial class MemoryViewerFlyout : UserControl, IAsyncLoadedContent
    {
        public MemoryViewerFlyout()
        {
            SizeChanged += (_, e) =>
            {
                ItemsWidth = e.NewSize.Width / (e.NewSize.Width > 480 ? 5 : 4);
            };
            this.InitializeComponent();
            DataContext = new MemoryViewerFlyoutViewModel();
            ViewModel.InitializationCompleted += (s, e) =>
            {
                LoadingPending = false;
                LoadingCompleted?.Invoke(this, EventArgs.Empty);
            };
            Unloaded += (s, e) =>
            {
                this.Bindings.StopTracking();
                ViewModel.Cleanup();
                DataContext = null;
                LoadingCompleted = null;
            };
        }

        public MemoryViewerFlyoutViewModel ViewModel => DataContext.To<MemoryViewerFlyoutViewModel>();

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
            nameof(ItemsHeight), typeof(double), typeof(MemoryViewerFlyout), new PropertyMetadata(76d));

        /// <inheritdoc cref="IAsyncLoadedContent"/>
        public event EventHandler LoadingCompleted;

        /// <inheritdoc cref="IAsyncLoadedContent"/>
        public bool LoadingPending { get; private set; } = true;
    }
}
