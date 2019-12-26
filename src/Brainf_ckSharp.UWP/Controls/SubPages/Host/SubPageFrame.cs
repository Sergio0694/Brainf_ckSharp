using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#nullable enable

namespace Brainf_ckSharp.UWP.Controls.SubPages.Host
{
    /// <summary>
    /// A <see cref="ContentControl"/> that acts as a frame for a sub page
    /// </summary>
    public sealed class SubPageFrame : ContentControl
    {
        /// <summary>
        /// Gets or sets the title for the current frame
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
            typeof(SubPageFrame),
            new PropertyMetadata(string.Empty));
    }
}
