using Brainf_ckSharp.Legacy;
using Brainf_ckSharp.Legacy.ReturnTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ck_sharp.Unit
{
    [TestClass]
    [TestCategory(nameof(SyntaxTests))]
    public class SyntaxTests
    {
        [TestMethod]
        public void Test1()
        {
            const string script = "+++++";
            Assert.IsTrue(Brainf_ckInterpreter.CheckSourceSyntax(script).Valid);
        }

        [TestMethod]
        public void Test2()
        {
            const string script = "+++++[]";
            Assert.IsTrue(Brainf_ckInterpreter.CheckSourceSyntax(script).Valid);
        }

        [TestMethod]
        public void Test3()
        {
            const string script = "++(+)";
            Assert.IsTrue(Brainf_ckInterpreter.CheckSourceSyntax(script).Valid);
        }

        [TestMethod]
        public void Test3False1()
        {
            const string script = "++(gfrvef)";
            SyntaxValidationResult result = Brainf_ckInterpreter.CheckSourceSyntax(script);
            Assert.IsFalse(result.Valid);
            Assert.IsTrue(result.ErrorPosition == 9);
        }

        [TestMethod]
        public void Test3False2()
        {
            const string script = "++()";
            SyntaxValidationResult result = Brainf_ckInterpreter.CheckSourceSyntax(script);
            Assert.IsFalse(result.Valid);
            Assert.IsTrue(result.ErrorPosition == 3);
        }

        [TestMethod]
        public void Test4()
        {
            const string script = "++(+)[][(+)]";
            Assert.IsTrue(Brainf_ckInterpreter.CheckSourceSyntax(script).Valid);
        }

        [TestMethod]
        public void Test5()
        {
            const string script = "++>>>(+)[]+++(+)(-)(>>>)";
            Assert.IsTrue(Brainf_ckInterpreter.CheckSourceSyntax(script).Valid);
        }

        [TestMethod]
        public void Test6()
        {
            const string script = "+++[++(>>>+])";
            SyntaxValidationResult result = Brainf_ckInterpreter.CheckSourceSyntax(script);
            Assert.IsFalse(result.Valid);
            Assert.IsTrue(result.ErrorPosition == 11);
        }

        [TestMethod]
        public void Test7()
        {
            const string script = "++++[[[(+)](-)]++(+)](>)";
            Assert.IsTrue(Brainf_ckInterpreter.CheckSourceSyntax(script).Valid);
        }

        [TestMethod]
        public void Test8()
        {
            const string script = "++>>>()+)[]+++(+)(-)(>>>)";
            SyntaxValidationResult result = Brainf_ckInterpreter.CheckSourceSyntax(script);
            Assert.IsFalse(result.Valid);
            Assert.IsTrue(result.ErrorPosition == 8);
        }

        [TestMethod]
        public void Test9()
        {
            const string script = "+++[+]+(>>>+])";
            SyntaxValidationResult result = Brainf_ckInterpreter.CheckSourceSyntax(script);
            Assert.IsFalse(result.Valid);
            Assert.IsTrue(result.ErrorPosition == 12);
        }
    }
}
