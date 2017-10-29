using System;
using Brainf_ck_sharp.ReturnTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ck_sharp.Unit
{
    [TestClass]
    [TestCategory(nameof(BasicScripts))]
    public class BasicScripts
    {
        [TestMethod]
        public void BaseOperators1()
        {
            const String script = "+++++";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Success));
            Assert.AreEqual(result.Output, String.Empty);
            Assert.IsTrue(result.MachineState.Current.Value == 5);
        }

        [TestMethod]
        public void BaseOperators2()
        {
            const String script = "+++++---";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Success));
            Assert.AreEqual(result.Output, String.Empty);
            Assert.IsTrue(result.MachineState.Current.Value == 2);
        }

        [TestMethod]
        public void BaseOperators3()
        {
            const String script = ",++";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, "0");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Success));
            Assert.AreEqual(result.Output, String.Empty);
            Assert.IsTrue(result.MachineState.Current.Value == 50);
        }

        [TestMethod]
        public void BaseOperators4()
        {
            const String script = ",+++++++.";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, "0");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Success));
            Assert.AreEqual(result.Output, "7");
            Assert.IsTrue(result.MachineState.Current.Value == 55);
        }
    }
}
