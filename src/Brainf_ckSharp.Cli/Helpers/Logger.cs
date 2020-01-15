using System;

namespace Brainf_ckSharp.Cli.Helpers
{
    /// <summary>
    /// A helper <see langword="class"/> with methods to display data on the output console
    /// </summary>
    internal static class Logger
    {
        /// <summary>
        /// Writes a tagged info on the console output
        /// </summary>
        /// <param name="tagColor">The tag color to use</param>
        /// <param name="tag">The tag to display for the info</param>
        /// <param name="body">The body of the info</param>
        public static void Write(
            ConsoleColor tagColor,
            string tag,
            string body)
        {
            Write(tagColor, ConsoleColor.White, tag, body);
        }

        /// <summary>
        /// Writes a tagged info on the console output
        /// </summary>
        /// <param name="tagColor">The tag color to use</param>
        /// <param name="bodyColor">The body color to use</param>
        /// <param name="tag">The tag to display for the info</param>
        /// <param name="body">The body of the info</param>
        public static void Write(
            ConsoleColor tagColor,
            ConsoleColor bodyColor,
            string tag,
            string body)
        {
            Console.ForegroundColor = tagColor;
            Console.Write($"[{tag.ToUpperInvariant()}] ");
            Console.ForegroundColor = bodyColor;
            Console.WriteLine(body);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
