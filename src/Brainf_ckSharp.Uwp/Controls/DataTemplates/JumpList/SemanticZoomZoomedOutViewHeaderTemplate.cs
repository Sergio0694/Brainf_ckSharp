using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Brainf_ckSharp.Uwp.Controls.DataTemplates.JumpList
{
    /// <summary>
    /// A <see cref="Control"/> that represents a zoomed out template for a grouped collection
    /// </summary>
    public sealed class SemanticZoomZoomedOutViewHeaderTemplate : Control
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
            typeof(SemanticZoomZoomedOutViewHeaderTemplate),
            new PropertyMetadata(DependencyProperty.UnsetValue));

        /// <summary>
        /// Gets or sets the Description for the current control
        /// </summary>
        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        /// <summary>
        /// The dependency property for <see cref="Description"/>
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            nameof(Description),
            typeof(string),
            typeof(SemanticZoomZoomedOutViewHeaderTemplate),
            new PropertyMetadata(DependencyProperty.UnsetValue));
    }
}
