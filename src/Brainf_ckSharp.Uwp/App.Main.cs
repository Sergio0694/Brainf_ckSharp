using System;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.System;
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
            Services = ConfigureServices();

            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public new static App Current => (App)Application.Current;

        /// <summary>
        /// Gets the unique id associated with this app instance
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// The entry point for the whole application
        /// </summary>
        public static void Main(string[] args)
        {
            // Check if this invocation already has a target instance (from the shell)
            if (AppInstance.RecommendedInstance is null)
            {
                string key = Guid.NewGuid().ToString("N");

                IActivatedEventArgs activatedEventArgs = AppInstance.GetActivatedEventArgs();

                // If the activation requests a file, check if it is already in use
                if (activatedEventArgs is FileActivatedEventArgs fileArgs &&
                    fileArgs.Files.FirstOrDefault() is StorageFile storageFile &&
                    TryGetInstanceForFilePath(storageFile.Path, out AppInstance? fileInstance))
                {
                    fileInstance!.RedirectActivationTo();

                    return;
                }
                
                // If the activation uses a protocol, handle the possible cases.
                // Currently there are two possible protocols being used:
                //   - [/switch?key=...] is used to request a direct switch to an existing instance.
                //     When this protocol is used, the target instance is always assumed to exist.
                //     In this case, we just retrieve the target instance by key and redirect.
                //   - [/file?path=...] is used to request a file activation from a timeline item.
                //     In this case we don't have a file instance, but only the target path.
                //     The activation is similar as the cases above: we first try to look for an
                //     existing instance working on that file, and in that case we just switch there.
                //     Otherwise we proceed with the usual registration, and the activation override
                //     will take care of trying to retrieve the target file, and open it if possible.
                if (activatedEventArgs is ProtocolActivatedEventArgs protocolArgs)
                {
                    // Direct switch requested to a target instance
                    if (protocolArgs.Uri.LocalPath.Equals("/switch"))
                    {
                        string targetKey = protocolArgs.Uri.Query.Substring("?key=".Length);

                        AppInstance.FindOrRegisterInstanceForKey(targetKey).RedirectActivationTo();

                        return;
                    }
                    
                    // File request by path from a timeline item
                    if (protocolArgs.Uri.LocalPath.Equals("/file"))
                    {
                        string
                            escapedPath = protocolArgs.Uri.Query.Substring("?path=".Length),
                            unescapedPath = Uri.UnescapeDataString(escapedPath);

                        // Activate the target instance as above, if one exists
                        if (TryGetInstanceForFilePath(unescapedPath, out AppInstance? pathInstance))
                        {
                            pathInstance!.RedirectActivationTo();

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
                // Get the filename associated to the current instance
                string keyPath = Path.Combine(temporaryPath, entry.Key);

                if (!File.Exists(keyPath)) continue;

                string currentPath = File.ReadAllText(keyPath);

                // If the path matches, track the target instance
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
            if (!TryGetInstanceForFilePath(file.Path, out AppInstance? instance)) return false;

            Uri uri = new($"brainf-ck:///switch?key={instance!.Key}");

            _ = Launcher.LaunchUriAsync(uri);

            return true;
        }
    }
}
