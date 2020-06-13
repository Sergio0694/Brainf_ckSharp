using System;
using System.Diagnostics.Contracts;
using Microsoft.Toolkit.Uwp.Extensions;

namespace Brainf_ckSharp.Uwp.Converters
{
    /// <summary>
    /// A <see langword="class"/> with a collection of helper functions for bindings to <see cref="Enum"/> values
    /// </summary>
    public static class EnumConverter
    {
        /// <summary>
        /// Returns a localized version of the input <see cref="Enum"/> value
        /// </summary>
        /// <param name="value">The input value to localize</param>
        /// <returns>A localized representation for <paramref name="value"/></returns>
        [Pure]
        [Obsolete("This method is needed to trick the XAML compiler, use the generic version instead")]
        public static string Convert(object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a localized version of the input <see cref="Enum"/> value
        /// </summary>
        /// <typeparam name="T">The input<see cref="Enum"/> value to localize</typeparam>
        /// <param name="value">The input value to localize</param>
        /// <returns>A localized representation for <paramref name="value"/></returns>
        [Pure]
        public static string Convert<T>(T value) where T : struct, Enum
        {
            return $"{typeof(T).Name}/{value}".GetLocalized();
        }
    }
}
