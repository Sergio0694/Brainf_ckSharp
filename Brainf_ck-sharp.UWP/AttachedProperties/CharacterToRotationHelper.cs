using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Brainf_ck_sharp_UWP.Helpers;

namespace Brainf_ck_sharp_UWP.AttachedProperties
{
    /// <summary>
    /// An attached property that sets a <see cref="RotateTransform"/> object to a <see cref="TextBlock"/> control if needed
    /// </summary>
    public class CharacterToRotationHelper
    {
        public static char GetCharacter(TextBlock element)
        {
            return element.GetValue(CharacterProperty).To<char>();
        }

        public static void SetCharacter(TextBlock element, char value)
        {
            element?.SetValue(CharacterProperty, value);
        }

        public static readonly DependencyProperty CharacterProperty =
            DependencyProperty.RegisterAttached("Character", typeof(char), typeof(CharacterToRotationHelper), 
                new PropertyMetadata(DependencyProperty.UnsetValue, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBlock @this = d.To<TextBlock>();
            char c = e.NewValue.To<char>();
            bool valid = c == 0 || c > 31 && c < 128 || c > 159;
            @this.RenderTransform = valid
            ? null
            : new CompositeTransform
            {
                Rotation = -70,
                CenterX = 12,
                CenterY = 16,
                ScaleX = 0.9,
                ScaleY = 0.9
            };
        }
    }
}
