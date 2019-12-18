using System;
using Brainf_ck_sharp.NET.Enums;
using Brainf_ck_sharp.NET.Models;
using Brainf_ck_sharp.NET.Models.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ck_sharp.NET.Unit
{
    [TestClass]
    public class SessionTest
    {
        [TestMethod]
        public void BaseOperators1()
        {
            const string script = "+++++";

            Option<InterpreterSession> result = Brainf_ckInterpreter.TryCreateSession(script, Array.Empty<int>());

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.IsTrue(result.Value!.MoveNext());
            Assert.IsNotNull(result.Value.Current);
            Assert.AreEqual(result.Value.Current.ExitCode, ExitCode.NoOutput);
            Assert.AreEqual(result.Value.Current.Stdout, string.Empty);
            Assert.AreEqual(result.Value.Current.MachineState.Current.Value, 5);
        }
    }
}
