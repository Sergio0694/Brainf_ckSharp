using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Helpers.WindowsAPIs
{
    /// <summary>
    /// A simple <see langword="class"/> with some methods to perform clipboard operations
    /// </summary>
    public static class ClipboardHelper
    {
        /// <summary>
        /// Tries to copy the input text into the clipboard
        /// </summary>
        /// <param name="text">The text to copy</param>
        public static bool TryCopyToClipboard([NotNull] this string text)
        {
            try
            {
                DataPackage package = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
                package.SetText(text);
                Clipboard.SetContent(package);
                return true;
            }
            catch
            {
                // Whops!
                return false;
            }
        }

        /// <summary>
        /// Tries to extract either a plain text or an RTF text from the clipboard
        /// </summary>
        public static async Task<(string Text, string Format)> TryGetTextAsync()
        {
            DataPackageView view = Clipboard.GetContent();
            (string Text, string Format) result;
            if (view.Contains(StandardDataFormats.Text)) result = (await view.GetTextAsync(), StandardDataFormats.Text);
            else if (view.Contains(StandardDataFormats.Rtf)) result = (await view.GetRtfAsync(), StandardDataFormats.Rtf);
            else result = default;
            view.ReportOperationCompleted(DataPackageOperation.Copy);
            return result;
        }
    }
}
