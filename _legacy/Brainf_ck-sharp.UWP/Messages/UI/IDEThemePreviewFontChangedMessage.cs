using Brainf_ck_sharp_UWP.Helpers.CodeFormatting;
using Brainf_ck_sharp_UWP.Messages.Abstract;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Messages.UI
{
    /// <summary>
    /// A message that signals whenever the user changes the current font selection
    /// </summary>
    public sealed class IDEThemePreviewFontChangedMessage : ValueChangedMessageBase<InstalledFont>
    {
        /// <inheritdoc cref="ValueChangedMessageBase{T}"/>
        public IDEThemePreviewFontChangedMessage([NotNull] InstalledFont font) : base(font) { }
    }
}
