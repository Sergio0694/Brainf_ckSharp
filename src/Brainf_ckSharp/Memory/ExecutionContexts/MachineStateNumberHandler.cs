using System.Numerics;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Memory.Interfaces;

namespace Brainf_ckSharp.Memory.ExecutionContexts;

/// <summary>
/// Implementations of <see cref="IMachineStateNumberHandler{TValue}"/> for available options.
/// </summary>
internal static class MachineStateNumberHandler
{
    /// <summary>
    /// An <see cref="IMachineStateNumberHandler{TValue}"/> implementation with overflow.
    /// </summary>
    /// <typeparam name="TValue">The type of values in each memory cell</typeparam>
    public readonly struct Overflow<TValue> : IMachineStateNumberHandler<TValue>
        where TValue : unmanaged, IBinaryInteger<TValue>
    {
        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryDecrement(ref TValue value)
        {
            value--;

            return true;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryDecrement(ref TValue value, int count, ref int totalOperations)
        {
            value -= TValue.CreateTruncating(count);

            totalOperations += count;

            return true;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryIncrement(ref TValue value)
        {
            value++;

            return true;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryIncrement(ref TValue value, int count, ref int totalOperations)
        {
            value += TValue.CreateTruncating(count);

            totalOperations += count;

            return true;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryInput(ref TValue value, char c)
        {
            value = TValue.CreateTruncating(c);

            return true;
        }
    }

    /// <summary>
    /// An <see cref="IMachineStateNumberHandler{TValue}"/> implementation with no overflow.
    /// </summary>
    /// <typeparam name="TValue">The type of values in each memory cell</typeparam>
    public readonly struct NoOverflow<TValue> : IMachineStateNumberHandler<TValue>
        where TValue : unmanaged, IBinaryInteger<TValue>, IMinMaxValue<TValue>
    {
        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryDecrement(ref TValue value)
        {
            if (value != TValue.Zero)
            {
                value--;

                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryDecrement(ref TValue value, int count, ref int totalOperations)
        {
            TValue decrement = TValue.CreateTruncating(count);

            if (value >= decrement)
            {
                value -= decrement;

                totalOperations += count;

                return true;
            }

            totalOperations += int.CreateTruncating(value);

            value = TValue.Zero;

            return false;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryIncrement(ref TValue value)
        {
            if (value != TValue.MaxValue)
            {
                value++;

                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryIncrement(ref TValue value, int count, ref int totalOperations)
        {
            TValue increment = TValue.CreateTruncating(count);

            if (TValue.MaxValue - value >= increment)
            {
                value += increment;

                totalOperations += count;

                return true;
            }

            totalOperations += int.CreateTruncating(TValue.MaxValue - value);

            value = TValue.MaxValue;

            return false;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryInput(ref TValue value, char c)
        {
            TValue input = TValue.CreateTruncating(c);

            if (input <= TValue.MaxValue)
            {
                value = input;

                return true;
            }

            return false;
        }
    }
}
