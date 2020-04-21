using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Brainf_ckSharp.Services;

#nullable enable

namespace Brainf_ckSharp.Uwp.Services.Clipboard
{
    /// <summary>
    /// A <see langword="class"/> that interacts with the system clipboard
    /// </summary>
    public sealed class ClipboardService : IClipboardService
    {
        /// <inheritdoc/>
        public bool TryCopy(string text, bool flush = true)
        {
            try
            {
                DataPackage package = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
                package.SetText(text);
                Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(package);

                if (flush) Windows.ApplicationModel.DataTransfer.Clipboard.Flush();

                return true;
            }
            catch
            {
                // Whops!
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<string?> TryGetTextAsync()
        {
            try
            {
                DataPackageView view = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();

                // Try to extract the requested content
                string? item;
                if (view.Contains(StandardDataFormats.Text))
                {
                    item = await view.GetTextAsync();
                    view.ReportOperationCompleted(DataPackageOperation.Copy);
                }
                else item = null;

                return item;
            }
            catch
            {
                // Y u do dis?
                return null;
            }
        }
    }
}
