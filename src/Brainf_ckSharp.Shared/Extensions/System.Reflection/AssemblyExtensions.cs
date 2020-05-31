using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Brainf_ckSharp.Shared.Extensions.System.Reflection
{
    /// <summary>
    /// An extension <see langword="class"/> for the <see cref="Assembly"/> type
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Returns the contents of a specified manifest file, as a <see cref="string"/>
        /// </summary>
        /// <param name="assembly">The target <see cref="Assembly"/> instance</param>
        /// <param name="filename">The name of the file to read</param>
        /// <returns>The text contents of the specified manifest file</returns>
        [Pure]
        public static string ReadTextFromEmbeddedResourceFile(this Assembly assembly, string filename)
        {
            string manifestFilename = assembly.GetManifestResourceNames().First(name => name.EndsWith(filename));

            using Stream stream = assembly.GetManifestResourceStream(manifestFilename);
            using StreamReader reader = new StreamReader(stream);

            return reader.ReadToEnd().Trim();
        }
    }
}
