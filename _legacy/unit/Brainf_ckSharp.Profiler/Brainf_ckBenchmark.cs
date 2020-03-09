using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Brainf_ckSharp;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Base;
using Brainf_ckInterpreterOld = Brainf_ckSharp.Legacy.Brainf_ckInterpreter;
using Brainf_ckInterpreterNew = Brainf_ckSharp.Brainf_ckInterpreter;

namespace Brainf_ck_sharp.Profiler
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.NetCoreApp21)]
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    public class Brainf_ckBenchmark
    {
        [Params("HelloWorld", "Sum", "Multiply", "Division", "Fibonacci")]
        public string Test;

        private string Script;
        private string Stdin;

        [GlobalSetup]
        public void Setup()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string filename = assembly.GetManifestResourceNames().First(name => name.EndsWith($"{Test}.txt"));

            using Stream stream = assembly.GetManifestResourceStream(filename);
            using StreamReader reader = new StreamReader(stream);

            Stdin = Regex.Match(reader.ReadLine(), @$"\[([^]]+)\]").Groups[1].Value;

            string expected = Regex.Match(reader.ReadLine(), @$"\[([^]]+)\]").Groups[1].Value;

            Script = reader.ReadToEnd();

            if (!expected.Equals(Legacy())) throw new InvalidOperationException("Legacy test failed");
            if (!expected.Equals(Debug())) throw new InvalidOperationException("Debug test failed");
            if (!expected.Equals(Release())) throw new InvalidOperationException("Release test failed");
        }

        [Benchmark(Baseline = true)]
        public string Legacy()
        {
            return Brainf_ckInterpreterOld.Run(Script, Stdin).Output;
        }

        [Benchmark]
        public string Debug()
        {
            Option<InterpreterSession> result = Brainf_ckInterpreterNew
                .CreateDebugConfiguration()
                .WithSource(Script)
                .WithStdin(Stdin)
                .WithMemorySize(64)
                .WithOverflowMode(OverflowMode.UshortWithNoOverflow)
                .TryRun();

            using InterpreterSession enumerator = result.Value!;

            enumerator.MoveNext();
            return enumerator.Current.Stdout;
        }

        [Benchmark]
        public string Release()
        {
            Option<InterpreterResult> result = Brainf_ckInterpreterNew
                .CreateReleaseConfiguration()
                .WithSource(Script)
                .WithStdin(Stdin)
                .WithMemorySize(64)
                .WithOverflowMode(OverflowMode.UshortWithNoOverflow)
                .TryRun();

            result.Value!.MachineState.Dispose();

            return result.Value.Stdout;
        }
    }
}
