using System;
using Brainf_ck_sharp.ReturnTypes;
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
            const String script = "+++++";
            Assert.IsTrue(Brainf_ckInterpreter.CheckSourceSyntax(script).Valid);
        }

        [TestMethod]
        public void Test2()
        {
            const String script = "+++++[]";
            Assert.IsTrue(Brainf_ckInterpreter.CheckSourceSyntax(script).Valid);
        }

        [TestMethod]
        public void Test3()
        {
            const String script = "++(+)";
            Assert.IsTrue(Brainf_ckInterpreter.CheckSourceSyntax(script).Valid);
        }

        [TestMethod]
        public void Test3False1()
        {
            const String script = "++(gfrvef)";
            SyntaxValidationResult result = Brainf_ckInterpreter.CheckSourceSyntax(script);
            Assert.IsFalse(result.Valid);
            Assert.IsTrue(result.ErrorPosition == 9);
        }

        [TestMethod]
        public void Test3False2()
        {
            const String script = "++()";
            SyntaxValidationResult result = Brainf_ckInterpreter.CheckSourceSyntax(script);
            Assert.IsFalse(result.Valid);
            Assert.IsTrue(result.ErrorPosition == 3);
        }

        [TestMethod]
        public void Test4()
        {
            const String script = "++(+)[][(+)]";
            Assert.IsTrue(Brainf_ckInterpreter.CheckSourceSyntax(script).Valid);
        }

        [TestMethod]
        public void Test5()
        {
            const String script = "++>>>(+)[]+++(+)(-)(>>>)";
            Assert.IsTrue(Brainf_ckInterpreter.CheckSourceSyntax(script).Valid);
        }

        [TestMethod]
        public void Test6()
        {
            const String script = "+++[++(>>>+])";
            SyntaxValidationResult result = Brainf_ckInterpreter.CheckSourceSyntax(script);
            Assert.IsFalse(result.Valid);
            Assert.IsTrue(result.ErrorPosition == 11);
        }

        [TestMethod]
        public void Test7()
        {
            const String script = "++++[[[(+)](-)]++(+)](>)";
            Assert.IsTrue(Brainf_ckInterpreter.CheckSourceSyntax(script).Valid);
        }

        [TestMethod]
        public void Test8()
        {
            const String script = "++>>>()+)[]+++(+)(-)(>>>)";
            SyntaxValidationResult result = Brainf_ckInterpreter.CheckSourceSyntax(script);
            Assert.IsFalse(result.Valid);
            Assert.IsTrue(result.ErrorPosition == 8);
        }

        [TestMethod]
        public void Test9()
        {
            const String script = "+++[+]+(>>>+])";
            SyntaxValidationResult result = Brainf_ckInterpreter.CheckSourceSyntax(script);
            Assert.IsFalse(result.Valid);
            Assert.IsTrue(result.ErrorPosition == 12);
        }
    }
}
