using Brainf_ck_sharp.ReturnTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ck_sharp.Unit
{
    [TestClass]
    [TestCategory(nameof(ScriptTests))]
    public class ScriptTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            const string hello = "[]+++++[>+++++[>+++>++++[>+>+<<-]>>>+++++>+<<<<<<-]<-]>>---.>>+.>++++++++..+++.>>+++++++.<------.<.+++.------.<-.>>>+.";
            InterpreterResult result = Brainf_ckInterpreter.Run(hello, string.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Success));
            Assert.AreEqual(result.Output, "Hello world!");
        }

        [TestMethod]
        public void TestMethod2()
        {
            const string sum = "[,.,.,,.]++++++[>++++++++<-]>>,>>>,<<<[>+>+<<-]<[>+>-<<-]>[<+>-]>>>[>+>+<<-]<<<<[>+>>>>-<<<<<-]>[<+>-]>>>>>>,>>>,<<<[>+>+<<-]<<<<<<<[>+>>>>>>>-<<<<<<<<-]>>>>>>>>>>[>+>+<<-]<<<<<<<<<[>>>>>>>>>>-<<<<<<<<<<<+>-]>>>>>>>>>>>>>++[>++++++[>+++>++++>+++++<<<-]<-]>>---->----->+[<]<<<<<<<<<<<<<[>.[-]]>[[-]>]>>.[-]>>>>>>>>>>.>.<.<<<<<<<<[>.[-]]>[[-]>]>>.[-]>>>>.>>.[-]<[-]<.[-]<<<<<<<<<<<<<<[>>>++++++++++<<<-]>>>>>>[>>>++++++++++<<<-]>>>[<<<<<<<<<+>>>>>>>>>-]<<<<<<[<<<<+>>>>-]<<<<[>+<-]<[-]>>[>+++[>+++<-]>+<<[>+>>+<<<-]>>>[<<<+>>>-]<<[->->+<[>>>]>[<++++++++++>---------->>>>+<]<<<<<]>[-]>[<<+>>-]>>>>[<<<<<+>>>>>-]<<<<<<<[-]+>>]<<[+++++[>++++++++<-]>.[-]<<<]<";
            InterpreterResult result = Brainf_ckInterpreter.Run(sum, "2375");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Success));
            Assert.AreEqual(result.Output, "23 + 75 = 98");
        }

        [TestMethod]
        public void TestMethod3()
        {
            const string mul = "[,.,.,,.]++++++[>++++++++<-]>>,>>>,<<<[>+>+<<-]<[>+>-<<-]>[<+>-]>>>[>+>+<<-]<<<<[>+>>>>-<<<<<-]>[<+>-]>>>>>>,>>>,<<<[>+>+<<-]<<<<<<<[>+>>>>>>>-<<<<<<<<-]>>>>>>>>>>[>+>+<<-]<<<<<<<<<[>>>>>>>>>>-<<<<<<<<<<<+>-]>>>>>>>>>>>>>++[>++++++[>+++>++++>+++++<<<-]<-]>>---->------>+[<]<<<<<<<<<<<<<[>.[-]]>[[-]>]>>.[-]>>>>>>>>>>.>.<.<<<<<<<<[>.[-]]>[[-]>]>>.[-]>>>>.>>.[-]<[-]<.[-]<<<<<<<<<<<<<<[>>>++++++++++<<<-]>>>>>>[>>>++++++++++<<<-]>>>[<<<<<<<<<+>>>>>>>>>-]<<<<<<[<<<<+>>>>-]<<<<[>[>+>+<<-]>>[<<+>>-]<<<-]<[-]>>[-]>[>+++[>+++<-]>+<<[>+>>+<<<-]>>>[<<<+>>>-]<<[->->+<[>>>]>[<++++++++++>---------->>>>+<]<<<<<]>[-]>[<<+>>-]>>>>[<<<<<+>>>>>-]<<<<<<<[-]+>>]<<[+++++[>++++++++<-]>.[-]<<<]<<";
            InterpreterResult result = Brainf_ckInterpreter.Run(mul, "9985");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Success));
            Assert.AreEqual(result.Output, "99 * 85 = 8415");
        }
    }
}
