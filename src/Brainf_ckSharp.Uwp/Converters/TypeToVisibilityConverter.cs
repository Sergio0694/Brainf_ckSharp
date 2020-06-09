using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Microsoft.Toolkit.HighPerformance.Extensions;

namespace Brainf_ckSharp.Uwp.Converters
{
    /// <summary>
    /// A converter that returns a <see cref="Visibility"/> value if a given value is of a specified type
    /// </summary>
    public sealed class TypeToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets the <see cref="Type"/> to match values against
        /// </summary>
        public Type TargetType { get; set; }

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (Visibility)(value?.GetType() != TargetType).ToInt();
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
