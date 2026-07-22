using JoyfulReaperLib.JRText;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JoyfulReaperLibTests;

[TestClass]
public class GreedyWrapTests
{
    [TestMethod]
    public void LineWrap_ReturnsEmptyForNullOrEmptyInput()
    {
        GreedyWrap wrapper = new GreedyWrap(5);

        Assert.AreEqual(string.Empty, wrapper.LineWrap(null));
        Assert.AreEqual(string.Empty, wrapper.LineWrap(string.Empty));
    }

    [TestMethod]
    public void LineWrap_WrapsBasicText()
    {
        GreedyWrap wrapper = new GreedyWrap(5);

        Assert.AreEqual($"hello{Environment.NewLine}world", wrapper.LineWrap("hello world"));
    }

    [TestMethod]
    public void LineWrap_SplitsLongWordsWhenEnabled()
    {
        GreedyWrap wrapper = new GreedyWrap(5, wrapWordsLongerThanLineWidth: true);

        Assert.AreEqual($"abcd-{Environment.NewLine}ef", wrapper.LineWrap("abcdef"));
    }

    [TestMethod]
    public void LineWrap_LeavesLongWordsIntactWhenDisabled()
    {
        GreedyWrap wrapper = new GreedyWrap(5, wrapWordsLongerThanLineWidth: false);

        Assert.AreEqual("abcdef", wrapper.LineWrap("abcdef"));
    }

    [TestMethod]
    public void LineWrap_DoesNotThrowNearBoundary()
    {
        GreedyWrap wrapper = new GreedyWrap(4, wrapWordsLongerThanLineWidth: true);

        string result = wrapper.LineWrap("abcd e");

        Assert.AreEqual($"abcd{Environment.NewLine}e", result);
    }

    [TestMethod]
    public void Constructor_RejectsInvalidLineWidth()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new GreedyWrap(0));
    }
}
