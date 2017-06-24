using System;
using System.Linq;
using Brainf_ck_sharp;
using Brainf_ck_sharp.ReturnTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ck_sharp_Test
{
    [TestClass]
    [TestCategory(nameof(BreakpointTests))]
    public class BreakpointTests
    {
        [TestMethod]
        public void BreakpointTest1()
        {
            String[] script = { "+++++" };
            InterpreterExecutionSession result = Brainf_ckInterpreter.InitializeSession(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(!result.CanContinue);
            Assert.IsTrue(result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.IsTrue(result.CurrentResult.MachineState.Current.Value == 5);
        }

        [TestMethod]
        public void BreakpointTest2short()
        {
            String[] script = { "++", "-" };
            InterpreterExecutionSession result = Brainf_ckInterpreter.InitializeSession(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.CanContinue);
            Assert.IsTrue(result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.BreakpointReached) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.IsTrue(result.CurrentResult.MachineState.Current.Value == 2);
             InterpreterExecutionSession step = result.Continue();
            Assert.IsNotNull(step);
            Assert.IsFalse(step.CanContinue);
            Assert.IsTrue(step.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          step.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.IsTrue(step.CurrentResult.MachineState.Current.Value == 1);
        }

        [TestMethod]
        public void BreakpointTest2()
        {
            String[] script = { "+++++", "---" };
            InterpreterExecutionSession result = Brainf_ckInterpreter.InitializeSession(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.CanContinue);
            Assert.IsTrue(result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.BreakpointReached) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.IsTrue(result.CurrentResult.MachineState.Current.Value == 5);
            InterpreterExecutionSession step = result.Continue();
            Assert.IsNotNull(step);
            Assert.IsFalse(step.CanContinue);
            Assert.IsTrue(step.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          step.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.IsTrue(step.CurrentResult.MachineState.Current.Value == 2);
        }

        [TestMethod]
        public void BreakpointTest3short()
        {
            String[] script = { "++[", ">+<-]>" };
            InterpreterExecutionSession result = Brainf_ckInterpreter.InitializeSession(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.CanContinue);
            Assert.IsTrue(result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.BreakpointReached) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.IsTrue(result.CurrentResult.MachineState.Current.Value == 2);
            InterpreterExecutionSession step = result.RunToCompletion();
            Assert.IsNotNull(step);
            Assert.IsFalse(step.CanContinue);
            Assert.IsTrue(step.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          step.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.IsTrue(step.CurrentResult.MachineState.Current.Value == 2);
            while (result.CanContinue) result = result.Continue();
            Assert.IsNotNull(result);
            Assert.IsFalse(result.CanContinue);
            Assert.IsTrue(result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.IsTrue(result.CurrentResult.MachineState.Current.Value == 2);
        }

        [TestMethod]
        public void BreakpointTest3()
        {
            String[] script = { "+++++[", ">+<-]>" };
            InterpreterExecutionSession result = Brainf_ckInterpreter.InitializeSession(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.CanContinue);
            Assert.IsTrue(result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.BreakpointReached) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.IsTrue(result.CurrentResult.MachineState.Current.Value == 5);
            InterpreterExecutionSession step = result.RunToCompletion();
            Assert.IsNotNull(step);
            Assert.IsFalse(step.CanContinue);
            Assert.IsTrue(step.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          step.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.IsTrue(step.CurrentResult.MachineState.Current.Value == 5);
        }

        [TestMethod]
        public void BreakpointTest4()
        {
            String[] code = { "+++++[", ">+<-]>" };
            InterpreterExecutionSession result = Brainf_ckInterpreter.InitializeSession(code, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.CanContinue);
            Assert.IsTrue(result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.BreakpointReached) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.IsTrue(result.CurrentResult.MachineState.Current.Value == 5);
            result = result.Continue();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.CanContinue);
            Assert.IsTrue(result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.BreakpointReached));
            while (result.CanContinue) result = result.Continue();
            InterpreterResult test = Brainf_ckInterpreter.Run(code.Aggregate(String.Empty, (s, v) => s + v), String.Empty);
            Assert.IsTrue(test.MachineState.Current == result.CurrentResult.MachineState.Current);
        }

        [TestMethod]
        public void BreakpointTest5()
        {
            String[] hello = { "[]+++++[>+++++[>+++>++", "++[>+>+<<-]>>>+++++>+<<<<<<", "-]<-]>>---.>>+.>++++++++..+++.>>+", "++++++.<------.<.+++.------.<-.>>>+." };
            InterpreterExecutionSession result = Brainf_ckInterpreter.InitializeSession(hello, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.CanContinue);
            Assert.IsTrue(result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.BreakpointReached));
            InterpreterExecutionSession completion = result.RunToCompletion();
            Assert.IsNotNull(completion);
            Assert.IsFalse(completion.CanContinue);
            Assert.IsTrue(completion.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          completion.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.TextOutput));
            while (result.CanContinue) result = result.Continue();
            Assert.IsFalse(result.CanContinue);
            Assert.IsTrue(result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.TextOutput));
            InterpreterResult test = Brainf_ckInterpreter.Run(hello.Aggregate(String.Empty, (s, v) => s + v), String.Empty);
            Assert.AreEqual(test.Output, result.CurrentResult.Output);
            Assert.AreEqual(test.Output, completion.CurrentResult.Output);
        }

        [TestMethod]
        public void BreakpointTest6()
        {
            String[] script = { "+++++[>+<", "-]>" };
            InterpreterExecutionSession result = Brainf_ckInterpreter.InitializeSession(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.CanContinue);
            Assert.IsTrue(result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.BreakpointReached) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            InterpreterExecutionSession completion = result.RunToCompletion();
            while (result.CanContinue) result = result.Continue();
            Assert.IsNotNull(completion);
            Assert.IsFalse(completion.CanContinue);
            Assert.IsFalse(result.CanContinue);
            InterpreterResult test = Brainf_ckInterpreter.Run(script.Aggregate(String.Empty, (s, v) => s + v), String.Empty);
            Assert.IsTrue(completion.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          completion.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.IsTrue(test.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          test.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.IsTrue(result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.IsTrue(completion.CurrentResult.MachineState.Current == test.MachineState.Current &&
                          result.CurrentResult.MachineState.Current == test.MachineState.Current);
        }

        [TestMethod]
        public void BreakpointTest7()
        {
            String[] script = { "++[>++[>", "+<", "-]<-]>>" };
            InterpreterExecutionSession result = Brainf_ckInterpreter.InitializeSession(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.CanContinue);
            Assert.IsTrue(result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.BreakpointReached) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            InterpreterExecutionSession completion = result.RunToCompletion();
            while (result.CanContinue) result = result.Continue();
            Assert.IsNotNull(completion);
            Assert.IsFalse(completion.CanContinue);
            Assert.IsFalse(result.CanContinue);
            InterpreterResult test = Brainf_ckInterpreter.Run(script.Aggregate(String.Empty, (s, v) => s + v), String.Empty);
            Assert.IsTrue(completion.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          completion.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.IsTrue(test.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          test.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.IsTrue(result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.IsTrue(completion.CurrentResult.MachineState.Current == test.MachineState.Current);
            Assert.IsTrue(result.CurrentResult.MachineState.Current == test.MachineState.Current);
        }

        [TestMethod]
        public void BreakpointTest8()
        {
            String[] script = { "++[>++[>", "+<", "-]<-]>>" };
            InterpreterExecutionSession result = Brainf_ckInterpreter.InitializeSession(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.CanContinue);
            Assert.IsTrue(result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.BreakpointReached) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            InterpreterExecutionSession completion = result.RunToCompletion();
            while (result.CanContinue) result = result.Continue();
            Assert.IsNotNull(completion);
            Assert.IsFalse(completion.CanContinue);
            Assert.IsFalse(result.CanContinue);
            InterpreterResult test = Brainf_ckInterpreter.Run(script.Aggregate(String.Empty, (s, v) => s + v), String.Empty);
            Assert.IsTrue(completion.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          completion.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.IsTrue(test.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          test.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.IsTrue(result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.IsTrue(completion.CurrentResult.MachineState.Current == test.MachineState.Current);
            Assert.IsTrue(result.CurrentResult.MachineState.Current == test.MachineState.Current);
        }

        [TestMethod]
        public void TestMethod9()
        {
            String[] script = { "[,.,.,,.]++++++[>+++++++", "+<-]>>,>>>,<<<[>+>+<<-]<[>+>-<<-]>[<+>-]>>>[>+>+<<", "-]<<<<[>+", ">", ">>>-<<<<<-]>[<+>-]>>>>>>,>>>,<<<[>+>+<<-]<<<<<<<[>+>>>>>>>-<<<<<<<<-]>>>>>>>>>>[>+>+<<-]<<", "<<<<<<<[>>>>>>>>>>-<<<<<<<<<<<+>-]>>>>>>>>>>>>>++[>++++++[>+++>++++>+++++<<<", "-]<-", "]>>---->------>+[<]<<<<<<<<<<<<<[>.[-]", "]", ">[[", "-]>]>>.[-]>>", ">>>>>>>>.>.<.<<<<<<<<[>.[-]]>[[-]>]>>.[-]>>>>", ".>>.[", "-]<[-]<.[-]<<<<<<<<<<<<<<[>>>++++++++++<", "<<-]>>>>>>[>>>++++++++++<<<-]>>>[<<<<<<<<<+>>>>>>>>>-]<<<<<<[<<<<+>>>>-]<<<", "<", "[", ">[>+>+<<-]>>[<", "<+>>-]<<<-]<[-]>>[-]>[>+++[>+++<-]>+<<[>+>>+<<<-]>>>[<<<+>>>-]<<[->->+<[>>>]>[<++++++++++>---------->>>>+<]<<<<<]>[", "-", "]>[<<+>>-]>>>>[<<<<<+>>>>>-]<<<<<<<[-]+>>]<<[+++++[>++++++++<-]>.[-]<<<]<<" };
            const String argument = "9985", result = "99 * 85 = 8415";
            InterpreterResult test = Brainf_ckInterpreter.Run(script.Aggregate(String.Empty, (s, v) => s + v), argument);
            Assert.IsNotNull(test);
            Assert.IsTrue(test.HasFlag(InterpreterExitCode.Success));
            Assert.AreEqual(test.Output, result);
            InterpreterExecutionSession
                initial = Brainf_ckInterpreter.InitializeSession(script, argument),
                skip2 = initial.Continue().Continue().RunToCompletion(),
                direct = initial.RunToCompletion();
            while (initial.CanContinue) initial = initial.Continue();
            foreach (InterpreterExecutionSession session in new[] { skip2, direct, initial })
            {
                Assert.IsNotNull(session);
                Assert.IsTrue(session.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                              session.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.TextOutput));
                Assert.AreEqual(session.CurrentResult.Output, test.Output);
            }
        }
    }
}
