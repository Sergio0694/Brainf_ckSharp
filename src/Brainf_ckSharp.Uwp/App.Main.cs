using System;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml;
using Brainf_ckSharp.Services;

#nullable enable

namespace Brainf_ckSharp.Uwp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default <see cref="Application"/> class
    /// </summary>
    public sealed partial class App : Application, IFilesManagerService
    {
        /// <summary>
        /// Creates a new <see cref="App"/> instance
        /// </summary>
        /// <param name="id">The unique id associated with this app instance</param>
        public App(string id)
        {
            Id = id;

            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the unique id associated with this app instance
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public new static App Current => (App)Application.Current;

        /// <summary>
        /// The entry point for the whole application
        /// </summary>
        public static void Main(string[] args)
        {
            // Check if this invocation already has a target instance (from the shell)
            if (AppInstance.RecommendedInstance is null)
            {
                string key = Guid.NewGuid().ToString("N");

                // If the activation requests a file, check if it is already in use
                if (AppInstance.GetActivatedEventArgs() is FileActivatedEventArgs fileArgs &&
                    fileArgs.Files.FirstOrDefault() is StorageFile storageFile &&
                    TryGetInstanceForFilePath(storageFile.Path, out AppInstance? instance))
                {
                    instance!.RedirectActivationTo();
                }
                else
                {
                    // This will be a new app instance, which needs to be registered
                    _ = AppInstance.FindOrRegisterInstanceForKey(key);

                    // Start the app as usual, passing the current key
                    Start(_ => new App(key));
                }
            }
            else AppInstance.RecommendedInstance.RedirectActivationTo();
        }

        /// <summary>
        /// Tries to get the <see cref="AppInstance"/> currently working on a file with the specified path
        /// </summary>
        /// <param name="path">The path of the target file</param>
        /// <param name="instance">The resulting <see cref="AppInstance"/>, if found</param>
        /// <returns>Whether or not a target <see cref="AppInstance"/> has been found</returns>
        private static bool TryGetInstanceForFilePath(string path, out AppInstance? instance)
        {
            string temporaryPath = ApplicationData.Current.TemporaryFolder.Path;

            foreach (AppInstance entry in AppInstance.GetInstances())
            {
                // Read the file with the same key as the current instance
                string keyPath = Path.Combine(temporaryPath, entry.Key);

                if (!File.Exists(keyPath)) continue;

                string currentPath = File.ReadAllText(keyPath);

                // If an active instance is find, just switch to it
                if (path.Equals(currentPath))
                {
                    instance = entry;

                    return true;
                }
            }

            instance = null;

            return false;
        }

        /// <inheritdoc/>
        public void RegisterFile(IFile? file)
        {
            string
                temporaryPath = ApplicationData.Current.TemporaryFolder.Path,
                keyPath = Path.Combine(temporaryPath, Id);

            if (file is null) File.Delete(keyPath);
            else File.WriteAllText(keyPath, file.Path);
        }

        /// <inheritdoc/>
        public bool IsRegistered(IFile file)
        {
            return TryGetInstanceForFilePath(file.Path, out _);
        }

        /// <inheritdoc/>
        public bool TrySwitchTo(IFile file)
        {
            throw new NotImplementedException();
        }
    }
}
