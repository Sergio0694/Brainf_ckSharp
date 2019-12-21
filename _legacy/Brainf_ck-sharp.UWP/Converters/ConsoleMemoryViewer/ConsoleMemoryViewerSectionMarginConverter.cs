using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Brainf_ck_sharp_UWP.DataModels.ConsoleMemoryViewer;
using Brainf_ck_sharp_UWP.Helpers.Extensions;

namespace Brainf_ck_sharp_UWP.Converters.ConsoleMemoryViewer
{
    public class ConsoleMemoryViewerSectionMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string[] @params = parameter.To<string>().Split('_');
            if (@params.Length != 5) throw new ArgumentException("Invalid margin parameter");
            double[] values = @params.Select(double.Parse).ToArray();
            switch (value.To<ConsoleMemoryViewerSection>())
            {
                case ConsoleMemoryViewerSection.MemoryCells:
                    return new Thickness(values[0], values[1], values[2], values[3]);
                case ConsoleMemoryViewerSection.FunctionsList:
                    return new Thickness(values[0], values[1], values[2], values[4]);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
