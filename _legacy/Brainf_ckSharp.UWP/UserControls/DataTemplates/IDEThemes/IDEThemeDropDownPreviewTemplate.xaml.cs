using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp.Legacy.UWP.DataModels.Misc.Themes;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;

namespace Brainf_ck_sharp.Legacy.UWP.UserControls.DataTemplates.IDEThemes
{
    public sealed partial class IDEThemeDropDownPreviewTemplate : UserControl
    {
        public IDEThemeDropDownPreviewTemplate()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the current <see cref="SelectableIDEThemeInfo"/> instance to display in the preview
        /// </summary>
        public SelectableIDEThemeInfo Theme
        {
            get => GetValue(ThemeProperty).To<SelectableIDEThemeInfo>();
            set => SetValue(ThemeProperty, value);
        }

        public static readonly DependencyProperty ThemeProperty = DependencyProperty.Register(
            nameof(Theme), typeof(SelectableIDEThemeInfo), typeof(IDEThemeDropDownPreviewTemplate), new PropertyMetadata(default(SelectableIDEThemeInfo), OnThemePropertyChanged));

        private static void OnThemePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SelectableIDEThemeInfo theme = e.NewValue.To<SelectableIDEThemeInfo>();
            d.To<IDEThemeDropDownPreviewTemplate>().PreviewTemplate.Theme = theme;
        }
    }
}
