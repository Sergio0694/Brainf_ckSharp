using System;
using Brainf_ck_sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ck_sharp_Test
{
    [TestClass]
    [TestCategory(nameof(LoopTests))]
    public class LoopTests
    {
        [TestMethod]
        public void Loop1()
        {
            const String script = "[]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue((result.ExitCode & InterpreterExitCode.Success) == InterpreterExitCode.Success);
            Assert.AreEqual(result.Output, String.Empty);
            Assert.IsTrue(result.MachineState.Current == 0);
        }

        [TestMethod]
        public void Loop2()
        {
            const String script = ",[-]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, "0");
            Assert.IsNotNull(result);
            Assert.IsTrue((result.ExitCode & InterpreterExitCode.Success) == InterpreterExitCode.Success);
            Assert.AreEqual(result.Output, String.Empty);
            Assert.IsTrue(result.MachineState.Current == 0);
        }

        [TestMethod]
        public void Loop3()
        {
            const String script = ",[>+<-]>.";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, "0");
            Assert.IsNotNull(result);
            Assert.IsTrue((result.ExitCode & InterpreterExitCode.Success) == InterpreterExitCode.Success);
            Assert.AreEqual(result.Output, "0");
            Assert.IsTrue(result.MachineState.Current == 48);
        }

        [TestMethod]
        public void Loop4()
        {
            const String script = "++[>++[>+<-]<-]>,[>+<-]>.";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, "0");
            Assert.IsNotNull(result);
            Assert.IsTrue((result.ExitCode & InterpreterExitCode.Success) == InterpreterExitCode.Success);
            Assert.AreEqual(result.Output, "4");
            Assert.IsTrue(result.MachineState.Current == 52);
        }
    }
}
