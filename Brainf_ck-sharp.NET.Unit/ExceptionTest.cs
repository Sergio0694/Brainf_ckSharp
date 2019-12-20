using System.Threading;
using Brainf_ck_sharp.NET.Enums;
using Brainf_ck_sharp.NET.Models;
using Brainf_ck_sharp.NET.Models.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ck_sharp.NET.Unit
{
    [TestClass]
    public class ExceptionTest
    {
        [TestMethod]
        public void NegativeValue()
        {
            const string script = "+++>>-++";

            Option<InterpreterResult> result = Brainf_ckInterpreter.TryRun(script);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(result.Value!.ExitCode, ExitCode.NegativeValue);
            Assert.AreEqual(result.Value.Stdout, string.Empty);
            Assert.AreEqual(result.Value.MachineState.Current.Value, 0);
            Assert.AreEqual(result.Value.StackTrace.Count, 1);
            Assert.AreEqual(result.Value.StackTrace[0], "+++>>-");
        }

        [TestMethod]
        public void ThresholdExceeded()
        {
            CancellationTokenSource cts = new CancellationTokenSource(500);

            const string script = "+[+-]";

            Option<InterpreterResult> result = Brainf_ckInterpreter.TryRun(script, cts.Token);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(result.Value!.ExitCode, ExitCode.ThresholdExceeded);
            Assert.AreEqual(result.Value.Stdout, string.Empty);
            Assert.AreEqual(result.Value.StackTrace.Count, 1);
        }

        [TestMethod]
        public void StackOverflow()
        {
            const string script = "(:):";

            Option<InterpreterResult> result = Brainf_ckInterpreter.TryRun(script);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(result.Value!.ExitCode, ExitCode.StackLimitExceeded);
            Assert.AreEqual(result.Value.Stdout, string.Empty);
            Assert.AreEqual(result.Value.StackTrace.Count, 512);
            Assert.AreEqual(result.Value.StackTrace[0], ":");
            Assert.AreEqual(result.Value.StackTrace[^1], "(:):");
        }

        [TestMethod]
        public void StdinBufferExhausted()
        {
            const string script = ",";

            Option<InterpreterResult> result = Brainf_ckInterpreter.TryRun(script);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(result.Value!.ExitCode, ExitCode.StdinBufferExhausted);
            Assert.AreEqual(result.Value.Stdout, string.Empty);
            Assert.AreEqual(result.Value.StackTrace.Count, 1);
            Assert.AreEqual(result.Value.StackTrace[0], ",");
        }

        [TestMethod]
        public void StdoutBufferLimitExceeded()
        {
            const string script = ",[.]";

            Option<InterpreterResult> result = Brainf_ckInterpreter.TryRun(script, "a");

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(result.Value!.ExitCode, ExitCode.StdoutBufferLimitExceeded);
            Assert.AreEqual(result.Value.Stdout, new string('a', 1024));
            Assert.AreEqual(result.Value.StackTrace.Count, 1);
            Assert.AreEqual(result.Value.StackTrace[0], ",[.");
        }
    }
}
