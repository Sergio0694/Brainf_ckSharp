using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.DataModels.Misc;
using Brainf_ck_sharp_UWP.Helpers;

namespace Brainf_ck_sharp_UWP.TemplateSelectors
{
    /// <summary>
    /// A template selector for the state of a line in the IDE
    /// </summary>
    public class GitDiffLineStatusTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (container is FrameworkElement parent)
            {
                switch (item.To<GitDiffLineStatus>())
                {
                    case GitDiffLineStatus.Undefined: return parent.FindResource<DataTemplate>("DiffEmptyLineTemplate");
                    case GitDiffLineStatus.Edited: return parent.FindResource<DataTemplate>("DiffChangedLineTemplate");
                    case GitDiffLineStatus.Saved: return parent.FindResource<DataTemplate>("DiffSavedLineTemplate");
                }
            }
            return null;
        }
    }
}
