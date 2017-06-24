using System;
using Windows.UI.Xaml.Data;
using Brainf_ck_sharp_UWP.DataModels.Misc;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.Extensions;

namespace Brainf_ck_sharp_UWP.Converters
{
    /// <summary>
    /// A converter that returns a text representiation of a <see cref="ScriptExceptionType"/> value
    /// </summary>
    public class ExceptionTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Convert(value.To<ScriptExceptionType>());
        }

        /// <summary>
        /// Converts the given type into its representation
        /// </summary>
        /// <param name="type">The input type</param>
        public static String Convert(ScriptExceptionType type)
        {
            switch (type)
            {
                case ScriptExceptionType.SyntaxError:
                    return LocalizationManager.GetResource("SyntaxError");
                case ScriptExceptionType.RuntimeError:
                    return LocalizationManager.GetResource("Exception");
                case ScriptExceptionType.ThresholdExceeded:
                    return LocalizationManager.GetResource("Threshold");
                default:
                    return LocalizationManager.GetResource("InternalError");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
