using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts.UserGuide.Templates
{
    public sealed partial class GuideItemDescriptionControl : UserControl
    {
        public GuideItemDescriptionControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the font size for the item name
        /// </summary>
        public double ItemNameFontSize
        {
            get => ItemNameBlock.FontSize;
            set => ItemNameBlock.FontSize = value;
        }

        /// <summary>
        /// Gets or sets the name of the current item to describe
        /// </summary>
        public string ItemName
        {
            get => ItemNameBlock.Text;
            set => ItemNameBlock.Text = value;
        }

        /// <summary>
        /// Gets or sets the foreground brush for the current item name
        /// </summary>
        public Brush ItemNameForegroundBrush
        {
            get => ItemNameBlock.Foreground;
            set => ItemNameBlock.Foreground = value;
        }

        /// <summary>
        /// Gets or sets the description for the current item
        /// </summary>
        public string Description
        {
            get => DescriptionBlock.Text;
            set => DescriptionBlock.Text = value;
        }

        /// <summary>
        /// Gets or sets the collection of <see cref="Inline"/> elements to display in the current control
        /// </summary>
        [NotNull]
        public InlineCollection DescriptionInlines
        {
            get => DescriptionBlock.Inlines;
            set
            {
                if (DescriptionInlines != value)
                {
                    DescriptionBlock.Inlines.Clear();
                    foreach (Inline inline in value) DescriptionBlock.Inlines.Add(inline);
                }
            }
        }
    }
}
