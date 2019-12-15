using Brainf_ck_sharp.NET.Enums;
using Brainf_ck_sharp.NET.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ck_sharp.NET.Unit
{
    [TestClass]
    public class BasicScriptTest
    {
        [TestMethod]
        public void BaseOperators1()
        {
            const string script = "+++++";

            InterpreterResult result = Brainf_ckInterpreter.Run(script);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.ExitCode, ExitCode.NoOutput);
            Assert.AreEqual(result.Stdout, string.Empty);
            Assert.AreEqual(result.MachineState.Current.Value, 5);
        }

        [TestMethod]
        public void BaseOperators2()
        {
            const string script = "+++++---";

            InterpreterResult result = Brainf_ckInterpreter.Run(script);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.ExitCode, ExitCode.NoOutput);
            Assert.AreEqual(result.Stdout, string.Empty);
            Assert.AreEqual(result.MachineState.Current.Value, 2);
        }

        [TestMethod]
        public void BaseOperators3()
        {
            const string script = ",++.";

            InterpreterResult result = Brainf_ckInterpreter.Run(script, "0");

            Assert.IsNotNull(result);
            Assert.AreEqual(result.ExitCode, ExitCode.TextOutput);
            Assert.AreEqual(result.Stdout, "2");
            Assert.AreEqual(result.MachineState.Current.Value, 50);
        }
    }
}
