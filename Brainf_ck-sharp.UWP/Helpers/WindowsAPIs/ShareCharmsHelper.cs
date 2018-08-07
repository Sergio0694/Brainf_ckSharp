using System;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.DataTransfer;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Helpers.WindowsAPIs
{
    /// <summary>
    /// A static class that manages the DataTransferManager instance for the current application
    /// </summary>
    public static class ShareCharmsHelper
    {
        /// <summary>
        /// Gets the current transfer manager to use
        /// </summary>
        private static readonly DataTransferManager _DataTransferManager = DataTransferManager.GetForCurrentView();

        // Monitors whether or not the share handler has already been added
        private static bool _Initialized;

        // The data that's going to be shared
        private static ShareInfo _Info;

        static void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            // Make sure there is some content to share
            if (_Info == null) throw new InvalidOperationException("There isn't a valid content to share");

            // Get the current request and the content
            DataRequest request = args.Request;
            ShareInfo info = _Info;
            _Info = null;

            // Set the data to share
            request.Data.Properties.Title = info.Title;
            request.Data.Properties.Description = LocalizationManager.GetResource("SharedFromOneLocker");
            request.Data.SetText(info.SharedText);
        }

        /// <summary>
        /// Shares a text through the system share UI
        /// </summary>
        /// <param name="title">The title of the text to share</param>
        /// <param name="text">The actual text to share</param>
        public static void ShareText([NotNull] string title, [NotNull] string text)
        {
            // Makes sure the DataTransferManager is initialized
            if (!_Initialized)
            {
                _DataTransferManager.DataRequested += DataTransferManager_DataRequested;
                _Initialized = true;
            }

            // Stores the data to share and invokes the share call
            _Info = new ShareInfo(title, text);
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
        /// A private class that store the data to share
        /// </summary>
        private class ShareInfo
        {
            /// <summary>
            /// Gets the title of the share operation
            /// </summary>
            public string Title { get; }

            /// <summary>
            /// Gets the current text to share
            /// </summary>
            public string SharedText { get; }

            public ShareInfo(string title, string text)
            {
                Title = title;
                SharedText = text;
            }
        }
    }
}
