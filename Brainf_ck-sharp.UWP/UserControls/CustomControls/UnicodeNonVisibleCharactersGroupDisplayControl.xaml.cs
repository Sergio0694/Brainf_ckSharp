using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Helpers.Extensions;

namespace Brainf_ck_sharp_UWP.UserControls.CustomControls
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
        public String Value
        {
            get { return (String)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value), typeof(String), typeof(UnicodeNonVisibleCharactersGroupDisplayControl), 
            new PropertyMetadata(default(String), OnValuePropertyChanged));

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.To<UnicodeNonVisibleCharactersGroupDisplayControl>().ValueBlock.Text = e.NewValue.To<String>() ?? String.Empty;
        }

        /// <summary>
        /// Gets or sets the description to display for this characters group
        /// </summary>
        public String Description
        {
            get { return (String)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            nameof(Description), typeof(String), typeof(UnicodeNonVisibleCharactersGroupDisplayControl),
            new PropertyMetadata(default(String), OnDescriptionPropertyPropertyChanged));

        private static void OnDescriptionPropertyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.To<UnicodeNonVisibleCharactersGroupDisplayControl>().DescriptionBlock.Text = e.NewValue.To<String>() ?? String.Empty;
        }
    }
}
