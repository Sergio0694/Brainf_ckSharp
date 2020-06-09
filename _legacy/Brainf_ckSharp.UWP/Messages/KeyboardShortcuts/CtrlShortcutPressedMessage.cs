using Windows.System;

namespace Brainf_ck_sharp.Legacy.UWP.Messages.KeyboardShortcuts
{
    /// <summary>
    /// A message that signals whenever the user uses a keyboard shortcut that involves the <see cref="VirtualKeyModifiers.Control"/> modifier
    /// </summary>
    public sealed class CtrlShortcutPressedMessage
    {
        /// <summary>
        /// Gets the main <see cref="VirtualKey"/> for the shortcut
        /// </summary>
        public VirtualKey Key { get; }

        /// <summary>
        /// Gets the modifiers combination for the current shortcut
        /// </summary>
        public VirtualKeyModifiers Modifiers { get; }

        /// <summary>
        /// Creates a new message for a given keyboard shortcut
        /// </summary>
        /// <param name="key">The main pressed key</param>
        /// <param name="modifiers">The modifiers combination</param>
        public CtrlShortcutPressedMessage(VirtualKey key, VirtualKeyModifiers modifiers)
        {
            Key = key;
            Modifiers = modifiers;
        }
    }
}
