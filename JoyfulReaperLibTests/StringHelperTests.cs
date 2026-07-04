/*
 * JoyfulReaperLibrary
 *
 * Copyright (c) 2026 Kyle Givler
 * Licensed under the MIT License.
 */

using JoyfulReaperLib.JRText;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JoyfulReaperLibTests;

[TestClass]
public class StringTests
{
    [TestMethod]
    public void AssignNullIfEmpty_ReturnsExpectedValues()
    {
        Assert.IsNull(StringHelper.AssignNullIfEmpty(null));
        Assert.IsNull(StringHelper.AssignNullIfEmpty("   "));
        Assert.AreEqual("value", StringHelper.AssignNullIfEmpty("value"));
    }

    [TestMethod]
    public void Reverse_ReturnsExpectedValues()
    {
        Assert.AreEqual("poop", StringHelper.Reverse("poop"));
        Assert.AreEqual("yppaH", StringHelper.Reverse("Happy"));
    }

    [TestMethod]
    public void IsPalindrome_ReturnsExpectedValues()
    {
        Assert.IsTrue(StringHelper.IsPalindrome("poop"));
        Assert.IsFalse(StringHelper.IsPalindrome("Happy"));
    }

    [TestMethod]
    public void VowelAnalysis_ReturnsExpectedCounts()
    {
        const string input = "This is the input 123!";
        StringHelper.VowelAnalysis(input, out int consonants, out int whitespace, out int numbers, out int unknown);

        Assert.AreEqual(5, StringHelper.VowelAnalysis(input, out _, out _, out _, out _));
        Assert.AreEqual(9, consonants);
        Assert.AreEqual(4, whitespace);
        Assert.AreEqual(3, numbers);
        Assert.AreEqual(1, unknown);
    }
}
