using System;
using Brainf_ck_sharp;
using Brainf_ck_sharp.ReturnTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ck_sharp_Test
{
    [TestClass]
    [TestCategory(nameof(PBrainTests))]
    public class PBrainTests
    {
        [TestMethod]
        public void Test1()
        {
            const String script = "(+++):>:";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.HasFlag(InterpreterExitCode.Success) &&
                          result.HasFlag(InterpreterExitCode.NoOutput));
            Assert.IsTrue(result.MachineState[0].Value == 3 && result.MachineState[1].Value == 3);
            Assert.IsTrue(result.TotalOperations == 9);
        }

        [TestMethod]
        public void Test2()
        {
            const String script = "+(,[>+<-]>.)>+:";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, "a");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.HasFlag(InterpreterExitCode.Success) &&
                          result.HasFlag(InterpreterExitCode.TextOutput));
            Assert.IsTrue(result.MachineState.Current.Value == 'a');
            Assert.IsTrue(result.Output.Equals("a"));
            Assert.IsTrue(result.TotalOperations == 8 + 'a' * 5);
        }

        [TestMethod]
        public void BreakpointTest1()
        {
            String[] script = { "(+):+++++" };
            InterpreterExecutionSession result = Brainf_ckInterpreter.InitializeSession(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.CanContinue);
            Assert.IsTrue(result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.IsTrue(result.CurrentResult.MachineState.Current.Value == 6);
        }

        [TestMethod]
        public void BreakpointTest2()
        {
            String[] script = { "++(>-+)>++:" };
            InterpreterExecutionSession result = Brainf_ckInterpreter.InitializeSession(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.CanContinue);
            Assert.IsTrue(result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Failure) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NegativeValue) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.IsTrue(result.CurrentResult.MachineState.Current.Value == 0);
        }
    }
}
