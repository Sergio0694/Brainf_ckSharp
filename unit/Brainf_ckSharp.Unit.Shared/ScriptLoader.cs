using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

#nullable enable

namespace Brainf_ckSharp.Unit.Shared
{
    /// <summary>
    /// Helper method to load embedded scripts
    /// </summary>
    public static class ScriptLoader
    {
        /// <summary>
        /// Loads an embedded script with the specified name
        /// </summary>
        /// <param name="name">The name of the script to load</param>
        /// <returns>The data for the requested script</returns>
        [Pure]
        public static (string Stdin, string Output, string Source) LoadScriptByName(string name)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string filename = assembly.GetManifestResourceNames().First(path => path.EndsWith($"{name}.txt"));

            using Stream stream = assembly.GetManifestResourceStream(filename)!;
            using StreamReader reader = new StreamReader(stream);

            return (
                Regex.Match(reader.ReadLine()!, @"\[([^]]+)\]").Groups[1].Value,
                Regex.Match(reader.ReadLine()!, @"\[([^]]+)\]").Groups[1].Value,
                reader.ReadToEnd());
        }
    }
}
