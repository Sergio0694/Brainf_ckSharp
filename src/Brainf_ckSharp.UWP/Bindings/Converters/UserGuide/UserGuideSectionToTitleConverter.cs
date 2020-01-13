using System;
using Windows.UI.Xaml.Data;
using Brainf_ckSharp.Uwp.Enums;

namespace Brainf_ckSharp.Uwp.Bindings.Converters.UserGuide
{
    /// <summary>
    /// A converter that returns an uppercase title for a given <see cref="UserGuideSection"/> value
    /// </summary>
    public sealed class UserGuideSectionToTitleConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == UserGuideSection.Introduction) return "INTRODUCTION";
            if (value == UserGuideSection.PBrain) return "PBRAIN";
            if (value == UserGuideSection.Debugging) return "DEBUGGING";
            if (value == UserGuideSection.Samples) return "SAMPLES";

            throw new ArgumentException($"Invalid input value: {value}", nameof(value));
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
