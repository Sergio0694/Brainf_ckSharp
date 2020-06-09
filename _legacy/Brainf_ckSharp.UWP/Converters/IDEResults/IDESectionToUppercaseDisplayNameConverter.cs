using System;
using Windows.UI.Xaml.Data;
using Brainf_ck_sharp.Legacy.UWP.DataModels.IDEResults;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;

namespace Brainf_ck_sharp.Legacy.UWP.Converters.IDEResults
{
    /// <summary>
    /// A converter that returns a readable string that describes the current section, in uppercase
    /// </summary>
    public class IDESectionToUppercaseDisplayNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return IDESectionToDisplayNameConverter.Convert(value.To<IDEResultSection>()).ToUpperInvariant();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
