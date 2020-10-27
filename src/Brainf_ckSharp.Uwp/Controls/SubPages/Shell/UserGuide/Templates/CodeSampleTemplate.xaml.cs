using System;
using System.IO;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Uwp.Controls.Ide;

namespace Brainf_ckSharp.Uwp.Controls.SubPages.Shell.UserGuide.Templates
{
    public sealed partial class CodeSampleTemplate : UserControl
    {
        public CodeSampleTemplate()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the title for the current code sample
        /// </summary>
        public string Title
        {
            get => TitleBlock.Text;
            set => TitleBlock.Text = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="Uri"/> for the code sample to load
        /// </summary>
        public Uri SampleUri { get; set; }

        /// <summary>
        /// Loads the requested code sample when the IDE control is loaded
        /// </summary>
        /// <param name="sender">The target <see cref="Brainf_ckIde"/> control</param>
        /// <param name="e">The empty <see cref="RoutedEventArgs"/> instance for the event</param>
        private async void Brainf_ckIde_OnLoaded(object sender, RoutedEventArgs e)
        {
            // URIs created from XAML to local files will use the "ms-resource:///Files/" base path,
            // whereas the StorageFile API requires a URI with the "ms-appx:///" schema,
            // with the local path starting immediately from the root of the installation folder.
            Uri appxUri = new($"ms-appx:///{SampleUri.LocalPath.Replace("/Files/", string.Empty)}");

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(appxUri);

            using Stream stream = await file.OpenStreamForReadAsync();
            using StreamReader reader = new(stream);

            string text = await reader.ReadToEndAsync();

            ((Brainf_ckIde)sender).LoadText(text);
        }
    }
}
