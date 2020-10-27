using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Brainf_ckSharp.Uwp.Controls.DataTemplates.JumpList
{
    /// <summary>
    /// A <see cref="Control"/> that represents a zoomed in template for a grouped collection
    /// </summary>
    public sealed class SemanticZoomZoomedInViewHeaderTemplate : Control
    {
        /// <summary>
        /// Gets or sets the title for the current control
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        /// <summary>
        /// The dependency property for <see cref="Title"/>
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(SemanticZoomZoomedInViewHeaderTemplate),
            new(DependencyProperty.UnsetValue));
    }
}
