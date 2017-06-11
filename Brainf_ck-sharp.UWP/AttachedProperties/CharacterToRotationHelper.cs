using System;
using System.Collections.Generic;
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

        // Transforms map to use (values tested in the XAML designer)
        private static readonly IReadOnlyDictionary<int, CompositeTransform> TransformsMap = new Dictionary<int, CompositeTransform>
        {
            { 1, new CompositeTransform
                {
                    Rotation = -70,
                    CenterX = 11,
                    CenterY = 15,
                    ScaleX = 0.8,
                    ScaleY = 0.8
                }
            },
            { 2, new CompositeTransform
                {
                    Rotation = -70,
                    CenterX = 12,
                    CenterY = 16,
                    ScaleX = 0.7,
                    ScaleY = 0.7
                }
            },
            { 3, new CompositeTransform
                {
                    Rotation = -70,
                    CenterX = 14,
                    CenterY = 18,
                    ScaleX = 0.8,
                    ScaleY = 0.8
                }
            }
        };

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBlock @this = d.To<TextBlock>();
            char c = e.NewValue.To<char>();
            bool valid = c == 0 || c > 31 && c < 128 || c > 159;
            int digits = (int)Math.Floor(Math.Log10(c) + 1);
            @this.RenderTransform = valid
            ? null
            : TransformsMap[digits];
        }
    }
}
