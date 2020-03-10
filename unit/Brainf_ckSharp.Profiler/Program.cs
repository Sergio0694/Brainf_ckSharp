using BenchmarkDotNet.Running;

namespace Brainf_ckSharp.Profiler
{
    class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<Brainf_ckBenchmark>();
        }
    }
}
