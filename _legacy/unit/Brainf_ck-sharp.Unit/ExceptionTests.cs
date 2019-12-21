using Brainf_ck_sharp.ReturnTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ck_sharp.Unit
{
    [TestClass]
    [TestCategory(nameof(ExceptionTests))]
    public class ExceptionTests
    {
        [TestMethod]
        public void ExceptionTest1()
        {
            const string script = "-";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, string.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Failure) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.NegativeValue));
            Assert.AreEqual(result.Output, string.Empty);
        }

        [TestMethod]
        public void ExceptionTest2()
        {
            const string script = "+[+]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, string.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Failure) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.MaxValueExceeded));
            Assert.AreEqual(result.Output, string.Empty);
        }

        [TestMethod]
        public void ExceptionTest3()
        {
            const string script = "+[>+]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, string.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Failure) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.UpperBoundExceeded));
            Assert.AreEqual(result.Output, string.Empty);
        }

        [TestMethod]
        public void ExceptionTest4()
        {
            const string script = "<";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, string.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Failure) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.LowerBoundExceeded));
            Assert.AreEqual(result.Output, string.Empty);
        }

        [TestMethod]
        public void ExceptionTest5()
        {
            const string script = ",";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, string.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Failure) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.StdinBufferExhausted));
            Assert.AreEqual(result.Output, string.Empty);
        }

        [TestMethod]
        public void ExceptionTest6()
        {
            const string script = "+[]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, string.Empty, threshold: 1000);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Failure) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.ThresholdExceeded));
            Assert.AreEqual(result.Output, string.Empty);
            Assert.AreEqual(result.ExceptionInfo?.ErrorPosition, 1);
            Assert.AreEqual(result.ExceptionInfo?.FaultedOperator, '[');
        }

        [TestMethod]
        public void ExceptionTest7()
        {
            const string script = "+[]]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, string.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Failure) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.SyntaxError));
            Assert.AreEqual(result.Output, string.Empty);
        }

        [TestMethod]
        public void ExceptionText7Extended()
        {
            const string script = "[ [] ok [[ [] ok [] [] [] ] ok ] [ ] ] [ ]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, string.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.AreEqual(result.Output, string.Empty);
        }

        [TestMethod]
        public void ExceptionTest8()
        {
            const string script = "ncencewonwe";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, string.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Failure) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.NoCodeInterpreted));
            Assert.AreEqual(result.Output, string.Empty);
        }

        [TestMethod]
        public void ExceptionTest9()
        {
            const string script = ",[.-]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, "€");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Failure) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.TextOutput) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.StdoutBufferLimitExceeded));
            Assert.AreEqual(result.Output.Length, Brainf_ckInterpreter.StdoutBufferSizeLimit);
        }

        [TestMethod]
        public void StackTraceTest1()
        {
            const string script = "+++++[>+++>-<<-]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, string.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Failure) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.NegativeValue));
            Assert.AreEqual(result.Output, string.Empty);
            Assert.IsTrue(result.ExceptionInfo?.StackTrace.Count == 2);
            Assert.AreEqual(result.ExceptionInfo?.StackTrace[0], ">+++>-");
            Assert.AreEqual(result.ExceptionInfo?.StackTrace[1], "+++++[");
            Assert.AreEqual(result.ExceptionInfo?.ErrorPosition, 11);
            Assert.AreEqual(result.ExceptionInfo?.FaultedOperator, '-');
        }

        [TestMethod]
        public void StackTraceTest2()
        {
            const string script = "+++++>-[>+++>-<<-]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, string.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Failure) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.NegativeValue));
            Assert.AreEqual(result.Output, string.Empty);
            Assert.IsTrue(result.ExceptionInfo?.StackTrace.Count == 1);
            Assert.AreEqual(result.ExceptionInfo?.StackTrace[0], "+++++>-");
            Assert.AreEqual(result.ExceptionInfo?.ErrorPosition, 6);
            Assert.AreEqual(result.ExceptionInfo?.FaultedOperator, '-');
        }

        [TestMethod]
        public void SkippedLoopTest1()
        {
            const string script = "+>[++-]>-";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, string.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.AreEqual(result.Output, string.Empty);
            Assert.IsTrue(result.TotalOperations == 5);
        }

        [TestMethod]
        public void StackTraceTest3()
        {
            const string script = ",[-]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, "a");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.AreEqual(result.Output, string.Empty);
            InterpreterExecutionSession session = Brainf_ckInterpreter.InitializeSession(new[] { ",[", "-]" }, "a");
            while (session.CanContinue) session.Continue();
            Assert.IsTrue(result.TotalOperations == session.CurrentResult.TotalOperations);
        }

        [TestMethod]
        public void StackTraceTest4()
        {
            const string script = ",[--]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, "a");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Failure) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.NegativeValue));
            Assert.AreEqual(result.Output, string.Empty);
            InterpreterExecutionSession session = Brainf_ckInterpreter.InitializeSession(new[] { ",[-", "-]" }, "a");
            while (session.CanContinue) session.Continue();
            Assert.IsTrue(result.TotalOperations == session.CurrentResult.TotalOperations);
        }

        [TestMethod]
        public void DisposeTest()
        {
            string[] script = { "++[", ">+<-]>" };
            using (InterpreterExecutionSession result = Brainf_ckInterpreter.InitializeSession(script, string.Empty))
            {
                Assert.IsTrue(result != null);
                result.RunToCompletion();
                Assert.IsTrue(!result.CanContinue);
                Assert.IsTrue(result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success));
            }
        }
    }
}
