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
    public void TestLuhn()
    {
        Assert.AreEqual(true, Luhn.LuhnValidate("125"));
        Assert.AreEqual(5, Luhn.ComputeCheckDigit("12"));
        Assert.AreEqual("125", Luhn.LuhnCreate("12", out _));
    }
}