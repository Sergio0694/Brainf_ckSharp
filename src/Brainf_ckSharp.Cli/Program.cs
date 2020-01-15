using System;
using System.IO;
using System.Threading;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Base;
using CommandLine;

namespace Brainf_ckSharp.Cli
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            ParserResult<Options> parserResult = Parser.Default.ParseArguments<Options>(args);

            parserResult.WithParsed(options =>
            {
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
                    Console.WriteLine(interpreterResult.Value.Stdout);

                    if (options.Stdout is { }) File.WriteAllText(options.Stdout, interpreterResult.Value.Stdout);
                }
                else Console.WriteLine(interpreterResult.ValidationResult.ErrorType);
            });

            parserResult.WithNotParsed(errors =>
            {
                foreach (Error error in errors)
                    Console.WriteLine(error);
            });
        }
    }
}
