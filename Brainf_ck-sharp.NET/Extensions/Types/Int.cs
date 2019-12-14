using System.Runtime.CompilerServices;

namespace Brainf_ck_sharp.NET.Extensions.Types
{
    /// <summary>
    /// A <see langword="class"/> that represents an <see cref="int"/> allocated on the heap
    /// </summary>
    public sealed class Int
    {
        /// <summary>
        /// The actual <see cref="int"/> value for the current instance
        /// </summary>
        private int _Value;

        /// <summary>
        /// Casts the current <see cref="Int"/> instance to an <see cref="int"/> value
        /// </summary>
        /// <param name="obj">The input <see cref="Int"/> instaance</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(Int obj) => obj._Value;

        /// <summary>
        /// Creates a new <see cref="Int"/> instance from a given <see cref="int"/> value
        /// </summary>
        /// <param name="n">The input <see cref="int"/> value to read</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Int(int n) => new Int { _Value = n };

        /// <summary>
        /// Increments a given <see cref="Int"/> instance by one
        /// </summary>
        /// <param name="obj">The target <see cref="Int"/> instance to modify</param>
        /// <returns>The same input <see cref="Int"/> instance</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int operator ++(Int obj)
        {
            obj._Value += 1;
            return obj;
        }

        /// <summary>
        /// Increments a given <see cref="Int"/> instance by the specified amount
        /// </summary>
        /// <param name="obj">The target <see cref="Int"/> instance to modify</param>
        /// <param name="n">The <see cref="int"/> value to add to the current <see cref="Int"/> instance</param>
        /// <returns>The same input <see cref="Int"/> instance</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int operator +(Int obj, int n)
        {
            obj._Value += n;
            return obj;
        }
    }
}
