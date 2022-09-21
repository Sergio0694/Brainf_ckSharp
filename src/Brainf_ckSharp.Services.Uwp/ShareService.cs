using System.Runtime.InteropServices;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Uwp.Services.Files;
using CommunityToolkit.Diagnostics;

#nullable enable

namespace Brainf_ckSharp.Uwp.Services.Share;

/// <summary>
/// A <see langword="class"/> that handles data sharing from the app
/// </summary>
public sealed class ShareService : IShareService
{
    /// <summary>
    /// The data that is scheduled to be shared
    /// </summary>
    private ShareInfoBase? _Info;

    /// <summary>
    /// Gets the current transfer manager to use
    /// </summary>
    private DataTransferManager? _DataTransferManager;

    private void InitializeDataTransferManager()
    {
        if (!(_DataTransferManager is null)) return;

        _DataTransferManager = DataTransferManager.GetForCurrentView();
        _DataTransferManager.DataRequested += DataTransferManager_DataRequested;
    }

    /// <summary>
    /// Handles a share data request
    /// </summary>
    /// <param name="sender">The source <see cref="Windows.ApplicationModel.DataTransfer.DataTransferManager"/> instance</param>
    /// <param name="args">The info of the current data request</param>
    private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
    {
        // Make sure there is some content to share
        if (_Info == null) ThrowHelper.ThrowInvalidOperationException("There isn't a valid content to share");

        DataRequest request = args.Request;

        // Set the data to share
        request.Data.Properties.Title = _Info.Title;
        request.Data.Properties.Description = "Shared from Brainf*ck#";

        switch (_Info)
        {
            case FileShareInfo fileShare:
                request.Data.SetStorageItems(new [] { fileShare.File }, true);
                break;
            default:
                ThrowHelper.ThrowArgumentException(nameof(_Info), "Invalid share info type");
                break;
        }

        _Info = null;
    }

    /// <inheritdoc/>
    public void Share(string title, IFile file)
    {
        // Resolve the platform specific file instance.
        // If any other type is passed, this will just throw.
        StorageFile storageFile = ((File)file).StorageFile;

        InitializeDataTransferManager();

        _Info = new FileShareInfo(title, storageFile);

        try
        {
            DataTransferManager.ShowShareUI();
        }
        catch (COMException)
        {
            // Busy application: there is another pending share request
            _Info = null;
        }

    }

    /// <summary>
    /// A <see langword="class"/> that stores info on data to share
    /// </summary>
    private abstract class ShareInfoBase
    {
        /// <summary>
        /// Creates a new <see cref="ShareInfoBase"/> instance with the specified parameters
        /// </summary>
        /// <param name="title">The title of the data to share</param>
        protected ShareInfoBase(string title) => Title = title;

        /// <summary>
        /// Gets the title of the share operation
        /// </summary>
        public string Title { get; }
    }

    /// <summary>
    /// A <see langword="class"/> that stores info on a file to share
    /// </summary>
    private sealed class FileShareInfo : ShareInfoBase
    {
        /// <summary>
        /// Creates a new <see cref="ShareInfoBase"/> instance with the specified parameters
        /// </summary>
        /// <param name="title">The title of the data to share</param>
        /// <param name="file">The file to share</param>
        public FileShareInfo(string title, StorageFile file) : base(title)
        {
            File = file;
        }

        /// <summary>
        /// Gets the current file to share
        /// </summary>
        public StorageFile File { get; }
    }
}
