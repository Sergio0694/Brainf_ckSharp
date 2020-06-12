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
    public sealed partial class App : Application
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
                    fileArgs.Files.FirstOrDefault() is StorageFile storageFile)
                {
                    string temporaryPath = ApplicationData.Current.TemporaryFolder.Path;

                    foreach (AppInstance instance in AppInstance.GetInstances())
                    {
                        // Read the file with the same key as the current instance
                        string keyPath = Path.Combine(temporaryPath, instance.Key);

                        if (!File.Exists(keyPath)) continue;

                        string currentPath = File.ReadAllText(keyPath);

                        // If an active instance is find, just switch to it
                        if (storageFile.Path.Equals(currentPath))
                        {
                            instance.RedirectActivationTo();

                            return;
                        }
                    }
                }

                // This will be a new app instance, which needs to be registered
                _ = AppInstance.FindOrRegisterInstanceForKey(key);

                // Start the app as usual, passing the current key
                Start(_ => new App(key));
            }
            else AppInstance.RecommendedInstance.RedirectActivationTo();
        }

        /// <summary>
        /// Registers the currently opened file for the current application instance
        /// </summary>
        /// <param name="file">The file currently opened, if present</param>
        public void RegisterFilePath(IFile? file)
        {
            string
                temporaryPath = ApplicationData.Current.TemporaryFolder.Path,
                keyPath = Path.Combine(temporaryPath, Id);

            if (file is null) File.Delete(keyPath);
            else File.WriteAllText(keyPath, file.Path);
        }
    }
}
