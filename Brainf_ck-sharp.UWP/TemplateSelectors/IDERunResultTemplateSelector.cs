using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.DataModels.IDEResults;
using Brainf_ck_sharp_UWP.Helpers.Extensions;

namespace Brainf_ck_sharp_UWP.TemplateSelectors
{
    /// <summary>
    /// A template selector for the flyout that shows the results for a script run in the IDE
    /// </summary>
    public class IDERunResultTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (container is FrameworkElement parent)
            {
                switch (item.To<IDEResultSectionDataBase>())
                {
                    case IDEResultSectionSessionData section when section.Section == IDEResultSection.Stdout:
                        return parent.FindResource<DataTemplate>("StdoutTemplate");
                    case IDEResultSectionSessionData section when section.Section == IDEResultSection.SourceCode:
                        return parent.FindResource<DataTemplate>("SourceCodeTemplate");
                    case IDEResultSectionSessionData section when section.Section == IDEResultSection.StackTrace:
                        return parent.FindResource<DataTemplate>("StackTraceTemplate");
                    case IDEResultSectionSessionData section when section.Section == IDEResultSection.ErrorLocation ||
                        section.Section == IDEResultSection.BreakpointReached:
                        return parent.FindResource<DataTemplate>("StopPositionTemplate");
                    case IDEResultExceptionInfoData _:
                        return parent.FindResource<DataTemplate>("ExceptionInfoTemplate");
                    case IDEResultSectionStateData _:
                        return parent.FindResource<DataTemplate>("MemoryStateTemplate");
                }
            }
            return null;
        }
    }
}
