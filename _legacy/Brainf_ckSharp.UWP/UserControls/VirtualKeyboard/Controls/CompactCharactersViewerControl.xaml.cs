using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;
using Brainf_ck_sharp.Legacy.UWP.ViewModels;

namespace Brainf_ck_sharp.Legacy.UWP.UserControls.VirtualKeyboard.Controls
{
    public sealed partial class CompactCharactersViewerControl : UserControl
    {
        public CompactCharactersViewerControl()
        {
            SizeChanged += (_, e) =>
            {
                ItemsWidth = e.NewSize.Width / (e.NewSize.Width > 560 ? 5 : 4);
            };
            this.InitializeComponent();
            DataContext = new CompactCharactersViewerControlViewModel();
        }

        public CompactCharactersViewerControlViewModel ViewModel => DataContext.To<CompactCharactersViewerControlViewModel>();

        public double ItemsWidth
        {
            get => (double)GetValue(ItemsWidthProperty);
            set => SetValue(ItemsWidthProperty, value);
        }

        public static readonly DependencyProperty ItemsWidthProperty = DependencyProperty.Register(
            nameof(ItemsWidth), typeof(double), typeof(CompactCharactersViewerControl), new PropertyMetadata(default(double)));

        public double ItemsHeight
        {
            get => (double)GetValue(ItemsHeightProperty);
            set => SetValue(ItemsHeightProperty, value);
        }

        public static readonly DependencyProperty ItemsHeightProperty = DependencyProperty.Register(
            nameof(ItemsHeight), typeof(double), typeof(CompactCharactersViewerControl), new PropertyMetadata(60));
    }
}
