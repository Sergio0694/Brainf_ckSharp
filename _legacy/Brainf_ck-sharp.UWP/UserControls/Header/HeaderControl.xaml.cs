using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;
using UICompositionAnimations;
using UICompositionAnimations.Enums;

namespace Brainf_ck_sharp.Legacy.UWP.UserControls.Header
{
    public sealed partial class HeaderControl : UserControl
    {
        public HeaderControl()
        {
            this.InitializeComponent();
        }

        public void SelectConsole() => SelectedHeaderIndex = 0;

        public void SelectIDE() => SelectedHeaderIndex = 1;

        /// <summary>
        /// Gets or sets the currently selected index for the header control
        /// </summary>
        public int SelectedHeaderIndex
        {
            get => (int)GetValue(SelectedHeaderIndexProperty);
            set => SetValue(SelectedHeaderIndexProperty, value);
        }

        public static readonly DependencyProperty SelectedHeaderIndexProperty = DependencyProperty.Register(
            nameof(SelectedHeaderIndex), typeof(int), typeof(HeaderControl), new PropertyMetadata(default(int), OnSelectedHeaderIndexPropertyChanged));

        private static void OnSelectedHeaderIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeaderControl @this = d.To<HeaderControl>();
            int index = e.NewValue.To<int>();
            @this.ConsoleButton.IsSelected = index == 0;
            @this.IDEButton.IsSelected = index == 1;
            @this.SelectedRectangle.StartCompositionTranslationAnimation(index * 80, 0, 250, null, EasingFunctionNames.CircleEaseOut);
        }
    }
}
