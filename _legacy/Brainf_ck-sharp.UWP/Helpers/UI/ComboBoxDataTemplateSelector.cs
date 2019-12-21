using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;

namespace Brainf_ck_sharp.Legacy.UWP.Helpers.UI
{
    /// <summary>
    /// A template selector for the ComboBox control
    /// </summary>
    public class ComboBoxDataTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the template for the single selected item shown in the control
        /// </summary>
        public DataTemplate SelectedTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template to use when showing the expanded drop down menu
        /// </summary>
        public DataTemplate DropDownTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return container.FindParent<ComboBoxItem>() == null ? SelectedTemplate : DropDownTemplate;
        }
    }
}
