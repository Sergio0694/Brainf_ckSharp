using System;
using Windows.UI.Xaml.Data;
using Brainf_ckSharp.UWP.Enums;

namespace Brainf_ckSharp.UWP.Bindings.Converters.UserGuide
{
    /// <summary>
    /// A converter that returns an uppercase title for a given <see cref="UserGuideSection"/> value
    /// </summary>
    public sealed class UserGuideSectionToTitleConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value switch
            {
                UserGuideSection.Introduction => "INTRODUCTION",
                UserGuideSection.PBrain => "PBRAIN",
                UserGuideSection.Debugging => "DEBUGGING",
                UserGuideSection.Samples => "SAMPLES",
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
