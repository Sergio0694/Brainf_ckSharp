using System;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.DataModels.SQLite
{
    /// <summary>
    /// A class that contains the information on a sample source code
    /// </summary>
    public sealed class SampleCodeRecord
    {
        /// <summary>
        /// Gets the name of the sample source code file
        /// </summary>
        [NotNull]
        public String Filename { get; }

        /// <summary>
        /// Gets the in-app name to use for the sample source code
        /// </summary>
        [NotNull]
        public String FriendlyName { get; }
        
        /// <summary>
        /// Gets a <see cref="Guid"/> to identify for the sample source code
        /// </summary>
        public Guid Uid { get; }

        /// <summary>
        /// Creates a new instance with the given parameters
        /// </summary>
        /// <param name="filename">The sample code filename</param>
        /// <param name="friendlyName">The name to use to display the sample code in the app</param>
        /// <param name="uid">A <see cref="Guid"/> for the sample source code</param>
        public SampleCodeRecord([NotNull] String filename, [NotNull] String friendlyName, Guid uid)
        {
            Filename = filename;
            FriendlyName = friendlyName;
            Uid = uid;
        }
    }
}
