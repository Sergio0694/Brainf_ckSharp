using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using UICompositionAnimations.Helpers.PointerEvents;

namespace Brainf_ck_sharp_UWP.UserControls.DataTemplates.JumpList
{
    public sealed partial class JumpListZoomedInHeaderTemplate : UserControl
    {
        public JumpListZoomedInHeaderTemplate()
        {
            this.InitializeComponent();
            this.ManageControlPointerStates((_, value) => VisualStateManager.GoToState(this, value ? "Highlight" : "Default", false));
        }

        /// <summary>
        /// Gets or sets the title to display in the control
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title), typeof(string), typeof(JumpListZoomedInHeaderTemplate), new PropertyMetadata(default(string), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.To<JumpListZoomedInHeaderTemplate>().Block.Text = e.NewValue.To<string>() ?? string.Empty;
        }
    }
}
