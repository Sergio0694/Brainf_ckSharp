using System;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Brainf_ck_sharp_UWP.Converters
{
    /// <summary>
    /// A simple converter that returns two margin values depending on the screen DPI setting
    /// </summary>
    public class DisplayScalingToMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DisplayInformation info = DisplayInformation.GetForCurrentView();
            return new Thickness(0, info.RawPixelsPerViewPixel < 2 ? -1 : -0.5, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
