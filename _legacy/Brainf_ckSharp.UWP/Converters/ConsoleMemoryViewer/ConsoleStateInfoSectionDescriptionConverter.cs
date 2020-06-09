using System;
using Windows.UI.Xaml.Data;
using Brainf_ck_sharp.Legacy.UWP.DataModels.ConsoleMemoryViewer;
using Brainf_ck_sharp.Legacy.UWP.Helpers.UI;

namespace Brainf_ck_sharp.Legacy.UWP.Converters.ConsoleMemoryViewer
{
    /// <summary>
    /// A converter that displays a description for each section in the console state viewer
    /// </summary>
    public class ConsoleStateInfoSectionDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch (value)
            {
                case MemoryViewerMemoryCellsSectionData state:
                    return $"{state.State.Count} {LocalizationManager.GetResource("MemoryCells")}";
                case MemoryViewerFunctionsSectionData state:
                    return $"{state.Functions.Count} {LocalizationManager.GetResource(state.Functions.Count > 1 ? "DefinedFunctions" : "DefinedFunction")}";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
