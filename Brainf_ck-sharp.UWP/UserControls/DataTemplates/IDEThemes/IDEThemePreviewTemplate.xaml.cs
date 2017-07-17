using Windows.UI.Xaml;
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

        /// <summary>
        /// Gets or sets the current <see cref="IDEThemeInfo"/> instance to display in the preview
        /// </summary>
        public IDEThemeInfo Theme
        {
            get => GetValue(ThemeProperty).To<IDEThemeInfo>();
            set => SetValue(ThemeProperty, value);
        }

        public static readonly DependencyProperty ThemeProperty = DependencyProperty.Register(
            nameof(Theme), typeof(IDEThemeInfo), typeof(IDEThemePreviewTemplate), new PropertyMetadata(default(IDEThemeInfo), OnThemePropertyChanged));

        private static void OnThemePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IDEThemePreviewTemplate @this = d.To<IDEThemePreviewTemplate>();
            IDEThemeInfo info = e.NewValue.To<IDEThemeInfo>();
            if (info != null)
            {
                @this.TitleBlock.Text = info.Name;
                @this.BackgroundGrid.Background = info.Background.ToBrush();
                @this.BreakpointsCanvas.Background = info.BreakpointsPaneBackground.ToBrush();
                @this.CommaRun.Foreground = info.HighlightMap[','].ToBrush();
                @this.OperatorsRun.Foreground = @this.OperatorsRun2.Foreground = @this.OperatorsRun3.Foreground = info.HighlightMap['+'].ToBrush();
                @this.BracketRun.Foreground = @this.BracketRun2.Foreground = info.HighlightMap['['].ToBrush();
                @this.ArrowRun.Foreground = @this.ArrowRun2.Foreground = @this.ArrowRun3.Foreground = info.HighlightMap['>'].ToBrush();
                @this.LineNumbersBlock.Foreground = info.LineNumberColor.ToBrush();
                @this.CommentRun.Foreground = @this.CommentRun2.Foreground = info.CommentsColor.ToBrush();
                @this.DotRun.Foreground = info.HighlightMap['.'].ToBrush();
            }
        }
    }
}
