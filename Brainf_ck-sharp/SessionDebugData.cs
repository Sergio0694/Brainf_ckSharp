using System;
using System.Collections.Generic;
using System.Text;
using Brainf_ck_sharp.Helpers;
using JetBrains.Annotations;

namespace Brainf_ck_sharp
{
    internal sealed class SessionDebugData
    {
        public IReadOnlyList<Brainf_ckBinaryItem> Source { get; }

        public Queue<char> Stdin { get; }

        public StringBuilder Stdout { get; }

        public IReadOnlyList<uint> Breakpoints { get; }

        public int? Threshold { get; }

        public SessionDebugData([NotNull] IReadOnlyList<Brainf_ckBinaryItem> source, [NotNull] Queue<char> stdin, 
            [NotNull] StringBuilder stdout, int? threshold, IReadOnlyList<uint> breakpoints)
        {
            Source = source;
            Stdin = stdin;
            Stdout = stdout;
            Threshold = threshold;
            Breakpoints = breakpoints;
        }

        public SessionDebugData Clone()
        {
            return new SessionDebugData(Source, new Queue<char>(Stdin), new StringBuilder(Stdout.ToString()), Threshold, Breakpoints);
        }
    }
}
