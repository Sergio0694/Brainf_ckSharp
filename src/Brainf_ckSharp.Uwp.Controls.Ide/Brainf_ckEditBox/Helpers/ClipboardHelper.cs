using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Uwp.Controls.Ide.Extensions.Windows.UI.Text;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Ide.Helpers;

/// <summary>
/// A helper <see langword="class"/> that retrieves text from the system clipboard
/// </summary>
internal static class ClipboardHelper
{
    /// <summary>
    /// A <see cref="RichEditBox"/> provuder used to parse RTF data from the clipboard
    /// </summary>
    private static readonly RichEditBox EditBox = new();

    /// <summary>
    /// Tries to copy some text to the clipboard
    /// </summary>
    /// <param name="text">The text to copy</param>
    /// <returns>Whether or not the operation was successful</returns>
    public static bool TryCopy(string text)
    {
        try
        {
            DataPackage package = new() { RequestedOperation = DataPackageOperation.Copy };

            package.SetText(text);

            Clipboard.SetContent(package);
            Clipboard.Flush();

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Checks whether or not there is some text available in the clipboard
    /// </summary>
    /// <returns>Whether or not there is text available to copy</returns>
    [Pure]
    public static bool IsTextAvailable()
    {
        try
        {
            DataPackageView view = Clipboard.GetContent();

            return
                view.Contains(StandardDataFormats.Text) ||
                view.Contains(StandardDataFormats.Rtf);
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }

    /// <summary>
    /// Tries to get plain text content from the clipboard
    /// </summary>
    /// <returns>The plain text content from the clipboard, or <see langword="null"/></returns>
    [Pure]
    public static async Task<string?> TryGetTextAsync()
    {
        DataPackageView view;

        try
        {
            view = Clipboard.GetContent();
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }

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
