using Brainf_ck_sharp.NET.Enums;
using Brainf_ck_sharp.NET.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ck_sharp.NET.Unit
{
    /// <summary>
    /// A test <see langword="class"/> for syntax validation of Brainf*ck/PBrain scripts
    /// </summary>
    [TestClass]
    public class SyntaxTest
    {
        // Tests a valid script
        private static void AssertIsValid(string script)
        {
            SyntaxValidationResult result = Brainf_ckParser.IsSyntaxValid(script);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(result.ErrorOffset, -1);
            Assert.AreEqual(result.ErrorType, SyntaxError.None);
        }

        [TestMethod]
        public void OnlyPlus() => AssertIsValid("+++++");

        [TestMethod]
        public void PlusAndEmptyLoop() => AssertIsValid("+++++[]");

        [TestMethod]
        public void PlusAndSmallFunction() => AssertIsValid("++(+)");

        [TestMethod]
        public void NestedLoops() => AssertIsValid("[++.]++[-][++[++]]");

        [TestMethod]
        public void LoopsAndFunctions() => AssertIsValid("++(+)[][++[-]]++.>(+)");

        [TestMethod]
        public void MultipleFunctions() => AssertIsValid("++>>>(+)[]+++(+)(-)(>>>)");

        [TestMethod]
        public void HelloWorld() => AssertIsValid("[]+++++[>+++++[>+++>++++[>+>+<<-]>>>+++++>+<<<<<<-]<-]>>---.>>+.>++++++++..+++.>>+++++++.<------.<.+++.------.<-.>>>+.");

        [TestMethod]
        public void Fibonacci() => AssertIsValid("[.,-.,,,(<).]++++++++[>++++>++++++<<-]>>.[>+>+<<-]>>[<<+>>-],>,<<[>->-<<-]>[<++++++++++>-]>[<<+>>-]<<-1>>>+>>>+(->[>+<-]>[>>++++++++++<<[>+>>+<<<-]>>>[<<<+>>>-]<<[->->+<[>>>]=>[<++++++++++>---------->>>>+<]<<<<<]>[-]>[<<+>>-]>>>>[<<<<<+>>>>>-]<<<<<<<[-]+>>]<<[+++++[>++++++++<-]>.[-]<<<])<<<<<(<[<<.>>->>>1[>+>+>>+<<<<-]>>[<<+>>-]<<<[>+<-]>>[<<+>>-]>>:+<<<<<:]):");

        // Tests an invalid script
        private static void AssertIsInvalid(string script, int position, SyntaxError error)
        {
            SyntaxValidationResult result = Brainf_ckParser.IsSyntaxValid(script);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorOffset, position);
            Assert.AreEqual(result.ErrorType, error);
        }

        [TestMethod]
        public void MismatchedBrackets1() => AssertIsInvalid("++[+>]]", 6, SyntaxError.MismatchedSquareBracket);

        [TestMethod]
        public void MismatchedBrackets2() => AssertIsInvalid("]", 0, SyntaxError.MismatchedSquareBracket);

        [TestMethod]
        public void IncompleteLoop() => AssertIsInvalid("++[+>", 2, SyntaxError.IncompleteLoop);

        [TestMethod]
        public void MismatchedParenthesis() => AssertIsInvalid("++(++>)+)", 8, SyntaxError.MismatchedParenthesis);

        [TestMethod]
        public void NestedFunctionDeclaration() => AssertIsInvalid("++(++>(++>)+)", 6, SyntaxError.NestedFunctionDeclaration);

        [TestMethod]
        public void EmptyFunction1() => AssertIsInvalid("++()>", 3, SyntaxError.EmptyFunctionDeclaration);

        [TestMethod]
        public void EmptyFunction2() => AssertIsInvalid("++(hello!)", 9, SyntaxError.EmptyFunctionDeclaration);

        [TestMethod]
        public void IncompleteFunctionDeclaration() => AssertIsInvalid("++(+++>", 2, SyntaxError.IncompleteFunctionDeclaration);

        [TestMethod]
        public void NoOperators1() => AssertIsInvalid("", -1, SyntaxError.MissingOperators);

        [TestMethod]
        public void NoOperators2() => AssertIsInvalid("Hello world!", -1, SyntaxError.MissingOperators);
    }
}
