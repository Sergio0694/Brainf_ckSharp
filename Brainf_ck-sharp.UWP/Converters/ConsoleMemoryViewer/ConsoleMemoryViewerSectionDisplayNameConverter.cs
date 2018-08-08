using System;
using Windows.UI.Xaml.Data;
using Brainf_ck_sharp_UWP.DataModels.ConsoleMemoryViewer;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.Extensions;

namespace Brainf_ck_sharp_UWP.Converters.ConsoleMemoryViewer
{
    /// <summary>
    /// A converter that returns a readable string that describes the current section
    /// </summary>
    public class ConsoleMemoryViewerSectionDisplayNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Convert(value.To<ConsoleMemoryViewerSection>());
        }

        /// <summary>
        /// Converts a given result to its display value
        /// </summary>
        /// <param name="section">The input result to convert</param>
        public static string Convert(ConsoleMemoryViewerSection section)
        {
            switch (section)
            {
                case ConsoleMemoryViewerSection.MemoryCells:
                    return LocalizationManager.GetResource("MemoryStateTitle");
                case ConsoleMemoryViewerSection.FunctionsList:
                    return LocalizationManager.GetResource("FunctionDefinitionsTitle");
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
