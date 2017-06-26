using System;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.DataModels.Misc
{
    public sealed class ChangelogReleaseInfo
    {
        public Version BuildVersion { get; }

        public DateTime ReleaseDate { get; }

        public ChangelogReleaseInfo([NotNull] Version version, DateTime release)
        {
            BuildVersion = version;
            ReleaseDate = release;
        }
    }
}
