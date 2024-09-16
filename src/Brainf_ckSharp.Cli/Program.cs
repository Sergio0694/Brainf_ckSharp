using System;
using System.IO;
using System.Threading;
using Brainf_ckSharp.Cli;
using Brainf_ckSharp;
using Brainf_ckSharp.Cli.Helpers;
using Brainf_ckSharp.Configurations;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Base;
using CommandLine;

Parser parser = new(options =>
{
    options.CaseInsensitiveEnumValues = true;
    options.IgnoreUnknownArguments = false;
    options.AutoHelp = true;
    options.HelpWriter = Console.Out;
});

ParserResult<Options> parserResult = parser.ParseArguments<Options>(args);

Logger.NewLine();

parserResult.WithParsed(options =>
{
    // Get the source code to execute
    string source;
    if (options.SourceFile is { } && File.Exists(options.SourceFile))
    {
        source = File.ReadAllText(options.SourceFile);
    }
    else
    {
        source = options.Source!;
    }

    // Get the stdin to use
    string stdin;
    if (options.StdinFile is { } && File.Exists(options.StdinFile))
    {
        stdin = File.ReadAllText(options.StdinFile);
    }
    else
    {
        stdin = options.Stdin ?? string.Empty;
    }

    // Create the cancellation token source
    CancellationTokenSource cts = options.Timeout == 0
        ? new CancellationTokenSource()
        : new CancellationTokenSource(TimeSpan.FromSeconds(options.Timeout));

    // Execute the code
    Option<InterpreterResult> interpreterResult = Brainf_ckInterpreter.TryRun(new ReleaseConfiguration
    {
        Source = source.AsMemory(),
        Stdin = stdin.AsMemory(),
        MemorySize = options.MemorySize,
        DataType = options.DataType,
        ExecutionOptions = options.ExecutionOptions,
        ExecutionToken = cts.Token
    });

    // Display the execution result
    if (interpreterResult.Value is { })
    {
        // Runtime error, if any
        if (!interpreterResult.Value.ExitCode.HasFlag(ExitCode.Success))
        {
            Logger.Write(
                ConsoleColor.DarkRed,
                "runtime error",
                interpreterResult.Value.ExitCode switch
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

        // Stdout, if any
        Logger.Write(
            ConsoleColor.DarkGray,
            ConsoleColor.Yellow,
            "stdout",
            interpreterResult.Value.Stdout);
        Logger.NewLine();

        if (options.Stdout is { })
        {
            File.WriteAllText(options.Stdout, interpreterResult.Value.Stdout);
        }

        // Additional info, if verbose mode is on
        if (options.Verbose)
        {
            // Source
            Logger.Write(ConsoleColor.DarkGray, "source");
            Logger.Highlight(interpreterResult.Value.SourceCode);
            Logger.NewLine();
            Logger.NewLine();

            // Elapsed time
            Logger.Write(
                ConsoleColor.DarkGray,
                "elapsed time",
                interpreterResult.Value.ElapsedTime.ToString("g"));

            // Total operations
            Logger.Write(
                ConsoleColor.DarkGray,
                "total operations",
                interpreterResult.Value.TotalOperations.ToString());
        }
    }
    else
    {
        // Syntax error
        Logger.Write(
            ConsoleColor.DarkRed,
            "syntax error",
            interpreterResult.ValidationResult.ErrorType switch
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
        Logger.Write(
            ConsoleColor.DarkYellow,
            "error offset",
            interpreterResult.ValidationResult.ErrorOffset.ToString());
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
    {
        if (error is HelpRequestedError)
        {
            continue;
        }

        Logger.Write(
            ConsoleColor.Red,
            "argument(s) error",
            error switch
            {
                BadFormatTokenError e => $"Bad token: \"{e.Token}\"",
                MissingValueOptionError e => $"Missing value: \"{e.NameInfo.LongName}\"",
                UnknownOptionError e => $"Unknown option: \"{e.Token}\"",
                MissingRequiredOptionError e => $"Missing option: \"{e.NameInfo.LongName}\"",
                MutuallyExclusiveSetError e => $"Mutually exclusive options set: \"{e.SetName}\"",
                BadFormatConversionError e => $"Conversion error: \"{e.NameInfo.LongName}\"",
                RepeatedOptionError e => $"Repeated option: \"{e.NameInfo.LongName}\"",
                MissingGroupOptionError e => $"Missing group option: \"{e.Group}\"",
                _ => throw new ArgumentOutOfRangeException(nameof(error), $"Unexpected error: {error.Tag}")
            });
    }
});
