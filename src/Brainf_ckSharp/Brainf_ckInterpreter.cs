using System.Runtime.CompilerServices;
using Brainf_ckSharp.Configurations;

namespace Brainf_ckSharp;

/// <summary>
/// A <see langword="class"/> responsible for interpreting and debugging Brainf*ck/PBrain scripts
/// </summary>
public static partial class Brainf_ckInterpreter
{
    /// <summary>
    /// Creates a new <see cref="DebugConfiguration"/> instance to prepare a script execution in DEBUG mode
    /// </summary>
    /// <returns>A <see cref="DebugConfiguration"/> instance to prepare a script execution</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DebugConfiguration CreateDebugConfiguration()
    {
        return default;
    }

    /// <summary>
    /// Creates a new <see cref="ReleaseConfiguration"/> instance to prepare a script execution in RELEASE mode
    /// </summary>
    /// <returns>A <see cref="ReleaseConfiguration"/> instance to prepare a script execution</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReleaseConfiguration CreateReleaseConfiguration()
    {
        return default;
    }
}
