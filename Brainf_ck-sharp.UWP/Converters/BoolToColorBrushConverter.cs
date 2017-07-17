﻿using System;
using Windows.UI.Xaml.Data;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Microsoft.Toolkit.Uwp;

namespace Brainf_ck_sharp_UWP.Converters
{
    /// <summary>
    /// A converter that picks between two input colors depending on the target value
    /// </summary>
    public class BoolToColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            String[] colors = parameter.To<String>()?.Split('_');
            if (colors?.Length != 2) return null;
            return colors[value.To<bool>() ? 0 : 1].ToColor().ToBrush();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
