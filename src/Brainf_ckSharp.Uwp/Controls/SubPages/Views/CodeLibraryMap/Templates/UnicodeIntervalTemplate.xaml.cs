using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Brainf_ckSharp.Uwp.Controls.SubPages.Views.CodeLibraryMap.Templates
{
    public sealed partial class UnicodeIntervalTemplate : UserControl
    {
        /// <summary>
        /// Creates a new <see cref="UnicodeIntervalTemplate"/> instance
        /// </summary>
        public UnicodeIntervalTemplate()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the value to display for this characters interval
        /// </summary>
        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value),
            typeof(string),
            typeof(UnicodeIntervalTemplate),
            new PropertyMetadata(string.Empty, OnValuePropertyChanged));

        /// <summary>
        /// Updates the <see cref="TextBlock.Text"/> property on <see cref="ValueBlock"/> when <see cref="Value"/> changes
        /// </summary>
        /// <param name="d">The source <see cref="UnicodeIntervalTemplate"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance with the new <see cref="Value"/> value</param>
        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UnicodeIntervalTemplate @this = (UnicodeIntervalTemplate)d;
            string text = (string)e.NewValue;

            @this.ValueBlock.Text = text;
        }

        /// <summary>
        /// Gets or sets the description to display for this characters interval
        /// </summary>
        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            nameof(Description),
                typeof(string),
                typeof(UnicodeIntervalTemplate),
            new PropertyMetadata(string.Empty, OnDescriptionPropertyPropertyChanged));

        /// <summary>
        /// Updates the <see cref="TextBlock.Text"/> property on <see cref="DescriptionBlock"/> when <see cref="Description"/> changes
        /// </summary>
        /// <param name="d">The source <see cref="UnicodeIntervalTemplate"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance with the new <see cref="Description"/> value</param>
        private static void OnDescriptionPropertyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UnicodeIntervalTemplate @this = (UnicodeIntervalTemplate)d;
            string text = (string)e.NewValue;

            @this.DescriptionBlock.Text = text;
        }
    }
}
