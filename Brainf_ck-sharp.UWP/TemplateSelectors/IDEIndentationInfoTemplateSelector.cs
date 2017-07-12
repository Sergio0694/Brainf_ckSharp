﻿using Windows.UI.Xaml;
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
                    case IDEIndentationOpenBracketLineInfo open:
                        return parent.FindResource<DataTemplate>(open.LineType == IDEIndentationInfoLineType.OpenBracket
                            ? "IDEIndentationInfoOpenBracketTemplate"
                            : "IDESelfContainedIndentationInfoOpenBracketTemplate");
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
