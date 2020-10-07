using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ckSharp.Unit
{
    [TestClass]
    public class FunctionTest
    {
        [TestMethod]
        public void SingleCall()
        {
            const string script = "+(,[>+<-]>.)>+:";

            Option<InterpreterResult> result = Brainf_ckInterpreter
                .CreateReleaseConfiguration()
                .WithSource(script)
                .WithStdin("a")
                .TryRun();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(result.Value!.ExitCode, ExitCode.Success);
            Assert.AreEqual(result.Value!.MachineState[0].Value, 1);
            Assert.AreEqual(result.Value!.MachineState[0].Index, 0);
            Assert.AreEqual(result.Value!.MachineState[0].IsSelected, false);
            Assert.AreEqual(result.Value!.MachineState[2].Value, 'a');
            Assert.AreEqual(result.Value!.MachineState[2].Index, 2);
            Assert.AreEqual(result.Value!.MachineState[2].IsSelected, true);
            Assert.AreEqual(result.Value.MachineState.Current.Index, 2);
            Assert.AreEqual(result.Value.MachineState.Current.Character, 'a');
            Assert.AreEqual(result.Value.MachineState.Current.IsSelected, true);
            Assert.AreEqual(result.Value.Stdout, "a");
            Assert.AreEqual(result.Value.Functions.Count, 1);
            Assert.AreEqual(result.Value.Functions[0].Index, 0);
            Assert.AreEqual(result.Value.Functions[0].Offset, 1);
            Assert.AreEqual(result.Value.Functions[0].Value, 1);
            Assert.AreEqual(result.Value.Functions[0].Body, ",[>+<-]>.");
        }

        [TestMethod]
        public void MultipleCalls()
        {
            const string script = "(+++):>:";

            Option<InterpreterResult> result = Brainf_ckInterpreter
                .CreateReleaseConfiguration()
                .WithSource(script)
                .TryRun();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(result.Value!.ExitCode, ExitCode.Success);
            Assert.AreEqual(result.Value.MachineState[0].Value, 3);
            Assert.AreEqual(result.Value.MachineState[1].Value, 3);
            Assert.AreEqual(result.Value.Functions.Count, 1);
            Assert.AreEqual(result.Value.Functions[0].Index, 0);
            Assert.AreEqual(result.Value.Functions[0].Offset, 0);
            Assert.AreEqual(result.Value.Functions[0].Value, 0);
            Assert.AreEqual(result.Value.Functions[0].Body, "+++");
        }

        [TestMethod]
        public void Recursion()
        {
            const string script = ">,<(>[>+<-<:]):>[<<+>>-]<<.[-]";

            Option<InterpreterResult> result = Brainf_ckInterpreter
                .CreateReleaseConfiguration()
                .WithSource(script)
                .WithStdin("%")
                .TryRun();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(result.Value!.ExitCode, ExitCode.Success);
            Assert.AreEqual(result.Value.MachineState.Current.Value, 0);
            Assert.AreEqual(result.Value.Stdout, "%");
        }
    }
}
