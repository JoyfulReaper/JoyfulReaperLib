/*
 * JoyfulReaperLibrary
 *
 * Copyright (c) 2026 Kyle Givler
 * Licensed under the MIT License.
 */

using JoyfulReaperLib.JRNumbers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JoyfulReaperLibTests;

[TestClass]
public class BaseConversionTests
{
    [TestMethod]
    public void DecimalToBinary_ConvertsExpectedValues()
    {
        Assert.AreEqual("0", BaseConverter.DecimalToBinary(0));
        Assert.AreEqual("101", BaseConverter.DecimalToBinary(5));
        Assert.AreEqual("11111111111111111111111111111011", BaseConverter.DecimalToBinary(-5));
    }

    [TestMethod]
    public void BinaryToDecimal_ConvertsExpectedValues()
    {
        Assert.AreEqual(5, BaseConverter.BinaryToDecimal("101"));
        Assert.AreEqual(10, BaseConverter.BinaryToDecimal("1010"));
        Assert.AreEqual(500, BaseConverter.BinaryToDecimal("111110100"));
    }

    [TestMethod]
    public void BinaryToDecimal_RejectsInvalidInput()
    {
        Assert.Throws<FormatException>(() => BaseConverter.BinaryToDecimal("102"));
        Assert.Throws<ArgumentException>(() => BaseConverter.BinaryToDecimal(string.Empty));
        Assert.Throws<ArgumentNullException>(() => BaseConverter.BinaryToDecimal(null!));
    }
}
