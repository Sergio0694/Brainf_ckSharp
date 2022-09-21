using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ckSharp.Unit;

public partial class DebugTest
{
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

    [TestMethod]
    public void Mandelbrot() => ScriptTest.TestScript(Run);
}

public partial class ReleaseTest
{
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

    [TestMethod]
    public void Mandelbrot() => ScriptTest.TestScript(Run);
}
