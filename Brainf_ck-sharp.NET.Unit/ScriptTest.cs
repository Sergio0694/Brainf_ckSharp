using Brainf_ck_sharp.NET.Enums;
using Brainf_ck_sharp.NET.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ck_sharp.NET.Unit
{
    [TestClass]
    public class ScriptTest
    {
        [TestMethod]
        public void HelloWorld()
        {
            const string script = "[]+++++[>+++++[>+++>++++[>+>+<<-]>>>+++++>+<<<<<<-]<-]>>---.>>+.>++++++++..+++.>>+++++++.<------.<.+++.------.<-.>>>+.";

            InterpreterResult result = Brainf_ckInterpreter.Run(script);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.ExitCode, ExitCode.TextOutput);
            Assert.AreEqual(result.Stdout, "Hello world!");
        }

        [TestMethod]
        public void Sum()
        {
            const string script = "[,.,.,,.]++++++[>++++++++<-]>>,>>>,<<<[>+>+<<-]<[>+>-<<-]>[<+>-]>>>[>+>+<<-]<<<<[>+>>>>-<<<<<-]>[<+>-]>>>>>>,>>>,<<<[>+>+<<-]<<<<<<<[>+>>>>>>>-<<<<<<<<-]>>>>>>>>>>[>+>+<<-]<<<<<<<<<[>>>>>>>>>>-<<<<<<<<<<<+>-]>>>>>>>>>>>>>++[>++++++[>+++>++++>+++++<<<-]<-]>>---->----->+[<]<<<<<<<<<<<<<[>.[-]]>[[-]>]>>.[-]>>>>>>>>>>.>.<.<<<<<<<<[>.[-]]>[[-]>]>>.[-]>>>>.>>.[-]<[-]<.[-]<<<<<<<<<<<<<<[>>>++++++++++<<<-]>>>>>>[>>>++++++++++<<<-]>>>[<<<<<<<<<+>>>>>>>>>-]<<<<<<[<<<<+>>>>-]<<<<[>+<-]<[-]>>[>+++[>+++<-]>+<<[>+>>+<<<-]>>>[<<<+>>>-]<<[->->+<[>>>]>[<++++++++++>---------->>>>+<]<<<<<]>[-]>[<<+>>-]>>>>[<<<<<+>>>>>-]<<<<<<<[-]+>>]<<[+++++[>++++++++<-]>.[-]<<<]<";
            
            InterpreterResult result = Brainf_ckInterpreter.Run(script, "2375");

            Assert.IsNotNull(result);
            Assert.AreEqual(result.ExitCode, ExitCode.TextOutput);
            Assert.AreEqual(result.Stdout, "23 + 75 = 98");
        }

        [TestMethod]
        public void Multiplication()
        {
            const string script = "[,.,.,,.]++++++[>++++++++<-]>>,>>>,<<<[>+>+<<-]<[>+>-<<-]>[<+>-]>>>[>+>+<<-]<<<<[>+>>>>-<<<<<-]>[<+>-]>>>>>>,>>>,<<<[>+>+<<-]<<<<<<<[>+>>>>>>>-<<<<<<<<-]>>>>>>>>>>[>+>+<<-]<<<<<<<<<[>>>>>>>>>>-<<<<<<<<<<<+>-]>>>>>>>>>>>>>++[>++++++[>+++>++++>+++++<<<-]<-]>>---->------>+[<]<<<<<<<<<<<<<[>.[-]]>[[-]>]>>.[-]>>>>>>>>>>.>.<.<<<<<<<<[>.[-]]>[[-]>]>>.[-]>>>>.>>.[-]<[-]<.[-]<<<<<<<<<<<<<<[>>>++++++++++<<<-]>>>>>>[>>>++++++++++<<<-]>>>[<<<<<<<<<+>>>>>>>>>-]<<<<<<[<<<<+>>>>-]<<<<[>[>+>+<<-]>>[<<+>>-]<<<-]<[-]>>[-]>[>+++[>+++<-]>+<<[>+>>+<<<-]>>>[<<<+>>>-]<<[->->+<[>>>]>[<++++++++++>---------->>>>+<]<<<<<]>[-]>[<<+>>-]>>>>[<<<<<+>>>>>-]<<<<<<<[-]+>>]<<[+++++[>++++++++<-]>.[-]<<<]<<";
            
            InterpreterResult result = Brainf_ckInterpreter.Run(script, "9985", 64, OverflowMode.UshortWithNoOverflow);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.ExitCode, ExitCode.TextOutput);
            Assert.AreEqual(result.Stdout, "99 * 85 = 8415");
        }

        [TestMethod]
        public void Fibonacci()
        {
            const string script = "[.,-.,,,(<).]++++++++[>++++>++++++<<-]>>.[>+>+<<-]>>[<<+>>-],>,<<[>->-<<-]>[<++++++++++>-]>[<<+>>-]<<-1>>>+>>>+(->[>+<-]>[>>++++++++++<<[>+>>+<<<-]>>>[<<<+>>>-]<<[->->+<[>>>]=>[<++++++++++>---------->>>>+<]<<<<<]>[-]>[<<+>>-]>>>>[<<<<<+>>>>>-]<<<<<<<[-]+>>]<<[+++++[>++++++++<-]>.[-]<<<])<<<<<(<[<<.>>->>>1[>+>+>>+<<<<-]>>[<<+>>-]<<<[>+<-]>>[<<+>>-]>>:+<<<<<:]):";
            
            InterpreterResult result = Brainf_ckInterpreter.Run(script, "24", 64, OverflowMode.UshortWithNoOverflow);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.ExitCode, ExitCode.TextOutput);
            Assert.AreEqual(result.Stdout, "0 1 1 2 3 5 8 13 21 34 55 89 144 233 377 610 987 1597 2584 4181 6765 10946 17711 28657");
        }
    }
}
