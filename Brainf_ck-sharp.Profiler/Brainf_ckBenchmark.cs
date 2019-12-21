using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Brainf_ck_sharp.NET.Enums;
using Brainf_ck_sharp.NET.Models;
using Brainf_ck_sharp.NET.Models.Base;
using Brainf_ckInterpreterOld = Brainf_ck_sharp.Brainf_ckInterpreter;
using Brainf_ckInterpreterNew = Brainf_ck_sharp.NET.Brainf_ckInterpreter;

namespace Brainf_ck_sharp.Profiler
{
    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    [CategoriesColumn]
    public class Brainf_ckBenchmark
    {
        [GlobalSetup]
        public void Setup()
        {
            if (HelloWorldOld() != HelloWorldNew() ||
                SumOld() != SumNew() ||
                MultiplyOld() != MultiplyNew() ||
                FibonacciOld() != FibonacciNew())
            {
                throw new InvalidOperationException("Error in the initialization");
            }
        }

        private const string HelloWorldScript = "[\n\tThis is a simple script\n\tthat just prints \"Hello world!\" to the Stdout buffer\n]\n+++++ first loop\n[\n\t>+++++ 25 in 2nd cell\n\t[\n\t\t>+++ 75 in 3rd\n\t\t>++++ 100 in 4th\n\t\t[\n\t\t\t>+>+ duplicate\n\t\t\t<<- the value\n\t\t]\n\t\t>>>+++++ 125 in 7th\n\t\t>+ 25 in 8th\n\t\t<<<<<<- loop end\n\t]\n\t<- return to 1st cell\n]\n>>---. H\n>>+. e\n>++++++++.. ll\n+++. o\n>>+++++++. space\n<------. W\n<. o\n+++. r\n------. l\n<-. d\n>>>+. !";

        private const string HelloWorldStdin = "";

        /// <summary>
        /// Prints "Hello world!"
        /// </summary>
        [Benchmark(Baseline = true)]
        [BenchmarkCategory("HelloWorld")]
        public string HelloWorldOld() => Brainf_ckInterpreterOld.Run(HelloWorldScript, HelloWorldStdin).Output;

        /// <summary>
        /// Prints "Hello world!"
        /// </summary>
        [Benchmark]
        [BenchmarkCategory("HelloWorld")]
        public string HelloWorldNew()
        {
            Option<InterpreterResult> option = Brainf_ckInterpreterNew.TryRun(HelloWorldScript, HelloWorldStdin, 64, OverflowMode.UshortWithNoOverflow);
            option.Value!.MachineState.Dispose();

            return option.Value.Stdout;
        }

