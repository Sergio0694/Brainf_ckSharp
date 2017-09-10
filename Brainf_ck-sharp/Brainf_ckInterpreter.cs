using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Brainf_ck_sharp.Enums;
using Brainf_ck_sharp.Helpers;
using Brainf_ck_sharp.MemoryState;
using Brainf_ck_sharp.ReturnTypes;
using JetBrains.Annotations;

namespace Brainf_ck_sharp
{
    /// <summary>
    /// A simple class that handles all the Brainf_ck code and interprets it
    /// </summary>
    public static class Brainf_ckInterpreter
    {
        /// <summary>
        /// Gets the collection of valid Brainf_ck operators
        /// </summary>
        [NotNull]
        public static IReadOnlyCollection<char> Operators { get; } = new HashSet<char>(new[] { '+', '-', '>', '<', '.', ',', '[', ']', '(', ')', ':' });

        /// <summary>
        /// Gets the default size of the available memory
        /// </summary>
        public const int DefaultMemorySize = 64;

        /// <summary>
        /// Gets the maximum allowed size for the output buffer
        /// </summary>
        public const int StdoutBufferSizeLimit = 1024;

        /// <summary>
        /// Gets the maximum number of functions that can be defined in a single script
        /// </summary>
        public const int FunctionDefinitionsLimit = 128;

        /// <summary>
        /// Gets the maximum number of recursive calls that can be performed by a script
        /// </summary>
        public const int MaximumStackSize = 1024;

        #region Public APIs

        /// <summary>
        /// Executes the given script and returns the result
        /// </summary>
        /// <param name="source">The source code with the script to execute</param>
        /// <param name="arguments">The arguments for the script</param>
        /// <param name="mode">Indicates the desired overflow mode for the script to run</param>
        /// <param name="size">The size of the memory to use to run the script</param>
        /// <param name="threshold">The optional time threshold to run the script</param>
        [PublicAPI]
        [Pure, NotNull]
        public static InterpreterResult Run(
            [NotNull] String source, [NotNull] String arguments, 
            OverflowMode mode = OverflowMode.ShortNoOverflow, int size = DefaultMemorySize, int? threshold = null)
        {
            return TryRun(source, arguments, new TouringMachineState(size), mode, threshold);
        }

        /// <summary>
        /// Executes the given script and returns the result
        /// </summary>
        /// <param name="source">The source code with the script to execute</param>
        /// <param name="arguments">The arguments for the script</param>
        /// <param name="state">The initial memory state to run the script</param>
        /// <param name="mode">Indicates the desired overflow mode for the script to run</param>
        /// <param name="threshold">The optional time threshold to run the script</param>
        [PublicAPI]
        [Pure, NotNull]
        public static InterpreterResult Run(
            [NotNull] String source, [NotNull] String arguments,
            [NotNull] IReadonlyTouringMachineState state, OverflowMode mode = OverflowMode.ShortNoOverflow, int? threshold = null)
        {
            if (state is TouringMachineState touring)
            {
                return TryRun(source, arguments, touring.Clone(), mode, threshold);
            }
            throw new ArgumentException();
        }

