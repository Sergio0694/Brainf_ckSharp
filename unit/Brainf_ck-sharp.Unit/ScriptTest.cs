using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Unit.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ckSharp.Unit
{
    [TestClass]
    public class ScriptTest
    {
        // Executes a script in DEBUG mode
        [Pure]
        private static InterpreterResult RunInDebugConfiguration(string script, string stdin)
        {
            var session = Brainf_ckInterpreter
                .CreateDebugConfiguration()
                .WithSource(script)
                .WithStdin(stdin)
                .WithMemorySize(64)
                .WithOverflowMode(OverflowMode.UshortWithNoOverflow)
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
        private static InterpreterResult RunInReleaseConfiguration(string script, string stdin)
        {
            var result = Brainf_ckInterpreter
                .CreateReleaseConfiguration()
                .WithSource(script)
                .WithStdin(stdin)
                .WithMemorySize(64)
                .WithOverflowMode(OverflowMode.UshortWithNoOverflow)
                .TryRun();

            Assert.IsNotNull(result.Value);
            Assert.IsTrue(result.ValidationResult.IsSuccess);

            return result.Value!;
        }

        // Tests a script in DEBUG and RELEASE configuration
        private static void TestScript([CallerMemberName] string? name = null)
        {
            var (stdin, expected, script) = ScriptLoader.LoadScriptByName(name);

            // DEBUG
            var debug = RunInDebugConfiguration(script, stdin);

            Assert.IsNotNull(debug);
            Assert.AreEqual(debug.ExitCode, ExitCode.Success);
            Assert.AreEqual(debug.Stdout, expected);

            // RELEASE
            var release = RunInReleaseConfiguration(script, stdin);

            Assert.IsNotNull(release);
            Assert.AreEqual(release.ExitCode, ExitCode.Success);
            Assert.AreEqual(release.Stdout, expected);
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