        private const string SumScript = "[\n\tTo use this program, you\'ll need to enter 4 numbers in the buffer,\n\ttwo for each number. If you want to sum a number that has a single digit,\n\tyou\'ll need to add a 0 before it.\n\tFor example, to sum 7 and 64, enter 0764 in the buffer.\n]\n++++++[\n\t>++++++++<- 48 in cell 1\n]\n>>,>>>, read first number as x1; x2\n<<<[>+>+<<-] clone x1\n<\n[>+>-<<-] sub 48 to copied x1\n>\n[<+>-] restore 48\n>>> move to x2\n[>+>+<<-] clone x2\n<<<< move to 48[\n\t>+>>>>- sub 48 to x2\n\t<<<<<-\n]\n>\n[<+>-] restore copy of 48\n>>>>>> move after x2\n,>>>, read second one as y1; y2\n<<<\n[>+>+<<-] clone y1\n<<<<<<<[\n\t>+>>>>>>>- sub 48 to y1\n\t<<<<<<<<-\n]\n>>>>>>>>>> move to y2\n[>+>+<<-] clone it\n<<<<<<<<<[\n\t>>>>>>>>>>- sub 48 to y2\n\t<<<<<<<<<<<+>-\n]\n>>>>>>>>>>>>>++[\n\t>++++++[\n\t\t>+++ 36\n\t\t>++++ 48\n\t\t>+++++ 60\n\t\t<<<-\n\t]\n\t<-\n]\n>>---- space\n>----- plus symbol\n>+ = symbol\n<<<<<<<<<<<<<<<< go back to cell #0[\n\t>. print x1 if not 0\n\t[-]\n]\n> move to x2 in Unicode\n[[-]>] resets cells if needed\n>>. print x2\n[-]\n>>>>>>>>>>.>.<. print the sum operator\n<<<<<<<< move back to y1[\n\t>. print y1 if not 0\n\t[-]\n]\n> move to y1 in Unicode\n[[-]>] reset loop\n>>. print y2\n[-]\n>>>>.>>.\n[-] restore = symbol\n<[-] restore sum symbol\n<. print \" = \"\n[-] restore space character\n<<<<<<<<<<<<<< back to x1[\n\t>>>++++++++++ 10*x1\n\t<<<-\n]\n>>>>>>[\n\t>>>++++++++++ 10*y1\n\t<<<-\n]\n>>>[\n\t<<<<<<<<<+ move y to 4th\n\t>>>>>>>>>-\n]\n<<<<<<[\n\t<<<<+> move x to 3rd\n\t>>>-\n]\n<<<< return to x\n[\n\t>+ sum x to y\n\t<-\n]\n<\n[-] reset Unicode x value\n>> go to result\n[\n\t>+++[\n\t\t>+++<- 10 in 3rd cell\n\t]\n\t>+<<[\n\t\t>+>>+<<<- 2*x\n\t]\n\t>>>[\n\t\t<<<+ move the copy\n\t\t>>>- \n\t]\n\t<< return to 1st cell\n\t[\n\t\t->->+ sub 1 to 10\n\t\t< return to 3rd\n\t\t[\n\t\t\t>>> if != 0 skip\n\t\t]\n\t\t>[\n\t\t\t<++++++++++\n\t\t\t>----------\n\t\t\t>>>>+<\n\t\t]\n\t\t<<<<<\n\t]\n\t>[-] reset temp cell\n\t>[\n\t\t<<+ move first value\n\t\t>>-\n\t]\n\t>>>>[\n\t\t<<<<<+ move value\n\t\t>>>>>-\n\t]\n\t<<<<<<<\n\t[-] reset previous temp cell\n\t+ set that cell to 1\n\t>>\n]\n<< start from the last number\n[\n\t+++++[\n\t\t>++++++++<-\n\t]\n\t>. sum 48 and print it\n\t[-] reset cell\n\t<<< go to previous number\n]\n< return to cell 0 at the end";

        private const string SumStdin = "2375";

        /// <summary>
        /// Prints "23 + 75 = 98"
        /// </summary>
        [Benchmark(Baseline = true)]
        [BenchmarkCategory("Sum")]
        public string SumOld() => Brainf_ckInterpreterOld.Run(SumScript, SumStdin).Output;

        /// <summary>
        /// Prints "23 + 75 = 98"
        /// </summary>
        [Benchmark]
        [BenchmarkCategory("Sum")]
        public string SumNew()
        {
            Option<InterpreterResult> option = Brainf_ckInterpreterNew.TryRun(SumScript, SumStdin, 64, OverflowMode.UshortWithNoOverflow);
            option.Value!.MachineState.Dispose();

            return option.Value.Stdout;
        }

        private const string MultiplyScript = "[,.,.,,.]++++++[>++++++++<-]>>,>>>,<<<[>+>+<<-]<[>+>-<<-]>[<+>-]>>>[>+>+<<-]<<<<[>+>>>>-<<<<<-]>[<+>-]>>>>>>,>>>,<<<[>+>+<<-]<<<<<<<[>+>>>>>>>-<<<<<<<<-]>>>>>>>>>>[>+>+<<-]<<<<<<<<<[>>>>>>>>>>-<<<<<<<<<<<+>-]>>>>>>>>>>>>>++[>++++++[>+++>++++>+++++<<<-]<-]>>---->------>+[<]<<<<<<<<<<<<<[>.[-]]>[[-]>]>>.[-]>>>>>>>>>>.>.<.<<<<<<<<[>.[-]]>[[-]>]>>.[-]>>>>.>>.[-]<[-]<.[-]<<<<<<<<<<<<<<[>>>++++++++++<<<-]>>>>>>[>>>++++++++++<<<-]>>>[<<<<<<<<<+>>>>>>>>>-]<<<<<<[<<<<+>>>>-]<<<<[>[>+>+<<-]>>[<<+>>-]<<<-]<[-]>>[-]>[>+++[>+++<-]>+<<[>+>>+<<<-]>>>[<<<+>>>-]<<[->->+<[>>>]>[<++++++++++>---------->>>>+<]<<<<<]>[-]>[<<+>>-]>>>>[<<<<<+>>>>>-]<<<<<<<[-]+>>]<<[+++++[>++++++++<-]>.[-]<<<]<<";

