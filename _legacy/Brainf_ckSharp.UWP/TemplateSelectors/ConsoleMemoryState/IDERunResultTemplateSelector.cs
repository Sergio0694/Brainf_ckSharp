using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp.Legacy.UWP.DataModels.ConsoleMemoryViewer;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;

namespace Brainf_ck_sharp.Legacy.UWP.TemplateSelectors.ConsoleMemoryState
{
    /// <summary>
    /// A template selector for the flyout that shows the complete console memory state info
    /// </summary>
    public class ConsoleFullMemoryViewerSectionTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (container is FrameworkElement parent)
            {
                switch (item.To<MemoryViewerSectionBase>().SectionType)
                {
                    case ConsoleMemoryViewerSection.FunctionsList:
                        return parent.FindResource<DataTemplate>("FunctionDefinitionsTemplate");
                    case ConsoleMemoryViewerSection.MemoryCells:
                        return parent.FindResource<DataTemplate>("MemoryStateTemplate");
                    default:
                        throw new ArgumentOutOfRangeException("Invalid section type");
                }
            }
            return null;
        }
    }
}
