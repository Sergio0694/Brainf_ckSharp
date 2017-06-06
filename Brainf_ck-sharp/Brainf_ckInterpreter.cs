using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Brainf_ck_sharp.Helpers;
using Brainf_ck_sharp.ReturnTypes;
using JetBrains.Annotations;

namespace Brainf_ck_sharp
{
    /// <summary>
    /// Classe statica che interpreta ed esegue il debug in modo asincrono dei codici sorgenti in Brainfuck
    /// </summary>
    public static class Brainf_ckInterpreter
    {
        /// <summary>
        /// Gets the collection of valid Brainf_ck operators
        /// </summary>
        [NotNull]
        public static readonly IReadOnlyCollection<char> Operators = new[] { '+', '-', '>', '<', '.', ',', '[', ']' };

        #region Public APIs

        /// <summary>
        /// Executes the given script and returns the result
        /// </summary>
        /// <param name="source">The source code with the script to execute</param>
        /// <param name="arguments">The arguments for the script</param>
        /// <param name="size">The size of the memory to use to run the script</param>
        /// <param name="threshold">The optional time threshold to run the script</param>
        [PublicAPI]
        [Pure, NotNull]
        public static InterpreterResult Run([NotNull] String source, [NotNull] String arguments,
            int size = 64, int? threshold = null)
        {
            return TryRun(source, arguments, new TouringMachineState(size), threshold);
        }

        /// <summary>
        /// Executes the given script and returns the result
        /// </summary>
        /// <param name="source">The source code with the script to execute</param>
        /// <param name="arguments">The arguments for the script</param>
        /// <param name="state">The initial memory state to run the script</param>
        /// <param name="threshold">The optional time threshold to run the script</param>
        [PublicAPI]
        [Pure, NotNull]
        public static InterpreterResult Run([NotNull] String source, [NotNull] String arguments,
            [NotNull] TouringMachineState state, int? threshold = null)
        {
            return TryRun(source, arguments, state.Clone(), threshold);
        }

        /// <summary>
        /// Initializes an execution session with the input source code
        /// </summary>
        /// <param name="source">The source code to use to initialize the session. A breakpoint will be added at the start of each code chunk after the first one</param>
        /// <param name="arguments">The optional arguments for the script</param>
        /// <param name="size">The size of the memory state to use for the session</param>
        /// <param name="threshold">An optional time threshold for the execution of the whole session</param>
        [PublicAPI]
        [Pure, NotNull]
        public static InterpreterExecutionSession InitializeSession([NotNull] IReadOnlyList<String> source, [NotNull] String arguments,
            int size = 64, int? threshold = null)
        {
            TouringMachineState state = new TouringMachineState(size);
            IReadOnlyList<IReadOnlyList<char>> chunks = source.Select(FindExecutableCode).ToArray();
            if (chunks.Count == 0 || chunks.Any(group => group.Count == 0))
            {
                return new InterpreterExecutionSession(
                    new InterpreterResult(InterpreterExitCode.Failure | InterpreterExitCode.NoCodeInterpreted, state,
                    TimeSpan.Zero, String.Empty, String.Empty, null, null), null);
            }

            List<Brainf_ckBinaryItem> executable = new List<Brainf_ckBinaryItem>();
            List<uint> breakpoints = new List<uint>();
            uint offset = 0;
            for (int i = 0; i < chunks.Count; i++)
            {
                if (i > 0) breakpoints.Add(offset);
                executable.AddRange(chunks[i].Select(c => new Brainf_ckBinaryItem(offset++, c)));
            }

            // Check the code syntax
            if (!CheckSourceSyntax(executable))
            {
                return new InterpreterExecutionSession(
                    new InterpreterResult(InterpreterExitCode.Failure | InterpreterExitCode.MismatchedParentheses, state,
                    TimeSpan.Zero, String.Empty, executable.Select(op => op.Operator).AggregateToString(), null, null), null);
            }

            // Prepare the input and output arguments
            Queue<char> input = arguments.Length > 0 ? new Queue<char>(arguments) : new Queue<char>();
            StringBuilder output = new StringBuilder();

            InterpreterResult result = TryRun(executable, input, output, state, threshold, TimeSpan.Zero, null, breakpoints.Count > 0 ? breakpoints : null);
            return new InterpreterExecutionSession(result, new SessionDebugData(executable, input, output, threshold, breakpoints));
        }

