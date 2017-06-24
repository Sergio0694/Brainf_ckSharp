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
                if ((sender.GetKeyState(VirtualKey.LeftShift) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down)
                {
                    ShiftEsc?.Invoke(null, EventArgs.Empty);
                }
                else Esc?.Invoke(null, EventArgs.Empty);
                return;
            }

            // Check if the events can be raised
            if (modifier == VirtualKey.Control)
            {
                if (args.VirtualKey == VirtualKey.F) CtrlF?.Invoke(null, EventArgs.Empty);
                else if (args.VirtualKey == VirtualKey.H) CtrlH?.Invoke(null, EventArgs.Empty);
                else if (args.VirtualKey == VirtualKey.S) CtrlS?.Invoke(null, EventArgs.Empty);
                else if (args.VirtualKey == VirtualKey.B) CtrlB?.Invoke(null, EventArgs.Empty);
                else if (args.VirtualKey == VirtualKey.Q) CtrlQ?.Invoke(null, EventArgs.Empty);
                else if (args.VirtualKey == VirtualKey.T) CtrlT?.Invoke(null, EventArgs.Empty);
                else if (args.VirtualKey == VirtualKey.P)
                {
                    if ((sender.GetKeyState(VirtualKey.Menu) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down) CtrlAltP?.Invoke(null, EventArgs.Empty);
                    else CtrlP?.Invoke(null, EventArgs.Empty);
                }
            }
        }

        #region Public events

        /// <summary>
        /// Raised whenever the user presses the Esc key
        /// </summary>
        public static event EventHandler Esc;

        /// <summary>
        /// Raised whenever the user presses Ctrl + F
        /// </summary>
        public static event EventHandler CtrlF;

        /// <summary>
        /// Raised whenever the user presses Ctrl + S
        /// </summary>
        public static event EventHandler CtrlS;

        /// <summary>
        /// Raised whenever the user presses Ctrl + H
        /// </summary>
        public static event EventHandler CtrlH;

        /// <summary>
        /// Raised whenever the user presses Ctrl + B
        /// </summary>
        public static event EventHandler CtrlB;

        /// <summary>
        /// Raised whenever the user presses Ctrl + Q
        /// </summary>
        public static event EventHandler CtrlQ;

        /// <summary>
        /// Raised whenever the user presses Ctrl + T
        /// </summary>
        public static event EventHandler CtrlT;

        /// <summary>
        /// Raised whenever the user presses Ctrl + P
        /// </summary>
        public static event EventHandler CtrlP;

        /// <summary>
        /// Raised whenever the user presses Ctrl + Alt + Q
        /// </summary>
        public static event EventHandler CtrlAltP;

        /// <summary>
        /// Raised whenever the user presses Shift + Esc
        /// </summary>
        public static event EventHandler ShiftEsc;

        #endregion
    }
}
