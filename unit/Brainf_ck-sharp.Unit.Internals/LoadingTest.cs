using Brainf_ck_sharp.NET;
using Brainf_ck_sharp.NET.Buffers;
using Brainf_ck_sharp.NET.Extensions.Types;
using Brainf_ck_sharp.NET.Models;
using Brainf_ck_sharp.NET.Models.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ck_sharp.Unit.Internals
{
    /// <summary>
    /// A test <see langword="class"/> to test internal loading APIs from the <see cref="Brainf_ckInterpreter"/> <see langword="class"/>
    /// </summary>
    [TestClass]
    public class LoadingTest
    {
        [TestMethod]
        public void EmptyStackTrace()
        {
            using UnsafeMemoryBuffer<byte>? operators = Brainf_ckParser.TryParse("++[>++>-]>+", out _);

            Assert.IsNotNull(operators);

            using UnsafeMemoryBuffer<StackFrame> stackFrames = UnsafeMemoryBuffer<StackFrame>.Allocate(512, false);

            stackFrames[0] = new StackFrame(new Range(0, operators!.Size), 10);

            HaltedExecutionInfo? exceptionInfo = Brainf_ckInterpreter.LoadDebugInfo(operators.Memory, stackFrames.Memory, -1);

            Assert.IsNull(exceptionInfo);
        }

        [TestMethod]
        public void RootBreakpoint()
        {
            using UnsafeMemoryBuffer<byte>? operators = Brainf_ckParser.TryParse("++[>++>-]>+", out _);

            Assert.IsNotNull(operators);

            using UnsafeMemoryBuffer<StackFrame> stackFrames = UnsafeMemoryBuffer<StackFrame>.Allocate(512, false);

            stackFrames[0] = new StackFrame(new Range(0, operators!.Size), 7);

            HaltedExecutionInfo? exceptionInfo = Brainf_ckInterpreter.LoadDebugInfo(operators.Memory, stackFrames.Memory, 0);

            Assert.IsNotNull(exceptionInfo);
            Assert.AreEqual(exceptionInfo!.StackTrace.Count, 1);
            Assert.AreEqual(exceptionInfo.StackTrace[0], "++[>++>-");
            Assert.AreEqual(exceptionInfo.HaltingOperator, '-');
            Assert.AreEqual(exceptionInfo.HaltingOffset, 7);
        }

        [TestMethod]
        public void FunctionCallBreakpoint()
        {
            using UnsafeMemoryBuffer<byte>? operators = Brainf_ckParser.TryParse("(+>):+", out _);

            Assert.IsNotNull(operators);

            using UnsafeMemoryBuffer<StackFrame> stackFrames = UnsafeMemoryBuffer<StackFrame>.Allocate(512, false);

            stackFrames[0] = new StackFrame(new Range(0, operators!.Size), 5);
            stackFrames[1] = new StackFrame(new Range(1, 3), 2);

            HaltedExecutionInfo? exceptionInfo = Brainf_ckInterpreter.LoadDebugInfo(operators.Memory, stackFrames.Memory, 1);

            Assert.IsNotNull(exceptionInfo);
            Assert.AreEqual(exceptionInfo!.StackTrace.Count, 2);
            Assert.AreEqual(exceptionInfo.StackTrace[0], "+>");
            Assert.AreEqual(exceptionInfo.StackTrace[1], "(+>):");
            Assert.AreEqual(exceptionInfo.HaltingOperator, '>');
            Assert.AreEqual(exceptionInfo.HaltingOffset, 2);
        }
    }
}
