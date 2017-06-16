﻿using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Helpers.Extensions
{
    /// <summary>
    /// A simple static class with some useful extension methods
    /// </summary>
    public static class MiscExtensions
    {
        /// <summary>
        /// Performs a direct cast on the given object to a specific type
        /// </summary>
        /// <typeparam name="T">The tye to return</typeparam>
        /// <param name="o">The object to cast</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T To<T>([CanBeNull] this object o) => (T)o;

        /// <summary>
        /// Suppresses the warnings when calling an async method without awaiting it
        /// </summary>
        /// <param name="task">The Task returned by the async call</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Forget(this Task task) { }

        /// <summary>
        /// Suppresses the warnings when calling an async method without awaiting it
        /// </summary>
        /// <param name="action">The IAsyncAction returned by the async call</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Forget(this IAsyncAction action) { }

        /// <summary>
        /// Finds the coordinates in a multiline string for the given index
        /// </summary>
        /// <param name="text">The input text</param>
        /// <param name="index">The target text index</param>
        /// <param name="newline">The newline character to use</param>
        public static (int Y, int X) FindCoordinates([NotNull] this String text, int index, char newline = '\r')
        {
            int
                row = 1,
                col = 1;
            for (int i = 0; i < index; i++)
            {
                if (text[i] == '\r')
                {
                    row++;
                    col = 1;
                }
                else col++;
            }
            return (row, col);
        }
    }
}