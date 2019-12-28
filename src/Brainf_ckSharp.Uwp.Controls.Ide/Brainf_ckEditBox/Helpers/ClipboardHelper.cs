using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Ide.Helpers
{
    /// <summary>
    /// A helper <see langword="class"/> that retrieves text from the system clipboard
    /// </summary>
    internal static class ClipboardHelper
    {
        /// <summary>
        /// A <see cref="RichEditBox"/> provuder used to parse RTF data from the clipboard
        /// </summary>
        private static readonly RichEditBox EditBox = new RichEditBox();

        /// <summary>
        /// Tries to get plain text content from the clipboard
        /// </summary>
        /// <returns>The plain text content from the clipboard, or <see langword="null"/></returns>
        [Pure]
        public static async Task<string?> TryGetTextAsync()
        {
            DataPackageView view = Clipboard.GetContent();

            // If the content is plain text, return it directly
            if (view.Contains(StandardDataFormats.Text))
            {
                string text = await view.GetTextAsync();
                view.ReportOperationCompleted(DataPackageOperation.Copy);

                return text;
            }

            // Handle RTF text explicitly
            if (view.Contains(StandardDataFormats.Rtf))
            {
                string rtf = await view.GetRtfAsync();
                view.ReportOperationCompleted(DataPackageOperation.Copy);

                try
                {
                    // Set the RTF text and extract it as plain text
                    EditBox.Document.SetText(TextSetOptions.FormatRtf, rtf);

                    return EditBox.Document.GetText();
                }
                catch
                {
                    // Something went wrong, abort
                    return null;
                }
                finally
                {
                    // Reset the text to possibly reclaim memory
                    EditBox.Document.SetText(TextSetOptions.None, string.Empty);
                }
            }

            return null;
        }
    }
}
