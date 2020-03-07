using System;
using System.Buffers;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ckSharp.Unit.Internals
{
    /// <summary>
    /// A test <see langword="class"/> to test internal APIs from the <see cref="Brainf_ckParser"/> <see langword="class"/>
    /// </summary>
    [TestClass]
    public class ParsingTest
    {
        [TestMethod]
        public void ExtractSource()
        {
            using PinnedUnmanagedMemoryOwner<byte> operators = PinnedUnmanagedMemoryOwner<byte>.Allocate(11, false);

            operators[0] = Operators.Plus;
            operators[1] = Operators.Minus;
            operators[2] = Operators.ForwardPtr;
            operators[3] = Operators.BackwardPtr;
            operators[4] = Operators.PrintChar;
            operators[5] = Operators.ReadChar;
            operators[6] = Operators.LoopStart;
            operators[7] = Operators.LoopEnd;
            operators[8] = Operators.FunctionStart;
            operators[9] = Operators.FunctionEnd;
            operators[10] = Operators.FunctionCall;

            string source = Brainf_ckParser.Debug.ExtractSource(operators.Span);

            Assert.IsNotNull(source);
            Assert.AreEqual(source, "+-><.,[]():");
        }

        [TestMethod]
        public void TryParseInDebugMode()
        {
            const string script = "[\n\tTest script\n]\n+++++[\n\t>++ 5 x 2 = 10\n\t<- Loop decrement\n]\n> Move to cell 1";

            using PinnedUnmanagedMemoryOwner<byte>? operators = Brainf_ckParser.Debug.TryParse(script, out SyntaxValidationResult result);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(result.ErrorType, SyntaxError.None);
            Assert.AreEqual(result.ErrorOffset, -1);
            Assert.AreEqual(result.OperatorsCount, 15);

            Assert.IsNotNull(operators);
            Assert.AreEqual(operators!.Size, 15);

            string source = Brainf_ckParser.Debug.ExtractSource(operators.Span);

            Assert.IsNotNull(source);
            Assert.AreEqual(source, "[]+++++[>++<-]>");
        }

        [TestMethod]
        public void TryParseInReleaseMode()
        {
            const string script = "[\n\tTest script\n]\n+++++[\n\t>++ 5 x 2 = 10\n\t<- Loop decrement\n]\n> Move to cell 1";

            using PinnedUnmanagedMemoryOwner<Brainf_ckOperation>? operations = Brainf_ckParser.Release.TryParse(script, out SyntaxValidationResult result);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(result.ErrorType, SyntaxError.None);
            Assert.AreEqual(result.ErrorOffset, -1);
            Assert.AreEqual(result.OperatorsCount, 15);

            Assert.IsNotNull(operations);
            Assert.AreEqual(operations!.Size, 10);

            string source = Brainf_ckParser.Release.ExtractSource(operations.Span);

            Assert.IsNotNull(source);
            Assert.AreEqual(source, "[]+++++[>++<-]>");
        }
    }
}
