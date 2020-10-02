using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Brainf_ckSharp.Git.Unit.Helpers
{
    /// <summary>
    /// A <see langword="class"/> with helper methods to load resource files
    /// </summary>
    internal sealed class ResourceLoader
    {
        /// <summary>
        /// Loads a pair of test samples with the specified key
        /// </summary>
        /// <param name="key">The key of the test samples to retrieve</param>
        /// <returns>A pair of reference and updated text files with the specified key</returns>
        [Pure]
        public static (string Old, string New) LoadTestSample(string key)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            string
                oldPath = assembly.GetManifestResourceNames().First(name => name.EndsWith($"{key}Old.txt")),
                newPath = assembly.GetManifestResourceNames().First(name => name.EndsWith($"{key}New.txt"));

            using Stream oldStream = assembly.GetManifestResourceStream(oldPath)!;
            using StreamReader oldReader = new StreamReader(oldStream);

            using Stream newStream = assembly.GetManifestResourceStream(newPath)!;
            using StreamReader newReader = new StreamReader(newStream);

            string
                oldText = oldReader.ReadToEnd(),
                newText = newReader.ReadToEnd();

            return (oldText, newText);
        }
    }
}
