using System.Runtime.CompilerServices;
using Brainf_ckSharp.Memory.Interfaces;

namespace Brainf_ckSharp.Memory.ExecutionContexts;

/// <summary>
/// Implementations of <see cref="IMachineStateSize"/> for the available sizes.
/// </summary>
internal static class MachineStateSize
{
    /// <summary>
    /// An <see cref="IMachineStateSize"/> with a size of 32.
    /// </summary>
    public readonly struct _32 : IMachineStateSize
    {
        /// <inheritdoc/>
        public static int Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 32;
        }
    }

    /// <summary>
    /// An <see cref="IMachineStateSize"/> with a size of 64.
    /// </summary>
    public readonly struct _64 : IMachineStateSize
    {
        /// <inheritdoc/>
        public static int Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 64;
        }
    }

    /// <summary>
    /// An <see cref="IMachineStateSize"/> with a size of 128.
    /// </summary>
    public readonly struct _128 : IMachineStateSize
    {
        /// <inheritdoc/>
        public static int Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 128;
        }
    }

    /// <summary>
    /// An <see cref="IMachineStateSize"/> with a size of 256.
    /// </summary>
    public readonly struct _256 : IMachineStateSize
    {
        /// <inheritdoc/>
        public static int Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 256;
        }
    }

    /// <summary>
    /// An <see cref="IMachineStateSize"/> with a size of 512.
    /// </summary>
    public readonly struct _512 : IMachineStateSize
    {
        /// <inheritdoc/>
        public static int Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 512;
        }
    }

    /// <summary>
    /// An <see cref="IMachineStateSize"/> with a size of 1024.
    /// </summary>
    public readonly struct _1024 : IMachineStateSize
    {
        /// <inheritdoc/>
        public static int Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 1024;
        }
    }

    /// <summary>
    /// An <see cref="IMachineStateSize"/> with a size of 2048.
    /// </summary>
    public readonly struct _2048 : IMachineStateSize
    {
        /// <inheritdoc/>
        public static int Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 2048;
        }
    }

    /// <summary>
    /// An <see cref="IMachineStateSize"/> with a size of 4096.
    /// </summary>
    public readonly struct _4096 : IMachineStateSize
    {
        /// <inheritdoc/>
        public static int Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 4096;
        }
    }

    /// <summary>
    /// An <see cref="IMachineStateSize"/> with a size of 8192.
    /// </summary>
    public readonly struct _8192 : IMachineStateSize
    {
        /// <inheritdoc/>
        public static int Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 8192;
        }
    }

    /// <summary>
    /// An <see cref="IMachineStateSize"/> with a size of 16384.
    /// </summary>
    public readonly struct _16384 : IMachineStateSize
    {
        /// <inheritdoc/>
        public static int Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 16384;
        }
    }

    /// <summary>
    /// An <see cref="IMachineStateSize"/> with a size of 32768.
    /// </summary>
    public readonly struct _32768 : IMachineStateSize
    {
        /// <inheritdoc/>
        public static int Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 32768;
        }
    }
}