        /// <summary>
        /// Checks whether or not the syntax in the input source code is valid
        /// </summary>
        /// <param name="source">The source code to analyze</param>
        /// <returns>A bool value that indicates whether or not the source code is valid, and the position of the first syntax error, if there is at least one</returns>
        [PublicAPI]
        [Pure]
        public static (bool Valid, int ErrorPosition) CheckSourceSyntax([NotNull] String source)
        {
            // Iterate over all the characters in the source
            int height = 0;
            for (int i = 0; i < source.Length; i++)
            {
                // Check the parentheses
                if (source[i] == '[') height++;
                else if (source[i] == ']')
                {
                    if (height == 0) return (false, i);
                    height--;
                }
            }

            // Edge case or valid return
            return height == 0 ? (true, 0) : (false, source.Length - 1);
        }

        #endregion

        #region Interpreter implementation

        /// <summary>
        /// Executes the input script
        /// </summary>
        /// <param name="source">The source code with the script to execute</param>
        /// <param name="arguments">The arguments for the script</param>
        /// <param name="state">The initial memory state to run the script</param>
        /// <param name="threshold">The optional time threshold to run the script</param>
        [Pure, NotNull]
        private static InterpreterResult TryRun([NotNull] String source, [NotNull] String arguments,
            [NotNull] TouringMachineState state, int? threshold)
        {
            // Get the operators to execute and check if the source is empty
            IReadOnlyList<Brainf_ckBinaryItem> executable = FindExecutableCode(source).Select((c, i) => new Brainf_ckBinaryItem((uint)i, c)).ToArray();
            if (executable.Count == 0)
            {
                return new InterpreterResult(InterpreterExitCode.Failure | InterpreterExitCode.NoCodeInterpreted, state,
                    TimeSpan.Zero, String.Empty, String.Empty, null, null);
            }

            // Check the code syntax
            if (!CheckSourceSyntax(executable))
            {
                return new InterpreterResult(InterpreterExitCode.Failure | InterpreterExitCode.MismatchedParentheses, state, TimeSpan.Zero, String.Empty,
                    executable.Select(op => op.Operator).AggregateToString(), null, null);
            }

            // Prepare the input and output arguments
            Queue<char> input = arguments.Length > 0 ? new Queue<char>(arguments) : new Queue<char>();
            StringBuilder output = new StringBuilder();

            // Execute the code
            return TryRun(executable, input, output, state, threshold, TimeSpan.Zero, null, null);
        }

