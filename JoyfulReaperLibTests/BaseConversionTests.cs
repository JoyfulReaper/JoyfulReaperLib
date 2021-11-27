using JoyfulReaperLib.JRNumbers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoyfulReaperLibTests
{
    [TestClass]
    public class BaseConversionTests
    {
        [TestMethod]
        public void TestDecimalToBinary()
        {
            Assert.AreEqual(5, BaseConverter.BinaryToDecimal("101"));
            Assert.AreEqual(2, BaseConverter.BinaryToDecimal("10"));
            Assert.AreEqual(1, BaseConverter.BinaryToDecimal("1"));
            Assert.AreEqual(0, BaseConverter.BinaryToDecimal("0"));
            Assert.AreEqual(10, BaseConverter.BinaryToDecimal("1010"));
            Assert.AreEqual(15, BaseConverter.BinaryToDecimal("1111"));
            Assert.AreEqual(500, BaseConverter.BinaryToDecimal("111110100"));
            Assert.AreEqual(200, BaseConverter.BinaryToDecimal("11001000"));
            Assert.AreEqual(11, BaseConverter.BinaryToDecimal("1011"));
        }

        [TestMethod]
        public void TestBinaryToDecimal()
        {
            Assert.AreEqual("101", BaseConverter.DecimalToBinary(5));
            Assert.AreEqual("10", BaseConverter.DecimalToBinary(2));
            Assert.AreEqual("1", BaseConverter.DecimalToBinary(1));
            Assert.AreEqual("0", BaseConverter.DecimalToBinary(0));
            Assert.AreEqual("1010", BaseConverter.DecimalToBinary(10));
            Assert.AreEqual("1111", BaseConverter.DecimalToBinary(15));
            Assert.AreEqual("111110100", BaseConverter.DecimalToBinary(500));
            Assert.AreEqual("11001000", BaseConverter.DecimalToBinary(200));
            Assert.AreEqual("1011", BaseConverter.DecimalToBinary(11));
        }
    }
}
