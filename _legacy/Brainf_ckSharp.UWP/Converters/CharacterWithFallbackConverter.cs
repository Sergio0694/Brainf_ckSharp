﻿using System;
using Windows.UI.Xaml.Data;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;

namespace Brainf_ck_sharp.Legacy.UWP.Converters
{
    /// <summary>
    /// A converter that returns a printable character or the string representation of its value
    /// </summary>
    public class CharacterWithFallbackConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            char c = value.To<char>();
            return c > 32 && c < 127 || c > 160 && c != 173 ? c.ToString() : ((int)c).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}