using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using Brainf_ckSharp.Configurations;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Memory.Interfaces;

#pragma warning disable CS0282

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
    /// <param name="source">The source code to parse and execute</param>
    /// <returns>The input <see cref="DebugConfiguration"/> instance</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly DebugConfiguration WithSource(in this DebugConfiguration configuration, string source)
    {
        Unsafe.AsRef(configuration.Source) = source.AsMemory();
        
        return ref configuration;
    }

    /// <summary>
    /// Sets the source code to parse and execute for a given configuration
    /// </summary>
    /// <param name="configuration">The input <see cref="DebugConfiguration"/> instance</param>
    /// <param name="source">The source code to parse and execute</param>
    /// <returns>The input <see cref="DebugConfiguration"/> instance</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly DebugConfiguration WithSource(in this DebugConfiguration configuration, ReadOnlyMemory<char> source)
    {
        Unsafe.AsRef(configuration.Source) = source;
        
        return ref configuration;
    }

    /// <summary>
    /// Sets the stdin buffer to read for a given configuration
    /// </summary>
    /// <param name="configuration">The input <see cref="DebugConfiguration"/> instance</param>
    /// <param name="stdin">The input buffer to read data from</param>
    /// <returns>The input <see cref="DebugConfiguration"/> instance</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly DebugConfiguration WithStdin(in this DebugConfiguration configuration, string stdin)
    {
        Unsafe.AsRef(configuration.Stdin) = stdin.AsMemory();
        
        return ref configuration;
    }

    /// <summary>
    /// Sets the stdin buffer to read for a given configuration
    /// </summary>
    /// <param name="configuration">The input <see cref="DebugConfiguration"/> instance</param>
    /// <param name="stdin">The input buffer to read data from</param>
    /// <returns>The input <see cref="DebugConfiguration"/> instance</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly DebugConfiguration WithStdin(in this DebugConfiguration configuration, ReadOnlyMemory<char> stdin)
    {
        Unsafe.AsRef(configuration.Stdin) = stdin;
        
        return ref configuration;
    }

    /// <summary>
    /// Sets the initial machine state for a given configuration
    /// </summary>
    /// <param name="configuration">The input <see cref="DebugConfiguration"/> instance</param>
    /// <param name="initialState">The initial state machine to use to start running the script from</param>
    /// <returns>The input <see cref="DebugConfiguration"/> instance</returns>
    /// <remarks>This property will override the values of <see cref="DebugConfiguration.MemorySize"/> and <see cref="DebugConfiguration.OverflowMode"/></remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly DebugConfiguration WithInitialState(in this DebugConfiguration configuration, IReadOnlyMachineState initialState)
    {
        Unsafe.AsRef(configuration.InitialState) = initialState;
        
        return ref configuration;
    }

    /// <summary>
    /// Sets the memory size for a given configuration
    /// </summary>
    /// <param name="configuration">The input <see cref="DebugConfiguration"/> instance</param>
    /// <param name="memorySize">The size of the state machine to create to run the script</param>
    /// <returns>The input <see cref="DebugConfiguration"/> instance</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly DebugConfiguration WithMemorySize(in this DebugConfiguration configuration, int memorySize)
    {
        Unsafe.AsRef(configuration.MemorySize) = memorySize;
        
        return ref configuration;
    }

    /// <summary>
    /// Sets the overflow mode for a given configuration
    /// </summary>
    /// <param name="configuration">The input <see cref="DebugConfiguration"/> instance</param>
    /// <param name="overflowMode">The overflow mode to use in the state machine used to run the script</param>
    /// <returns>The input <see cref="DebugConfiguration"/> instance</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly DebugConfiguration WithOverflowMode(in this DebugConfiguration configuration, OverflowMode overflowMode)
    {
        Unsafe.AsRef(configuration.OverflowMode) = overflowMode;
        
        return ref configuration;
    }

    /// <summary>
    /// Sets the execution token for a given configuration
    /// </summary>
    /// <param name="configuration">The input <see cref="DebugConfiguration"/> instance</param>
    /// <param name="executionToken">A <see cref="CancellationToken"/> that can be used to halt the execution of long running scripts</param>
    /// <returns>The input <see cref="DebugConfiguration"/> instance</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly DebugConfiguration WithExecutionToken(in this DebugConfiguration configuration, CancellationToken executionToken)
    {
        Unsafe.AsRef(configuration.ExecutionToken) = executionToken;
        
        return ref configuration;
    }
}

