using Windows.Devices.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using UICompositionAnimations;
using UICompositionAnimations.Brushes;
using UICompositionAnimations.Enums;

namespace Brainf_ck_sharp_UWP.UserControls.InheritedControls
{
    /// <summary>
    /// A <see cref="Grid"/> control that automatically displays a light border and hover background on its content
    /// </summary>
    public sealed class LightBorderAndBackgroundOverlay : Grid
    {
        // The light hover background border
        private readonly Border _LightBackground = new Border { Opacity = 0 };

        // The light edge border
        private readonly Border _LightBorder = new Border { BorderThickness = new Thickness(1), Opacity = 0.4 };

        // The grid that will contain the light elements
        private readonly Grid _BordersContainer = new Grid { Opacity = 0, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };

        public LightBorderAndBackgroundOverlay()
        {
            // Setup the visual tree
            _BordersContainer.Children.Add(_LightBackground);
            _BordersContainer.Children.Add(_LightBorder);
            Children.Add(_BordersContainer);

            // Animate the lights in and out
            this.ManageControlPointerStates((pointer, value) =>
            {
                if (pointer != PointerDeviceType.Mouse) return;
                _LightBackground.StartXAMLTransformFadeAnimation(null, value ? 0.6 : 0, 200, null, EasingFunctionNames.Linear);
            });
            Loaded += (s, e) => _BordersContainer.StartXAMLTransformFadeAnimation(null, 1, 200, 1000, EasingFunctionNames.Linear);
            Unloaded += (s, e) => _BordersContainer.Opacity = 0;
        }

        /// <summary>
        /// Gets or sets the thickness of the light border
        /// </summary>
        public Thickness LightBorderThickness
        {
            get => _LightBorder.BorderThickness;
            set => _LightBorder.BorderThickness = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="LightingBrush"/> to use as the reveal highlight border effect
        /// </summary>
        public LightingBrush LightBorderBrush
        {
            get => _LightBorder.BorderBrush as LightingBrush;
            set => _LightBorder.BorderBrush = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="LightingBrush"/> to use as the reveal highlight hover background effect
        /// </summary>
        public LightingBrush LightBackgroundBrush
        {
            get => _LightBackground.Background as LightingBrush;
            set => _LightBackground.Background = value;
        }

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
            d.To<LightBorderAndBackgroundOverlay>()._LightBorder.StartXAMLTransformFadeAnimation(
                null, e.NewValue.To<bool>() ? 0.4 : 0, 200, null, EasingFunctionNames.Linear);
        }
    }
}
