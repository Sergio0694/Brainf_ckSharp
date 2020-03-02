using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.DataTemplates.Misc
{
    public sealed partial class FeaturedLinkTemplate : UserControl
    {
        public FeaturedLinkTemplate()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the <see cref="ImageSource"/> for the image to display
        /// </summary>
        public ImageSource? Image
        {
            get => (ImageSource)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        /// <summary>
        /// The dependency property for <see cref="Image"/>
        /// </summary>
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(
            nameof(Image),
            typeof(ImageSource),
            typeof(FeaturedLinkTemplate),
            new PropertyMetadata(default(ImageSource)));

        /// <summary>
        /// Gets or sets the <see cref="Uri"/> for the featured link
        /// </summary>
        public Uri? NavigationUri { get; set; }

        // Opens the featured link
        private void RootButton_Clicked(object sender, RoutedEventArgs e) => _ = Launcher.LaunchUriAsync(NavigationUri ?? throw new InvalidOperationException("No valid uri available"));
    }
}
