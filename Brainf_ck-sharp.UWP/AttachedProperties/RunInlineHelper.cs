using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.AttachedProperties
{
    /// <summary>
    /// An attached property that binds a text value to a <see cref="Run"/> object
    /// </summary>
    public static class RunInlineHelper
    {
        [UsedImplicitly] // XAML attached property
        public static string GetBindableText(Run element)
        {
            return element.GetValue(BindableTextProperty).To<string>();
        }

        public static void SetBindableText(Run element, string value)
        {
            element?.SetValue(BindableTextProperty, value);
        }

        public static readonly DependencyProperty BindableTextProperty =
            DependencyProperty.RegisterAttached("BindableText", typeof(string), typeof(RunInlineHelper), new PropertyMetadata(string.Empty, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.To<Run>().Text = e.NewValue.To<string>() ?? string.Empty;
        }
    }
}
