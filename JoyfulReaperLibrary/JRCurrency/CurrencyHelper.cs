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

namespace JoyfulReaperLib.JRCurrency
{
    public static class CurrencyHelper
    {
        /// <summary>
        /// Calculate change from a purchase
        /// </summary>
        /// <param name="totalChange">The total amount of change to return</param>
        /// <param name="currencyUnits">The CurrencyUnits available to give change in</param>
        /// <returns>A list of CurrencyUnits with their Quantities set in order to give totalChange with the fewest possible "coins"</returns>
        public static List<CurrencyUnit> CalculateChange(decimal totalChange, List<CurrencyUnit> currencyUnits)
        {
            var change = new List<CurrencyUnit>();
            foreach (var currencyUnit in currencyUnits)
            {
                int count;

                count = (int)(totalChange / currencyUnit.Value);
                totalChange %= currencyUnit.Value;

                change.Add(new CurrencyUnit(currencyUnit.Value, currencyUnit.Name, currencyUnit.PluralName, count));
                currencyUnit.Quantity = count;
            }

            if (totalChange != 0)
            {
                throw new ArgumentException("Unable to successfully make change!", nameof(totalChange));
            }

            return change;
        }

        /// <summary>
        /// Get a List of common US Coins
        /// </summary>
        /// <returns>List of common US Coins</returns>
        public static List<CurrencyUnit> GetUSDCommonCoins()
        {
            List<CurrencyUnit> coins = new List<CurrencyUnit>();

            coins.Add(new CurrencyUnit(0.25m, "quarter"));
            coins.Add(new CurrencyUnit(0.10m, "dime"));
            coins.Add(new CurrencyUnit(0.05m, "nickel"));
            coins.Add(new CurrencyUnit(0.01m, "penny", "pennies"));

            return coins;
        }

        /// <summary>
        /// Get a list of common US paper currency
        /// </summary>
        /// <returns>list of common US paper currency</returns>
        public static List<CurrencyUnit> GetUSDCommonBills()
        {
            List<CurrencyUnit> bills = new List<CurrencyUnit>();

            bills.Add(new CurrencyUnit(1.00m, "one dollar bill"));
            bills.Add(new CurrencyUnit(5.00m, "five dollar bill"));
            bills.Add(new CurrencyUnit(10.00m, "ten dollar bill"));
            bills.Add(new CurrencyUnit(20.00m, "twenty dollar bill"));
            bills.Add(new CurrencyUnit(50.00m, "fifty dollar bill"));
            bills.Add(new CurrencyUnit(100.00m, "one hundred dollar bill"));

            return bills;
        }
    }
}
