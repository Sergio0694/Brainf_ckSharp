using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;
using Brainf_ck_sharp.Legacy.UWP.PopupService.Interfaces;
using Brainf_ck_sharp.Legacy.UWP.ViewModels.FlyoutsViewModels;
using UICompositionAnimations;
using UICompositionAnimations.Enums;

namespace Brainf_ck_sharp.Legacy.UWP.UserControls.Flyouts
{
    public sealed partial class UnicodeCharactersGuideFlyout : UserControl, IAsyncLoadedContent
    {
        public UnicodeCharactersGuideFlyout()
        {
            SizeChanged += (_, e) =>
            {
                ItemsWidth = e.NewSize.Width / (e.NewSize.Width > 480 ? 5 : 4);
            };
            this.InitializeComponent();
            FirstGroupControl.SetVisualOpacity(0);
            SecondGroupControl.SetVisualOpacity(0);
            DataContext = new UnicodeCharactersGuideFlyoutViewModel(
                () => FirstGroupControl.StartCompositionFadeSlideAnimation(null, 1, TranslationAxis.Y, 12, 0, 200, null, null, EasingFunctionNames.CircleEaseOut),
                () => SecondGroupControl.StartCompositionFadeSlideAnimation(null, 1, TranslationAxis.Y, 12, 0, 200, null, null, EasingFunctionNames.CircleEaseOut));
            ViewModel.LoadingCompleted += (s, e) =>
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

        public UnicodeCharactersGuideFlyoutViewModel ViewModel => DataContext.To<UnicodeCharactersGuideFlyoutViewModel>();

        public double ItemsWidth
        {
            get => (double)GetValue(ItemsWidthProperty);
            set => SetValue(ItemsWidthProperty, value);
        }

        public static readonly DependencyProperty ItemsWidthProperty = DependencyProperty.Register(
            nameof(ItemsWidth), typeof(double), typeof(UnicodeCharactersGuideFlyout), new PropertyMetadata(default(double)));

        public double ItemsHeight
        {
            get => (double)GetValue(ItemsHeightProperty);
            set => SetValue(ItemsHeightProperty, value);
        }

        public static readonly DependencyProperty ItemsHeightProperty = DependencyProperty.Register(
            nameof(ItemsHeight), typeof(double), typeof(UnicodeCharactersGuideFlyout), new PropertyMetadata(52d));

        /// <inheritdoc cref="IAsyncLoadedContent"/>
        public event EventHandler LoadingCompleted;

        /// <inheritdoc cref="IAsyncLoadedContent"/>
        public bool LoadingPending { get; private set; } = true;
    }
}
