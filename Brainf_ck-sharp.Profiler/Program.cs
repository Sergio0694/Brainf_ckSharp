using BenchmarkDotNet.Running;

namespace Brainf_ck_sharp.Profiler
{
    class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<Brainf_ckBenchmark>();
        }
    }
}
