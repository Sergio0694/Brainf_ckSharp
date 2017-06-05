using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace Brainf_ck_sharp
{
    internal sealed class SessionDebugData
    {
        public IReadOnlyList<char> Source { get; }

        public Queue<char> Stdin { get; }

        public StringBuilder Stdout { get; }

        public int? Threshold { get; }

        public SessionDebugData([NotNull] IReadOnlyList<char> source, [NotNull] Queue<char> stdin, [NotNull] StringBuilder stdout, int? threshold)
        {
            Source = source;
            Stdin = stdin;
            Stdout = stdout;
            Threshold = threshold;
        }

        public SessionDebugData Clone()
        {
            return new SessionDebugData(Source, new Queue<char>(Stdin), new StringBuilder(Stdout.ToString()), Threshold);
        }
    }
}
