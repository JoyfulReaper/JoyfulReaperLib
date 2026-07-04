/*
MIT License

Copyright(c) 2020 Kyle Givler
https://github.com/JoyfulReaper

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace JoyfulReaperLib.JRCurrency;

public static class CurrencyHelper
{
    /// <summary>
    /// Calculate change using a greedy algorithm.
    /// </summary>
    /// <param name="totalChange">The total amount of change to return.</param>
    /// <param name="currencyUnits">The currency units available to give change in.</param>
    /// <returns>A list of currency units with quantities needed to make the requested change.</returns>
    public static List<CurrencyUnit> CalculateChange(decimal totalChange, IEnumerable<CurrencyUnit> currencyUnits)
    {
        if (totalChange < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(totalChange), "Total change cannot be negative.");
        }

        ArgumentNullException.ThrowIfNull(currencyUnits);

        List<CurrencyUnit> orderedUnits = currencyUnits.OrderByDescending(x => x.Value).ToList();
        if (orderedUnits.Count == 0)
        {
            throw new ArgumentException("At least one currency unit is required.", nameof(currencyUnits));
        }

        List<CurrencyUnit> change = new List<CurrencyUnit>();
        decimal remaining = totalChange;

        foreach (CurrencyUnit currencyUnit in orderedUnits)
        {
            int count = (int)(remaining / currencyUnit.Value);
            remaining %= currencyUnit.Value;

            if (count > 0)
            {
                change.Add(currencyUnit.WithQuantity(count));
            }
        }

        if (remaining != 0)
        {
            throw new ArgumentException("Unable to successfully make change with the provided currency units.", nameof(totalChange));
        }

        return change;
    }

    /// <summary>
    /// Get a list of common US coins.
    /// </summary>
    /// <returns>List of common US coins.</returns>
    public static List<CurrencyUnit> GetUSDCommonCoins()
    {
        return
        [
            new CurrencyUnit(0.25m, "quarter"),
            new CurrencyUnit(0.10m, "dime"),
            new CurrencyUnit(0.05m, "nickel"),
            new CurrencyUnit(0.01m, "penny", "pennies"),
        ];
    }

    /// <summary>
    /// Get a list of common US paper currency.
    /// </summary>
    /// <returns>List of common US paper currency.</returns>
    public static List<CurrencyUnit> GetUSDCommonBills()
    {
        return
        [
            new CurrencyUnit(1.00m, "one dollar bill"),
            new CurrencyUnit(5.00m, "five dollar bill"),
            new CurrencyUnit(10.00m, "ten dollar bill"),
            new CurrencyUnit(20.00m, "twenty dollar bill"),
            new CurrencyUnit(50.00m, "fifty dollar bill"),
            new CurrencyUnit(100.00m, "one hundred dollar bill"),
        ];
    }
}
