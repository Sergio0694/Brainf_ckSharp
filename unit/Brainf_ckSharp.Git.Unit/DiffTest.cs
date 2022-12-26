using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Git.Enums;
using Brainf_ckSharp.Git.Unit.Helpers;
using CommunityToolkit.HighPerformance.Buffers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brainf_ckSharp.Git.Unit;

[TestClass]
public class DiffTest
{
    [TestMethod]
    public void EmptyString()
    {
        Test(Array.Empty<LineModificationType>(), string.Empty, string.Empty);
    }

    [TestMethod]
    public void BlankText()
    {
        Test(new[] { LineModificationType.None, LineModificationType.None }, "\r", "\r");
    }

    [TestMethod]
    public void Small()
    {
        LineModificationType[] expected =
        {
            LineModificationType.None,
            LineModificationType.None,
            LineModificationType.Modified,
            LineModificationType.None,
            LineModificationType.None,
            LineModificationType.Modified,
            LineModificationType.Modified,
            LineModificationType.None,
            LineModificationType.None,
            LineModificationType.None,
            LineModificationType.Modified,
            LineModificationType.None,
            LineModificationType.None,
            LineModificationType.Modified,
            LineModificationType.None
        };

        Test(expected);
    }

    [TestMethod]
    public void Medium()
    {
        LineModificationType[] expected =
        {
            // 1
            LineModificationType.Modified,
            LineModificationType.Modified,
            LineModificationType.Modified,
            LineModificationType.Modified,
            LineModificationType.None,
            LineModificationType.None,
            LineModificationType.Modified,
            LineModificationType.Modified,
            LineModificationType.None,

            // 10
            LineModificationType.None,
            LineModificationType.None,
            LineModificationType.Modified,
            LineModificationType.None,
            LineModificationType.None,
            LineModificationType.None,
            LineModificationType.None,
            LineModificationType.None,
            LineModificationType.None,
            LineModificationType.None,

            // 20
            LineModificationType.Modified,
            LineModificationType.None,
            LineModificationType.None,
            LineModificationType.Modified,
            LineModificationType.Modified,
            LineModificationType.None,
            LineModificationType.None,
            LineModificationType.None,
            LineModificationType.Modified,
            LineModificationType.None,

            // 30
            LineModificationType.Modified,
            LineModificationType.Modified,
            LineModificationType.Modified,
            LineModificationType.None,
            LineModificationType.None,
            LineModificationType.None
        };

        Test(expected);
    }

    [TestMethod]
    public void Large()
    {
        LineModificationType[] expected = new LineModificationType[96];

        foreach (int edit in new[]
        {
            10, 11, 12,
            28, 29, 30, 31, 32,
            66, 67, 68, 69, 70, 71,
            87, 88
        })
        {
            expected[edit] = LineModificationType.Modified;
        }

        Test(expected);
    }

    /// <summary>
    /// Runs a test for a pair of resources with the specified name
    /// </summary>
    /// <param name="expected">The expected results for the test</param>
    /// <param name="key">The key of the test resources to load</param>
    private static void Test(LineModificationType[] expected, [CallerMemberName] string key = null!)
    {
        var data = ResourceLoader.LoadTestSample(key);

        string
            oldText = data.Old.Replace("\n", string.Empty),
            newText = data.New.Replace("\n", string.Empty);

        Test(expected, oldText, newText);
    }

    /// <summary>
    /// Runs a test for a pair of input text files
    /// </summary>
    /// <param name="expected">The expected results for the test</param>
    /// <param name="oldText">The reference text to compare to</param>
    /// <param name="newText">The updated text to compare</param>
    private static void Test(LineModificationType[] expected, string oldText, string newText)
    {
        MemoryOwner<LineModificationType> result = LineDiffer.ComputeDiff(oldText, newText, '\r');

        try
        {
            Assert.AreEqual(expected.Length, result.Length);

            if (expected.Length == 0) return;

            Assert.IsTrue(expected.SequenceEqual(result.Span.ToArray()));
        }
        finally
        {
            result.Dispose();
        }
    }
}
