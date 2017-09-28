using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.DataModels.Misc.IDEIndentationGuides;
using Brainf_ck_sharp_UWP.Helpers.Extensions;

namespace Brainf_ck_sharp_UWP.TemplateSelectors
{
    public class IDEIndentationInfoTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (container is FrameworkElement parent)
            {
                switch (item)
                {
                    case IDEIndentationOpenLoopBracketLineInfo open:
                        if (open.Type == IDEIndentationInfoOpenLoopBracketType.InFunction)
                        {
                            return parent.FindResource<DataTemplate>(open.LineType == IDEIndentationInfoLineType.OpenLoopBracket
                                ? "IDEIndentationInfoInFunctionOpenBracketTemplate"
                                : "IDESelfContainedIndentationInfoInFunctionOpenBracketTemplate");
                        }
                        else return parent.FindResource<DataTemplate>(open.LineType == IDEIndentationInfoLineType.OpenLoopBracket
                            ? "IDEIndentationInfoOpenBracketTemplate"
                            : "IDESelfContainedIndentationInfoOpenBracketTemplate");
                    case IDEIndentationFunctionBracketInfo function:
                        return parent.FindResource<DataTemplate>(function.LineType == IDEIndentationInfoLineType.OpenFunctionBracket
                            ? "IDEIndentationInfoOpenFunctionTemplate"
                            : "IDESelfContainedIndentationInfoFunctionTemplate");
                    case IDEIndentationLineInfo info when info.LineType == IDEIndentationInfoLineType.Straight: 
                        return parent.FindResource<DataTemplate>("IDEIndentationInfoStraightLineTemplate");
                    case IDEIndentationLineInfo info when info.LineType == IDEIndentationInfoLineType.ClosedBracket:
                        return parent.FindResource<DataTemplate>("IDEIndentationInfoClosedBracketTemplate");
                    case IDEIndentationLineInfo info when info.LineType == IDEIndentationInfoLineType.Empty:
                        return parent.FindResource<DataTemplate>("IDEIndentationInfoEmptyLineTemplate");
                }
            }
            return null;
        }
    }
}
