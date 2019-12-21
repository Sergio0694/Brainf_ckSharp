using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;

namespace Brainf_ck_sharp.Legacy.UWP.UserControls.CustomControls
{
    public sealed partial class UnicodeNonVisibleCharactersGroupDisplayControl : UserControl
    {
        public UnicodeNonVisibleCharactersGroupDisplayControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the value to display for this characters group
        /// </summary>
        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value), typeof(string), typeof(UnicodeNonVisibleCharactersGroupDisplayControl), 
            new PropertyMetadata(default(string), OnValuePropertyChanged));

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.To<UnicodeNonVisibleCharactersGroupDisplayControl>().ValueBlock.Text = e.NewValue.To<string>() ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the description to display for this characters group
        /// </summary>
        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            nameof(Description), typeof(string), typeof(UnicodeNonVisibleCharactersGroupDisplayControl),
            new PropertyMetadata(default(string), OnDescriptionPropertyPropertyChanged));

        private static void OnDescriptionPropertyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.To<UnicodeNonVisibleCharactersGroupDisplayControl>().DescriptionBlock.Text = e.NewValue.To<string>() ?? string.Empty;
        }
    }
}
