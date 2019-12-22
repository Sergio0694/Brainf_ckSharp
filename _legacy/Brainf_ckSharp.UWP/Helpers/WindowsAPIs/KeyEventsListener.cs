﻿using System.Linq;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Brainf_ck_sharp.Legacy.UWP.Messages.Actions;
using Brainf_ck_sharp.Legacy.UWP.Messages.KeyboardShortcuts;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.Legacy.UWP.Helpers.WindowsAPIs
{
    /// <summary>
    /// A static class that listens to the global <see cref="CoreDispatcher.AcceleratorKeyActivated"/> event for the current app window
    /// </summary>
    public sealed class KeyEventsListener
    {
        // The singleton instance (needed to subscribe to some non-static events)
        [NotNull]
        public static KeyEventsListener Instance { get; } = new KeyEventsListener();

        private bool _IsEnabled;

        /// <summary>
        /// Gets or sets whether or not the class is currently monitoring the KeyDown event
        /// </summary>
        public bool IsEnabled
        {
            get => _IsEnabled;
            set
            {
                if (IsEnabled != value)
                {
                    if (value)
                    {
                        Window.Current.Dispatcher.AcceleratorKeyActivated += Dispatcher_AcceleratorKeyActivated;
                        Window.Current.Content.CharacterReceived += Content_CharacterReceived;
                    }
                    else
                    {
                        Window.Current.Dispatcher.AcceleratorKeyActivated -= Dispatcher_AcceleratorKeyActivated;
                        Window.Current.Content.CharacterReceived -= Content_CharacterReceived;
                    }
                    _IsEnabled = value;
                }
            }
        }

        // Raises the proper keyboard events
        private void Dispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            // Only handle key down events
            if (args.EventType != CoreAcceleratorKeyEventType.KeyDown && args.EventType != CoreAcceleratorKeyEventType.SystemKeyDown) return;

            // Esc key
            if (args.VirtualKey == VirtualKey.Escape)
            {
                Messenger.Default.Send(new EscKeyPressedMessage());
                return;
            }

            // Delete key
            if (args.VirtualKey == VirtualKey.Back)
            {
                Messenger.Default.Send(new DeleteKeyPressedMessage());
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

        // Signals that a single character has been received
        private void Content_CharacterReceived(UIElement sender, CharacterReceivedRoutedEventArgs args)
        {
            Messenger.Default.Send(new CharacterReceivedMessage(args.Character));
        }
    }
}