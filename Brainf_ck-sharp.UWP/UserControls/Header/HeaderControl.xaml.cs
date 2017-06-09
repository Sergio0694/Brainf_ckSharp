using System;
using Windows.UI.Xaml.Controls;

namespace Brainf_ck_sharp_UWP.UserControls.Header
{
    public sealed partial class HeaderControl : UserControl
    {
        public HeaderControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Raised whenever the header selection changes
        /// </summary>
        public event EventHandler<int> SelectedIndezChanged;

        public void DeselectIDEButton()
        {
            if (IDEButton != null)
                IDEButton.IsSelected = false;
            SelectedIndezChanged?.Invoke(this, 0);
        }

        public void DeselectConsoleButton()
        {
            if (ConsoleButton != null)
                ConsoleButton.IsSelected = false;
            SelectedIndezChanged?.Invoke(this, 1);
        }
    }
}
