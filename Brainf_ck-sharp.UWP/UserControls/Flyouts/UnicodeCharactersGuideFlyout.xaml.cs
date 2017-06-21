using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.PopupService.Interfaces;
using Brainf_ck_sharp_UWP.ViewModels;
using UICompositionAnimations;
using UICompositionAnimations.Enums;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts
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
            FirstGroupGrid.SetVisualOpacity(0);
            SecondGroupGrid.SetVisualOpacity(0);
            DataContext = new UnicodeCharactersGuideFlyoutViewModel(
                () => FirstGroupGrid.StartCompositionFadeSlideAnimation(null, 1, TranslationAxis.Y, 12, 0, 200, null, null, EasingFunctionNames.CircleEaseOut),
                () => SecondGroupGrid.StartCompositionFadeSlideAnimation(null, 1, TranslationAxis.Y, 12, 0, 200, null, null, EasingFunctionNames.CircleEaseOut));
            ViewModel.LoadingCompleted += (s, e) =>
            {
                LoadingPending = false;
                LoadingCompleted?.Invoke(this, EventArgs.Empty);
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
