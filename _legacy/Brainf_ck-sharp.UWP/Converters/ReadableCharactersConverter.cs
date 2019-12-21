using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;

namespace Brainf_ck_sharp.Legacy.UWP.Converters
{
    public class ReadableCharactersConverter : IValueConverter
    {
        // A dictionary with some substitutions for special characters to display
        private static readonly IReadOnlyDictionary<int, string> SpecialCharactersDisplayMap = new Dictionary<int, string>
        {
            { 32, "SP" },
            { 127, "DEL" },
            { 160, "NBSP" },
            { 173, "SHY" }
        };

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int i = value.To<int>();
            return SpecialCharactersDisplayMap.TryGetValue(i, out string s) 
                ? s 
                : System.Convert.ToChar(i).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
