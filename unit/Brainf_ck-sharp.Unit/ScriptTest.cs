using System.Diagnostics.Contracts;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ckSharp.Unit
{
    [TestClass]
    public class ScriptTest
    {
        // Executes a script in DEBUG mode
        [Pure]
        private static InterpreterResult RunInDebugConfiguration(string script, string stdin)
        {
            var session = Brainf_ckInterpreter
                .CreateDebugConfiguration()
                .WithSource(script)
                .WithStdin(stdin)
                .WithMemorySize(64)
                .WithOverflowMode(OverflowMode.UshortWithNoOverflow)
                .TryRun();

            Assert.IsNotNull(session.Value);
            Assert.IsTrue(session.ValidationResult.IsSuccess);

            using (session.Value)
            {
                session.Value!.MoveNext();

                Assert.IsNotNull(session.Value.Current);

                return session.Value.Current;
            }
        }

        // Executes a script in RELEASE mode
        [Pure]
        private static InterpreterResult RunInReleaseConfiguration(string script, string stdin)
        {
            var result = Brainf_ckInterpreter
                .CreateReleaseConfiguration()
                .WithSource(script)
                .WithStdin(stdin)
                .WithMemorySize(64)
                .WithOverflowMode(OverflowMode.UshortWithNoOverflow)
                .TryRun();

            Assert.IsNotNull(result.Value);
            Assert.IsTrue(result.ValidationResult.IsSuccess);

            return result.Value!;
        }

        // Tests a script in DEBUG and RELEASE configuration
        private static void TestScript(string script, string stdin, string expected)
        {
            // DEBUG
            var debug = RunInDebugConfiguration(script, stdin);

            Assert.IsNotNull(debug);
            Assert.AreEqual(debug.ExitCode, ExitCode.Success);
            Assert.AreEqual(debug.Stdout, expected);

            // RELEASE
            var release = RunInReleaseConfiguration(script, stdin);

            Assert.IsNotNull(release);
            Assert.AreEqual(release.ExitCode, ExitCode.Success);
            Assert.AreEqual(release.Stdout, expected);
        }

        [TestMethod]
        public void HelloWorld()
        {
            const string script = "[]+++++[>+++++[>+++>++++[>+>+<<-]>>>+++++>+<<<<<<-]<-]>>---.>>+.>++++++++..+++.>>+++++++.<------.<.+++.------.<-.>>>+.";
            const string stdin = "";
            const string expected = "Hello world!";

            TestScript(script, stdin, expected);
        }

        [TestMethod]
        public void Sum()
        {
            const string script = "[,.,.,,.]++++++[>++++++++<-]>>,>>>,<<<[>+>+<<-]<[>+>-<<-]>[<+>-]>>>[>+>+<<-]<<<<[>+>>>>-<<<<<-]>[<+>-]>>>>>>,>>>,<<<[>+>+<<-]<<<<<<<[>+>>>>>>>-<<<<<<<<-]>>>>>>>>>>[>+>+<<-]<<<<<<<<<[>>>>>>>>>>-<<<<<<<<<<<+>-]>>>>>>>>>>>>>++[>++++++[>+++>++++>+++++<<<-]<-]>>---->----->+[<]<<<<<<<<<<<<<[>.[-]]>[[-]>]>>.[-]>>>>>>>>>>.>.<.<<<<<<<<[>.[-]]>[[-]>]>>.[-]>>>>.>>.[-]<[-]<.[-]<<<<<<<<<<<<<<[>>>++++++++++<<<-]>>>>>>[>>>++++++++++<<<-]>>>[<<<<<<<<<+>>>>>>>>>-]<<<<<<[<<<<+>>>>-]<<<<[>+<-]<[-]>>[>+++[>+++<-]>+<<[>+>>+<<<-]>>>[<<<+>>>-]<<[->->+<[>>>]>[<++++++++++>---------->>>>+<]<<<<<]>[-]>[<<+>>-]>>>>[<<<<<+>>>>>-]<<<<<<<[-]+>>]<<[+++++[>++++++++<-]>.[-]<<<]<";
            const string stdin = "2375";
            const string expected = "23 + 75 = 98";

            TestScript(script, stdin, expected);
        }

        [TestMethod]
        public void Multiplication()
        {
            const string script = "[,.,.,,.]++++++[>++++++++<-]>>,>>>,<<<[>+>+<<-]<[>+>-<<-]>[<+>-]>>>[>+>+<<-]<<<<[>+>>>>-<<<<<-]>[<+>-]>>>>>>,>>>,<<<[>+>+<<-]<<<<<<<[>+>>>>>>>-<<<<<<<<-]>>>>>>>>>>[>+>+<<-]<<<<<<<<<[>>>>>>>>>>-<<<<<<<<<<<+>-]>>>>>>>>>>>>>++[>++++++[>+++>++++>+++++<<<-]<-]>>---->------>+[<]<<<<<<<<<<<<<[>.[-]]>[[-]>]>>.[-]>>>>>>>>>>.>.<.<<<<<<<<[>.[-]]>[[-]>]>>.[-]>>>>.>>.[-]<[-]<.[-]<<<<<<<<<<<<<<[>>>++++++++++<<<-]>>>>>>[>>>++++++++++<<<-]>>>[<<<<<<<<<+>>>>>>>>>-]<<<<<<[<<<<+>>>>-]<<<<[>[>+>+<<-]>>[<<+>>-]<<<-]<[-]>>[-]>[>+++[>+++<-]>+<<[>+>>+<<<-]>>>[<<<+>>>-]<<[->->+<[>>>]>[<++++++++++>---------->>>>+<]<<<<<]>[-]>[<<+>>-]>>>>[<<<<<+>>>>>-]<<<<<<<[-]+>>]<<[+++++[>++++++++<-]>.[-]<<<]<<";
            const string stdin = "9985";
            const string expected = "99 * 85 = 8415";

            TestScript(script, stdin, expected);
        }

        [TestMethod]
        public void Fibonacci()
        {
            const string script = "[.,-.,,,.]++++++++[>++++>++++++<<-]>>.[>+>+<<-]>>[<<+>>-],>,<<[>->-<<-]>[<++++++++++>-]>[<<+>>-]<<-1>>>+>>>+(->[>+<-]>[>>++++++++++<<[>+>>+<<<-]>>>[<<<+>>>-]<<[->->+<[>>>]=>[<++++++++++>---------->>>>+<]<<<<<]>[-]>[<<+>>-]>>>>[<<<<<+>>>>>-]<<<<<<<[-]+>>]<<[+++++[>++++++++<-]>.[-]<<<])<<<<<(<[<<.>>->>>1[>+>+>>+<<<<-]>>[<<+>>-]<<<[>+<-]>>[<<+>>-]>>:+<<<<<:]):";
            const string stdin = "24";
            const string expected = "0 1 1 2 3 5 8 13 21 34 55 89 144 233 377 610 987 1597 2584 4181 6765 10946 17711 28657";

            TestScript(script, stdin, expected);
        }
    }
}
