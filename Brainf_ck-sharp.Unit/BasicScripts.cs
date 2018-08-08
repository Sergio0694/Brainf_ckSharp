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
            const string script = "+++++";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, string.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Success));
            Assert.AreEqual(result.Output, string.Empty);
            Assert.IsTrue(result.MachineState.Current.Value == 5);
        }

        [TestMethod]
        public void BaseOperators2()
        {
            const string script = "+++++---";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, string.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Success));
            Assert.AreEqual(result.Output, string.Empty);
            Assert.IsTrue(result.MachineState.Current.Value == 2);
        }

        [TestMethod]
        public void BaseOperators3()
        {
            const string script = ",++";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, "0");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Success));
            Assert.AreEqual(result.Output, string.Empty);
            Assert.IsTrue(result.MachineState.Current.Value == 50);
        }

        [TestMethod]
        public void BaseOperators4()
        {
            const string script = ",+++++++.";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, "0");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Success));
            Assert.AreEqual(result.Output, "7");
            Assert.IsTrue(result.MachineState.Current.Value == 55);
        }
    }
}
