using System;
using Windows.UI.Xaml.Markup;

namespace Brainf_ckSharp.Uwp.MarkupExtensions
{
    /// <summary>
    /// A markup extension that returns a collection of values of a specific <see langword="enum"/>
    /// </summary>
    [MarkupExtensionReturnType(ReturnType = typeof(Array))]
    public sealed class EnumValuesExtension : MarkupExtension
    {
        /// <summary>
        /// Gets or sets the <see cref="System.Type"/> of the target <see langword="enum"/>
        /// </summary>
        public Type Type { get; set; }

        /// <inheritdoc/>
        protected override object ProvideValue() => Enum.GetValues(Type);
    }
}