        /// <summary>
        /// Executes a script or continues the execution of a script
        /// </summary>
        /// <param name="executable">The source code of the scrpt to execute</param>
        /// <param name="input">The stdin buffer</param>
        /// <param name="output">The stdout buffer</param>
        /// <param name="state">The curret memory state to run or continue the script</param>
        /// <param name="threshold">The optional time threshold for the execution of the script</param>
        /// <param name="elapsed">The elapsed time since the beginning of the script (if there's an execution session in progress)</param>
        /// <param name="jump">The optional position of a previously reached breakpoint to use to resume the execution</param>
        /// <param name="breakpoints">The optional list of breakpoints in the input source code</param>
        [Pure, NotNull]
        private static InterpreterResult TryRun([NotNull] IReadOnlyList<Brainf_ckBinaryItem> executable, [NotNull] Queue<char> input, [NotNull] StringBuilder output,
            [NotNull] TouringMachineState state, int? threshold, TimeSpan elapsed, uint? jump, IReadOnlyList<uint> breakpoints)
        {
            // Preliminary tests
            if (executable.Count == 0) throw new ArgumentException("The source code can't be empty");
            if (threshold <= 0) throw new ArgumentOutOfRangeException("The threshold must be a positive value");
            if (jump < 0) throw new ArgumentOutOfRangeException("The target breakpoint position must be a positive number");
            if (jump.HasValue && (jump > executable.Count - 1 || breakpoints?.Contains(jump.Value) == false))
            {
                throw new ArgumentOutOfRangeException("The target breakpoint position isn't valid");
            }
            if (breakpoints?.Count == 0) throw new ArgumentException("The breakpoints list can't be empty");

            // Start the stopwatch to monitor the execution
            Stopwatch timer = new Stopwatch();
            timer.Start();

            // Internal recursive function that interpretes the code
            (InterpreterExitCode, IEnumerable<IEnumerable<char>>, uint, bool) TryRunCore(IReadOnlyList<Brainf_ckBinaryItem> operators, uint depth, bool reached)
            {
                // Outer do-while that repeats the code if there's a loop
                bool repeat = false;
                do
                {
                    // Check the current elapsed time
                    if (threshold.HasValue && timer.ElapsedMilliseconds > threshold.Value + elapsed.TotalMilliseconds)
                    {
                        return (InterpreterExitCode.Failure | InterpreterExitCode.ThresholdExceeded, new[] { new char[0] }, depth, false);
                    }

                    // Iterate over all the commands
                    int skip = 0;
                    for (int i = 0; i < operators.Count; i++)
                    {
                        // Skip the current character if inside a loop that points to a 0 cell
                        if (skip > 0)
                        {
                            skip--;
                            continue;
                        }

                        // Check the breakpoints if the current call isn't expected to go straight to the end of the script
                        if (jump == null && breakpoints?.Contains(operators[i].Offset) == true || // First breakpoint in the code
                            jump != null && breakpoints?.Contains(operators[i].Offset) == true && reached) // New breakpoint after restoring the execution
                        {
                            // First breakpoint in the current session
                            return (InterpreterExitCode.Success |
                                    InterpreterExitCode.BreakpointReached, new[] { operators.Select(op => op.Operator).Take(i + 1) }, depth + (uint)i, true);
                        }

                        // Keep track when the target breakpoint is reached and the previous execution is restored
                        if (jump == operators[i].Offset && !reached) reached = true;

                        // Parse the current operator
                        switch (operators[i].Operator)
                        {
                            // ptr++
                            case '>':
                                if (jump != null && !reached) continue;
                                if (state.CanMoveNext) state.MoveNext();
                                else return (InterpreterExitCode.Failure |
                                             InterpreterExitCode.ExceptionThrown |
                                             InterpreterExitCode.UpperBoundExceeded, new[] { operators.Select(op => op.Operator).Take(i + 1) }, depth + (uint)i, reached);
                                break;

                            // ptr--
                            case '<':
                                if (jump != null && !reached) continue;
                                if (state.CanMoveBack) state.MoveBack();
                                else return (InterpreterExitCode.Failure |
                                             InterpreterExitCode.ExceptionThrown |
                                             InterpreterExitCode.LowerBoundExceeded, new[] { operators.Select(op => op.Operator).Take(i + 1) }, depth + (uint)i, reached);
                                break;

                            // *ptr++
                            case '+':
                                if (jump != null && !reached) continue;
                                if (state.CanIncrement) state.Plus();
                                else return (InterpreterExitCode.Failure |
                                             InterpreterExitCode.ExceptionThrown |
                                             InterpreterExitCode.MaxValueExceeded, new[] { operators.Select(op => op.Operator).Take(i + 1) }, depth + (uint)i, reached);
                                break;

                            // *ptr--
                            case '-':
                                if (jump != null && !reached) continue;
                                if (state.CanDecrement) state.Minus();
                                else return (InterpreterExitCode.Failure |
                                             InterpreterExitCode.ExceptionThrown |
                                             InterpreterExitCode.NegativeValue, new[] { operators.Select(op => op.Operator).Take(i + 1) }, depth + (uint)i, reached);
                                break;

                            // while (*ptr) {
                            case '[':

                                // Extract the loop code and append the final ] character
                                IReadOnlyList<Brainf_ckBinaryItem> loop = ExtractInnerLoop(operators, i).ToArray();
                                skip = loop.Count; // Don't count the last ] character in the loop body

                                // Execute the loop if the current value is greater than 0
                                if (state.Current > 0 || jump != null && !reached)
                                {
                                    (InterpreterExitCode code, IEnumerable<IEnumerable<char>> loopFrames, uint target, bool inner) = TryRunCore(loop, depth + (uint)i + 1, reached);
                                    reached |= inner;
                                    if ((code & InterpreterExitCode.Success) == 0 ||
                                        (code & InterpreterExitCode.BreakpointReached) == InterpreterExitCode.BreakpointReached)
                                    {
                                        return (code, loopFrames.Concat(new[] { operators.Select(op => op.Operator).Take(i + 1) }), target, reached);
                                    }
                                }
                                break;

                            // }
                            case ']':
                                if (state.Current == 0 || jump != null && !reached)
                                {
                                    // Loop end
                                    return (InterpreterExitCode.Success, null, depth + (uint)i, reached);
                                }
                                else
                                {
                                    // Jump back and execute the loop body again
                                    repeat = true;
                                    continue;
                                }

                            // putch(*ptr)
                            case '.':
                                if (jump != null && !reached) continue;
                                output.Append(Convert.ToChar(state.Current));
                                break;

                            // *ptr = getch()
                            case ',':
                                if (jump != null && !reached) continue;
                                if (input.Count > 0) state.Input(input.Dequeue());
                                else return (InterpreterExitCode.Failure |
                                             InterpreterExitCode.ExceptionThrown |
                                             InterpreterExitCode.StrinBufferExhausted, new[] { operators.Select(op => op.Operator).Take(i + 1) }, depth + (uint)i, reached);
                                break;
                        }
                    }
                } while (repeat);
                return (InterpreterExitCode.Success, null, depth + (uint)operators.Count, reached);
            }

            // Execute the code and stop the timer
            (InterpreterExitCode result, IEnumerable<IEnumerable<char>> frames, uint position, _) = TryRunCore(executable, 0, false);
            timer.Stop();

            // Reconstruct the stack trace that generated the error
            IReadOnlyList<String> stackTrace = frames == null
                ? null
                : (from frame in frames
                   select frame.AggregateToString()).ToArray();

            // Return the interpreter result with all the necessary info
            String text = output.ToString();
            return new InterpreterResult(
                result | (text.Length > 0 ? InterpreterExitCode.TextOutput : InterpreterExitCode.NoOutput),
                state, timer.Elapsed.Add(elapsed), text, executable.Select(op => op.Operator).AggregateToString(), stackTrace,
                (result & InterpreterExitCode.BreakpointReached) == InterpreterExitCode.BreakpointReached ? (uint?)position : null);
        }

