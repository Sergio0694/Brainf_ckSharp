﻿using System;
using System.IO;
using System.Threading;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Base;
using CommandLine;

namespace Brainf_ckSharp.Cli
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Parser parser = new Parser(options => options.CaseInsensitiveEnumValues = true);
            ParserResult<Options> parserResult = parser.ParseArguments<Options>(args);

            parserResult.WithParsed(options =>
            {
                Console.WriteLine();

                // Get the source code to execute
                string source;
                if (options.SourceFile is { } && File.Exists(options.SourceFile))
                {
                    source = File.ReadAllText(options.SourceFile);
                }
                else source = options.Source!;

                // Create the cancellation token source
                CancellationTokenSource cts = options.Timeout == 0
                    ? new CancellationTokenSource()
                    : new CancellationTokenSource(TimeSpan.FromSeconds(options.Timeout));

                // Execute the code
                Option<InterpreterResult> interpreterResult = Brainf_ckInterpreter.TryRun(
                    source,
                    options.Stdin ?? string.Empty,
                    options.MemorySize,
                    options.OverflowMode,
                    cts.Token);

                // Display the execution result
                if (interpreterResult.Value is { })
                {
                    // Runtime error, if any
                    if (!interpreterResult.Value.ExitCode.HasFlag(ExitCode.Success))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write("[RUNTIME ERROR] ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(interpreterResult.Value.ExitCode switch
                        {
                            ExitCode.UpperBoundExceeded => "Upper bound exceeded",
                            ExitCode.LowerBoundExceeded => "Lower bound exceeded",
                            ExitCode.NegativeValue => "Negative value",
                            ExitCode.MaxValueExceeded => "Maximum value exceeded",
                            ExitCode.StdinBufferExhausted => "Stdin buffer exhausted",
                            ExitCode.StdoutBufferLimitExceeded => "Stdout buffer limit exceeded",
                            ExitCode.DuplicateFunctionDefinition => "Duplicate function definition",
                            ExitCode.UndefinedFunctionCalled => "Undefined function call",
                            ExitCode.ThresholdExceeded => "Threshold exceeded",
                            _ => throw new ArgumentOutOfRangeException(nameof(ExitCode), $"Invalid error: {interpreterResult.Value.ExitCode}")
                        });
                    }

                    // Stdout
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("[STDOUT] ");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(interpreterResult.Value.Stdout);
                    Console.ForegroundColor = ConsoleColor.White;

                    if (options.Stdout is { }) File.WriteAllText(options.Stdout, interpreterResult.Value.Stdout);

                    // Additional info, if verbose mode is on
                    if (options.Verbose)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("[ELAPSED TIME] ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(interpreterResult.Value.ElapsedTime);

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("[TOTAL OPERATORS] ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(interpreterResult.Value.TotalOperations);
                    }
                }
                else
                {
                    // Syntax error
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("[SYNTAX ERROR] ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(interpreterResult.ValidationResult.ErrorType switch
                    {
                        SyntaxError.MismatchedSquareBracket => "Mismatched square bracket",
                        SyntaxError.IncompleteLoop => "Incomplete loop",
                        SyntaxError.MismatchedParenthesis => "Mismatched parenthesis",
                        SyntaxError.InvalidFunctionDeclaration => "Invalid function declaration",
                        SyntaxError.NestedFunctionDeclaration => "Nested function declaration",
                        SyntaxError.EmptyFunctionDeclaration => "Empty function declaration",
                        SyntaxError.IncompleteFunctionDeclaration => "Incomplete function declaration",
                        SyntaxError.MissingOperators => "Missing operators",
                        _ => throw new ArgumentOutOfRangeException(nameof(SyntaxError), $"Invalid error: {interpreterResult.ValidationResult.ErrorType}")
                    });

                    // Error position
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("[ERROR OFFSET] ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(interpreterResult.ValidationResult.ErrorOffset);
                }

                // Notify with a sound
                if (options.Beep)
                {
                    Console.Beep();
                    Thread.Sleep(150);
                    Console.Beep();
                }
            });

            parserResult.WithNotParsed(errors =>
            {
                foreach (Error error in errors)
                    Console.WriteLine(error);
            });
        }
    }
}
