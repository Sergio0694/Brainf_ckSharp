using System;
using BenchmarkDotNet.Attributes;
using Brainf_ckSharp;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Base;
using Brainf_ckSharp.Unit.Shared;
using Brainf_ckSharp.Unit.Shared.Models;
using Brainf_ckInterpreterOld = Brainf_ckSharp.Legacy.Brainf_ckInterpreter;
using Brainf_ckInterpreterNew = Brainf_ckSharp.Brainf_ckInterpreter;
using LegacyInterpreterResult = Brainf_ckSharp.Legacy.ReturnTypes.InterpreterResult;
using OverflowModeOld = Brainf_ckSharp.Legacy.Enums.OverflowMode;
using OverflowModeNew = Brainf_ckSharp.Enums.OverflowMode;

namespace Brainf_ck_sharp.Profiler
{
    [MemoryDiagnoser]
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

        /// <summary>
        /// The overflow mode for the legacy interpreter
        /// </summary>
        private OverflowModeOld LegacyOverflowMode;

        [GlobalSetup]
        public void Setup()
        {
            Script = ScriptLoader.LoadScriptByName(Name!);

            LegacyOverflowMode = Script.OverflowMode switch
            {
                OverflowModeNew.UshortWithNoOverflow => OverflowModeOld.ShortNoOverflow,
                OverflowModeNew.ByteWithOverflow => OverflowModeOld.ByteOverflow,
                _ => throw new ArgumentOutOfRangeException(nameof(Script.OverflowMode), Script.OverflowMode.ToString())
            };

            if (!Script.Stdout.Equals(Legacy())) throw new InvalidOperationException("Legacy test failed");
            if (!Script.Stdout.Equals(Debug())) throw new InvalidOperationException("Debug test failed");
            if (!Script.Stdout.Equals(Release())) throw new InvalidOperationException("Release test failed");
        }

        [Benchmark(Baseline = true)]
        public string Legacy()
        {
            LegacyInterpreterResult result = Brainf_ckInterpreterOld.Run(
                Script!.Source,
                Script.Stdin,
                LegacyOverflowMode,
                Script.MemorySize);

            return result.Output;
        }

        [Benchmark]
        public string Debug()
        {
            Option<InterpreterSession> result = Brainf_ckInterpreterNew
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
            Option<InterpreterResult> result = Brainf_ckInterpreterNew
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
