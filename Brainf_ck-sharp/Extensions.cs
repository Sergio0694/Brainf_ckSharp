using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Brainf_ck_sharp
{
    public static class Extensions
    {
        public static String AggregateToString([NotNull] this IEnumerable<char> source)
        {
            return source.Aggregate(new StringBuilder(), (b, c) =>
            {
                b.Append(c);
                return b;
            }).ToString();
        }
    }
}
