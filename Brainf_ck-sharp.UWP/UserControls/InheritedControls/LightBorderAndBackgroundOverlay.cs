using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using UICompositionAnimations;
using UICompositionAnimations.Brushes;
using UICompositionAnimations.Enums;
using UICompositionAnimations.Helpers;

namespace Brainf_ck_sharp_UWP.UserControls.InheritedControls
{
    /// <summary>
    /// A <see cref="Grid"/> control that automatically displays a light border and hover background on its content
    /// </summary>
    public sealed class LightBorderAndBackgroundOverlay : Grid
    {
        // The light hover background border
        private readonly Border _LightBackground;

        // The light edge border
        private readonly Border _LightBorder;

        public LightBorderAndBackgroundOverlay()
        {
            // Platform test
            if (ApiInformationHelper.IsMobileDevice) return;

            // UI initialization
            _LightBackground = new Border { Opacity = 0 };
            _LightBorder = new Border {BorderThickness = new Thickness(1), Opacity = 0.4 };
            Grid bordersContainer = new Grid { Opacity = 0, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };

            // Setup the visual tree
            bordersContainer.Children.Add(_LightBackground);
            bordersContainer.Children.Add(_LightBorder);
            Children.Add(bordersContainer);

            // Animate the lights in and out
            this.ManageLightsPointerStates(value =>
            {
                _LightBackground.StartXAMLTransformFadeAnimation(null, value ? 0.6 : 0, 200, null, EasingFunctionNames.Linear);
            });
            Loaded += (s, e) => bordersContainer.StartXAMLTransformFadeAnimation(null, 1, 200, LoadingFadeInDelay, EasingFunctionNames.Linear);
            Unloaded += (s, e) => bordersContainer.Opacity = 0;
        }

        /// <summary>
        /// Gets or sets the thickness of the light border
        /// </summary>
        public Thickness LightBorderThickness
        {
            get => _LightBorder?.BorderThickness ?? new Thickness();
            set
            {
                if (_LightBorder != null) _LightBorder.BorderThickness = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="LightingBrush"/> to use as the reveal highlight border effect
        /// </summary>
        public Brush LightBorderBrush
        {
            get => _LightBorder?.BorderBrush;
            set
            {
                if (_LightBorder != null) _LightBorder.BorderBrush = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="LightingBrush"/> to use as the reveal highlight hover background effect
        /// </summary>
        public Brush LightBackgroundBrush
        {
            get => _LightBackground?.Background;
            set
            {
                if (_LightBackground != null) _LightBackground.Background = value;
            }
        }

        /// <summary>
        /// Gets or sets the duration of the initial fade in animation for the light brushes
        /// </summary>
        public int LoadingFadeInDelay { get; set; } = 1000;

        /// <summary>
        /// Gets or sets whether or not the lights are currently enabled and visible
        /// </summary>
        public bool LightsEnabled
        {
            get { return GetValue(LightsEnabledProperty).To<bool>(); }
            set { SetValue(LightsEnabledProperty, value); }
        }

        public static readonly DependencyProperty LightsEnabledProperty = DependencyProperty.Register(
            nameof(LightsEnabled), typeof(bool), typeof(LightBorderAndBackgroundOverlay), new PropertyMetadata(true, OnLightsEnabledPropertyChanged));

        private static void OnLightsEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.To<LightBorderAndBackgroundOverlay>()._LightBorder?.StartXAMLTransformFadeAnimation(
                null, e.NewValue.To<bool>() ? 0.4 : 0, 200, null, EasingFunctionNames.Linear);
        }
    }
}