        /// <summary>
        /// Extracts the body of a loop from the given source code partition (including the last ] operator in the loop body)
        /// </summary>
        /// <param name="source">The source code to use to extract the loop body</param>
        /// <param name="index">The index of the [ operator at the beginning of the loop to extract</param>
        [Pure, NotNull]
        private static IEnumerable<Brainf_ckBinaryItem> ExtractInnerLoop([NotNull] IReadOnlyList<Brainf_ckBinaryItem> source, int index)
        {
            // Initial checks
            if (source.Count == 0) throw new ArgumentException("The source code is empty");
            if (index < 0 || index > source.Count - 2) throw new ArgumentOutOfRangeException("The target index is invalid");
            if (source[index].Operator != '[') throw new ArgumentException("The target index doesn't point to the beginning of a loop");

            // Iterate from the first character of the loop to the final ] operator
            int height = 0;
            for (int i = index + 1; i < source.Count; i++)
            {
                if (source[i].Operator == '[') height++;
                else if (source[i].Operator == ']')
                {
                    if (height == 0) return source.Skip(index + 1).Take(i - index);
                    height--;
                }
            }
            throw new ArgumentException("The source code doesn't contain a well formatted nested loop at the given position");
        }

        #endregion

        #region Tools

        /// <summary>
        /// Continues the input execution session to its next step
        /// </summary>
        /// <param name="session">The session to continue</param>
        [Pure, NotNull]
        internal static InterpreterExecutionSession ContinueSession([NotNull] InterpreterExecutionSession session)
        {
            InterpreterResult step = TryRun(session.DebugData.Source, session.DebugData.Stdin, session.DebugData.Stdout,
                session.CurrentResult.MachineState, session.DebugData.Threshold, session.CurrentResult.ElapsedTime,
                session.CurrentResult.BreakpointPosition, session.DebugData.Breakpoints);
            return new InterpreterExecutionSession(step, session.DebugData);
        }

        /// <summary>
        /// Resumes the execution of the input session and runs it to the end
        /// </summary>
        /// <param name="session">The session to run</param>
        [Pure, NotNull]
        internal static InterpreterExecutionSession RunSessionToCompletion([NotNull] InterpreterExecutionSession session)
        {
            InterpreterResult step = TryRun(session.DebugData.Source, session.DebugData.Stdin, session.DebugData.Stdout,
                session.CurrentResult.MachineState, session.DebugData.Threshold, session.CurrentResult.ElapsedTime,
                session.CurrentResult.BreakpointPosition, null);
            return new InterpreterExecutionSession(step, session.DebugData);
        }

        /// <summary>
        /// Extracts the valid operators from a raw source code
        /// </summary>
        /// <param name="source">The input source code</param>
        [NotNull, LinqTunnel]
        private static IReadOnlyList<char> FindExecutableCode([NotNull] String source) =>
        (from c in source
            where Operators.Contains(c)
            select c).ToArray();

        /// <summary>
        /// Checks whether or not the syntax in the input operators is valid
        /// </summary>
        /// <param name="operators">The operators sequence</param>
        [Pure]
        private static bool CheckSourceSyntax([NotNull] IEnumerable<Brainf_ckBinaryItem> operators)
        {
            // Iterate over all the characters in the source
            int height = 0;
            foreach (char c in operators.Select(op => op.Operator))
            {
                // Check the parentheses
                if (c == '[') height++;
                else if (c == ']')
                {
                    if (height == 0) return false;
                    height--;
                }
            }
            return height == 0;
        }

        #endregion
    }
}
