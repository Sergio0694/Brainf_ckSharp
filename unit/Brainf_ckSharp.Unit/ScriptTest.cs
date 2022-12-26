using System;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Unit.Shared;
using Brainf_ckSharp.Unit.Shared.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ckSharp.Unit;

public class ScriptTest
{
    // Tests a script with a given runner
    public static void TestScript(Func<Script, InterpreterResult> runner, [CallerMemberName] string? name = null)
    {
        var script = ScriptLoader.LoadScriptByName(name!);
        var debug = runner(script);

        Assert.IsNotNull(debug);
        Assert.AreEqual(debug.ExitCode, ExitCode.Success);
        Assert.AreEqual(debug.Stdout, script.Stdout);
    }
}

[TestClass]
public partial class DebugTest
{
    // Executes a script in DEBUG mode
    private static InterpreterResult Run(Script script)
    {
        var session = Brainf_ckInterpreter
            .CreateDebugConfiguration()
            .WithSource(script.Source)
            .WithStdin(script.Stdin)
            .WithMemorySize(script.MemorySize)
            .WithOverflowMode(script.OverflowMode)
            .TryRun();

        Assert.IsNotNull(session.Value);
        Assert.IsTrue(session.ValidationResult.IsSuccess);

        using (session.Value)
        {
            session.Value!.MoveNext();

            Assert.IsNotNull(session.Value.Current);

            return session.Value.Current;
        }
    }
}

[TestClass]
public partial class ReleaseTest
{
    // Executes a script in RELEASE mode
    private static InterpreterResult Run(Script script)
    {
        var result = Brainf_ckInterpreter
            .CreateReleaseConfiguration()
            .WithSource(script.Source)
            .WithStdin(script.Stdin)
            .WithMemorySize(script.MemorySize)
            .WithOverflowMode(script.OverflowMode)
            .TryRun();

        Assert.IsNotNull(result.Value);
        Assert.IsTrue(result.ValidationResult.IsSuccess);

        return result.Value!;
    }
}
