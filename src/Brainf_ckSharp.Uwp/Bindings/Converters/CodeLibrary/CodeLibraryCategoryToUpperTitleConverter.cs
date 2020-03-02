using System;
using Windows.UI.Xaml.Data;
using Brainf_ckSharp.Uwp.Enums;

namespace Brainf_ckSharp.Uwp.Bindings.Converters.CodeLibrary
{
    /// <summary>
    /// A converter that returns an uppercase title for a given <see cref="CodeLibraryCategory"/> value
    /// </summary>
    public sealed class CodeLibraryCategoryToUpperTitleConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == CodeLibraryCategory.Favorites) return "FAVORITES";
            if (value == CodeLibraryCategory.Recent) return "RECENT";
            if (value == CodeLibraryCategory.Samples) return "SAMPLES";

            throw new ArgumentException($"Invalid input value: {value}", nameof(value));
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
