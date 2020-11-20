using Microsoft.VisualStudio.TestTools.UnitTesting;
using JoyfulReaperLib.JRCurrency;
using System.Collections.Generic;
using System.Linq;

namespace JoyfulReaperLibTests
{
    [TestClass]
    public class CurrencyTests
    {
        [TestMethod]
        public void TestChange()
        {
            List<CurrencyUnit> coins = CurrencyHelper.GetUSDCommonCoins();
            decimal changeDue = 1.69m;

            var change = CurrencyHelper.CalculateChange(changeDue, coins);

            Assert.AreEqual(6, change.Where(x=> x.Name == "quarter").First().Quantity);
            Assert.AreEqual(1, change.Where(x => x.Name == "dime").First().Quantity);
            Assert.AreEqual(1, change.Where(x => x.Name == "nickel").First().Quantity);
            Assert.AreEqual(4, change.Where(x => x.Name == "penny").First().Quantity);
        }
    }
}
