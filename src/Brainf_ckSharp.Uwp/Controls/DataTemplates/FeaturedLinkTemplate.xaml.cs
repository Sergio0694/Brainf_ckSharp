using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using Microsoft.Extensions.DependencyInjection;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.DataTemplates
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
        /// Gets or sets the <see cref="string"/> with the URL to navigate to
        /// </summary>
        public string? NavigationUrl
        {
            get => (string?)GetValue(NavigationUrlProperty);
            set => SetValue(NavigationUrlProperty, value);
        }

        /// <summary>
        /// The dependency property for <see cref="Image"/>
        /// </summary>
        public static readonly DependencyProperty NavigationUrlProperty = DependencyProperty.Register(
            nameof(NavigationUrl),
            typeof(string),
            typeof(FeaturedLinkTemplate),
            new PropertyMetadata(default(string)));

        // Opens the featured link
        private void RootButton_Clicked(object sender, RoutedEventArgs e)
        {
            _ = Launcher.LaunchUriAsync(new Uri(NavigationUrl ?? throw new InvalidOperationException("No valid uri available")));

            App.Current.Services.GetRequiredService<IAnalyticsService>().Log(EventNames.PayPalDonationOpened);
        }
    }
}
