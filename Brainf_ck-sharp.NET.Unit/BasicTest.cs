using Brainf_ck_sharp.NET.Enums;
using Brainf_ck_sharp.NET.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ck_sharp.NET.Unit
{
    [TestClass]
    public class BasicTest
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

        [TestMethod]
        public void SimpleLoop()
        {
            const string script = "+++++[>++<-]>";

            InterpreterResult result = Brainf_ckInterpreter.Run(script);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.ExitCode, ExitCode.NoOutput);
            Assert.AreEqual(result.Stdout, string.Empty);
            Assert.AreEqual(result.MachineState.Current.Value, 10);
        }

        [TestMethod]
        public void EmptyLoop()
        {
            const string script = "[]";

            InterpreterResult result = Brainf_ckInterpreter.Run(script);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.ExitCode, ExitCode.NoOutput);
            Assert.AreEqual(result.Stdout, string.Empty);
            Assert.AreEqual(result.MachineState.Current.Value, 0);
        }

        [TestMethod]
        public void ResetLoop()
        {
            const string script = ",[-]";

            InterpreterResult result = Brainf_ckInterpreter.Run(script, "0");

            Assert.IsNotNull(result);
            Assert.AreEqual(result.ExitCode, ExitCode.NoOutput);
            Assert.AreEqual(result.Stdout, string.Empty);
            Assert.AreEqual(result.MachineState.Current.Value, 0);
        }

        [TestMethod]
        public void CopyLoop()
        {
            const string script = ",[>+<-]>.";

            InterpreterResult result = Brainf_ckInterpreter.Run(script, "0");

            Assert.IsNotNull(result);
            Assert.AreEqual(result.ExitCode, ExitCode.TextOutput);
            Assert.AreEqual(result.Stdout, "0");
            Assert.AreEqual(result.MachineState.Current.Value, 48);
        }

        [TestMethod]
        public void NestedLoops()
        {
            const string script = "++[>++[>+<-]<-]>,[>+<-]>.";

            InterpreterResult result = Brainf_ckInterpreter.Run(script, "0");
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ExitCode, ExitCode.TextOutput);
            Assert.AreEqual(result.Stdout, "4");
            Assert.AreEqual(result.MachineState.Current.Value, 52);
        }

        [TestMethod]
        public void LoopWithPrints()
        {
            const string script = ",>,+[<.+>-]";

            InterpreterResult result = Brainf_ckInterpreter.Run(script, "A9");

            Assert.IsNotNull(result);
            Assert.AreEqual(result.ExitCode, ExitCode.TextOutput);
            Assert.AreEqual(result.Stdout, "ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz");
        }
    }
}
