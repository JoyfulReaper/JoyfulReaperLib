/*
 * JoyfulReaperLibrary
 *
 * Copyright (c) 2026 Kyle Givler
 * Licensed under the MIT License.
 */

using JoyfulReaperLib.JRAlgorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JoyfulReaperLibTests;

[TestClass]
public class AlgorithTests
{
    [TestMethod]
    public void LuhnValidation_ReturnsExpectedValues()
    {
        Assert.IsTrue(Luhn.LuhnValidate("79927398713"));
        Assert.IsFalse(Luhn.LuhnValidate("79927398710"));
        Assert.IsFalse(Luhn.LuhnValidate(null));
        Assert.IsFalse(Luhn.LuhnValidate(string.Empty));
        Assert.IsFalse(Luhn.LuhnValidate("ABC123"));
    }

    [TestMethod]
    public void ComputeCheckDigit_RejectsInvalidInput()
    {
        Assert.AreEqual(3, Luhn.ComputeCheckDigit("7992739871"));
        Assert.Throws<ArgumentException>(() => Luhn.ComputeCheckDigit("12A"));
        Assert.Throws<ArgumentException>(() => Luhn.ComputeCheckDigit(string.Empty));
    }

    [TestMethod]
    public void VisaValidation_UsesPrefixAndLengthRules()
    {
        Assert.IsTrue(Luhn.LuhnValidate("4111111111111111", Luhn.CheckType.Visa));
        Assert.IsFalse(Luhn.LuhnValidate("5111111111111111", Luhn.CheckType.Visa));
        Assert.IsFalse(Luhn.LuhnValidate("411111111111", Luhn.CheckType.Visa));
    }
}
