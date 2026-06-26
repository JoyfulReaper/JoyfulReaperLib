/*
 * JoyfulReaperLibrary
 * 
 *  Copyright (c) 2026 Kyle Givler
 * Licensed under the MIT License.
 */

using JoyfulReaperLib.JRMath.Fractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JoyfulReaperLibTests
{
    [TestClass]
    public class FractionTests
    {
        [TestMethod]
        public void TestCtor()
        {
            FractionModel f = new FractionModel(1, 2);
            Assert.AreEqual(1, f.Numerator);
            Assert.AreEqual(2, f.Denominator);
        }
    }
}
