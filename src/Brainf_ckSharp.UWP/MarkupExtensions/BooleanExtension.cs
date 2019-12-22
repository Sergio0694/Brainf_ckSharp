using Windows.UI.Xaml.Markup;

namespace Brainf_ckSharp.UWP.MarkupExtensions
{
    /// <summary>
    /// A markup extension that converts <see cref="bool"/> values in XAML to their boxed version
    /// </summary>
    [MarkupExtensionReturnType(ReturnType = typeof(object))]
    public sealed class BooleanExtension : MarkupExtension
    {
        /// <summary>
        /// Gets or sets the <see cref="bool"/> value for the current instance
        /// </summary>
        public bool Value { get; set; }

        /// <inheritdoc/>
        protected override object ProvideValue() => Value;
    }
}
