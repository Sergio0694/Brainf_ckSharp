using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Uwp.Bindings.Functions;
using Brainf_ckSharp.Uwp.Models;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.SubPages.Views.CodeLibraryMap.Templates
{
    public sealed partial class UnicodeVisibleCharacterTemplate : UserControl
    {
        public UnicodeVisibleCharacterTemplate()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the value to display for this characters group
        /// </summary>
        public UnicodeCharacter Character
        {
            get => (UnicodeCharacter)GetValue(CharacterProperty);
            set => SetValue(CharacterProperty, value);
        }

        public static readonly DependencyProperty CharacterProperty = DependencyProperty.Register(
            nameof(Character),
            typeof(UnicodeCharacter),
            typeof(UnicodeVisibleCharacterTemplate),
            new PropertyMetadata(DependencyProperty.UnsetValue, OnCharacterPropertyChanged));

        /// <summary>
        /// Updates the <see cref="TextBlock.Text"/> property on <see cref="NumberBlock"/> and <see cref="ValueBlock"/> when <see cref="Character"/> changes
        /// </summary>
        /// <param name="d">The source <see cref="UnicodeVisibleCharacterTemplate"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance with the new <see cref="Character"/> value</param>
        private static void OnCharacterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UnicodeVisibleCharacterTemplate @this = (UnicodeVisibleCharacterTemplate)d;
            UnicodeCharacter value = (UnicodeCharacter)e.NewValue;

            @this.NumberBlock.Text = ((ushort)value.Value).ToString();
            @this.ValueBlock.Text = NumericFunctions.ConvertToVisibleText(value.Value);
        }
    }
}
