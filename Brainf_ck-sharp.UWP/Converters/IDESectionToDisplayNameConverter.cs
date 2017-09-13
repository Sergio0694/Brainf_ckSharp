using System;
using Windows.UI.Xaml.Data;
using Brainf_ck_sharp_UWP.DataModels.IDEResults;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.Extensions;

namespace Brainf_ck_sharp_UWP.Converters
{
    /// <summary>
    /// A converter that returns a readable string that describes the current section
    /// </summary>
    public class IDESectionToDisplayNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Convert(value.To<IDEResultSection>());
        }

        /// <summary>
        /// Converts a given result to its display value
        /// </summary>
        /// <param name="section">The input result to convert</param>
        public static String Convert(IDEResultSection section)
        {
            switch (section)
            {
                case IDEResultSection.ExceptionType:
                    return LocalizationManager.GetResource("ErrorType");
                case IDEResultSection.Stdout:
                    return "Stdout";
                case IDEResultSection.ErrorLocation:
                    return LocalizationManager.GetResource("ErrorLocation");
                case IDEResultSection.BreakpointReached:
                    return LocalizationManager.GetResource("BreakpointReached");
                case IDEResultSection.StackTrace:
                    return LocalizationManager.GetResource("StackTrace");
                case IDEResultSection.SourceCode:
                    return LocalizationManager.GetResource("SourceCode");
                case IDEResultSection.MemoryState:
                    return LocalizationManager.GetResource("MemoryStateTitle");
                case IDEResultSection.FunctionDefinitions:
                    return LocalizationManager.GetResource("FunctionDefinitionsTitle");
                case IDEResultSection.Stats:
                    return LocalizationManager.GetResource("Stats");
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
