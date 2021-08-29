using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using JoyfulReaperLib.JRNumbers;

namespace JoyfulReaperLibTests
{
    [TestClass]
    public class NumberTests
    {
        [TestMethod]
        public void TestNumberOfDigits()
        {
            Assert.AreEqual(1, NumberHelper.NumberOfDigits(1));
            Assert.AreEqual(5, NumberHelper.NumberOfDigits(12345));
            Assert.AreEqual(2, NumberHelper.NumberOfDigits(14));
            Assert.AreEqual(3, NumberHelper.NumberOfDigits(199));
            Assert.AreEqual(1, NumberHelper.NumberOfDigits(9));
            Assert.AreEqual(4, NumberHelper.NumberOfDigits(1235));

            Assert.AreEqual(1, NumberHelper.NumberOfDigits(-1));
            Assert.AreEqual(5, NumberHelper.NumberOfDigits(-12345));
            Assert.AreEqual(2, NumberHelper.NumberOfDigits(-14));
            Assert.AreEqual(3, NumberHelper.NumberOfDigits(-199));
            Assert.AreEqual(1, NumberHelper.NumberOfDigits(-9));
            Assert.AreEqual(4, NumberHelper.NumberOfDigits(-1235));
        }
    }
}