        /// <summary>
        /// Initializes an execution session with the input source code
        /// </summary>
        /// <param name="source">The source code to use to initialize the session. A breakpoint will be added at the start of each code chunk after the first one</param>
        /// <param name="arguments">The optional arguments for the script</param>
        /// <param name="mode">Indicates the desired overflow mode for the script to run</param>
        /// <param name="size">The size of the memory state to use for the session</param>
        /// <param name="threshold">An optional time threshold for the execution of the whole session</param>
        [PublicAPI]
        [Pure, NotNull]
        public static InterpreterExecutionSession InitializeSession([NotNull] IReadOnlyList<String> source, [NotNull] String arguments,
            OverflowMode mode = OverflowMode.ShortNoOverflow, int size = DefaultMemorySize, int? threshold = null)
        {
            // Failure function
            TouringMachineState state = new TouringMachineState(size);
            IEnumerable<InterpreterResult> GenerateFailure(InterpreterExitCode reason, String code)
            {
                return new[] { new InterpreterResult(InterpreterExitCode.Failure | reason, state, code) };
            }

            // Find the executable code
            IReadOnlyList<IReadOnlyList<char>> chunks = source.Select(chunk => FindExecutableCode(chunk).ToArray()).ToArray();
            if (chunks.Count == 0 || chunks.Any(group => group.Count == 0))
            {
                return new InterpreterExecutionSession(GenerateFailure(InterpreterExitCode.NoCodeInterpreted, String.Empty).GetEnumerator(), null);
            }

            // Reconstruct the binary to run
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
                    GenerateFailure(InterpreterExitCode.MismatchedParentheses, executable.Select(op => op.Operator).AggregateToString()).GetEnumerator(), null);
            }

