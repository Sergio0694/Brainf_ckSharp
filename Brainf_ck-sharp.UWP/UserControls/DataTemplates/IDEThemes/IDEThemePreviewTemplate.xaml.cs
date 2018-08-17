using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Brainf_ck_sharp_UWP.DataModels.Misc.Themes;
using Brainf_ck_sharp_UWP.Helpers.CodeFormatting;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.Helpers.Settings;
using Brainf_ck_sharp_UWP.Messages.UI;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.UserControls.DataTemplates.IDEThemes
{
    public sealed partial class IDEThemePreviewTemplate : UserControl
    {
        public IDEThemePreviewTemplate()
        {
            this.InitializeComponent();
            if (DesignMode.DesignModeEnabled) return;
            string name = AppSettingsManager.Instance.GetValue<string>(nameof(AppSettingsKeys.SelectedFontName));
            if (InstalledFont.TryGetFont(name, out InstalledFont font))
            {
                UpdateUIOnFontFamilyChanged(font.Family);
            }
            Messenger.Default.Register<IDEThemePreviewFontChangedMessage>(this, m =>
            {
                UpdateUIOnFontFamilyChanged(m.Value.Family);
            });
        }

        // Adjusts the UI when the font is changed
        private void UpdateUIOnFontFamilyChanged([NotNull] FontFamily font)
        {
            LineNumbersBlock.FontFamily = font;
            PreviewBlock.FontFamily = font;
            double lineHeight = "Xg".MeasureText(15, font).Height;
            LineHighlightBorder.Height = lineHeight;
            BracketsGuideClip.Rect = new Rect(0, 0, 1, lineHeight);
            BracketsGuideTransform.Y = lineHeight + 2;
            LineHighlightBorder.Height = lineHeight;
            LineHighlightTransform.Y = Theme?.InnerValue.LineHighlightStyle == LineHighlightStyle.Fill
                ? lineHeight * 2 + 2
                : lineHeight + 2;
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
            nameof(Theme), typeof(SelectableIDEThemeInfo), typeof(IDEThemePreviewTemplate), new PropertyMetadata(default(SelectableIDEThemeInfo), OnThemePropertyChanged));

        private static void OnThemePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IDEThemePreviewTemplate @this = d.To<IDEThemePreviewTemplate>();
            IDEThemeInfo info = e.NewValue.To<SelectableIDEThemeInfo>()?.InnerValue;
            if (info != null)
            {
                @this.TitleBlock.Text = info.Name;
                @this.BackgroundGrid.Background = info.Background.ToBrush().With(b => b.Opacity = 0.8);
                @this.BreakpointsCanvas.Background = @this.HeaderBorder.Background = info.BreakpointsPaneBackground.ToBrush().With(b => b.Opacity = 0.8);
                @this.CommaRun.Foreground = info.HighlightMap[','].ToBrush();
                @this.OperatorsRun.Foreground = @this.OperatorsRun2.Foreground = @this.OperatorsRun3.Foreground = info.HighlightMap['+'].ToBrush();
                @this.BracketRun.Foreground = @this.BracketRun2.Foreground = info.HighlightMap['['].ToBrush();
                @this.ArrowRun.Foreground = @this.ArrowRun2.Foreground = @this.ArrowRun3.Foreground = info.HighlightMap['>'].ToBrush();
                @this.LineNumbersBlock.Foreground = info.LineNumberColor.ToBrush();
                @this.CommentRun.Foreground = @this.CommentRun2.Foreground = info.CommentsColor.ToBrush();
                @this.DotRun.Foreground = info.HighlightMap['.'].ToBrush();
                @this.BracketsGuidePanel.Children.Clear();
                if (info.BracketsGuideStrokesLength == null)
                {
                    // Add the single vertical line
                    @this.BracketsGuidePanel.Children.Add(new Rectangle
                    {
                        Width = 1,
                        Height = 32,
                        Fill = info.BracketsGuideColor.ToBrush()
                    });
                }
                else
                {
                    // 6 is arbitrary, but large enough in this situation
                    for (int i = 0; i < 6; i++)
                    {
                        @this.BracketsGuidePanel.Children.Add(new Rectangle
                        {
                            Width = 1,
                            Height = info.BracketsGuideStrokesLength.Value,
                            Fill = info.BracketsGuideColor.ToBrush(),
                            Margin = i > 0 ? new Thickness(0, info.BracketsGuideStrokesLength.Value, 0, 0) : new Thickness()
                        });
                    }
                }
                if (info.LineHighlightStyle == LineHighlightStyle.Outline)
                {
                    @this.LineHighlightBorder.BorderBrush = info.LineHighlightColor.ToBrush();
                    @this.LineHighlightBorder.Background = Colors.Transparent.ToBrush();
                    Canvas.SetZIndex(@this.BracketsGuidePanel, 0);
                    Canvas.SetZIndex(@this.LineHighlightBorder, 1);
                    @this.LineHighlightTransform.Y = "Xg".MeasureText(15, @this.LineNumbersBlock.FontFamily).Height + 2;
                }
                else
                {
                    @this.LineHighlightBorder.Background = info.LineHighlightColor.ToBrush();
                    Canvas.SetZIndex(@this.BracketsGuidePanel, 1);
                    Canvas.SetZIndex(@this.LineHighlightBorder, 0);
                    @this.LineHighlightTransform.Y = "Xg".MeasureText(15, @this.LineNumbersBlock.FontFamily).Height * 2 + 2;
                }
                @this.LineHighlightBorder.BorderThickness = new Thickness(info.LineHighlightStyle == LineHighlightStyle.Outline ? 2 : 0);
            }
        }
    }
}
