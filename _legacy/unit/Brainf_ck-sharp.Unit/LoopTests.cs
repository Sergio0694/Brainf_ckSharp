using Brainf_ck_sharp.ReturnTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ck_sharp.Unit
{
    [TestClass]
    [TestCategory(nameof(LoopTests))]
    public class LoopTests
    {
        [TestMethod]
        public void Loop1()
        {
            const string script = "[]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, string.Empty);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.AreEqual(result.Output, string.Empty);
            Assert.IsTrue(result.MachineState.Current.Value == 0);
        }

        [TestMethod]
        public void Loop2()
        {
            const string script = ",[-]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, "0");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.NoOutput));
            Assert.AreEqual(result.Output, string.Empty);
            Assert.IsTrue(result.MachineState.Current.Value == 0);
        }

        [TestMethod]
        public void Loop3()
        {
            const string script = ",[>+<-]>.";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, "0");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.TextOutput));
            Assert.AreEqual(result.Output, "0");
            Assert.IsTrue(result.MachineState.Current.Value == 48);
        }

        [TestMethod]
        public void Loop4()
        {
            const string script = "++[>++[>+<-]<-]>,[>+<-]>.";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, "0");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.TextOutput));
            Assert.AreEqual(result.Output, "4");
            Assert.IsTrue(result.MachineState.Current.Value == 52);
        }
        
        [TestMethod]
        public void Loop5()
        {
            const string script = ",>,+[<.+>-]";
            InterpreterResult result = Brainf_ckInterpreter.Run(script, "A9");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                          result.ExitCode.HasFlag(InterpreterExitCode.TextOutput));
            Assert.AreEqual(result.Output, "ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz");
        }
    }
}
