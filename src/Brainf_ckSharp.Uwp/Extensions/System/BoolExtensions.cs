using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace System
{
    /// <summary>
    /// An extension <see langword="class"/> for the <see cref="bool"/> type
    /// </summary>
    public static class BoolExtensions
    {
        /// <summary>
        /// Converts the given <see cref="bool"/> value into an <see cref="int"/>
        /// </summary>
        /// <param name="flag">The input value to convert</param>
        /// <returns>1 if <paramref name="flag"/> is <see langword="true"/>, 0 otherwise</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this bool flag)
        {
            return Unsafe.As<bool, byte>(ref flag);
        }
    }
}
