using System;
using Brainf_ck_sharp;
using Brainf_ck_sharp.ReturnTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ck_sharp_Test
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
            Assert.IsTrue(result.HasFlag(InterpreterExitCode.Success));
            Assert.AreEqual(result.Output, String.Empty);
            Assert.IsTrue(result.MachineState.Current == 5);
        }

        [TestMethod]
        public void BaseOperators2()
        {
            const String script = "+++++---";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.HasFlag(InterpreterExitCode.Success));
            Assert.AreEqual(result.Output, String.Empty);
            Assert.IsTrue(result.MachineState.Current == 2);
        }

        [TestMethod]
        public void BaseOperators3()
        {
            const String script = ",++";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, "0");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.HasFlag(InterpreterExitCode.Success));
            Assert.AreEqual(result.Output, String.Empty);
            Assert.IsTrue(result.MachineState.Current == 50);
        }

        [TestMethod]
        public void BaseOperators4()
        {
            const String script = ",+++++++.";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, "0");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.HasFlag(InterpreterExitCode.Success));
            Assert.AreEqual(result.Output, "7");
            Assert.IsTrue(result.MachineState.Current == 55);
        }
    }
}
