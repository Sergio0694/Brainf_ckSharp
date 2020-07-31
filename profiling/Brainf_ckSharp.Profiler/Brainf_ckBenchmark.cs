using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Base;
using Brainf_ckSharp.Unit.Shared;
using Brainf_ckSharp.Unit.Shared.Models;

#nullable enable

namespace Brainf_ckSharp.Profiler
{
    [MemoryDiagnoser]
    [SimpleJob(RunStrategy.Monitoring, RuntimeMoniker.NetCoreApp21)]
    [SimpleJob(RunStrategy.Monitoring, RuntimeMoniker.NetCoreApp31)]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByParams)]
    public class Brainf_ckBenchmark
    {
        /// <summary>
        /// The name of the script to benchmark
        /// </summary>
        [Params("HelloWorld", "Sum", "Multiply", "Division", "Fibonacci", "Mandelbrot")]
        public string? Name;

        /// <summary>
        /// The currently loaded script to test
        /// </summary>
        private Script? Script;

        [GlobalSetup]
        public void Setup()
        {
            Script = ScriptLoader.LoadScriptByName(Name!);
        }

        [Benchmark(Baseline = true)]
        public string Debug()
        {
            Option<InterpreterSession> result = Brainf_ckInterpreter
                .CreateDebugConfiguration()
                .WithSource(Script!.Source)
                .WithStdin(Script.Stdin)
                .WithMemorySize(Script.MemorySize)
                .WithOverflowMode(Script.OverflowMode)
                .TryRun();

            using InterpreterSession enumerator = result.Value!;

            enumerator!.MoveNext();

            return enumerator.Current.Stdout;
        }

        [Benchmark]
        public string Release()
        {
            Option<InterpreterResult> result = Brainf_ckInterpreter
                .CreateReleaseConfiguration()
                .WithSource(Script!.Source)
                .WithStdin(Script.Stdin)
                .WithMemorySize(Script.MemorySize)
                .WithOverflowMode(Script.OverflowMode)
                .TryRun();

            result.Value!.MachineState.Dispose();

            return result.Value!.Stdout;
        }
    }
}
