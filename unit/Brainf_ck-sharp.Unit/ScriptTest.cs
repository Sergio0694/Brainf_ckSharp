using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Unit.Shared;
using Brainf_ckSharp.Unit.Shared.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ckSharp.Unit
{
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
    public class DebugTest
    {
        // Executes a script in DEBUG mode
        [Pure]
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

        [TestMethod]
        public void HelloWorld() => ScriptTest.TestScript(Run);

        [TestMethod]
        public void Sum() => ScriptTest.TestScript(Run);

        [TestMethod]
        public void Multiply() => ScriptTest.TestScript(Run);

        [TestMethod]
        public void Division() => ScriptTest.TestScript(Run);

        [TestMethod]
        public void Fibonacci() => ScriptTest.TestScript(Run);
    }

    [TestClass]
    public class ReleaseTest
    {
        // Executes a script in RELEASE mode
        [Pure]
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

        [TestMethod]
        public void HelloWorld() => ScriptTest.TestScript(Run);

        [TestMethod]
        public void Sum() => ScriptTest.TestScript(Run);

        [TestMethod]
        public void Multiply() => ScriptTest.TestScript(Run);

        [TestMethod]
        public void Division() => ScriptTest.TestScript(Run);

        [TestMethod]
        public void Fibonacci() => ScriptTest.TestScript(Run);
    }
}
