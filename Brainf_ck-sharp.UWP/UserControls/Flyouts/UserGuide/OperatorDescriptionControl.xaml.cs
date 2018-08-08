using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts.UserGuide
{
    public sealed partial class OperatorDescriptionControl : UserControl
    {
        public OperatorDescriptionControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the operator to display on the current control
        /// </summary>
        public string Operator
        {
            get => OperatorBlock.Text;
            set => OperatorBlock.Text = value;
        }

        /// <summary>
        /// Gets or sets the foreground brush for the current operator character
        /// </summary>
        public Brush OperatorForegroundBrush
        {
            get => OperatorBlock.Foreground;
            set => OperatorBlock.Foreground = value;
        }

        /// <summary>
        /// Gets or sets the description for the current operator
        /// </summary>
        public string Description
        {
            get => DescriptionBlock.Text;
            set => DescriptionBlock.Text = value;
        }
    }
}
