using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;

namespace Brainf_ck_sharp.Legacy.UWP.UserControls.CustomControls
{
    public sealed partial class IconButtonWithTextContent : UserControl
    {
        public IconButtonWithTextContent()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the icon to display in the button
        /// </summary>
        public string Icon
        {
            get => IconBlock.Text;
            set => IconBlock.Text = value;
        }

        /// <summary>
        /// Gets or sets the text content of the button
        /// </summary>
        public string Title
        {
            get => ContentBlock.Text;
            set => ContentBlock.Text = value;
        }

        /// <summary>
        /// Gets or sets the background brush for the button
        /// </summary>
        public Brush BackgroundBrush
        {
            get => GetValue(BackgroundBrushProperty).To<Brush>();
            set => SetValue(BackgroundBrushProperty, value);
        }

        public static readonly DependencyProperty BackgroundBrushProperty = DependencyProperty.Register(
            nameof(BackgroundBrush), typeof(Brush), typeof(IconButtonWithTextContent), new PropertyMetadata(default(Brush), OnBackgroundBrushPropertyChanged));

        private static void OnBackgroundBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.To<IconButtonWithTextContent>().ButtonControl.Background = e.NewValue as Brush;
        }

        /// <summary>
        /// Raised whenever the main button is clicked
        /// </summary>
        public event RoutedEventHandler Click;

        // Raises the Click event
        private void Root_Click(object sender, RoutedEventArgs e) => Click?.Invoke(sender, e);
    }
}