/// <summary>
/// Extensions for building RELEASE configurations
/// </summary>
public static class ReleaseConfigurationExtensions
{
    /// <summary>
    /// Sets the source code to parse and execute for a given configuration
    /// </summary>
    /// <param name="configuration">The input <see cref="ReleaseConfiguration"/> instance</param>
    /// <param name="source">The source code to parse and execute</param>
    /// <returns>The input <see cref="ReleaseConfiguration"/> instance</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly ReleaseConfiguration WithSource(in this ReleaseConfiguration configuration, string source)
    {
        Unsafe.AsRef(configuration.Source) = source.AsMemory();
        
        return ref configuration;
    }

    /// <summary>
    /// Sets the source code to parse and execute for a given configuration
    /// </summary>
    /// <param name="configuration">The input <see cref="ReleaseConfiguration"/> instance</param>
    /// <param name="source">The source code to parse and execute</param>
    /// <returns>The input <see cref="ReleaseConfiguration"/> instance</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly ReleaseConfiguration WithSource(in this ReleaseConfiguration configuration, ReadOnlyMemory<char> source)
    {
        Unsafe.AsRef(configuration.Source) = source;
        
        return ref configuration;
    }

    /// <summary>
    /// Sets the stdin buffer to read for a given configuration
    /// </summary>
    /// <param name="configuration">The input <see cref="ReleaseConfiguration"/> instance</param>
    /// <param name="stdin">The input buffer to read data from</param>
    /// <returns>The input <see cref="ReleaseConfiguration"/> instance</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly ReleaseConfiguration WithStdin(in this ReleaseConfiguration configuration, string stdin)
    {
        Unsafe.AsRef(configuration.Stdin) = stdin.AsMemory();
        
        return ref configuration;
    }

    /// <summary>
    /// Sets the stdin buffer to read for a given configuration
    /// </summary>
    /// <param name="configuration">The input <see cref="ReleaseConfiguration"/> instance</param>
    /// <param name="stdin">The input buffer to read data from</param>
    /// <returns>The input <see cref="ReleaseConfiguration"/> instance</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly ReleaseConfiguration WithStdin(in this ReleaseConfiguration configuration, ReadOnlyMemory<char> stdin)
    {
        Unsafe.AsRef(configuration.Stdin) = stdin;
        
        return ref configuration;
    }

    /// <summary>
    /// Sets the initial machine state for a given configuration
    /// </summary>
    /// <param name="configuration">The input <see cref="ReleaseConfiguration"/> instance</param>
    /// <param name="initialState">The initial state machine to use to start running the script from</param>
    /// <returns>The input <see cref="ReleaseConfiguration"/> instance</returns>
    /// <remarks>This property will override the values of <see cref="ReleaseConfiguration.MemorySize"/> and <see cref="ReleaseConfiguration.OverflowMode"/></remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly ReleaseConfiguration WithInitialState(in this ReleaseConfiguration configuration, IReadOnlyMachineState initialState)
    {
        Unsafe.AsRef(configuration.InitialState) = initialState;
        
        return ref configuration;
    }

    /// <summary>
    /// Sets the memory size for a given configuration
    /// </summary>
    /// <param name="configuration">The input <see cref="ReleaseConfiguration"/> instance</param>
    /// <param name="memorySize">The size of the state machine to create to run the script</param>
    /// <returns>The input <see cref="ReleaseConfiguration"/> instance</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly ReleaseConfiguration WithMemorySize(in this ReleaseConfiguration configuration, int memorySize)
    {
        Unsafe.AsRef(configuration.MemorySize) = memorySize;
        
        return ref configuration;
    }

    /// <summary>
    /// Sets the overflow mode for a given configuration
    /// </summary>
    /// <param name="configuration">The input <see cref="ReleaseConfiguration"/> instance</param>
    /// <param name="overflowMode">The overflow mode to use in the state machine used to run the script</param>
    /// <returns>The input <see cref="ReleaseConfiguration"/> instance</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly ReleaseConfiguration WithOverflowMode(in this ReleaseConfiguration configuration, OverflowMode overflowMode)
    {
        Unsafe.AsRef(configuration.OverflowMode) = overflowMode;
        
        return ref configuration;
    }

    /// <summary>
    /// Sets the execution token for a given configuration
    /// </summary>
    /// <param name="configuration">The input <see cref="ReleaseConfiguration"/> instance</param>
    /// <param name="executionToken">A <see cref="CancellationToken"/> that can be used to halt the execution of long running scripts</param>
    /// <returns>The input <see cref="ReleaseConfiguration"/> instance</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly ReleaseConfiguration WithExecutionToken(in this ReleaseConfiguration configuration, CancellationToken executionToken)
    {
        Unsafe.AsRef(configuration.ExecutionToken) = executionToken;
        
        return ref configuration;
    }
}
