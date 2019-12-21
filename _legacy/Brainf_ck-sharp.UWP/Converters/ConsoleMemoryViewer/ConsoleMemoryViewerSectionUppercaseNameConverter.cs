using System;
using Windows.UI.Xaml.Data;
using Brainf_ck_sharp.Legacy.UWP.DataModels.ConsoleMemoryViewer;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;

namespace Brainf_ck_sharp.Legacy.UWP.Converters.ConsoleMemoryViewer
{
    /// <summary>
    /// A converter that returns a readable string that describes the current section, in uppercase
    /// </summary>
    public class ConsoleMemoryViewerSectionUppercaseNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ConsoleMemoryViewerSectionDisplayNameConverter.Convert(value.To<ConsoleMemoryViewerSection>()).ToUpperInvariant();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