            // Execute the code
            CancellationTokenSource cts = new CancellationTokenSource();
            IEnumerable<InterpreterResult> results = TryRun(executable, arguments, state, mode, threshold, breakpoints, cts.Token);
            IEnumerator<InterpreterResult> enumerator = results.GetEnumerator();
            enumerator.MoveNext();
            return new InterpreterExecutionSession(enumerator, cts);
        }

        /// <summary>
        /// Checks whether or not the syntax in the input source code is valid
        /// </summary>
        /// <param name="source">The source code to analyze</param>
        /// <returns>A wrapper class that indicates whether or not the source code is valid, and the position of the first syntax error, if there is at least one</returns>
        [PublicAPI]
        [Pure]
        public static SyntaxValidationResult CheckSourceSyntax([NotNull] String source)
        {
            // Check function brackets parity
            bool open = false;
            for (int i = 0; i < source.Length; i++)
            {
                switch (source[i])
                {
                    case '(':
                        if (open) return new SyntaxValidationResult(false, i);
                        open = true;
                        break;
                    case ')':
                        if (open) open = false;
                        else return new SyntaxValidationResult(false, i);
                        break;
                }
            }

            // Prepare the inner check function
            SyntaxValidationResult CheckSyntaxCore(String code)
            {
                // Iterate over all the characters in the source
                int height = 0, error = 0;
                for (int i = 0; i < code.Length; i++)
                {
                    // Check the parentheses
                    if (code[i] == '[')
                    {
                        if (height == 0) error = i;
                        height++;
                    }
                    else if (code[i] == ']')
                    {
                        if (height == 0) return new SyntaxValidationResult(false, i);
                        height--;
                    }
                }

                // Edge case or valid return
                return height == 0 ? new SyntaxValidationResult(true, 0) : new SyntaxValidationResult(false, error);
            }

            // Check the syntax in every function body
            Match emptyCheck = Regex.Match(source, "[(][)]");
            if (emptyCheck.Success) return new SyntaxValidationResult(false, emptyCheck.Index + 1);
            MatchCollection matches = Regex.Matches(source, "[(](.+?)[)]");
            foreach (Match match in matches)
            {
                Group group = match.Groups[1];
                if (!group.Value.Any(Operators.Contains)) return new SyntaxValidationResult(false, group.Index + group.Length);
                SyntaxValidationResult result = CheckSyntaxCore(group.Value);
                if (!result.Valid) return new SyntaxValidationResult(false, result.ErrorPosition + group.Index);
            }

            // Finally check the whole code
            return CheckSyntaxCore(source);
        }
        
        /// <summary>
        /// Checks whether or not the given source code contains at least one executable operator
        /// </summary>
        /// <param name="source">The source code to analyze</param>
        [PublicAPI]
        [Pure]
        public static bool FindOperators([NotNull] String source) => FindExecutableCode(source).Any();

        #endregion

        #region Interpreter implementation

        /// <summary>
        /// Executes the input script
        /// </summary>
        /// <param name="source">The source code with the script to execute</param>
        /// <param name="arguments">The arguments for the script</param>
        /// <param name="state">The initial memory state to run the script</param>
        /// <param name="mode">Indicates the desired overflow mode for the script to run</param>
        /// <param name="threshold">The optional time threshold to run the script</param>
        [Pure, NotNull]
        private static InterpreterResult TryRun([NotNull] String source, [NotNull] String arguments,
            [NotNull] TouringMachineState state, OverflowMode mode, int? threshold)
        {
            // Get the operators to execute and check if the source is empty
            IReadOnlyList<Brainf_ckBinaryItem> executable = FindExecutableCode(source).Select((c, i) => new Brainf_ckBinaryItem((uint)i, c)).ToArray();
            if (executable.Count == 0)
            {
                return new InterpreterResult(InterpreterExitCode.Failure | InterpreterExitCode.NoCodeInterpreted, state, String.Empty);
            }

            // Check the code syntax
            if (!CheckSourceSyntax(executable))
            {
                return new InterpreterResult(InterpreterExitCode.Failure | InterpreterExitCode.MismatchedParentheses, state,
                    executable.Select(op => op.Operator).AggregateToString());
            }

            // Execute the code
            return TryRun(executable, arguments, state, mode, threshold, new uint[0], CancellationToken.None).Last();
        }

        /// <summary>
        /// Executes a script or continues the execution of a script
        /// </summary>
        /// <param name="executable">The source code of the scrpt to execute</param>
        /// <param name="arguments">The stdin buffer</param>
        /// <param name="state">The curret memory state to run or continue the script</param>
        /// <param name="mode">Indicates the desired overflow mode for the script to run</param>
        /// <param name="threshold">The optional time threshold for the execution of the script</param>
        /// <param name="breakpoints">The list of breakpoints in the input source code</param>
        /// <param name="token">A <see cref="CancellationToken"/> used to signal when to bypass new breakpoints reached by the script</param>
        [Pure, NotNull, ItemNotNull]
        private static IEnumerable<InterpreterResult> TryRun(
            [NotNull] IReadOnlyList<Brainf_ckBinaryItem> executable, [NotNull] String arguments,
            [NotNull] TouringMachineState state, OverflowMode mode, int? threshold, [NotNull] IReadOnlyList<uint> breakpoints, CancellationToken token)
        {
            // Preliminary tests
            if (executable.Count == 0) throw new ArgumentException("The source code can't be empty");
            if (threshold <= 0) throw new ArgumentOutOfRangeException("The threshold must be a positive value");

            // ReSharper disable once RedundantAssignment - Local parameters
            uint operations = 0; // This actually needs to be initialized before calling the function
            Stopwatch timer = new Stopwatch();
            Queue<char> input = arguments.Length > 0 ? new Queue<char>(arguments) : new Queue<char>();
            StringBuilder output = new StringBuilder();
            Dictionary<uint, IReadOnlyList<Brainf_ckBinaryItem>> functions = new Dictionary<uint, IReadOnlyList<Brainf_ckBinaryItem>>();
            String code = executable.Select(op => op.Operator).AggregateToString(); // Original source code

            // Internal recursive function that interpretes the code
            IEnumerable<InterpreterWorkingData> TryRunCore(IReadOnlyList<Brainf_ckBinaryItem> operators, uint position, ushort depth)
            {
                // Verify that each call has at least an operator to execute
                if (operators.Count == 0)
                {
                    yield return new InterpreterWorkingData(InterpreterExitCode.Failure | InterpreterExitCode.InternalException,
                                                            new[] { new Brainf_ckBinaryItem[0] }, 0, operations);
                    yield break;
                }

                // Outer do-while that repeats the code if there's a loop
                bool repeat = false;
                do
                {
                    // Check the current elapsed time
                    if (threshold.HasValue && timer.ElapsedMilliseconds > threshold.Value)
                    {
                        yield return new InterpreterWorkingData(InterpreterExitCode.Failure | InterpreterExitCode.ThresholdExceeded, 
                                                                new[] { new Brainf_ckBinaryItem[0] }, position, operations);
                    }

                    // Iterate over all the commands
                    int skip = 0;
                    for (int i = 0; i < operators.Count; i++)
                    {
                        // Skip the current character if inside a loop that points to a 0 cell
                        if (skip > 0)
                        {
                            i += skip - 1;
                            skip = 0;
                            continue;
                        }

                        // Check if a breakpoint has been reached
                        if (breakpoints.Contains(operators[i].Offset) && !token.IsCancellationRequested)
                        {
                            // First breakpoint in the current session
                            yield return new InterpreterWorkingData(InterpreterExitCode.Success | InterpreterExitCode.BreakpointReached,
                                                                    new[] { operators.Take(i + 1) }, position + (uint)i, operations);
                        }

                        // Parse the current operator
                        switch (operators[i].Operator)
                        {
                            // ptr++
                            case '>':
                                if (!state.CanMoveNext)
                                {
                                    yield return new InterpreterWorkingData(InterpreterExitCode.Failure |
                                                                            InterpreterExitCode.ExceptionThrown |
                                                                            InterpreterExitCode.UpperBoundExceeded,
                                                                            new[] { operators.Take(i + 1) }, position + (uint)i, operations);
                                    yield break;
                                }
                                state.MoveNext();
                                operations++;
                                break;

                            // ptr--
                            case '<':
                                if (!state.CanMoveBack)
                                {
                                    yield return new InterpreterWorkingData(InterpreterExitCode.Failure |
                                                                            InterpreterExitCode.ExceptionThrown |
                                                                            InterpreterExitCode.LowerBoundExceeded,
                                                                            new[] { operators.Take(i + 1) }, position + (uint)i, operations);
                                    yield break;
                                }
                                state.MoveBack();
                                operations++;
                                break;

                            // *ptr++
                            case '+':
                                if (mode == OverflowMode.ByteOverflow && state.IsAtByteMax) state.Input((char)0);
                                else if (state.CanIncrement) state.Plus();
                                else
                                {
                                    yield return new InterpreterWorkingData(InterpreterExitCode.Failure |
                                                                            InterpreterExitCode.ExceptionThrown |
                                                                            InterpreterExitCode.MaxValueExceeded,
                                                                            new[] { operators.Take(i + 1) }, position + (uint)i, operations);
                                    yield break;
                                }
                                operations++;
                                break;

                            // *ptr--
                            case '-':
                                if (state.CanDecrement) state.Minus();
                                else if (mode == OverflowMode.ByteOverflow) state.Input((char)byte.MaxValue);
                                else
                                {
                                    yield return new InterpreterWorkingData(InterpreterExitCode.Failure |
                                                                            InterpreterExitCode.ExceptionThrown |
                                                                            InterpreterExitCode.NegativeValue,
                                                                            new[] { operators.Take(i + 1) }, position + (uint)i, operations);
                                    yield break;
                                }
                                operations++;
                                break;

                            // while (*ptr) {
                            case '[':

                                // Edge case - memory reset loop [-]
                                if (state.Current.Value > 0 &&                                              // Loop enabled
                                    i + 2 < operators.Count &&                                              // The loop is contained in the current executable
                                    operators[i + 1].Operator == '-' && operators[i + 2].Operator == ']')   // The loop body only contains a - operator
                                {
                                    // Check the position of the breakpoints
                                    uint offset = operators[i].Offset;
                                    if (breakpoints?.Any(b => b > offset && b <= offset + 2) != true)
                                    {
                                        operations += state.Current.Value * 2 + 1;
                                        state.ResetCell();
                                        skip = 2;
                                        break;
                                    }
                                }

                                // Extract the loop code and append the final ] character
                                IReadOnlyList<Brainf_ckBinaryItem> loop = ExtractInnerLoop(operators, i).ToArray();
                                skip = loop.Count;

                                // Execute the loop if the current value is greater than 0
                                operations++;
                                if (state.Current.Value > 0)
                                {
                                    IEnumerable<InterpreterWorkingData> inner = TryRunCore(loop, position + (uint)i + 1, (ushort)(depth + 1));
                                    foreach (InterpreterWorkingData result in inner)
                                    {
                                        if (!result.ExitCode.HasFlag(InterpreterExitCode.Success))
                                        {
                                            yield return new InterpreterWorkingData(result.ExitCode,
                                                result.StackFrames.Concat(new[] { operators.Take(i + 1) }), result.Position, result.TotalOperations);
                                            yield break;
                                        }
                                        if (result.ExitCode.HasFlag(InterpreterExitCode.BreakpointReached)) yield return result;
                                    }
                                }
                                break;

                            // }
                            case ']':
                                operations++;
                                if (state.Current.Value == 0)
                                {
                                    // Loop end
                                    yield return new InterpreterWorkingData(InterpreterExitCode.Success, null, position + (uint)i, operations);
                                    yield break;
                                }
                                else
                                {
                                    // Jump back and execute the loop body again
                                    repeat = true;
                                    continue;
                                }

                            // putch(*ptr)
                            case '.':
                                if (output.Length >= StdoutBufferSizeLimit)
                                {
                                    yield return new InterpreterWorkingData(InterpreterExitCode.Failure |
                                                                            InterpreterExitCode.ExceptionThrown |
                                                                            InterpreterExitCode.StdoutBufferLimitExceeded,
                                                                            new[] { operators.Take(i + 1) }, position + (uint)i, operations);
                                    yield break;
                                }
                                output.Append(state.Current.Character);
                                operations++;
                                break;

                            // *ptr = getch()
                            case ',':
                                if (input.Count > 0)
                                {
                                    // Read the new character
                                    char c = input.Dequeue();
                                    if (mode == OverflowMode.ShortNoOverflow)
                                    {
                                        // Insert it if possible when the overflow is disabled
                                        if (c > short.MaxValue)
                                        {
                                            yield return new InterpreterWorkingData(InterpreterExitCode.Failure |
                                                                                    InterpreterExitCode.ExceptionThrown |
                                                                                    InterpreterExitCode.MaxValueExceeded,
                                                                                    new[] { operators.Take(i + 1) }, position + (uint)i, operations);
                                            yield break;
                                        }
                                        state.Input(c);
                                    }
                                    else state.Input((char)(c % byte.MaxValue));
                                }
                                else
                                {
                                    yield return new InterpreterWorkingData(InterpreterExitCode.Failure |
                                                                            InterpreterExitCode.ExceptionThrown |
                                                                            InterpreterExitCode.StdinBufferExhausted,
                                                                            new[] { operators.Take(i + 1) }, position + (uint)i, operations);
                                    yield break;
                                }
                                operations++;
                                break;

                            // func (
                            case '(':
                                if (functions.ContainsKey(state.Current.Value))
                                {
                                    yield return new InterpreterWorkingData(InterpreterExitCode.Failure |
                                                                            InterpreterExitCode.ExceptionThrown |
                                                                            InterpreterExitCode.DuplicateFunctionDefinition,
                                                                            new[] { operators.Take(i + 1) }, position + (uint)i, operations);
                                    yield break;
                                }
                                if (functions.Count == FunctionDefinitionsLimit)
                                {
                                    yield return new InterpreterWorkingData(InterpreterExitCode.Failure |
                                                                            InterpreterExitCode.ExceptionThrown |
                                                                            InterpreterExitCode.FunctionsLimitExceeded,
                                                                            new[] { operators.Take(i + 1) }, position + (uint)i, operations);
                                    yield break;
                                }

                                // Extract the function code
                                IReadOnlyList<Brainf_ckBinaryItem> function = ExtractFunction(operators, i).ToArray();
                                skip = function.Count + 1;

                                // Store the function for later use
                                functions.Add(state.Current.Value, function);
                                break;

                            // )
                            case ')': break; // End of a function body

                            // call
                            case ':':
                                if (!functions.ContainsKey(state.Current.Value))
                                {
                                    yield return new InterpreterWorkingData(InterpreterExitCode.Failure |
                                                                            InterpreterExitCode.ExceptionThrown |
                                                                            InterpreterExitCode.UndefinedFunctionCalled,
                                                                            new[] { operators.Take(i + 1) }, position + (uint)i, operations);
                                    yield break;
                                }
                                if (depth == MaximumStackSize)
                                {
                                    yield return new InterpreterWorkingData(InterpreterExitCode.Failure |
                                                                            InterpreterExitCode.ExceptionThrown |
                                                                            InterpreterExitCode.StackLimitExceeded,
                                                                            new[] { operators.Take(i + 1) }, position + (uint)i, operations);
                                    yield break;
                                }

                                // Call the function
                                operations++;
                                IEnumerable<InterpreterWorkingData> executed = TryRunCore(functions[state.Current.Value], position + (uint)i + 1, (ushort)(depth + 1));
                                foreach (InterpreterWorkingData result in executed)
                                {
                                    if (!result.ExitCode.HasFlag(InterpreterExitCode.Success))
                                    {
                                        yield return result;
                                        yield break;
                                    }
                                    if (result.ExitCode.HasFlag(InterpreterExitCode.BreakpointReached)) yield return result;
                                }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("Invalid operator");
                        }
                    }
                } while (repeat);
                yield return new InterpreterWorkingData(InterpreterExitCode.Success, null, position + (uint)operators.Count, operations);
            }

            // Wrap the results and process them
            bool valid = false;
            using (IEnumerator<InterpreterWorkingData> enumerator = TryRunCore(executable, 0, 1).GetEnumerator())
            {
                while (true)
                {
                    // Get the current partial result
                    timer.Start();
                    bool available = enumerator.MoveNext();
                    timer.Stop();
                    if (!available)
                    {
                        if (!valid) yield return new InterpreterResult(InterpreterExitCode.Failure | InterpreterExitCode.InternalException, state, code);
                        yield break;
                    }
                    valid = true;
                    InterpreterWorkingData data = enumerator.Current;

                    // Prepare the source and the exception info, if needed
                    InterpreterExceptionInfo info;
                    if (data.StackFrames != null)
                    {
                        IReadOnlyList<String> trace = data.StackFrames.Select(frame => new String(frame.Select(b => b.Operator).ToArray())).ToArray();
                        info = new InterpreterExceptionInfo(trace, (int)data.StackFrames.First(frame => frame.Any()).Last().Offset, code);
                    }
                    else info = null;

                    // Reconstruct the functions list
                    IReadOnlyList<FunctionDefinition> definitions = functions.Keys.OrderBy(key => key).Select(key =>
                    {
                        IReadOnlyList<Brainf_ckBinaryItem> blocks = functions[key];
                        return new FunctionDefinition(key, blocks[0].Offset, new String(blocks.Select(b => b.Operator).ToArray()));
                    }).ToArray();

                    // Return the interpreter result with all the necessary info
                    String text = output.ToString();
                    yield return new InterpreterResult(
                        data.ExitCode | (output.Length > 0 ? InterpreterExitCode.TextOutput : InterpreterExitCode.NoOutput), 
                        state, timer.Elapsed, text, code, data.TotalOperations,
                        info, (data.ExitCode & InterpreterExitCode.BreakpointReached) == InterpreterExitCode.BreakpointReached ? (uint?)data.Position : null, definitions);
                }
            }
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

        /// <summary>
        /// Extracts the body of a function from the given source code partition
        /// </summary>
        /// <param name="source">The source code to use to extract the function body</param>
        /// <param name="index">The index of the ( operator that starts the function definition</param>
        [Pure, NotNull]
        private static IEnumerable<Brainf_ckBinaryItem> ExtractFunction([NotNull] IReadOnlyList<Brainf_ckBinaryItem> source, int index)
        {
            // Initial checks
            if (source.Count == 0) throw new ArgumentException("The source code is empty");
            if (index < 0 || index > source.Count - 2) throw new ArgumentOutOfRangeException("The target index is invalid");
            if (source[index].Operator != '(') throw new ArgumentException("The target index doesn't point to the beginning of a function");

            // Iterate from the first character of the function to the final operator
            for (int i = index + 1; i < source.Count; i++)
            {
                if (source[i].Operator == ')') return source.Skip(index + 1).Take(i - index - 1);
            }
            throw new ArgumentException("The source code doesn't contain a well formatted function at the given position");
        }

        #endregion

        #region Tools

        /// <summary>
        /// Extracts the valid operators from a raw source code
        /// </summary>
        /// <param name="source">The input source code</param>
        [NotNull, LinqTunnel]
        private static IEnumerable<char> FindExecutableCode([NotNull] String source) => from c in source
                                                                                        where Operators.Contains(c)
                                                                                        select c;

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

        #region C translator

        /// <summary>
        /// Translates the input source code into its C equivalent
        /// </summary>
        /// <param name="source">The source code with the script to translate</param>
        /// <param name="size">The size of the memory to use in the resulting code</param>
        [PublicAPI]
        [Pure, NotNull]
        public static String TranslateToC([NotNull] String source, int size = DefaultMemorySize)
        {
            // Arguments check
            if (size <= 0) throw new ArgumentOutOfRangeException("The input size is not valid");
            if (source.Any(c => c == '(' || c == ')' || c == ':'))
            {
                throw new ArgumentException("The C translation function isn't supported when using PBrain operators");
            }
            SyntaxValidationResult validationResult = CheckSourceSyntax(source);
            if (!validationResult.Valid) throw new ArgumentException("The input source code isn't valid");

            // Get the operators sequence and initialize the builder
            source = Regex.Replace(source, ",{2,}", "."); // Optimize repeated , operators with a single operator
            IReadOnlyList<char> executable = FindExecutableCode(source).ToArray();
            StringBuilder builder = new StringBuilder();

            // Prepare the header
            builder.Append($"#include <stdio.h>\n\nint main() {{\n\tchar array[{size}] = {{ 0 }};\n\tchar* ptr = array;\n");

            // Local function to get the right tabs for each indented line
            int depth = 1;
            String GetTabs(int count)
            {
                StringBuilder tabBuilder = new StringBuilder();
                while (count-- > 0) tabBuilder.Append('\t');
                return tabBuilder.ToString();
            }

            // Convert the source
            foreach (char c in executable)
            {
                switch (c)
                {
                    case '>':
                        builder.Append($"{GetTabs(depth)}++ptr;\n");
                        break;
                    case '<':
                        builder.Append($"{GetTabs(depth)}--ptr;\n");
                        break;
                    case '+':
                        builder.Append($"{GetTabs(depth)}(*ptr)++;\n");
                        break;
                    case '-':
                        builder.Append($"{GetTabs(depth)}(*ptr)--;\n");
                        break;
                    case '.':
                        builder.Append($"{GetTabs(depth)}putchar(*ptr);\n");
                        break;
                    case ',':
                        builder.Append($"{GetTabs(depth)}while ((*ptr=getchar()) == '\\n') {{ }};\n");
                        break;
                    case '[':
                        builder.Append($"{GetTabs(depth++)}while (*ptr) {{\n");
                        break;
                    case ']':
                        builder.Append($"{GetTabs(--depth)}}}\n");
                        break;
                }
            }

            // Add the final statement and return the translated source
            builder.Append("\treturn 0;\n}");
            return builder.ToString();
        }

        #endregion
    }
}
