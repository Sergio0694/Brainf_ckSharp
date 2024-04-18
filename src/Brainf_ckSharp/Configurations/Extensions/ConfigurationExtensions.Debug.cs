using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Brainf_ckSharp.Configurations;

namespace Brainf_ckSharp;

/// <summary>
/// Extensions for building DEBUG configurations
/// </summary>
public static partial class DebugConfigurationExtensions
{
    /// <summary>
    /// Sets the source code to parse and execute for a given configuration
    /// </summary>
    /// <param name="configuration">The input <see cref="DebugConfiguration"/> instance</param>
    /// <param name="breakpoints">The sequence of indices for the breakpoints to apply to the script</param>
    /// <returns>The input <see cref="DebugConfiguration"/> instance</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly DebugConfiguration WithBreakpoints(in this DebugConfiguration configuration, ReadOnlyMemory<int> breakpoints)
    {
        Unsafe.AsRef(in configuration.Breakpoints) = breakpoints;

        return ref configuration;
    }

    /// <summary>
    /// Sets the debug token for a given configuration
    /// </summary>
    /// <param name="configuration">The input <see cref="DebugConfiguration"/> instance</param>
    /// <param name="debugToken">A <see cref="CancellationToken"/> that is used to ignore/respect existing breakpoints</param>
    /// <returns>The input <see cref="DebugConfiguration"/> instance</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly DebugConfiguration WithDebugToken(in this DebugConfiguration configuration, CancellationToken debugToken)
    {
        Unsafe.AsRef(in configuration.DebugToken) = debugToken;

        return ref configuration;
    }
}
