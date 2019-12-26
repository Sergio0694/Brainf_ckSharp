using System;
using Windows.Devices.Input;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using UICompositionAnimations.Helpers.PointerEvents;

#nullable enable

namespace Brainf_ckSharp.UWP.Controls.DataTemplates.Misc
{
    public sealed partial class FeaturedLinkTemplate : UserControl
    {
        public FeaturedLinkTemplate()
        {
            this.InitializeComponent();
            this.ManageControlPointerStates(TogglePointerVisualStates);
        }

        /// <summary>
        /// Gets or sets the <see cref="Uri"/> for the image to display
        /// </summary>
        public Uri? ImageUri
        {
            get => (Uri)GetValue(ImageUriProperty);
            set => SetValue(ImageUriProperty, value);
        }

        /// <summary>
        /// The dependency property for <see cref="ImageUri"/>
        /// </summary>
        public static readonly DependencyProperty ImageUriProperty = DependencyProperty.Register(
            nameof(ImageUri),
            typeof(Uri),
            typeof(FeaturedLinkTemplate),
            new PropertyMetadata(default(Uri)));

        /// <summary>
        /// Gets or sets the <see cref="Uri"/> for the featured link
        /// </summary>
        public Uri? NavigationUri { get; set; }

        // Executes the necessary animations when the pointer goes over/out of the control
        private void TogglePointerVisualStates(PointerDeviceType pointer, bool on)
        {
            // Scale animation for the thumbnail image
            if (pointer == PointerDeviceType.Mouse) (on ? StoryboardZoomIn : StoryboardZoomOut).Begin();
        }

        // Opens the featured link
        private void RootButton_Clicked(object sender, RoutedEventArgs e) => _ = Launcher.LaunchUriAsync(NavigationUri ?? throw new InvalidOperationException("No valid uri available"));
    }
}
