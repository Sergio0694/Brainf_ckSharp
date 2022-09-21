using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Memory.Interfaces;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ckSharp.Unit;

[TestClass]
public class BasicTest
{
    [TestMethod]
    public void BaseOperators1()
    {
        const string script = "+++++";

        Option<InterpreterResult> result = Brainf_ckInterpreter
            .CreateReleaseConfiguration()
            .WithSource(script)
            .TryRun();

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(result.Value!.ExitCode, ExitCode.Success);
        Assert.AreEqual(result.Value.Stdout, string.Empty);
        Assert.AreEqual(result.Value.MachineState.Current.Value, 5);
    }

    [TestMethod]
    public void BaseOperators2()
    {
        const string script = "+++++---";

        Option<InterpreterResult> result = Brainf_ckInterpreter
            .CreateReleaseConfiguration()
            .WithSource(script)
            .TryRun();

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(result.Value!.ExitCode, ExitCode.Success);
        Assert.AreEqual(result.Value.Stdout, string.Empty);
        Assert.AreEqual(result.Value.MachineState.Current.Value, 2);
    }

    [TestMethod]
    public void BaseOperators3()
    {
        const string script = ",++.";

        Option<InterpreterResult> result = Brainf_ckInterpreter
            .CreateReleaseConfiguration()
            .WithSource(script)
            .WithStdin("0")
            .TryRun();

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(result.Value!.ExitCode, ExitCode.Success);
        Assert.AreEqual(result.Value.Stdout, "2");
        Assert.AreEqual(result.Value.MachineState.Current.Value, 50);
    }

    [TestMethod]
    public void SimpleLoop()
    {
        const string script = "+++++[>++<-]>";

        Option<InterpreterResult> result = Brainf_ckInterpreter
            .CreateReleaseConfiguration()
            .WithSource(script)
            .TryRun();

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(result.Value!.ExitCode, ExitCode.Success);
        Assert.AreEqual(result.Value.Stdout, string.Empty);
        Assert.AreEqual(result.Value.MachineState.Current.Value, 10);
    }

    [TestMethod]
    public void EmptyLoop()
    {
        const string script = "[]";

        Option<InterpreterResult> result = Brainf_ckInterpreter
            .CreateReleaseConfiguration()
            .WithSource(script)
            .TryRun();

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(result.Value!.ExitCode, ExitCode.Success);
        Assert.AreEqual(result.Value.Stdout, string.Empty);
        Assert.AreEqual(result.Value.MachineState.Current.Value, 0);
    }

    [TestMethod]
    public void ResetLoop()
    {
        const string script = ",[-]";

        Option<InterpreterResult> result = Brainf_ckInterpreter
            .CreateReleaseConfiguration()
            .WithSource(script)
            .WithStdin("0")
            .TryRun();

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(result.Value!.ExitCode, ExitCode.Success);
        Assert.AreEqual(result.Value.Stdout, string.Empty);
        Assert.AreEqual(result.Value.MachineState.Current.Value, 0);
    }

    [TestMethod]
    public void CopyLoop()
    {
        const string script = ",[>+<-]>.";

        Option<InterpreterResult> result = Brainf_ckInterpreter
            .CreateReleaseConfiguration()
            .WithSource(script)
            .WithStdin("0")
            .TryRun();

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(result.Value!.ExitCode, ExitCode.Success);
        Assert.AreEqual(result.Value.Stdout, "0");
        Assert.AreEqual(result.Value.MachineState.Current.Value, 48);
    }

    [TestMethod]
    public void NestedLoops()
    {
        const string script = "++[>++[>+<-]<-]>,[>+<-]>.";

        Option<InterpreterResult> result = Brainf_ckInterpreter
            .CreateReleaseConfiguration()
            .WithSource(script)
            .WithStdin("0")
            .TryRun();

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(result.Value!.ExitCode, ExitCode.Success);
        Assert.AreEqual(result.Value.Stdout, "4");
        Assert.AreEqual(result.Value.MachineState.Current.Value, 52);
    }

    [TestMethod]
    public void LoopWithPrints()
    {
        const string script = ",>,+[<.+>-]";

        Option<InterpreterResult> result = Brainf_ckInterpreter
            .CreateReleaseConfiguration()
            .WithSource(script)
            .WithStdin("A9")
            .TryRun();

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(result.Value!.ExitCode, ExitCode.Success);
        Assert.AreEqual(result.Value.Stdout, "ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz");
    }

    [TestMethod]
    public void MemoryCells()
    {
        const string script = ",>+++>>+";

        IReadOnlyMachineState machineState = Brainf_ckInterpreter
            .CreateReleaseConfiguration()
            .WithSource(script)
            .WithStdin("a")
            .TryRun().Value!.MachineState;

        Assert.AreEqual(machineState[0].Index, 0);
        Assert.AreEqual(machineState[0].Value, (ushort)'a');
        Assert.AreEqual(machineState[0].IsSelected, false);

        Assert.AreEqual(machineState[1].Index, 1);
        Assert.AreEqual(machineState[1].Value, 3);
        Assert.AreEqual(machineState[1].IsSelected, false);

        Assert.AreEqual(machineState[2].Index, 2);
        Assert.AreEqual(machineState[2].Value, 0);
        Assert.AreEqual(machineState[2].IsSelected, false);

        Assert.AreEqual(machineState[3].Index, 3);
        Assert.AreEqual(machineState[3].Value, 1);
        Assert.AreEqual(machineState[3].IsSelected, true);
    }
}
