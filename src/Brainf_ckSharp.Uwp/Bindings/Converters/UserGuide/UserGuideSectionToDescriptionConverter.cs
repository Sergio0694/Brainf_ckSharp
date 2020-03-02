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
            if (value == UserGuideSection.Introduction) return "Learn how to use the Brainf*ck language";
            if (value == UserGuideSection.PBrain) return "Do more by using the PBrain extension operators";
            if (value == UserGuideSection.Debugging) return "Find errors more easily with debugging features";
            if (value == UserGuideSection.Samples) return "Some code samples to learn the basics";

            throw new ArgumentException($"Invalid input value: {value}", nameof(value));
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
