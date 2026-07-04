using JoyfulReaperLib.JRConsole;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JoyfulReaperLibTests;

[TestClass]
public class ConsoleHelperTests
{
    [TestMethod]
    public void GetColorSequence_KeepsLeadingWhitespaceFromUsingNegativeIndex()
    {
        var colors = new[] { ConsoleColor.Red, ConsoleColor.Green };

        var result = ConsoleHelper.GetColorSequence(colors, " hi", random: false);

        Assert.IsNull(result[0].Color);
        Assert.AreEqual(ConsoleColor.Red, result[1].Color);
        Assert.AreEqual(ConsoleColor.Green, result[2].Color);
    }

    [TestMethod]
    public void GetColorSequence_KeepsWhitespaceOnCurrentSequentialColor()
    {
        var colors = new[] { ConsoleColor.Red, ConsoleColor.Green };

        var result = ConsoleHelper.GetColorSequence(colors, "a b", random: false);

        Assert.AreEqual(ConsoleColor.Red, result[0].Color);
        Assert.AreEqual(ConsoleColor.Red, result[1].Color);
        Assert.AreEqual(ConsoleColor.Green, result[2].Color);
    }

    [TestMethod]
    public void GetColorSequence_RejectsMissingColors()
    {
        Assert.Throws<ArgumentNullException>(() => ConsoleHelper.GetColorSequence(null!, "test"));
        Assert.Throws<ArgumentException>(() => ConsoleHelper.GetColorSequence(Array.Empty<ConsoleColor>(), "test"));
    }
}
