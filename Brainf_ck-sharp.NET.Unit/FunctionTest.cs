using Brainf_ck_sharp.NET.Enums;
using Brainf_ck_sharp.NET.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ck_sharp.NET.Unit
{
    [TestClass]
    public class FunctionTest
    {
        [TestMethod]
        public void SingleCall()
        {
            const string script = "+(,[>+<-]>.)>+:";

            InterpreterResult result = Brainf_ckInterpreter.Run(script, "a");

            Assert.IsNotNull(result);
            Assert.AreEqual(result.ExitCode, ExitCode.TextOutput);
            Assert.AreEqual(result.MachineState.Current.Character, 'a');
            Assert.AreEqual(result.Stdout, "a");
        }

        [TestMethod]
        public void MultipleCalls()
        {
            const string script = "(+++):>:";

            InterpreterResult result = Brainf_ckInterpreter.Run(script);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.ExitCode, ExitCode.NoOutput);
            Assert.AreEqual(result.MachineState[0].Value, 3);
            Assert.AreEqual(result.MachineState[1].Value, 3);
        }

        [TestMethod]
        public void Recursion()
        {
            const string script = ">,<(>[>+<-<:]):>[<<+>>-]<<.[-]";

            InterpreterResult result = Brainf_ckInterpreter.Run(script, "%");

            Assert.IsNotNull(result);
            Assert.AreEqual(result.ExitCode, ExitCode.TextOutput);
            Assert.AreEqual(result.MachineState.Current.Value, 0);
            Assert.AreEqual(result.Stdout, "%");
        }
    }
}
