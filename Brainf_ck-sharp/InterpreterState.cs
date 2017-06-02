using System;
using System.Collections.Generic;
using System.Diagnostics;
using Branf_ck_sharp;
using JetBrains.Annotations;

namespace Brainf_ck_sharp
{
    internal sealed class InterpreterState
    {
        /// <summary>
        /// Creates a new instance with the input source code and a new running timer
        /// </summary>
        /// <param name="source">The source code for the new state</param>
        public InterpreterState([NotNull] IReadOnlyList<char> operators)
        {
            Timer = new Stopwatch();
            Timer.Start();
            Operators = operators;
        }

        /// <summary>
        /// Creates a new instance with the input source code and timer
        /// </summary>
        /// <param name="source">The source code for the new state</param>
        /// <param name="timer">The timer to assign to the new state</param>
        public InterpreterState([NotNull] IReadOnlyList<char> operators, [NotNull] Stopwatch timer)
        {
            Timer = timer;
            Operators = operators;
        }

        /// <summary>
        /// Gets the source code associated with the current interpreter state
        /// </summary>
        [NotNull]
        public IReadOnlyList<char> Operators { get; }

        public Stopwatch Timer { get; }
}
}