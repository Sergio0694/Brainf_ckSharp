using System;
using Windows.UI.Xaml;

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    public sealed partial class Brainf_ckIde
    {
        /// <summary>
        /// Gets or sets the reference text currently in use
        /// </summary>
        public string ReferenceText
        {
            get => (string)GetValue(ReferenceTextProperty);
            set => SetValue(ReferenceTextProperty, value.WithCarriageReturnLineEndings());
        }

        /// <summary>
        /// Gets the dependency property for <see cref="ReferenceText"/>.
        /// </summary>
        public static readonly DependencyProperty ReferenceTextProperty =
            DependencyProperty.Register(
                nameof(ReferenceText),
                typeof(string),
                typeof(Brainf_ckIde),
                new PropertyMetadata("\r", OnReferenceTextPropertyChanged));

        /// <summary>
        /// Updates the UI when <see cref="ReferenceText"/> changes
        /// </summary>
        /// <param name="d">The source <see cref="Brainf_ckIde"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance with the new <see cref="ReferenceText"/> value</param>
        private static void OnReferenceTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Brainf_ckIde @this = (Brainf_ckIde)d;
            string newText = @this.CodeEditBox.PlainText;

            @this.UpdateDiffInfo(newText);
        }
    }
}
