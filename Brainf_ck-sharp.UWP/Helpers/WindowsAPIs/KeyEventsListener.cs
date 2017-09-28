using System;
using System.Linq;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Brainf_ck_sharp_UWP.Helpers.WindowsAPIs
{
    /// <summary>
    /// A static class that listens to the global KeyDown event
    /// </summary>
    public static class KeyEventsListener
    {
        private static bool _IsEnabled;

        /// <summary>
        /// Gets or sets whether or not the class is currently monitoring the KeyDown event
        /// </summary>
        public static bool IsEnabled
        {
            get => _IsEnabled;
            set
            {
                if (IsEnabled != value)
                {
                    if (value)
                    {
                        Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;

                    }
                    else
                    {
                        Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
                    }
                    _IsEnabled = value;
                }
            }
        }

        private static void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            // Propagate the event if needed
            VirtualKey modifier = new[] { VirtualKey.LeftShift, VirtualKey.Control, VirtualKey.LeftMenu }.FirstOrDefault(key =>
                (sender.GetKeyState(key) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down);

            // Raise the Esc event if needed
            if (args.VirtualKey == VirtualKey.Escape)
            {
                if ((sender.GetKeyState(VirtualKey.LeftShift) & CoreVirtualKeyStates.Down) != CoreVirtualKeyStates.Down)
                {
                    Esc?.Invoke(null, EventArgs.Empty);
                }
                return;
            }

            // Check if the events can be raised
            if (modifier == VirtualKey.Control && args.VirtualKey == VirtualKey.S)
            {
                CtrlS?.Invoke(null, EventArgs.Empty);
            }
        }

        #region Public events

        /// <summary>
        /// Raised whenever the user presses the Esc key
        /// </summary>
        public static event EventHandler Esc;

        /// <summary>
        /// Raised whenever the user presses Ctrl + S
        /// </summary>
        public static event EventHandler CtrlS;

        #endregion
    }
}
