using System;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.Legacy.UWP.DataModels.Misc
{
    /// <summary>
    /// A simple class that wraps the info on a given release version of the app
    /// </summary>
    public sealed class ChangelogReleaseInfo
    {
        /// <summary>
        /// Gets the version for the current app release
        /// </summary>
        public Version BuildVersion { get; }

        /// <summary>
        /// Gets the release date for the current app version
        /// </summary>
        public DateTime ReleaseDate { get; }

        /// <summary>
        /// Creates a new instance for the given app version
        /// </summary>
        /// <param name="version">The app version to wrap</param>
        /// <param name="release">The release date for the given app version</param>
        public ChangelogReleaseInfo([NotNull] Version version, DateTime release)
        {
            BuildVersion = version;
            ReleaseDate = release;
        }
    }
}
