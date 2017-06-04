using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Branf_ck_sharp;
using JetBrains.Annotations;

namespace Brainf_ck_sharp
{
    /// <summary>
    /// Classe statica che interpreta ed esegue il debug in modo asincrono dei codici sorgenti in Brainfuck
    /// </summary>
    public static class Brainf_ckInterpreter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="arguments"></param>
        /// <param name="size"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        [PublicAPI]
        [Pure, NotNull]
        public static InterpreterResult Run([NotNull] String source, [NotNull] String arguments,
            int size = 64, int? threshold = null)
        {
            return TryRun(source, arguments, new TouringMachineState(size), threshold);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="arguments"></param>
        /// <param name="state"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        [PublicAPI]
        [Pure, NotNull]
        public static InterpreterResult Run([NotNull] String source, [NotNull] String arguments,
            [NotNull] TouringMachineState state, int? threshold = null)
        {
            return TryRun(source, arguments, state.Clone(), threshold);
        }

        /// <summary>
        /// Gets the collection of valid Brainf_ck operators
        /// </summary>
        [NotNull]
        public static readonly IReadOnlyCollection<char> Operators = new[] { '+', '-', '>', '<', '.', ',', '[', ']' };

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
        private static bool CheckSourceSyntax([NotNull] IEnumerable<char> operators)
        {
            // Iterate over all the characters in the source
            int height = 0;
            foreach (char c in operators)
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

        /// <summary>
        /// Checks whether or not the syntax in the input source code is valid
        /// </summary>
        /// <param name="source">The source code to analyze</param>
        /// <returns>A bool value that indicates whether or not the source code is valid, 
        /// and the position of the first syntax error, if there is at least one</returns>
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

        [Pure, NotNull]
        private static InterpreterResult TryRun([NotNull] String source, [NotNull] String arguments,
            [NotNull] TouringMachineState state, int? threshold)
        {
            // Get the operators to execute and check if the source is empty
            IReadOnlyList<char> executable = FindExecutableCode(source).ToArray();
            if (executable.Count == 0)
            {
                return new InterpreterResult(InterpreterExitCode.Failure | InterpreterExitCode.NoCodeInterpreted, state, TimeSpan.Zero, String.Empty, String.Empty, null);
            }

            // Check the code syntax
            if (!CheckSourceSyntax(executable))
            {
                return new InterpreterResult(InterpreterExitCode.Failure | InterpreterExitCode.MismatchedParentheses, state, TimeSpan.Zero, String.Empty,
                    executable.AggregateToString(), null);
            }

            // Prepare the input and output arguments
            Queue<char> input = arguments.Length > 0 ? new Queue<char>(arguments) : new Queue<char>();
            StringBuilder output = new StringBuilder();

            // Start the stopwatch to monitor the execution
            Stopwatch timer = new Stopwatch();
            timer.Start();

            // Internal recursive function that interpretes the code
            (InterpreterExitCode, IEnumerable<IEnumerable<char>>) TryRunCore(IReadOnlyList<char> operators)
            {
                // Outer do-while that repeats the code if there's a loop
                bool repeat = false;
                do
                {
                    // Check the current elapsed time
                    if (threshold.HasValue && timer.ElapsedMilliseconds > threshold.Value)
                    {
                        return (InterpreterExitCode.Failure | InterpreterExitCode.ThresholdExceeded, new[] { new char[0] });
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

                        // Parse the current operator
                        switch (operators[i])
                        {
                            // ptr++
                            case '>':
                                if (state.CanMoveNext) state.MoveNext();
                                else return (InterpreterExitCode.Failure |
                                             InterpreterExitCode.ExceptionThrown |
                                             InterpreterExitCode.UpperBoundExceeded, new[] { operators.Take(i + 1) });
                                break;

                            // ptr--
                            case '<':
                                if (state.CanMoveBack) state.MoveBack();
                                else return (InterpreterExitCode.Failure |
                                             InterpreterExitCode.ExceptionThrown |
                                             InterpreterExitCode.LowerBoundExceeded, new[] { operators.Take(i + 1) });
                                break;

                            // *ptr++
                            case '+':
                                if (state.CanIncrement) state.Plus();
                                else return (InterpreterExitCode.Failure |
                                             InterpreterExitCode.ExceptionThrown |
                                             InterpreterExitCode.MaxValueExceeded, new[] { operators.Take(i + 1) });
                                break;

                            // *ptr--
                            case '-':
                                if (state.CanDecrement) state.Minus();
                                else return (InterpreterExitCode.Failure |
                                             InterpreterExitCode.ExceptionThrown |
                                             InterpreterExitCode.NegativeValue, new[] { operators.Take(i + 1) });
                                break;

                            // while (*ptr) {
                            case '[':
                                IReadOnlyList<char> loop = ExtractInnerLoop(operators, i).Concat(new[] { ']' }).ToArray();
                                skip = loop.Count;
                                if (state.Current > 0)
                                {
                                    (InterpreterExitCode code, IEnumerable<IEnumerable<char>> loopFrames) = TryRunCore(loop);
                                    if ((code & InterpreterExitCode.Success) == 0)
                                    {
                                        return (code, new[] { operators.Take(i + 1) }.Concat(loopFrames));
                                    }
                                }
                                break;

                            // }
                            case ']':
                                if (state.Current == 0)
                                {
                                    // Loop end
                                    return (InterpreterExitCode.Success, null);
                                }
                                else
                                {
                                    // Jump back and execute the loop body again
                                    repeat = true;
                                    continue;
                                }

                            // putch(*ptr)
                            case '.':
                                output.Append(Convert.ToChar(state.Current));
                                break;

                            // *ptr = getch()
                            case ',':
                                if (input.Count > 0) state.Input(input.Dequeue());
                                else return (InterpreterExitCode.Failure |
                                             InterpreterExitCode.ExceptionThrown |
                                             InterpreterExitCode.StrinBufferExhausted, new[] { operators.Take(i + 1) });
                                break;
                        }
                    }
                } while (repeat);
                return (InterpreterExitCode.Success, null);
            }

            // Execute the code and stop the timer
            (InterpreterExitCode result, IEnumerable<IEnumerable<char>> frames) = TryRunCore(executable);
            timer.Stop();

            // Reconstruct the stack trace that generated the error and return the interpreter result
            Stack<String> stackTrace = frames == null ? null : new Stack<String>(
                from frame in frames
                select frame.AggregateToString());
            String text = output.ToString();
            return new InterpreterResult(
                result | (text.Length > 0 ? InterpreterExitCode.TextOutput : InterpreterExitCode.NoOutput),
                state, timer.Elapsed, text, executable.AggregateToString(), stackTrace);
        }

        /* TODO: update this old code
        /// <summary>
        /// Interpreta in modo ricorsivo e asincrono il codice sorgente, eseguendo tutte le operazioni anche in eventuali cicli annidati
        /// </summary>
        /// <typeparam name="T">Vale o PivotPage oppure DatiInterpreteHelper</typeparam>
        /// <param name="sorgente">Il codice sorgente di partenza o il frammento attuale da interpretare in una chiamata ricorsiva</param>
        /// <param name="datiBrainfuck">L'istanza con i dati su cui lavora l'interprete</param>
        /// <param name="output">La stringa temporanea su cui aggiungere gli eventuali caratteri in uscita prodotti dal codice</param>
        /// <param name="cronometro">Tiene il conto del tempo totale trascorso nell'interpretare il codice</param>
        /// <param name="contextualParameter"><para>Referenza alla pagina principale per visualizzare il Flyout oppure</para>
        /// <para>un'istanza di DatiInterpreteHelper con i dati di Debug da visualizzare in seguito</para></param>
        /// <param name="charBuffer">Parametro opzionale che indica il numero massimo di caratteri che può richiedere uno script eseguito nella console</param>
        /// <returns>Restituisce la stringa con l'output generato dal codice in ingresso</returns>
        private static async Task<String> Run(CompilerInternalData internalData, T contextualParameter, int charBuffer = 0)
        {
            int skipChar = 0;
            int? backupPosizione = null;
            if (typeof(T) == typeof(DatiInterpreteHelper))
            {
                backupPosizione = (contextualParameter as DatiInterpreteHelper).posizioneErrore;
            }
            Inizio:
            {
                if ((internalData.cronometro.ElapsedMilliseconds > 1500 && typeof(T) == typeof(PivotPage)) ||
                    (internalData.cronometro.ElapsedMilliseconds > 2500 && typeof(T) == typeof(DatiInterpreteHelper)))
                {
                    if (typeof(T) == typeof(DatiInterpreteHelper))
                    {
                        (contextualParameter as DatiInterpreteHelper).StackTrace.Pop();
                        (contextualParameter as DatiInterpreteHelper).StackTrace.Push(internalData.sourceCode.Substring(0, internalData.sourceCode.IndexOf(']')));
                    }
                    throw new OperationCanceledException();
                }
                for (int carattere = 0; carattere < internalData.sourceCode.Length; carattere++)
                {
                    if (typeof(T) == typeof(DatiInterpreteHelper))
                    {
                        if (internalData.sourceCode[carattere] == RichEditBoxHelper.CharBreakpoint && skipChar != 0)
                        {
                            (contextualParameter as DatiInterpreteHelper).skipBreakpoint = true;
                            return internalData.codeOutput;
                        }
                    }
                    if (skipChar != 0)
                    {
                        skipChar--;
                        continue;
                    }
                    if (typeof(T) == typeof(DatiInterpreteHelper))
                    {
                        (contextualParameter as DatiInterpreteHelper).posizioneErrore++;
                        (contextualParameter as DatiInterpreteHelper).aggiornaCimaStackTrace(internalData.sourceCode[carattere]);
                        if (internalData.sourceCode[carattere] == RichEditBoxHelper.CharBreakpoint)
                        {
                            (contextualParameter as DatiInterpreteHelper).breakpointStop = true;
                            return internalData.codeOutput;
                        }
                    }
                    switch (internalData.sourceCode[carattere])
                    {
                        case '>':
                        {
                            if (internalData.datiSessione.canGoUp)
                            {
                                internalData.datiSessione.indice++;
                            }
                            else
                            {
                                throw new Exception("UpperBoundExceededException");
                            }
                            break;
                        }
                        case '<':
                        {
                            if (internalData.datiSessione.canGoDown)
                            {
                                internalData.datiSessione.indice--;
                            }
                            else
                            {
                                throw new Exception("LowerBoundExceededException");
                            }
                            break;
                        }
                        case '+':
                        {
                            if (internalData.datiSessione.canIncrement)
                            {
                                internalData.datiSessione.Plus();
                            }
                            else
                            {
                                throw new Exception("MaxValueExceededException");
                            }
                            break;
                        }
                        case '-':
                        {
                            if (internalData.datiSessione.canDecrement)
                            {
                                internalData.datiSessione.Minus();
                            }
                            else
                            {
                                throw new Exception("NegativeValueException");
                            }
                            break;
                        }
                        case '[':
                        {
                            skipChar = calcolaCaratteriSalto(internalData[carattere + 1]) + 1;
                            if (internalData.datiSessione.attuale != 0)
                            {
                                CompilerInternalData sessioneParziale = new CompilerInternalData(internalData.cronometro)
                                {
                                    sourceCode = internalData[carattere + 1],
                                    datiSessione = internalData.datiSessione,
                                    codeOutput = internalData.codeOutput
                                };
                                if (typeof(T) == typeof(DatiInterpreteHelper))
                                {
                                    (contextualParameter as DatiInterpreteHelper).StackTrace.Push("");
                                }
                                internalData.codeOutput = await interpreta(sessioneParziale, contextualParameter, charBuffer);
                                if (typeof(T) == typeof(DatiInterpreteHelper))
                                {
                                    if ((contextualParameter as DatiInterpreteHelper).breakpointStop ||
                                        (contextualParameter as DatiInterpreteHelper).skipBreakpoint)
                                    {
                                        return internalData.codeOutput;
                                    }
                                }
                            }
                            else
                            {
                                if (typeof(T) == typeof(DatiInterpreteHelper))
                                {
                                    (contextualParameter as DatiInterpreteHelper).posizioneErrore += skipChar;
                                }
                            }
                            break;
                        }
                        case ']':
                        {
                            if (internalData.datiSessione.attuale == 0)
                            {
                                if (typeof(T) == typeof(DatiInterpreteHelper))
                                {
                                    String backupInnerStack = (contextualParameter as DatiInterpreteHelper).StackTrace.Pop();
                                    String baseStack = (contextualParameter as DatiInterpreteHelper).StackTrace.Pop();
                                    (contextualParameter as DatiInterpreteHelper).StackTrace.Push(baseStack + backupInnerStack);
                                }
                                return internalData.codeOutput;
                            }
                            else
                            {
                                if (typeof(T) == typeof(DatiInterpreteHelper))
                                {
                                    (contextualParameter as DatiInterpreteHelper).posizioneErrore = backupPosizione.Value;
                                    (contextualParameter as DatiInterpreteHelper).ripristinaCimaStackTrace();
                                }
                                goto Inizio;
                            }
                        }
                        case '.': internalData.codeOutput += Convert.ToString(Convert.ToChar(internalData.datiSessione.attuale)); break;
                        case ',':
                        {
                            if (typeof(T) == typeof(PivotPage))
                            {
                                internalData.cronometro.Stop();
                                if (--charBuffer < 0)
                                {
                                    throw new Exception("CharBufferLimitExceededException");
                                }
                                internalData.datiSessione.attuale = (char)(await stdinRequest(StdinInputType.SingleCharacter, contextualParameter as PivotPage));
                                internalData.cronometro.Start();
                            }
                            else
                            {
                                try
                                {
                                    internalData.datiSessione.attuale = (contextualParameter as DatiInterpreteHelper).carattereAttuale;
                                }
                                catch
                                {
                                    throw new Exception("ExhaustedStdinBufferException");
                                }
                            }
                            break;
                        }
                    }
                }
            }
            return internalData.codeOutput;
        } */

        /// <summary>
        /// Counts the number of operators to skip from the first one inside a terminated loop
        /// </summary>
        /// <param name="operators">The operastors to analyze</param>
        private static int CalculateSkippedOperators([NotNull] IEnumerable<char> operators)
        {
            // Count the total operators in the nested loops to skip
            int height = 0, jump = 0;
            foreach (char c in operators)
            {
                // Reach the end of the nested loop
                if (c == '[') height++;
                else if (c == ']')
                {
                    if (height == 0) break;
                    height--;
                }
                jump++;
            }
            return jump + 1; // Include the last ] operator
        }


        [Pure, NotNull]
        private static IEnumerable<char> ExtractInnerLoop([NotNull] IReadOnlyList<char> source, int index)
        {
            // Initial checks
            if (source.Count == 0) throw new ArgumentException("The source code is empty");
            if (index < 0 || index > source.Count - 2) throw new ArgumentOutOfRangeException("The target index is invalid");
            if (source[index] != '[') throw new ArgumentException("The target index doesn't point to the beginning of a loop");

            int height = 0;
            for (int i = index + 1; i < source.Count; i++)
            {
                if (source[i] == '[') height++;
                else if (source[i] == ']')
                {
                    if (height == 0) return source.Skip(index + 1).Take(i - (index + 1));
                    height--;
                }
            }
            throw new ArgumentException("The source code doesn't contain a well formatted nested loop at the given position");
        }
    }
}
