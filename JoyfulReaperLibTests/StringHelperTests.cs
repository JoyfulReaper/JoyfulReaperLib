using JoyfulReaperLib.JRText;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoyfulReaperLibTests
{
    [TestClass]
    public class StringTests
    {
        [TestMethod]
        public void TestReverse()
        {
            Assert.AreEqual("poop", StringHelper.Reverse("poop"));
            Assert.AreEqual("yppaH", StringHelper.Reverse("Happy"));
        }

        [TestMethod]
        public void TestIsPalindrome()
        {
            Assert.AreEqual(true, StringHelper.IsPalindrome("poop"));
            Assert.AreEqual(false, StringHelper.IsPalindrome("Happy"));
        }

        [TestMethod]
        public void TestVowelAnalysis()
        {
            string input = "This is the input 123!";
            StringHelper.VowelAnalysis(input, out int consonants, out int whitespace, out int numbers, out int unknown);

            Assert.AreEqual(9, consonants);
            Assert.AreEqual(4, whitespace);
            Assert.AreEqual(3, numbers);
            Assert.AreEqual(1, unknown);
        }
    }
}
