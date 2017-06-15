using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
using Brainf_ck_sharp_UWP.Helpers;

namespace Brainf_ck_sharp_UWP.AttachedProperties
{
    /// <summary>
    /// An attached property that binds a text value to a <see cref="Run"/> object
    /// </summary>
    public static class RunInlineHelper
    {
        public static String GetBindableText(Run element)
        {
            return element.GetValue(BindableTextProperty).To<String>();
        }

        public static void SetBindableText(Run element, String value)
        {
            element?.SetValue(BindableTextProperty, value);
        }

        public static readonly DependencyProperty BindableTextProperty =
            DependencyProperty.RegisterAttached("BindableText", typeof(String), typeof(RunInlineHelper), new PropertyMetadata(String.Empty, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.To<Run>().Text = e.NewValue.To<String>() ?? String.Empty;
        }
    }
}
