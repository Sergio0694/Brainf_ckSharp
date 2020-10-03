using BenchmarkDotNet.Running;

namespace Brainf_ckSharp.Profiler
{
    class Program
    {
        static void Main()
        {
            BenchmarkSwitcher.FromTypes(new[] { typeof(Brainf_ckBenchmark_Short), typeof(Brainf_ckBenchmark_Long) }).RunAllJoined();
        }
    }
}
