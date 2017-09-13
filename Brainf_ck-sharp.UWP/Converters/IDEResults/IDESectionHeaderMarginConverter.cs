using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Brainf_ck_sharp_UWP.DataModels.IDEResults;
using Brainf_ck_sharp_UWP.Helpers.Extensions;

namespace Brainf_ck_sharp_UWP.Converters.IDEResults
{
    public class IDESectionHeaderMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            String[] @params = parameter.To<String>().Split('_');
            if (@params.Length != 5) throw new ArgumentException("Invalid margin parameter");
            double[] values = @params.Select(double.Parse).ToArray();
            return new Thickness(values[0], values[1], values[2], values[value.To<IDEResultSection>() == IDEResultSection.FunctionDefinitions ? 4 : 3]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
