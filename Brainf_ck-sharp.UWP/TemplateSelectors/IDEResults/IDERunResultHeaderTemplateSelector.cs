using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.DataModels;
using Brainf_ck_sharp_UWP.DataModels.IDEResults;
using Brainf_ck_sharp_UWP.Helpers.Extensions;

namespace Brainf_ck_sharp_UWP.TemplateSelectors.IDEResults
{
    /// <summary>
    /// A header template selector for the flyout that shows the results for a script run in the IDE
    /// </summary>
    public class IDERunResultHeaderTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (container is FrameworkElement parent)
            {
                switch ((item as JumpListGroup<IDEResultSection, IDEResultSectionDataBase>)?.Key)
                {
                    case null: return null;
                    case IDEResultSection.MemoryState:
                        return parent.FindResource<DataTemplate>("HeaderWithBottomMarginTemplate");
                    case IDEResultSection.FunctionDefinitions:
                        return parent.FindResource<DataTemplate>("HeaderWithBottomMarginTemplate");
                    default:
                        return parent.FindResource<DataTemplate>("HeaderTemplate");
                }
            }
            return null;
        }
    }
}
