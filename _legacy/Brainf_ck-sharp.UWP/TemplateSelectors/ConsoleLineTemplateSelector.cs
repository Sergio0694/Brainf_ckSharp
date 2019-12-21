using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.DataModels.ConsoleModels;
using Brainf_ck_sharp_UWP.Helpers.Extensions;

namespace Brainf_ck_sharp_UWP.TemplateSelectors
{
    /// <summary>
    /// A template selector for the line to display in the console view
    /// </summary>
    public class ConsoleLineTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (container is FrameworkElement parent)
            {
                switch (item)
                {
                    case ConsoleUserCommand _: return parent.FindResource<DataTemplate>("UserCommandTemplate");
                    case ConsoleRestartCommand _: return parent.FindResource<DataTemplate>("RestartConsoleTemplate");
                    case ConsoleCommandResult _: return parent.FindResource<DataTemplate>("ConsoleResultTemplate");
                    case ConsoleExceptionResult _: return parent.FindResource<DataTemplate>("ConsoleExceptionResult");
                }
            }
            return null;
        }
    }
}
