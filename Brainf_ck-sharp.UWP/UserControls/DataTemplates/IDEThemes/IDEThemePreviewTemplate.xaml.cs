using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.DataModels.Misc;
using Brainf_ck_sharp_UWP.Helpers.Extensions;

namespace Brainf_ck_sharp_UWP.UserControls.DataTemplates.IDEThemes
{
    public sealed partial class IDEThemePreviewTemplate : UserControl
    {
        public IDEThemePreviewTemplate()
        {
            this.InitializeComponent();
        }

        public IDEThemeInfo ViewModel => DataContext.To<IDEThemeInfo>();
    }
}
