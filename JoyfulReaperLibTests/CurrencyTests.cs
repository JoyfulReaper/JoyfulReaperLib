/*
 * JoyfulReaperLibrary
 *
 * Copyright (c) 2026 Kyle Givler
 * Licensed under the MIT License.
 */

using JoyfulReaperLib.JRCurrency;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JoyfulReaperLibTests;

[TestClass]
public class CurrencyTests
{
    [TestMethod]
    public void CalculateChange_UsesGreedyUsdCoins()
    {
        List<CurrencyUnit> coins = CurrencyHelper.GetUSDCommonCoins();

        List<CurrencyUnit> change = CurrencyHelper.CalculateChange(1.69m, coins);

        Assert.AreEqual(6, change.Single(x => x.Name == "quarter").Quantity);
        Assert.AreEqual(1, change.Single(x => x.Name == "dime").Quantity);
        Assert.AreEqual(1, change.Single(x => x.Name == "nickel").Quantity);
        Assert.AreEqual(4, change.Single(x => x.Name == "penny").Quantity);
    }

    [TestMethod]
    public void CalculateChange_DoesNotMutateInputUnits()
    {
        List<CurrencyUnit> coins = CurrencyHelper.GetUSDCommonCoins();

        _ = CurrencyHelper.CalculateChange(0.30m, coins);

        Assert.IsTrue(coins.All(x => x.Quantity == 0));
    }

    [TestMethod]
    public void CurrencyUnit_RejectsInvalidValues()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CurrencyUnit(0m, "bad"));
        Assert.Throws<ArgumentException>(() => new CurrencyUnit(1m, ""));
        Assert.Throws<ArgumentOutOfRangeException>(() => new CurrencyUnit(1m, "coin", quantity: -1));
    }

    [TestMethod]
    public void CalculateChange_ThrowsWhenChangeIsImpossible()
    {
        List<CurrencyUnit> units =
        [
            new CurrencyUnit(0.10m, "dime")
        ];

        Assert.Throws<ArgumentException>(() => CurrencyHelper.CalculateChange(0.03m, units));
    }
}
