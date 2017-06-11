using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.ViewModels;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts
{
    public sealed partial class UnicodeCharactersGuideFlyout : UserControl
    {
        public UnicodeCharactersGuideFlyout()
        {
            SizeChanged += (_, e) =>
            {
                ItemsWidth = e.NewSize.Width / (e.NewSize.Width > 480 ? 5 : 4);
            };
            this.InitializeComponent();
            DataContext = new UnicodeCharactersGuideFlyoutViewModel();
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
    }
}
