using System;
using System.Threading;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Base;
using Brainf_ckSharp.Unit.Shared;
using Brainf_ckSharp.Unit.Shared.Models;

#nullable enable

namespace Brainf_ckSharp.Profiler
{
    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByParams)]
    public abstract class Brainf_ckBenchmarkBase
    {
        /// <summary>
        /// The currently loaded script to test
        /// </summary>
        private Script? Script;

        /// <summary>
        /// The name of the script to benchmark
        /// </summary>
        public virtual string? Name { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            TriggerTier1Jit();

            Script = ScriptLoader.LoadScriptByName(Name!);
        }

        /// <summary>
        /// Runs all the benchmarks a number of times to ensure Tier1 code is generated
        /// </summary>
        private void TriggerTier1Jit()
        {
            Script = ScriptLoader.LoadScriptByName("HelloWorld");

            for (int i = 0; i < 1000; i++)
            {
                Release();
            }

            Thread.Sleep(TimeSpan.FromSeconds(1));

            GC.Collect();
            GC.WaitForPendingFinalizers();
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

    [SimpleJob(RunStrategy.Throughput)]
    public class Brainf_ckBenchmark_Short : Brainf_ckBenchmarkBase
    {
        /// <inheritdoc/>
        [Params("HelloWorld", "Sum", "Multiply", "Division", "Fibonacci")]
        public override string? Name { get; set; }
    }

    [SimpleJob(RunStrategy.Monitoring)]
    public class Brainf_ckBenchmark_Long : Brainf_ckBenchmarkBase
    {
        /// <inheritdoc/>
        [Params("Mandelbrot")]
        public override string? Name { get; set; }
    }
}
