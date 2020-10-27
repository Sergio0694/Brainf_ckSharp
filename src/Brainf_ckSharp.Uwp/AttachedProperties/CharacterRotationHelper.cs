using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Brainf_ckSharp.Uwp.AttachedProperties
{
    /// <summary>
    /// An attached property that sets a <see cref="CompositeTransform"/> object to a <see cref="TextBlock"/> control if needed
    /// </summary>
    public static class CharacterRotationHelper
    {
        /// <summary>
        /// Gets the value of <see cref="CharacterProperty"/> for a given <see cref="TextBlock"/>
        /// </summary>
        /// <param name="element">The input <see cref="TextBlock"/> for which to get the property value</param>
        /// <returns>The value of the <see cref="CharacterProperty"/> property for the input <see cref="TextBlock"/> instance</returns>
        public static char GetCharacter(TextBlock element)
        {
            return (char)element.GetValue(CharacterProperty);
        }

        /// <summary>
        /// Sets the value of <see cref="CharacterProperty"/> for a given <see cref="TextBlock"/>
        /// </summary>
        /// <param name="element">The input <see cref="TextBlock"/> for which to set the property value</param>
        /// <param name="value">The value to set for the <see cref="CharacterProperty"/> property</param>
        public static void SetCharacter(TextBlock element, char value)
        {
            element?.SetValue(CharacterProperty, value);
        }

        /// <summary>
        /// An attached property that controls the render transform of a <see cref="TextBlock"/> based on a given character
        /// </summary>
        public static readonly DependencyProperty CharacterProperty = DependencyProperty.RegisterAttached(
            "Character",
            typeof(char),
            typeof(CharacterRotationHelper),
            new(DependencyProperty.UnsetValue, OnCharacterPropertyChanged));

        /// <summary>
        /// The mapping of <see cref="CompositeTransform"/> instance from number of digits
        /// </summary>
        /// <remarks>Using a <see cref="Dictionary{TKey,TValue}"/> for the field type to try to avoid the <see langword="callvirt"/> overhead</remarks>
        private static readonly Dictionary<int, CompositeTransform> TransformsMap = new()
        {
            [2] = new()
            {
                Rotation = -70,
                CenterX = 12,
                CenterY = 16,
                ScaleX = 0.85,
                ScaleY = 0.85,
                TranslateX = -4,
                TranslateY = -2
            },
            [3] = new()
            {
                Rotation = -70,
                CenterX = 14,
                CenterY = 18,
                ScaleX = 0.8,
                ScaleY = 0.8,
                TranslateX = -6,
                TranslateY = 0
            }
        };

        /// <summary>
        /// Updates the UI when <see cref="CharacterProperty"/> changes
        /// </summary>
        /// <param name="d">The source <see cref="DependencyObject"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> info for the current update</param>
        private static void OnCharacterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBlock @this = (TextBlock)d;
            char c = (char)e.NewValue;

            @this.RenderTransform = c switch
            {
                (char)32 => TransformsMap[2],
                (char)127 => TransformsMap[3],
                (char)160 => TransformsMap[3],
                (char)173 => TransformsMap[3],
                _ when !char.IsControl(c) => null,
                _ when c < 10 => null,
                _ when c < 100 => TransformsMap[2],
                _ => TransformsMap[3]
            };
        }
    }
}
