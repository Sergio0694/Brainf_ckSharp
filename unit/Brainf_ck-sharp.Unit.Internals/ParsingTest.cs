using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Opcodes;
using Microsoft.Toolkit.HighPerformance.Buffers;
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
            using MemoryOwner<Brainf_ckOperator> operators = MemoryOwner<Brainf_ckOperator>.Allocate(11);

            operators.Span[0] = Operators.Plus;
            operators.Span[1] = Operators.Minus;
            operators.Span[2] = Operators.ForwardPtr;
            operators.Span[3] = Operators.BackwardPtr;
            operators.Span[4] = Operators.PrintChar;
            operators.Span[5] = Operators.ReadChar;
            operators.Span[6] = Operators.LoopStart;
            operators.Span[7] = Operators.LoopEnd;
            operators.Span[8] = Operators.FunctionStart;
            operators.Span[9] = Operators.FunctionEnd;
            operators.Span[10] = Operators.FunctionCall;

            string source = Brainf_ckParser.ExtractSource<Brainf_ckOperator>(operators.Span);

            Assert.IsNotNull(source);
            Assert.AreEqual(source, "+-><.,[]():");
        }

        [TestMethod]
        public void TryParseInDebugMode()
        {
            const string script = "[\n\tTest script\n]\n+++++[\n\t>++ 5 x 2 = 10\n\t<- Loop decrement\n]\n> Move to cell 1";

            using MemoryOwner<Brainf_ckOperator>? operators = Brainf_ckParser.TryParse<Brainf_ckOperator>(script, out SyntaxValidationResult result);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(result.ErrorType, SyntaxError.None);
            Assert.AreEqual(result.ErrorOffset, -1);
            Assert.AreEqual(result.OperatorsCount, 15);

            Assert.IsNotNull(operators);
            Assert.AreEqual(operators!.Length, 15);

            string source = Brainf_ckParser.ExtractSource<Brainf_ckOperator>(operators.Span);

            Assert.IsNotNull(source);
            Assert.AreEqual(source, "[]+++++[>++<-]>");
        }

        [TestMethod]
        public void TryParseInReleaseMode()
        {
            const string script = "[\n\tTest script\n]\n+++++[\n\t>++ 5 x 2 = 10\n\t<- Loop decrement\n]\n> Move to cell 1";

            using MemoryOwner<Brainf_ckOperation>? operations = Brainf_ckParser.TryParse<Brainf_ckOperation>(script, out SyntaxValidationResult result);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(result.ErrorType, SyntaxError.None);
            Assert.AreEqual(result.ErrorOffset, -1);
            Assert.AreEqual(result.OperatorsCount, 15);

            Assert.IsNotNull(operations);
            Assert.AreEqual(operations!.Length, 10);

            string source = Brainf_ckParser.ExtractSource<Brainf_ckOperation>(operations.Span);

            Assert.IsNotNull(source);
            Assert.AreEqual(source, "[]+++++[>++<-]>");
        }
    }
}
