using System;
using Brainf_ck_sharp;
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
            Assert.IsTrue(result.HasFlag(InterpreterExitCode.Failure) &&
                          result.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.HasFlag(InterpreterExitCode.NegativeValue));
            Assert.AreEqual(result.Output, String.Empty);
        }

        [TestMethod]
        public void ExceptionTest2()
        {
            const String script = "+[+]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.HasFlag(InterpreterExitCode.Failure) &&
                          result.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.HasFlag(InterpreterExitCode.MaxValueExceeded));
            Assert.AreEqual(result.Output, String.Empty);
        }

        [TestMethod]
        public void ExceptionTest3()
        {
            const String script = "+[>+]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.HasFlag(InterpreterExitCode.Failure) &&
                          result.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.HasFlag(InterpreterExitCode.UpperBoundExceeded));
            Assert.AreEqual(result.Output, String.Empty);
        }

        [TestMethod]
        public void ExceptionTest4()
        {
            const String script = "<";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.HasFlag(InterpreterExitCode.Failure) &&
                          result.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.HasFlag(InterpreterExitCode.LowerBoundExceeded));
            Assert.AreEqual(result.Output, String.Empty);
        }

        [TestMethod]
        public void ExceptionTest5()
        {
            const String script = ",";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.HasFlag(InterpreterExitCode.Failure) &&
                          result.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.HasFlag(InterpreterExitCode.StrinBufferExhausted));
            Assert.AreEqual(result.Output, String.Empty);
        }

        [TestMethod]
        public void ExceptionTest6()
        {
            const String script = "+[]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, String.Empty, threshold: 1000);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.HasFlag(InterpreterExitCode.Failure) &&
                          result.HasFlag(InterpreterExitCode.ThresholdExceeded));
            Assert.AreEqual(result.Output, String.Empty);
        }

        [TestMethod]
        public void ExceptionTest7()
        {
            const String script = "+[]]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.HasFlag(InterpreterExitCode.Failure) &&
                          result.HasFlag(InterpreterExitCode.MismatchedParentheses));
            Assert.AreEqual(result.Output, String.Empty);
        }

        [TestMethod]
        public void ExceptionTest8()
        {
            const String script = "ncencewonwe";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.HasFlag(InterpreterExitCode.Failure) &&
                          result.HasFlag(InterpreterExitCode.NoCodeInterpreted));
            Assert.AreEqual(result.Output, String.Empty);
        }

        [TestMethod]
        public void StackTraceTest1()
        {
            const String script = "+++++[>+++>-<<-]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.HasFlag(InterpreterExitCode.Failure) &&
                          result.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.HasFlag(InterpreterExitCode.NegativeValue));
            Assert.AreEqual(result.Output, String.Empty);
            Assert.IsTrue(result.StackTrace?.Count == 2);
            Assert.AreEqual(result.StackTrace?.Pop(), ">+++>-");
            Assert.AreEqual(result.StackTrace?.Pop(), "+++++[");
        }

        [TestMethod]
        public void StackTraceTest2()
        {
            const String script = "+++++>-[>+++>-<<-]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.HasFlag(InterpreterExitCode.Failure) &&
                          result.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.HasFlag(InterpreterExitCode.NegativeValue));
            Assert.AreEqual(result.Output, String.Empty);
            Assert.IsTrue(result.StackTrace?.Count == 1);
            Assert.AreEqual(result.StackTrace?.Pop(), "+++++>-");
        }
    }
}
