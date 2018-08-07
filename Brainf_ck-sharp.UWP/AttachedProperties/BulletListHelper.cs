using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.AttachedProperties
{
    /// <summary>
    /// A class with an attached XAML property to parse bullet lists to display in a target <see cref="TextBlock"/> control
    /// </summary>
    public static class BulletListHelper
    {
        [UsedImplicitly] // XAML attached property
        public static IReadOnlyList<string> GetSourceList(TextBlock element)
        {
            return element.GetValue(SourceListProperty).To<IReadOnlyList<string>>();
        }

        public static void SetSourceList(TextBlock element, IReadOnlyList<string> value)
        {
            element?.SetValue(SourceListProperty, value);
        }

        public static readonly DependencyProperty SourceListProperty =
            DependencyProperty.RegisterAttached("SourceList", typeof(IReadOnlyList<string>), typeof(BulletListHelper), 
                new PropertyMetadata(DependencyProperty.UnsetValue, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBlock @this = d.To<TextBlock>();
            @this.Inlines.Clear();
            IReadOnlyList<string> inlines = e.NewValue.To<IReadOnlyList<string>>();
            if (inlines == null || inlines.Count == 0) return;
            for (int i = 0; i < inlines.Count; i++)
            {
                string inlineEnd = i == inlines.Count - 1 ? "" : "\n";
                @this.Inlines.Add(new Run { Text = $"• {inlines[i]}{inlineEnd}" });
            }
        }
    }
}
