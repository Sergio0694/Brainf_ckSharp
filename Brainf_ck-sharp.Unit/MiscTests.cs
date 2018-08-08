using System;
using System.Linq;
using Brainf_ck_sharp.ReturnTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ck_sharp.Unit
{
    [TestClass]
    [TestCategory(nameof(MiscTests))]
    public class MiscTests
    {
        [TestMethod]
        public void StackTrace1()
        {
            const string script = ">+++[>++-[>>+>>>>-]]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, string.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Failure) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.NegativeValue));
            Assert.AreEqual(result.Output, string.Empty);
            Assert.IsTrue(result.ExceptionInfo?.StackTrace.Count == 3);
        }

        [TestMethod]
        public void StackTrace2()
        {
            string[] script = { ">+++[>++-[>>", "+]]"};
            InterpreterExecutionSession result = Brainf_ckInterpreter.InitializeSession(script, string.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NoOutput) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.BreakpointReached));
            Assert.AreEqual(result.CurrentResult.Output, string.Empty);
            Assert.IsTrue(result.CurrentResult.ExceptionInfo?.StackTrace.Count == 3);
        }

        [TestMethod]
        public void StackTrace3()
        {
            string[] script = { "(>>>+", "++)>+++[>++-[>>+>>>>>:]]" };
            InterpreterExecutionSession result = Brainf_ckInterpreter.InitializeSession(script, string.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NoOutput) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.BreakpointReached));
            Assert.AreEqual(result.CurrentResult.Output, string.Empty);
            Assert.IsTrue(result.CurrentResult.ExceptionInfo?.StackTrace.Count == 4);
        }

        [TestMethod]
        public void TotalOperations1()
        {
            const string script = "+++++";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, string.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.AreEqual(result.Output, string.Empty);
            Assert.IsTrue(result.TotalOperations == 5);
        }

        [TestMethod]
        public void TotalOperations2()
        {
            const string script = "++[-]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script,String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.AreEqual(result.Output, string.Empty);
            Assert.IsTrue(result.TotalOperations == 7);
        }

        [TestMethod]
        public void TotalOperations3()
        {
            const string script = "++[>++[>+<-]<-]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, string.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.AreEqual(result.Output, string.Empty);
            Assert.IsTrue(result.TotalOperations == 37);
        }

        [TestMethod]
        public void TotalOperations3Header()
        {
            string[] script = { "[.", ",]++[>++[>+<", "-]<-]" };
            InterpreterResult result = Brainf_ckInterpreter.Run(script.Aggregate(String.Empty, (s, v) => s + v), string.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.AreEqual(result.Output, string.Empty);
            Assert.IsTrue(result.TotalOperations == 38);
            InterpreterExecutionSession
                session = Brainf_ckInterpreter.InitializeSession(script, string.Empty),
                completion = Brainf_ckInterpreter.InitializeSession(script, string.Empty);
            while (session.CanContinue) session.Continue();
            completion.RunToCompletion();
            Assert.IsTrue(completion.CurrentResult.TotalOperations == 38);
            Assert.IsTrue(session.CurrentResult.TotalOperations == 38);
        }

        [TestMethod]
        public void TotalOperations4()
        {
            string[] script = { "+[>", "+[>+", "<-]<-]" };
            InterpreterResult test = Brainf_ckInterpreter.Run(script.Aggregate(String.Empty, (s, g) => s + g), string.Empty);
            Assert.IsTrue(test.TotalOperations == 13);
            InterpreterExecutionSession
                session = Brainf_ckInterpreter.InitializeSession(script, string.Empty),
                completion = Brainf_ckInterpreter.InitializeSession(script, string.Empty);
            while (session.CanContinue) session.Continue();
            completion.RunToCompletion();
            Assert.IsTrue(completion.CurrentResult.TotalOperations == 13);
            Assert.IsTrue(session.CurrentResult.TotalOperations == 13);
        }

        [TestMethod]
        public void TotalOperations5()
        {
            string[] script = { "++[>", "+++[>+", "<-]<-]" };
            InterpreterResult test = Brainf_ckInterpreter.Run(script.Aggregate(String.Empty, (s, g) => s + g), string.Empty);
            InterpreterExecutionSession
                session = Brainf_ckInterpreter.InitializeSession(script, string.Empty),
                completion = Brainf_ckInterpreter.InitializeSession(script, string.Empty);
            while (session.CanContinue) session.Continue();
            completion.RunToCompletion();
            Assert.IsTrue(completion.CurrentResult.TotalOperations == test.TotalOperations);
            Assert.IsTrue(session.CurrentResult.TotalOperations == test.TotalOperations);
        }

        [TestMethod]
        public void TotalOperations5Loops()
        {
            string[] script = { "[.,]++[>++[>+", "+[>+<-]<-]<-]" };
            InterpreterResult test = Brainf_ckInterpreter.Run(script.Aggregate(String.Empty, (s, g) => s + g), string.Empty);
            InterpreterExecutionSession
                session = Brainf_ckInterpreter.InitializeSession(script, string.Empty),
                completion = Brainf_ckInterpreter.InitializeSession(script, string.Empty);
            while (session.CanContinue) session.Continue();
            completion.RunToCompletion();
            Assert.IsTrue(completion.CurrentResult.TotalOperations == test.TotalOperations);
            Assert.IsTrue(session.CurrentResult.TotalOperations == test.TotalOperations);
        }

        [TestMethod]
        public void TotalOperations6()
        {
            string[] hello = { "[]+++++[>+++++[>+++>++", "++[>+>+<<-]>>>+++++>+<<<<<<", "-]<-]>>---.>>+.>++++++++..+++.>>+", "++++++.<------.<.+++.------.<-.>>>+." };
            InterpreterResult test = Brainf_ckInterpreter.Run(hello.Aggregate(String.Empty, (s, g) => s + g), string.Empty);
            InterpreterExecutionSession
                session = Brainf_ckInterpreter.InitializeSession(hello, string.Empty),
                skip2 = Brainf_ckInterpreter.InitializeSession(hello, string.Empty),
                completion = Brainf_ckInterpreter.InitializeSession(hello, string.Empty);
            while (session.CanContinue) session.Continue();
            skip2.Continue();
            skip2.Continue();
            skip2.RunToCompletion();
            completion.RunToCompletion();
            Assert.IsTrue(completion.CurrentResult.TotalOperations == test.TotalOperations);
            Assert.IsTrue(session.CurrentResult.TotalOperations == test.TotalOperations);
            Assert.IsTrue(skip2.CurrentResult.TotalOperations == test.TotalOperations);
        }

        [TestMethod]
        public void TotalOperationsSmall()
        {
            string[] hello = { "++[", "-]" };
            InterpreterExecutionSession session = Brainf_ckInterpreter.InitializeSession(hello, "€");
            while (session.CanContinue) session.Continue();
            Assert.IsTrue(session.CurrentResult.TotalOperations == 7);
        }

        [TestMethod]
        public void TotalOperationsOptimized1()
        {
            const int operations = 16730;
            string[] hello = { ",[-", "]" };
            InterpreterResult test = Brainf_ckInterpreter.Run(hello.Aggregate(String.Empty, (s, g) => s + g), "€");
            Assert.IsTrue(test.TotalOperations == operations);
            InterpreterExecutionSession session = Brainf_ckInterpreter.InitializeSession(hello, "€");
            while (session.CanContinue) session.Continue();
            Assert.IsTrue(session.CurrentResult.TotalOperations == operations);
        }

        [TestMethod]
        public void TotalOperationsOptimized2()
        {
            const int operations = 33482;
            string[] hello = { ",+++[-", "]>+,++[", "-]++++." };
            InterpreterResult test = Brainf_ckInterpreter.Run(hello.Aggregate(String.Empty, (s, g) => s + g), "€€");
            Assert.IsTrue(test.TotalOperations == operations);
            InterpreterExecutionSession
                session = Brainf_ckInterpreter.InitializeSession(hello, "€€"),
                skip = Brainf_ckInterpreter.InitializeSession(hello, "€€"),
                completion = Brainf_ckInterpreter.InitializeSession(hello, "€€");
            while (session.CanContinue) session.Continue();
            skip.Continue();
            skip.RunToCompletion();
            completion.RunToCompletion();
            Assert.IsTrue(completion.CurrentResult.TotalOperations == test.TotalOperations);
            Assert.IsTrue(session.CurrentResult.TotalOperations == test.TotalOperations);
            Assert.IsTrue(skip.CurrentResult.TotalOperations == test.TotalOperations);
        }

        [TestMethod]
        public void TotalOperationsOptimized3()
        {
            string[] hello = { ",+++[-]", ">+++[-]", ",[", "-],[-", "]" };
            InterpreterResult test = Brainf_ckInterpreter.Run(hello.Aggregate(String.Empty, (s, g) => s + g), "€€€");
            InterpreterExecutionSession
                session = Brainf_ckInterpreter.InitializeSession(hello, "€€€"),
                skip = Brainf_ckInterpreter.InitializeSession(hello, "€€€"),
                completion = Brainf_ckInterpreter.InitializeSession(hello, "€€€");
            while (session.CanContinue) session.Continue();
            skip.Continue();
            skip.Continue();
            skip.Continue();
            skip.RunToCompletion();
            completion.RunToCompletion();
            Assert.IsTrue(completion.CurrentResult.TotalOperations == test.TotalOperations);
            Assert.IsTrue(session.CurrentResult.TotalOperations == test.TotalOperations);
            Assert.IsTrue(skip.CurrentResult.TotalOperations == test.TotalOperations);
        }

        [TestMethod]
        public void TotalOperations7()
        {
            string[] script = { "++++++[>+++++++", "+<-]>>,>>>,<<<[>+>+<<-]<[>+>-<<-]>[<+>-]>>>[>+>+<<", "-]<<<<[>+", ">", ">>>-<<<<<-]>[<+>-]>>>>>>,>>>,<<<[>+>+<<-]<<<<<<<[>+>>>>>>>-<<<<<<<<-]>>>>>>>>>>[>+>+<<-]<<", "<<<<<<<[>>>>>>>>>>-<<<<<<<<<<<+>-]>>>>>>>>>>>>>++[>++++++[>+++>++++>+++++<<<", "-]<-", "]>>---->------>+[<]<<<<<<<<<<<<<[>.[-]", "]", ">[[", "-]>]>>.[-]>>", ">>>>>>>>.>.<.<<<<<<<<[>.[-]]>[[-]>]>>.[-]>>>>", ".>>.[", "-]<[-]<.[-]<<<<<<<<<<<<<<[>>>++++++++++<", "<<-]>>>>>>[>>>++++++++++<<<-]>>>[<<<<<<<<<+>>>>>>>>>-]<<<<<<[<<<<+>>>>-]<<<", "<", "[", ">[>+>+<<-]>>[<", "<+>>-]<<<-]<[-]>>[-]>[>+++[>+++<-]>+<<[>+>>+<<<-]>>>[<<<+>>>-]<<[->->+<[>>>]>[<++++++++++>---------->>>>+<]<<<<<]>[", "-", "]>[<<+>>-]>>>>[<<<<<+>>>>>-]<<<<<<<[-]+>>]<<[+++++[>++++++++<-]>.[-]<<<]<<" };
            const string argument = "9985", result = "99 * 85 = 8415";
            InterpreterResult test = Brainf_ckInterpreter.Run(script.Aggregate(String.Empty, (s, g) => s + g), argument);
            InterpreterExecutionSession
                session = Brainf_ckInterpreter.InitializeSession(script, argument),
                skip2 = Brainf_ckInterpreter.InitializeSession(script, argument),
                completion = Brainf_ckInterpreter.InitializeSession(script, argument);
            while (session.CanContinue) session.Continue();
            skip2.Continue();
            skip2.Continue();
            skip2.RunToCompletion();
            completion.RunToCompletion();
            Assert.AreEqual(test.Output, result);
            Assert.AreEqual(completion.CurrentResult.Output, result);
            Assert.AreEqual(skip2.CurrentResult.Output, result);
            Assert.AreEqual(session.CurrentResult.Output, result);
            Assert.IsTrue(completion.CurrentResult.TotalOperations == test.TotalOperations);
            Assert.IsTrue(session.CurrentResult.TotalOperations == test.TotalOperations);
            Assert.IsTrue(skip2.CurrentResult.TotalOperations == test.TotalOperations);
        }

        [TestMethod]
        public void TotalOperations8()
        {
            string[] script = { "[,.,.,,.]++++++[>+++++++", "+<-]>>,>>>,<<<[>+>+<<-]<[>+>-<<-]>[<+>-]>>>[>+>+<<", "-]<<<<[>+", ">", ">>>-<<<<<-]>[<+>-]>>>>>>,>>>,<<<[>+>+<<-]<<<<<<<[>+>>>>>>>-<<<<<<<<-]>>>>>>>>>>[>+>+<<-]<<", "<<<<<<<[>>>>>>>>>>-<<<<<<<<<<<+>-]>>>>>>>>>>>>>++[>++++++[>+++>++++>+++++<<<", "-]<-", "]>>---->------>+[<]<<<<<<<<<<<<<[>.[-]", "]", ">[[", "-]>]>>.[-]>>", ">>>>>>>>.>.<.<<<<<<<<[>.[-]]>[[-]>]>>.[-]>>>>", ".>>.[", "-]<[-]<.[-]<<<<<<<<<<<<<<[>>>++++++++++<", "<<-]>>>>>>[>>>++++++++++<<<-]>>>[<<<<<<<<<+>>>>>>>>>-]<<<<<<[<<<<+>>>>-]<<<", "<", "[", ">[>+>+<<-]>>[<", "<+>>-]<<<-]<[-]>>[-]>[>+++[>+++<-]>+<<[>+>>+<<<-]>>>[<<<+>>>-]<<[->->+<[>>>]>[<++++++++++>---------->>>>+<]<<<<<]>[", "-", "]>[<<+>>-]>>>>[<<<<<+>>>>>-]<<<<<<<[-]+>>]<<[+++++[>++++++++<-]>.[-]<<<]<<" };
            const string argument = "9985", result = "99 * 85 = 8415";
            InterpreterResult test = Brainf_ckInterpreter.Run(script.Aggregate(String.Empty, (s, g) => s + g), argument);
            InterpreterExecutionSession
                session = Brainf_ckInterpreter.InitializeSession(script, argument),
                skip2 = Brainf_ckInterpreter.InitializeSession(script, argument),
                completion = Brainf_ckInterpreter.InitializeSession(script, argument);
            while (session.CanContinue) session.Continue();
            skip2.Continue();
            skip2.Continue();
            skip2.RunToCompletion();
            completion.RunToCompletion();
            Assert.AreEqual(test.Output, result);
            Assert.AreEqual(completion.CurrentResult.Output, result);
            Assert.AreEqual(skip2.CurrentResult.Output, result);
            Assert.AreEqual(session.CurrentResult.Output, result);
            Assert.IsTrue(completion.CurrentResult.TotalOperations == test.TotalOperations);
            Assert.IsTrue(session.CurrentResult.TotalOperations == test.TotalOperations);
            Assert.IsTrue(skip2.CurrentResult.TotalOperations == test.TotalOperations);
        }

        [TestMethod]
        public void TotalOperations9()
        {
            const string script = "+>[+++-[++>>>]]+";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, string.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.AreEqual(result.Output, string.Empty);
            Assert.IsTrue(result.TotalOperations == 4);
            InterpreterExecutionSession session = Brainf_ckInterpreter.InitializeSession(new[] { "+>[+++", "-[++>>>]]+" }, string.Empty);
            Assert.IsTrue(!session.CanContinue);
            Assert.IsTrue(session.CurrentResult.TotalOperations == result.TotalOperations);
        }
    }
}
