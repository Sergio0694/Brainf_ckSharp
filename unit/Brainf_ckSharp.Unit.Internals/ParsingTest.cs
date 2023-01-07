using System;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Opcodes;
using CommunityToolkit.HighPerformance.Buffers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ckSharp.Unit.Internals;

/// <summary>
/// A test <see langword="class"/> to test internal APIs from the <see cref="Brainf_ckParser"/> <see langword="class"/>
/// </summary>
[TestClass]
public class ParsingTest
{
    [TestMethod]
    public void ExtractSource()
    {
        Span<Brainf_ckOperator> operators = stackalloc Brainf_ckOperator[]
        {
            Operators.Plus,
            Operators.Minus,
            Operators.ForwardPtr,
            Operators.BackwardPtr,
            Operators.PrintChar,
            Operators.ReadChar,
            Operators.LoopStart,
            Operators.LoopEnd,
            Operators.FunctionStart,
            Operators.FunctionEnd,
            Operators.FunctionCall
        };

        string source = Brainf_ckParser.ExtractSource(operators);

        Assert.IsNotNull(source);
        Assert.AreEqual(source, "+-><.,[]():");
    }

    [TestMethod]
    public void TryParseInDebugMode()
    {
        const string script = "[\n\tTest script\n]\n+++++[\n\t>++ 5 x 2 = 10\n\t<- Loop decrement\n]\n> Move to cell 1";

        using MemoryOwner<Brainf_ckOperator>? operators = Brainf_ckParser.TryParse<Brainf_ckOperator>(script.AsSpan(), out SyntaxValidationResult result);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(result.ErrorType, SyntaxError.None);
        Assert.AreEqual(result.ErrorOffset, -1);
        Assert.AreEqual(result.OperatorsCount, 15);

        Assert.IsNotNull(operators);
        Assert.AreEqual(operators!.Length, 15);

        string source = Brainf_ckParser.ExtractSource(operators.Span);

        Assert.IsNotNull(source);
        Assert.AreEqual(source, "[]+++++[>++<-]>");
    }

    [TestMethod]
    public void TryParseInReleaseMode()
    {
        const string script = "[\n\tTest script\n]\n+++++[\n\t>++ 5 x 2 = 10\n\t<- Loop decrement\n]\n> Move to cell 1";

        using MemoryOwner<Brainf_ckOperation>? operations = Brainf_ckParser.TryParse<Brainf_ckOperation>(script.AsSpan(), out SyntaxValidationResult result);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(result.ErrorType, SyntaxError.None);
        Assert.AreEqual(result.ErrorOffset, -1);
        Assert.AreEqual(result.OperatorsCount, 15);

        Assert.IsNotNull(operations);
        Assert.AreEqual(operations!.Length, 10);

        string source = Brainf_ckParser.ExtractSource(operations.Span);

        Assert.IsNotNull(source);
        Assert.AreEqual(source, "[]+++++[>++<-]>");
    }

    [TestMethod]
    public void ValidateReleaseCompression()
    {
        Span<Brainf_ckOperation> operations = stackalloc[]
        {
            new Brainf_ckOperation(Operators.Plus, 5),
            new Brainf_ckOperation(Operators.Minus, 4),
            new Brainf_ckOperation(Operators.ForwardPtr, 7),
            new Brainf_ckOperation(Operators.BackwardPtr, 3),
            new Brainf_ckOperation(Operators.ForwardPtr, 1),
            new Brainf_ckOperation(Operators.BackwardPtr, 1),
            new Brainf_ckOperation(Operators.ForwardPtr, 2),
            new Brainf_ckOperation(Operators.FunctionStart, 1),
            new Brainf_ckOperation(Operators.Plus, 5),
            new Brainf_ckOperation(Operators.FunctionEnd, 1),
            new Brainf_ckOperation(Operators.FunctionCall, 1),
            new Brainf_ckOperation(Operators.FunctionCall, 1),
            new Brainf_ckOperation(Operators.FunctionCall, 1),
            new Brainf_ckOperation(Operators.FunctionCall, 1),
            new Brainf_ckOperation(Operators.FunctionCall, 1),
            new Brainf_ckOperation(Operators.FunctionCall, 1),
            new Brainf_ckOperation(Operators.LoopStart, 1),
            new Brainf_ckOperation(Operators.LoopStart, 1),
            new Brainf_ckOperation(Operators.Plus, 5),
            new Brainf_ckOperation(Operators.LoopEnd, 1),
            new Brainf_ckOperation(Operators.LoopEnd, 1),
            new Brainf_ckOperation(Operators.FunctionCall, 1),
            new Brainf_ckOperation(Operators.Plus, 15),
            new Brainf_ckOperation(Operators.Minus, 15),
            new Brainf_ckOperation(Operators.PrintChar, 1),
            new Brainf_ckOperation(Operators.PrintChar, 1),
            new Brainf_ckOperation(Operators.ReadChar, 1),
            new Brainf_ckOperation(Operators.ReadChar, 1),
        };

        string script = Brainf_ckParser.ExtractSource(operations);

        using MemoryOwner<Brainf_ckOperation>? buffer = Brainf_ckParser.TryParse<Brainf_ckOperation>(script.AsSpan(), out SyntaxValidationResult result);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(result.ErrorType, SyntaxError.None);
        Assert.AreEqual(result.ErrorOffset, -1);
        Assert.AreEqual(result.OperatorsCount, script.Length);

        CollectionAssert.AreEqual(operations.ToArray(), buffer!.Span.ToArray());
    }
}
