using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Brainf_ckSharp.Uwp.Controls.Ide.Extensions.System
{
    /// <summary>
    /// A <see langword="class"/> with some extension methods for the <see cref="uint"/> type
    /// </summary>
    internal static class UInt32Extensions
    {
        /// <summary>
        /// Writes a given <see cref="uint"/> value to a target buffer
        /// </summary>
        /// <param name="n">The input <see cref="uint"/> value</param>
        /// <param name="p">The pointer of the <see cref="char"/> buffer to write the number to</param>
        /// <param name="digits">The number of digits in <paramref name="n"/>, previously retrieved with <see cref="CountDigits"/></param>
        /// <remarks>This code is borrowed from <see href="https://github.com/dotnet/runtime/blob/master/src/libraries/System.Private.CoreLib/src/System/Number.Formatting.cs">dotnet/runtime</see></remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ToString(this uint n, char* p, int digits)
        {
            Debug.Assert(p != null);
            Debug.Assert(n.CountDigits() == digits);

            p += digits;

            do
            {
                uint
                    division = n / 10,
                    remainder = n - division * 10;

                n = division;
                *--p = (char)(remainder + '0');
            }
            while (n != 0);
        }

        /// <summary>
        /// Counts the number of digits into a given <see cref="uint"/> value
        /// </summary>
        /// <param name="value">The input <see cref="uint"/> value</param>
        /// <returns>The number of digits in <paramref name="value"/></returns>
        /// <remarks>This code is borrowed from <see href="https://github.com/dotnet/runtime/blob/master/src/libraries/System.Private.CoreLib/src/System/Buffers/Text/FormattingHelpers.CountDigits.cs">dotnet/runtime</see></remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountDigits(this uint value)
        {
            int digits = 1;

            if (value >= 100000)
            {
                value /= 100000;
                digits += 5;
            }

            if (value < 10) { }
            else if (value < 100) digits++;
            else if (value < 1000) digits += 2;
            else if (value < 10000) digits += 3;
            else digits += 4;

            return digits;
        }
    }
}
