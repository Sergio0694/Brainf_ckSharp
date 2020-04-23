using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Brainf_ckSharp.Uwp.Themes;
using Brainf_ckSharp.Uwp.Themes.Enums;

namespace Brainf_ckSharp.Uwp.Controls.DataTemplates
{
    public sealed partial class IdeThemePreviewTemplate : UserControl
    {
        public IdeThemePreviewTemplate()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the current <see cref="Brainf_ckTheme"/> instance to display in the preview
        /// </summary>
        public Brainf_ckTheme Theme
        {
            get => (Brainf_ckTheme)GetValue(ThemeProperty);
            set => SetValue(ThemeProperty, value);
        }

        /// <summary>
        /// The <see cref="DependencyProperty"/> backing <see cref="Theme"/>
        /// </summary>
        public static readonly DependencyProperty ThemeProperty = DependencyProperty.Register(
            nameof(Theme),
            typeof(Brainf_ckTheme),
            typeof(IdeThemePreviewTemplate),
            new PropertyMetadata(DependencyProperty.UnsetValue, OnThemePropertyChanged));

        /// <summary>
        /// Updates the UI when <see cref="Theme"/> changes
        /// </summary>
        /// <param name="d">The <see cref="ThemeProperty"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance for the current change</param>
        private static void OnThemePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IdeThemePreviewTemplate @this = (IdeThemePreviewTemplate)d;

            if (!(e.NewValue is Brainf_ckTheme value)) return;

            // General UI
            @this.TitleBlock.Text = value.Name;
            @this.BackgroundBrush.Color = value.Background;
            @this.FrameBackground.Color = value.BreakpointsPaneBackground;
            @this.LineNumbersForegroundBrush.Color = value.LineNumberColor;

            // Code sample
            @this.CommaRun.Foreground = value.GetBrush(',');
            @this.OperatorsRun.Foreground = @this.OperatorsRun2.Foreground = @this.OperatorsRun3.Foreground = value.GetBrush('+');
            @this.BracketRun.Foreground = @this.BracketRun2.Foreground = value.GetBrush('[');
            @this.ArrowRun.Foreground = @this.ArrowRun2.Foreground = @this.ArrowRun3.Foreground = value.GetBrush('>');
            @this.CommentRun.Foreground = @this.CommentRun2.Foreground = value.GetBrush(' ');
            @this.DotRun.Foreground = value.GetBrush('.');

            // Setup the vertical column guides
            @this.BracketsGuidePanel.Children.Clear();

            if (value.BracketsGuideStrokesLength == null)
            {
                // Add the single vertical line
                @this.BracketsGuidePanel.Children.Add(new Rectangle
                {
                    Width = 1,
                    Height = 32,
                    Fill = new SolidColorBrush(value.BracketsGuideColor)
                });
            }
            else
            {
                // 6 is arbitrary, but large enough here
                for (int i = 0; i < 6; i++)
                {
                    @this.BracketsGuidePanel.Children.Add(new Rectangle
                    {
                        Width = 1,
                        Height = value.BracketsGuideStrokesLength.Value,
                        Fill = new SolidColorBrush(value.BracketsGuideColor),
                        Margin = i > 0 ? new Thickness(0, value.BracketsGuideStrokesLength.Value, 0, 0) : default
                    });
                }
            }

            // Selected line highlight
            if (value.LineHighlightStyle == LineHighlightStyle.Outline)
            {
                @this.LineHighlightBorder.BorderBrush = new SolidColorBrush(value.LineHighlightColor);
                @this.LineHighlightBorder.Background = new SolidColorBrush(Colors.Transparent);
                Canvas.SetZIndex(@this.BracketsGuidePanel, 0);
                Canvas.SetZIndex(@this.LineHighlightBorder, 1);
                @this.LineHighlightTransform.Y = 16;
            }
            else
            {
                @this.LineHighlightBorder.Background = new SolidColorBrush(value.LineHighlightColor);
                Canvas.SetZIndex(@this.BracketsGuidePanel, 1);
                Canvas.SetZIndex(@this.LineHighlightBorder, 0);
                @this.LineHighlightTransform.Y = 32;
            }

            @this.LineHighlightBorder.BorderThickness = new Thickness(value.LineHighlightStyle == LineHighlightStyle.Outline ? 2 : 0);
        }
    }
}
