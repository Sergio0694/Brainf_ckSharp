using System;
using Brainf_ckSharp.Constants;

namespace Brainf_ckSharp.Cli.Helpers;

/// <summary>
/// A helper <see langword="class"/> with methods to display data on the output console
/// </summary>
internal static class Logger
{
    /// <summary>
    /// Inserts a new line to the console output
    /// </summary>
    public static void NewLine()
    {
        Console.Write(Environment.NewLine);
    }

    /// <summary>
    /// Writes an opening tag
    /// </summary>
    /// <param name="tagColor">The tag color to use</param>
    /// <param name="tag">The tag to display for the info</param>
    public static void Write(ConsoleColor tagColor, string tag)
    {
        Console.ForegroundColor = tagColor;
        Console.Write($"[{tag.ToUpperInvariant()}] ");
        Console.ForegroundColor = ConsoleColor.White;
    }

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

    /// <summary>
    /// Prints a parsed source code with syntax highlight to the console output
    /// </summary>
    /// <param name="source">The source code to display</param>
    public static void Highlight(string source)
    {
        foreach (char c in source)
        {
            Console.ForegroundColor = c switch
            {
                Characters.Plus => ConsoleColor.White,
                Characters.Minus => ConsoleColor.White,
                Characters.ForwardPtr => ConsoleColor.Gray,
                Characters.BackwardPtr => ConsoleColor.Gray,
                Characters.PrintChar => ConsoleColor.Red,
                Characters.ReadChar => ConsoleColor.DarkYellow,
                Characters.LoopStart => ConsoleColor.DarkCyan,
                Characters.LoopEnd => ConsoleColor.DarkCyan,
                Characters.FunctionStart => ConsoleColor.Cyan,
                Characters.FunctionEnd => ConsoleColor.Cyan,
                Characters.FunctionCall => ConsoleColor.Blue,
                _ => throw new ArgumentOutOfRangeException(nameof(source), $"Invalid source operator: {c}")
            };
            Console.Write(c);
        }

        Console.ForegroundColor = ConsoleColor.White;
    }
}
