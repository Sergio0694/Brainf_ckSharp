using System;
using Brainf_ck_sharp;
using Brainf_ck_sharp.ReturnTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ck_sharp_Test
{
    [TestClass]
    [TestCategory(nameof(ExceptionTests))]
    public class ExceptionTests
    {
        [TestMethod]
        public void ExceptionTest1()
        {
            const String script = "-";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Failure) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.NegativeValue));
            Assert.AreEqual(result.Output, String.Empty);
        }

        [TestMethod]
        public void ExceptionTest2()
        {
            const String script = "+[+]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Failure) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.MaxValueExceeded));
            Assert.AreEqual(result.Output, String.Empty);
        }

        [TestMethod]
        public void ExceptionTest3()
        {
            const String script = "+[>+]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Failure) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.UpperBoundExceeded));
            Assert.AreEqual(result.Output, String.Empty);
        }

        [TestMethod]
        public void ExceptionTest4()
        {
            const String script = "<";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Failure) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.LowerBoundExceeded));
            Assert.AreEqual(result.Output, String.Empty);
        }

        [TestMethod]
        public void ExceptionTest5()
        {
            const String script = ",";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Failure) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.StdinBufferExhausted));
            Assert.AreEqual(result.Output, String.Empty);
        }

        [TestMethod]
        public void ExceptionTest6()
        {
            const String script = "+[]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, String.Empty, threshold: 1000);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Failure) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.ThresholdExceeded));
            Assert.AreEqual(result.Output, String.Empty);
            Assert.AreEqual(result.ExceptionInfo?.ErrorPosition, 1);
            Assert.AreEqual(result.ExceptionInfo?.FaultedOperator, '[');
        }

        [TestMethod]
        public void ExceptionTest7()
        {
            const String script = "+[]]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Failure) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.MismatchedParentheses));
            Assert.AreEqual(result.Output, String.Empty);
        }

        [TestMethod]
        public void ExceptionText7Extended()
        {
            const String script = "[ [] ok [[ [] ok [] [] [] ] ok ] [ ] ] [ ]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.AreEqual(result.Output, String.Empty);
        }

        [TestMethod]
        public void ExceptionTest8()
        {
            const String script = "ncencewonwe";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Failure) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.NoCodeInterpreted));
            Assert.AreEqual(result.Output, String.Empty);
        }

        [TestMethod]
        public void ExceptionTest9()
        {
            const String script = ",[.-]";
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
            const String script = "+++++[>+++>-<<-]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Failure) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.NegativeValue));
            Assert.AreEqual(result.Output, String.Empty);
            Assert.IsTrue(result.ExceptionInfo?.StackTrace.Count == 2);
            Assert.AreEqual(result.ExceptionInfo?.StackTrace[0], ">+++>-");
            Assert.AreEqual(result.ExceptionInfo?.StackTrace[1], "+++++[");
            Assert.AreEqual(result.ExceptionInfo?.ErrorPosition, 11);
            Assert.AreEqual(result.ExceptionInfo?.FaultedOperator, '-');
        }

        [TestMethod]
        public void StackTraceTest2()
        {
            const String script = "+++++>-[>+++>-<<-]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Failure) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.NegativeValue));
            Assert.AreEqual(result.Output, String.Empty);
            Assert.IsTrue(result.ExceptionInfo?.StackTrace.Count == 1);
            Assert.AreEqual(result.ExceptionInfo?.StackTrace[0], "+++++>-");
            Assert.AreEqual(result.ExceptionInfo?.ErrorPosition, 6);
            Assert.AreEqual(result.ExceptionInfo?.FaultedOperator, '-');
        }

        [TestMethod]
        public void SkippedLoopTest1()
        {
            const String script = "+>[++-]>-";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.AreEqual(result.Output, String.Empty);
            Assert.IsTrue(result.TotalOperations == 4);
        }
    }
}
