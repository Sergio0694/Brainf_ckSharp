using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Text;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.Legacy.UWP.ViewModels.FlyoutsViewModels
{
    public class CustomRichEditBoxContextMenuViewModel : CodeSnippetsBrowserViewModel
    {
        public CustomRichEditBoxContextMenuViewModel([NotNull] ITextDocument document)
        {
            DataPackageView view = Clipboard.GetContent();
            CanPaste = view.Contains(StandardDataFormats.Text) || view.Contains(StandardDataFormats.Rtf);
            CanCopy = document.Selection.Length.Abs() > 0;
        }

        /// <summary>
        /// Gets whether or not some text is selected and can be copied into the clipboard
        /// </summary>
        public bool CanCopy { get; }

        /// <summary>
        /// Gets whether or not the clipboard contains content that can be pasted into the current document
        /// </summary>
        public bool CanPaste { get; }
    }
}
