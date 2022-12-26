using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Base;
using Brainf_ckSharp.Unit.Shared.Models;

#nullable enable

namespace Brainf_ckSharp.Uwp.Profiler;

public static class Brainf_ckBenchmark
{
    /// <summary>
    /// The number of benchmarking runs to run for each benchmark
    /// </summary>
    private const int NumberOfRuns = 10;

    /// <summary>
    /// Executes the available benchmarks
    /// </summary>
    /// <returns>A markdown table with the results</returns>
    public static async Task<string> RunAsync()
    {
        StorageFolder scriptsFolder = await Package.Current.InstalledLocation.GetFolderAsync(@"Assets\Scripts");
        IReadOnlyCollection<StorageFile> scriptFiles = await scriptsFolder.GetFilesAsync();

        StringBuilder builder = new();

        builder.AppendLine("|         Test |  Config. |         Mean |          Min |          Max |");
        builder.AppendLine("|-------------:|----------|-------------:|-------------:|-------------:|");

        int i = 0;
        foreach (StorageFile scriptFile in scriptFiles)
        {
            using var stream = await scriptFile.OpenStreamForReadAsync();
            using var reader = new StreamReader(stream);

            await Task.Run(() =>
            {
                string
                    name = $"{scriptFile.DisplayName} ".PadLeft(14),
                    text = reader.ReadToEnd();
                string[] parts = text.Split("|").Select(p => p.TrimStart().Replace("\r", string.Empty)).ToArray();

                Script script = new(
                    parts[0],
                    parts[1],
                    int.Parse(parts[2]),
                    Enum.Parse<OverflowMode>(parts[3]),
                    parts[4]);

                string
                    debug = Run<Debug>(script),
                    release = Run<Release>(script);

                if (i++ > 0)
                {
                    builder.AppendLine("|              |          |              |              |              |");
                }

                builder.AppendLine($"|{name}|    Debug |{debug}|");
                builder.AppendLine($"|{name}|  Release |{release}|");
            });
        }

        return builder.ToString();
    }

    /// <summary>
    /// Runs a given benchmark script
    /// </summary>
    /// <typeparam name="T">The type of benchmark to run</typeparam>
    /// <param name="script">The input script to run</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string Run<T>(Script script)
        where T : struct, IScriptRunner
    {
        T runner = default;

        GC.Collect();

        // Warmup
        _ = runner.Run(script);

        GC.Collect();

        Span<TimeSpan> times = stackalloc TimeSpan[NumberOfRuns];

        Stopwatch timer = new();

        for (int i = 0; i < NumberOfRuns; i++)
        {
            timer.Restart();

            _ = runner.Run(script);

            timer.Stop();

            times[i] = timer.Elapsed;
        }

        GC.Collect();

        return GetStatisticsForRun(times);
    }

    /// <summary>
    /// Creates a formatted representation of times from a given run result
    /// </summary>
    /// <param name="times">The times for a benchmark run</param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static string GetStatisticsForRun(ReadOnlySpan<TimeSpan> times)
    {
        long
            avg = 0,
            min = long.MaxValue,
            max = long.MinValue;

        foreach (TimeSpan time in times)
        {
            avg += time.Ticks;

            if (time.Ticks < min) min = time.Ticks;

            if (time.Ticks > max) max = time.Ticks;
        }

        avg -= min;
        avg -= max;

        avg /= times.Length - 2;

        string
            first = TimeSpan.FromTicks(avg).ToString("m':'s'.'ffffff").PadLeft(14),
            second = TimeSpan.FromTicks(min).ToString("m':'s'.'ffffff").PadLeft(14),
            third = TimeSpan.FromTicks(max).ToString("m':'s'.'ffffff").PadLeft(14);

        return string.Join('|', first, second, third);
    }

    /// <summary>
    /// An <see langword="interface"/> for a script runner in a given configuration
    /// </summary>
    public interface IScriptRunner
    {
        /// <summary>
        /// Runs a given <see cref="Script"/> instance.
        /// </summary>
        /// <param name="script">The script to run</param>
        /// <returns>The stdout result for <paramref name="script"/>.</returns>
        string Run(Script script);
    }

    /// <summary>
    /// A script runner in DEBUG mode
    /// </summary>
    private readonly struct Debug : IScriptRunner
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string Run(Script script)
        {
            Option<InterpreterSession> result = Brainf_ckInterpreter
                .CreateDebugConfiguration()
                .WithSource(script.Source)
                .WithStdin(script.Stdin)
                .WithMemorySize(script.MemorySize)
                .WithOverflowMode(script.OverflowMode)
                .TryRun();

            using InterpreterSession enumerator = result.Value!;

            enumerator.MoveNext();
            return enumerator.Current.Stdout;
        }
    }

    /// <summary>
    /// A script runner in RELEASE mode
    /// </summary>
    private readonly struct Release : IScriptRunner
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string Run(Script script)
        {
            Option<InterpreterResult> result = Brainf_ckInterpreter
                .CreateReleaseConfiguration()
                .WithSource(script.Source)
                .WithStdin(script.Stdin)
                .WithMemorySize(script.MemorySize)
                .WithOverflowMode(script.OverflowMode)
                .TryRun();

            result.Value!.MachineState.Dispose();

            return result.Value.Stdout;
        }
    }
}
