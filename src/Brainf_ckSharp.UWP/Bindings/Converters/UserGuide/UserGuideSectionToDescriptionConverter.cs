using System;
using Windows.UI.Xaml.Data;
using Brainf_ckSharp.Uwp.Enums;

namespace Brainf_ckSharp.Uwp.Bindings.Converters.UserGuide
{
    /// <summary>
    /// A converter that returns a description for a given <see cref="UserGuideSection"/> value
    /// </summary>
    public sealed class UserGuideSectionToDescriptionConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value switch
            {
                UserGuideSection.Introduction => "Learn how to use the Brainf*ck language",
                UserGuideSection.PBrain => "Do more by using the PBrain extension operators",
                UserGuideSection.Debugging => "Find errors more easily with debugging features",
                UserGuideSection.Samples => "Some code samples to learn the basics",
                _ => throw new ArgumentException($"Invalid input value: {value}", nameof(value))
            };
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
