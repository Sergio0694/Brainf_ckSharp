using Windows.System;
using Windows.UI.Xaml.Markup;

namespace Brainf_ckSharp.Uwp.MarkupExtensions
{
    /// <summary>
    /// A markup extension that converts <see cref="VirtualKey"/> values in XAML to their boxed version
    /// </summary>
    [MarkupExtensionReturnType(ReturnType = typeof(VirtualKey))]
    public sealed class VirtualKeyExtension : MarkupExtension
    {
        /// <summary>
        /// Gets or sets the <see cref="VirtualKey"/> value for the current instance
        /// </summary>
        public VirtualKey Value { get; set; }

        /// <inheritdoc/>
        protected override object ProvideValue() => Value;
    }
}
