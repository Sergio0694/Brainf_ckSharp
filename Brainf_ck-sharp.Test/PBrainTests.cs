﻿using System;
using Brainf_ck_sharp;
using Brainf_ck_sharp.ReturnTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ck_sharp_Test
{
    [TestClass]
    [TestCategory(nameof(PBrainTests))]
    public class PBrainTests
    {
        [TestMethod]
        public void SyntaxTest1()
        {
            const String script = "++(  +";
            SyntaxValidationResult result = Brainf_ckInterpreter.CheckSourceSyntax(script);
            Assert.IsTrue(!result.Valid && result.ErrorPosition == 2);
        }

        [TestMethod]
        public void Test1()
        {
            const String script = "(+++):>:";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.IsTrue(result.MachineState[0].Value == 3 && result.MachineState[1].Value == 3);
            Assert.IsTrue(result.TotalOperations == 10);
        }

        [TestMethod]
        public void Test2()
        {
            const String script = "+(,[>+<-]>.)>+:";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, "a");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.TextOutput));
            Assert.IsTrue(result.MachineState.Current.Value == 'a');
            Assert.IsTrue(result.Output.Equals("a"));
            Assert.IsTrue(result.TotalOperations == 9 + 'a' * 5);
        }

        [TestMethod]
        public void Test3()
        {
            const String script = ",[(+)-]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, "€");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Failure) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.FunctionsLimitExceeded) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
        }

        [TestMethod]
        public void Test4()
        {
            const String script = "(++++):::";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Failure) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.UndefinedFunctionCalled));
            Assert.IsTrue(result.ExceptionInfo?.ErrorPosition == 7);
            Assert.IsTrue(result.TotalOperations == 7);
        }

        [TestMethod]
        public void BreakpointTest1()
        {
            String[] script = { "(+):+++++" };
            InterpreterExecutionSession result = Brainf_ckInterpreter.InitializeSession(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.CanContinue);
            Assert.IsTrue(result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.IsTrue(result.CurrentResult.MachineState.Current.Value == 6);
        }

        [TestMethod]
        public void BreakpointTest2()
        {
            String[] script = { "++(>-+)>++:" };
            InterpreterExecutionSession result = Brainf_ckInterpreter.InitializeSession(script, String.Empty);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.CanContinue);
            Assert.IsTrue(result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Failure) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NegativeValue) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.IsTrue(result.CurrentResult.MachineState.Current.Value == 0);
        }

        [TestMethod]
        public void BreakpointTest3()
        {
            String[] script = { "++(>-+)", ">++:" };
            InterpreterExecutionSession result = Brainf_ckInterpreter.InitializeSession(script, String.Empty);
            Assert.IsNotNull(result);
            result.Continue();
            Assert.IsFalse(result.CanContinue);
            Assert.IsTrue(result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Failure) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.ExceptionThrown) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NegativeValue) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.IsTrue(result.CurrentResult.MachineState.Current.Value == 0);
        }

        [TestMethod]
        public void BreakpointTest4()
        {
            String[] script = { "++(>,+.)", ">++:" };
            InterpreterExecutionSession result = Brainf_ckInterpreter.InitializeSession(script, "a");
            Assert.IsNotNull(result);
            result.Continue();
            Assert.IsFalse(result.CanContinue);
            Assert.IsTrue(result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.TextOutput));
            Assert.IsTrue(result.CurrentResult.MachineState.Current.Character == 'b');
        }
    }
}