        private const string MultiplyStdin = "9985";

        /// <summary>
        /// Prints "99 * 85 = 8415"
        /// </summary>
        [Benchmark(Baseline = true)]
        [BenchmarkCategory("Multiply")]
        public string MultiplyOld() => Brainf_ckInterpreterOld.Run(MultiplyScript, MultiplyStdin).Output;

        /// <summary>
        /// Prints "99 * 85 = 8415"
        /// </summary>
        [Benchmark]
        [BenchmarkCategory("Multiply")]
        public string MultiplyNew()
        {
            Option<InterpreterResult> option = Brainf_ckInterpreterNew.TryRun(MultiplyScript, MultiplyStdin, 64, OverflowMode.UshortWithNoOverflow);
            option.Value!.MachineState.Dispose();

            return option.Value.Stdout;
        }

        private const string FibonaccyScript = "[\n\tThis program prints the first n numbers of the Fibonacci sequence.\n\tTo use it, enter a two-digits number in the input buffer.\n\tAs this program, just like any other script in this language, is not\n\tvery efficient, try to keep the number small enough for best results.\n]\n++++++++[ setup print characters\n\t>++++>++++++ space and '0'\n\t<<-\n]>>\n. print initial sequence number\n[>+>+<<-]>>[<<+>>-]\n,>,<<[ read input digits\n\t>->-<<- shift to base 10\n]\n>[<++++++++++>-]>[<<+>>-] get the actual input\n<<- decrement by 1 to fix iterations count\n>>>+ fibonacci counters in #5 and #6\n>>>+(->\n[>+<-]> leave 2 empty cells for later\n[\n\t>>++++++++++ 10 in 3rd cell\n\t<<[>+>>+<<<-]>>>[<<<+>>>-] duplicate x\n\t<<[\n\t\t->->+< sub 1 to copy and 10\n\t\t[>>>] if != 0 skip\n\t\t>[\n\t\t\t<++++++++++ restore 10\n\t\t\t>---------- keep track on result\n\t\t\t>>>>+< increment digit counter\n\t\t]\n\t\t<<<<<\n\t]\n\t>[-] reset temp cell\n\t>[<<+>>-] move resulting digit back\n\t>>>>[<<<<<+>>>>>-] move other counter back\n\t<<<<<<<[-] reset previous temp cell\n\t+>> set marker for later and loop again\n]\n<< start from the last number\n[\n\t+++++[>++++++++<-]>. sum 48 and print it\n\t[-]<<< reset and go back one digit\n]) @printNumber\n<<<<< go back to cell before temp values\n(<[\n\t<<.>> print the space character\n\t->>> move to temp #1\n\t[>+>+>>+<<<<-]>>[<<+>>-]<< duplicate\n\t<[>+<-] sum temp #0 and #1\n\t>>[<<+>>-] move backup to temp #0\n\t>>:+ print the current number\n\t<<<<<: recurse!\n]) @fibonacci\n:";

        private const string FibonacciStdin = "24";

        /// <summary>
        /// Prints "0 1 1 2 3 5 8 13 21 34 55 89 144 233 377 610 987 1597 2584 4181 6765 10946 17711 28657"
        /// </summary>
        [Benchmark(Baseline = true)]
        [BenchmarkCategory("Fibonacci")]
        public string FibonacciOld() => Brainf_ckInterpreterOld.Run(FibonaccyScript, FibonacciStdin).Output;

        /// <summary>
        /// Prints "0 1 1 2 3 5 8 13 21 34 55 89 144 233 377 610 987 1597 2584 4181 6765 10946 17711 28657"
        /// </summary>
        [Benchmark]
        [BenchmarkCategory("Fibonacci")]
        public string FibonacciNew()
        {
            Option<InterpreterResult> option = Brainf_ckInterpreterNew.TryRun(FibonaccyScript, FibonacciStdin, 64, OverflowMode.UshortWithNoOverflow);
            option.Value!.MachineState.Dispose();

            return option.Value.Stdout;
        }
    }
}
