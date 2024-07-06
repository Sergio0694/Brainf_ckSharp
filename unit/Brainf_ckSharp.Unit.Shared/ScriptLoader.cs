using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Unit.Shared.Models;

namespace Brainf_ckSharp.Unit.Shared;

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
    public static Script LoadScriptByName(string name)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string filename = assembly.GetManifestResourceNames().First(path => path.EndsWith($"{name}.txt"));

        using Stream stream = assembly.GetManifestResourceStream(filename)!;
        using StreamReader reader = new(stream);

        string text = reader.ReadToEnd();
        string[] parts = text.Split('|').Select(p => p.TrimStart().Replace("\r", string.Empty)).ToArray();

        return new Script
        {
            Stdin = parts[0],
            Stdout = parts[1],
            MemorySize = int.Parse(parts[2]),
            DataType = (DataType)Enum.Parse(typeof(DataType), parts[3]),
            ExecutionOptions = (ExecutionOptions)Enum.Parse(typeof(ExecutionOptions), parts[4]),
            Source = parts[5]
        };
    }
}
