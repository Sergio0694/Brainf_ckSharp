using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Brainf_ckSharp.Uwp.Controls.Ide.Helpers;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    public sealed partial class Brainf_ckEditBox
    {
        /// <summary>
        /// Enables the monitoring of clipboard contents
        /// </summary>
        /// <param name="isEnabled">Whether or not to monitor the clipboard contents</param>
        private void EnableClipboardMonitoring(bool isEnabled)
        {
            if (isEnabled) Clipboard.ContentChanged += Clipboard_ContentChanged;
            else Clipboard.ContentChanged -= Clipboard_ContentChanged;
        }

        /// <summary>
        /// Updates the <see cref="IsTextInClipboard"/> when the clipboard content changes
        /// </summary>
        /// <param name="sender">Always <see langword="null"/>, as the event is not an instance event</param>
        /// <param name="e">The <see cref="object"/> arguments for the event, ignored</param>
        private void Clipboard_ContentChanged(object sender, object e)
        {
            IsTextInClipboard = ClipboardHelper.IsTextAvailable();
        }

        /// <summary>
        /// Gets whether or not some text is selected
        /// </summary>
        public bool IsTextSelected
        {
            get => (bool)GetValue(IsTextSelectedProperty);
            private set => SetValue(IsTextSelectedProperty, value);
        }

        /// <summary>
        /// Gets whether or not there is some text in the clipboard that can be pasted
        /// </summary>
        public bool IsTextInClipboard
        {
            get => (bool)GetValue(IsTextInClipboardProperty);
            private set => SetValue(IsTextInClipboardProperty, value);
        }

        /// <summary>
        /// Gets the dependency property for <see cref="IsTextSelected"/>.
        /// </summary>
        public static readonly DependencyProperty IsTextSelectedProperty =
            DependencyProperty.Register(
                nameof(IsTextSelected),
                typeof(bool),
                typeof(Brainf_ckEditBox),
                new PropertyMetadata(false));

        /// <summary>
        /// Gets the dependency property for <see cref="IsTextInClipboard"/>.
        /// </summary>
        public static readonly DependencyProperty IsTextInClipboardProperty =
            DependencyProperty.Register(
                nameof(IsTextInClipboard),
                typeof(bool),
                typeof(Brainf_ckEditBox),
                new PropertyMetadata(false));
    }
}
