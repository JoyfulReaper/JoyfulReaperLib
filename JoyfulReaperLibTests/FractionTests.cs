using Microsoft.VisualStudio.TestTools.UnitTesting;
using JoyfulReaperLib.JRMath.Fractions;

namespace JoyfulReaperLibTests
{
    [TestClass]
    public class FractionTests
    {
        [TestMethod]
        public void TestCtor()
        {
            FractionModel f = new FractionModel(1, 2);
            Assert.AreEqual(f.Numerator, 1);
            Assert.AreEqual(f.Denominator, 2);
        }
    }
}
