using BenchmarkDotNet.Running;
using Brainf_ckSharp.Profiler;

BenchmarkSwitcher.FromTypes(new[] { typeof(Brainf_ckBenchmark_Short), typeof(Brainf_ckBenchmark_Long) }).RunAllJoined();
