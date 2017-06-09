using System;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Brainf_ck_sharp_UWP.UserControls.Header
{
    public sealed partial class HeaderControl : UserControl
    {
        public HeaderControl()
        {
            this.InitializeComponent();
        }

        public void DeselectIDEButton()
        {
            if (IDEButton != null)
                IDEButton.IsSelected = false;
        }

        public void DeselectConsoleButton()
        {
            if (ConsoleButton != null)
                ConsoleButton.IsSelected = false;
        }
    }
}
