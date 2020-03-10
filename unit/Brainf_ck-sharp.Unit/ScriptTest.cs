using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Unit.Shared;
using Brainf_ckSharp.Unit.Shared.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ckSharp.Unit
{
    [TestClass]
    public class ScriptTest
    {
        // Executes a script in DEBUG mode
        [Pure]
        private static InterpreterResult RunInDebugConfiguration(Script script)
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

        // Executes a script in RELEASE mode
        [Pure]
        private static InterpreterResult RunInReleaseConfiguration(Script script)
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

        // Tests a script in DEBUG and RELEASE configuration
        private static void TestScript([CallerMemberName] string? name = null)
        {
            var script = ScriptLoader.LoadScriptByName(name!);

            // DEBUG
            var debug = RunInDebugConfiguration(script);

            Assert.IsNotNull(debug);
            Assert.AreEqual(debug.ExitCode, ExitCode.Success);
            Assert.AreEqual(debug.Stdout, script.Stdout);

            // RELEASE
            var release = RunInReleaseConfiguration(script);

            Assert.IsNotNull(release);
            Assert.AreEqual(release.ExitCode, ExitCode.Success);
            Assert.AreEqual(release.Stdout, script.Stdout);
        }

        [TestMethod]
        public void HelloWorld() => TestScript();

        [TestMethod]
        public void Sum() => TestScript();

        [TestMethod]
        public void Multiply() => TestScript();

        [TestMethod]
        public void Division() => TestScript();

        [TestMethod]
        public void Fibonacci() => TestScript();
    }
}
