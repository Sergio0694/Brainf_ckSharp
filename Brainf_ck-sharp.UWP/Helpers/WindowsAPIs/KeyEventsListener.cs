using System.Linq;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Brainf_ck_sharp_UWP.Messages.KeyboardShortcuts;
using GalaSoft.MvvmLight.Messaging;

namespace Brainf_ck_sharp_UWP.Helpers.WindowsAPIs
{
    /// <summary>
    /// A static class that listens to the global <see cref="CoreDispatcher.AcceleratorKeyActivated"/> event for the current app window
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
                    if (value) Window.Current.Dispatcher.AcceleratorKeyActivated += Dispatcher_AcceleratorKeyActivated;
                    else Window.Current.Dispatcher.AcceleratorKeyActivated -= Dispatcher_AcceleratorKeyActivated;
                    _IsEnabled = value;
                }
            }
        }

        // Raises the proper keyboard events
        private static void Dispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            // Only handle key down events
            if (args.EventType != CoreAcceleratorKeyEventType.KeyDown && args.EventType != CoreAcceleratorKeyEventType.SystemKeyDown) return;

            // Raise the Esc event if needed
            if (args.VirtualKey == VirtualKey.Escape)
            {
                Messenger.Default.Send(new EscKeyPressedMessage());
                return;
            }

            // Propagate the event if needed
            VirtualKeyModifiers modifiers = new (VirtualKey Key, VirtualKeyModifiers Modifier)[]
            {
                (VirtualKey.Shift, VirtualKeyModifiers.Shift),
                (VirtualKey.Control, VirtualKeyModifiers.Control),
                (VirtualKey.Menu, VirtualKeyModifiers.Menu)
            }.Where(pair => (Window.Current.CoreWindow.GetKeyState(pair.Key) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down).Aggregate(VirtualKeyModifiers.None, (m, k) => m | k.Modifier);
            if (modifiers.HasFlag(VirtualKeyModifiers.Control)) Messenger.Default.Send(new CtrlShortcutPressedMessage(args.VirtualKey, modifiers));
        }
    }
}
