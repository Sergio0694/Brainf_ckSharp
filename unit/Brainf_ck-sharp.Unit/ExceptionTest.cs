using System.Linq;
using System.Threading;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ckSharp.Unit
{
    [TestClass]
    public class ExceptionTest
    {
        [TestMethod]
        public void NegativeValue()
        {
            const string script = "+++>>-++";

            using InterpreterSession? result = Brainf_ckInterpreter
                .CreateDebugConfiguration()
                .WithSource(script)
                .TryRun()
                .Value;

            Assert.IsNotNull(result);

            result!.MoveNext();

            Assert.IsNotNull(result.Current);
            Assert.AreEqual(result.Current.ExitCode, ExitCode.NegativeValue);
            Assert.AreEqual(result.Current.Stdout, string.Empty);
            Assert.AreEqual(result.Current.MachineState.Current.Value, 0);
            Assert.IsNotNull(result.Current.ExceptionInfo);
            Assert.AreEqual(result.Current.ExceptionInfo!.StackTrace.Count, 1);
            Assert.AreEqual(result.Current.ExceptionInfo.StackTrace[0], "+++>>-");
        }

        [TestMethod]
        public void ThresholdExceeded()
        {
            CancellationTokenSource cts = new CancellationTokenSource(500);

            const string script = "+[+-]";

            using InterpreterSession? result = Brainf_ckInterpreter
                .CreateDebugConfiguration()
                .WithSource(script)
                .WithExecutionToken(cts.Token)
                .TryRun()
                .Value;

            Assert.IsNotNull(result);

            result!.MoveNext();

            Assert.IsNotNull(result.Current);
            Assert.AreEqual(result.Current!.ExitCode, ExitCode.ThresholdExceeded);
            Assert.AreEqual(result.Current.Stdout, string.Empty);
            Assert.IsNotNull(result.Current.ExceptionInfo);
            Assert.AreEqual(result.Current.ExceptionInfo!.StackTrace.Count, 1);
        }

        [TestMethod]
        public void StackOverflow()
        {
            const string script = "(:):";

            using InterpreterSession? result = Brainf_ckInterpreter
                .CreateDebugConfiguration()
                .WithSource(script)
                .TryRun()
                .Value;

            Assert.IsNotNull(result);

            result!.MoveNext();

            Assert.IsNotNull(result.Current);
            Assert.AreEqual(result.Current!.ExitCode, ExitCode.StackLimitExceeded);
            Assert.AreEqual(result.Current.Stdout, string.Empty);
            Assert.IsNotNull(result.Current.ExceptionInfo);
            Assert.AreEqual(result.Current.ExceptionInfo!.StackTrace.Count, 512);
            Assert.AreEqual(result.Current.ExceptionInfo.StackTrace.First(), ":");
            Assert.AreEqual(result.Current.ExceptionInfo.StackTrace.Last(), "(:):");
        }

        [TestMethod]
        public void StdinBufferExhausted()
        {
            const string script = ",";

            using InterpreterSession? result = Brainf_ckInterpreter
                .CreateDebugConfiguration()
                .WithSource(script)
                .TryRun()
                .Value;

            Assert.IsNotNull(result);

            result!.MoveNext();

            Assert.IsNotNull(result.Current);
            Assert.AreEqual(result.Current!.ExitCode, ExitCode.StdinBufferExhausted);
            Assert.AreEqual(result.Current.Stdout, string.Empty);
            Assert.IsNotNull(result.Current.ExceptionInfo);
            Assert.AreEqual(result.Current.ExceptionInfo!.StackTrace.Count, 1);
            Assert.AreEqual(result.Current.ExceptionInfo.StackTrace[0], ",");
        }

        [TestMethod]
        public void StdoutBufferLimitExceeded()
        {
            const string script = ",[.]";

            using InterpreterSession? result = Brainf_ckInterpreter
                .CreateDebugConfiguration()
                .WithSource(script)
                .WithStdin("a")
                .TryRun()
                .Value;

            Assert.IsNotNull(result);

            result!.MoveNext();

            Assert.IsNotNull(result.Current);
            Assert.AreEqual(result.Current!.ExitCode, ExitCode.StdoutBufferLimitExceeded);
            Assert.AreEqual(result.Current.Stdout, new string('a', 1024 * 8));
            Assert.IsNotNull(result.Current.ExceptionInfo);
            Assert.AreEqual(result.Current.ExceptionInfo!.StackTrace.Count, 1);
            Assert.AreEqual(result.Current.ExceptionInfo.StackTrace[0], ",[.");
        }
    }
}
