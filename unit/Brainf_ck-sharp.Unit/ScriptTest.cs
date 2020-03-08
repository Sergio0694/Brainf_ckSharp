using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ckSharp.Unit
{
    [TestClass]
    public class ScriptTest
    {
        [TestMethod]
        public void HelloWorld()
        {
            const string script = "[]+++++[>+++++[>+++>++++[>+>+<<-]>>>+++++>+<<<<<<-]<-]>>---.>>+.>++++++++..+++.>>+++++++.<------.<.+++.------.<-.>>>+.";

            Option<InterpreterResult> result = Brainf_ckInterpreter
                .CreateReleaseConfiguration()
                .WithSource(script)
                .TryRun();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(result.Value!.ExitCode, ExitCode.Success);
            Assert.AreEqual(result.Value.Stdout, "Hello world!");
        }

        [TestMethod]
        public void Sum()
        {
            const string script = "[,.,.,,.]++++++[>++++++++<-]>>,>>>,<<<[>+>+<<-]<[>+>-<<-]>[<+>-]>>>[>+>+<<-]<<<<[>+>>>>-<<<<<-]>[<+>-]>>>>>>,>>>,<<<[>+>+<<-]<<<<<<<[>+>>>>>>>-<<<<<<<<-]>>>>>>>>>>[>+>+<<-]<<<<<<<<<[>>>>>>>>>>-<<<<<<<<<<<+>-]>>>>>>>>>>>>>++[>++++++[>+++>++++>+++++<<<-]<-]>>---->----->+[<]<<<<<<<<<<<<<[>.[-]]>[[-]>]>>.[-]>>>>>>>>>>.>.<.<<<<<<<<[>.[-]]>[[-]>]>>.[-]>>>>.>>.[-]<[-]<.[-]<<<<<<<<<<<<<<[>>>++++++++++<<<-]>>>>>>[>>>++++++++++<<<-]>>>[<<<<<<<<<+>>>>>>>>>-]<<<<<<[<<<<+>>>>-]<<<<[>+<-]<[-]>>[>+++[>+++<-]>+<<[>+>>+<<<-]>>>[<<<+>>>-]<<[->->+<[>>>]>[<++++++++++>---------->>>>+<]<<<<<]>[-]>[<<+>>-]>>>>[<<<<<+>>>>>-]<<<<<<<[-]+>>]<<[+++++[>++++++++<-]>.[-]<<<]<";

            Option<InterpreterResult> result = Brainf_ckInterpreter
                .CreateReleaseConfiguration()
                .WithSource(script)
                .WithStdin("2375")
                .TryRun();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(result.Value!.ExitCode, ExitCode.Success);
            Assert.AreEqual(result.Value.Stdout, "23 + 75 = 98");
        }

        [TestMethod]
        public void Multiplication()
        {
            const string script = "[,.,.,,.]++++++[>++++++++<-]>>,>>>,<<<[>+>+<<-]<[>+>-<<-]>[<+>-]>>>[>+>+<<-]<<<<[>+>>>>-<<<<<-]>[<+>-]>>>>>>,>>>,<<<[>+>+<<-]<<<<<<<[>+>>>>>>>-<<<<<<<<-]>>>>>>>>>>[>+>+<<-]<<<<<<<<<[>>>>>>>>>>-<<<<<<<<<<<+>-]>>>>>>>>>>>>>++[>++++++[>+++>++++>+++++<<<-]<-]>>---->------>+[<]<<<<<<<<<<<<<[>.[-]]>[[-]>]>>.[-]>>>>>>>>>>.>.<.<<<<<<<<[>.[-]]>[[-]>]>>.[-]>>>>.>>.[-]<[-]<.[-]<<<<<<<<<<<<<<[>>>++++++++++<<<-]>>>>>>[>>>++++++++++<<<-]>>>[<<<<<<<<<+>>>>>>>>>-]<<<<<<[<<<<+>>>>-]<<<<[>[>+>+<<-]>>[<<+>>-]<<<-]<[-]>>[-]>[>+++[>+++<-]>+<<[>+>>+<<<-]>>>[<<<+>>>-]<<[->->+<[>>>]>[<++++++++++>---------->>>>+<]<<<<<]>[-]>[<<+>>-]>>>>[<<<<<+>>>>>-]<<<<<<<[-]+>>]<<[+++++[>++++++++<-]>.[-]<<<]<<";

            Option<InterpreterResult> result = Brainf_ckInterpreter
                .CreateReleaseConfiguration()
                .WithSource(script)
                .WithStdin("9985")
                .WithMemorySize(64)
                .WithOverflowMode(OverflowMode.UshortWithNoOverflow)
                .TryRun();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(result.Value!.ExitCode, ExitCode.Success);
            Assert.AreEqual(result.Value.Stdout, "99 * 85 = 8415");
        }

        [TestMethod]
        public void Fibonacci()
        {
            const string script = "[.,-.,,,.]++++++++[>++++>++++++<<-]>>.[>+>+<<-]>>[<<+>>-],>,<<[>->-<<-]>[<++++++++++>-]>[<<+>>-]<<-1>>>+>>>+(->[>+<-]>[>>++++++++++<<[>+>>+<<<-]>>>[<<<+>>>-]<<[->->+<[>>>]=>[<++++++++++>---------->>>>+<]<<<<<]>[-]>[<<+>>-]>>>>[<<<<<+>>>>>-]<<<<<<<[-]+>>]<<[+++++[>++++++++<-]>.[-]<<<])<<<<<(<[<<.>>->>>1[>+>+>>+<<<<-]>>[<<+>>-]<<<[>+<-]>>[<<+>>-]>>:+<<<<<:]):";

            Option<InterpreterResult> result = Brainf_ckInterpreter
                .CreateReleaseConfiguration()
                .WithSource(script)
                .WithStdin("24")
                .WithMemorySize(64)
                .WithOverflowMode(OverflowMode.UshortWithNoOverflow)
                .TryRun();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(result.Value!.ExitCode, ExitCode.Success);
            Assert.AreEqual(result.Value.Stdout, "0 1 1 2 3 5 8 13 21 34 55 89 144 233 377 610 987 1597 2584 4181 6765 10946 17711 28657");
        }
    }
}
