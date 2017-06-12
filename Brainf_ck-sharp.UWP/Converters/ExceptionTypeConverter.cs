﻿using System;
using Windows.UI.Xaml.Data;
using Brainf_ck_sharp_UWP.DataModels.Misc;
using Brainf_ck_sharp_UWP.Helpers;

namespace Brainf_ck_sharp_UWP.Converters
{
    /// <summary>
    /// A converter that returns a text representiation of a <see cref="ScriptExceptionType"/> value
    /// </summary>
    public class ExceptionTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch (value.To<ScriptExceptionType>())
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
