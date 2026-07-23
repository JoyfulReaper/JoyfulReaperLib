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

namespace JoyfulReaperLib.JRCurrency;

public sealed class CurrencyUnit
{
    /// <summary>
    /// Name of the currency.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Value of the currency.
    /// </summary>
    public decimal Value { get; }

    /// <summary>
    /// Plural name of the currency.
    /// </summary>
    public string PluralName { get; }

    /// <summary>
    /// Quantity of this currency being represented.
    /// </summary>
    public int Quantity { get; }

    public CurrencyUnit(decimal value, string name, string pluralName = "", int quantity = 0)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Currency value must be greater than zero.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (quantity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity cannot be negative.");
        }

        Name = name;
        Value = value;
        PluralName = string.IsNullOrWhiteSpace(pluralName) ? $"{Name}s" : pluralName;
        Quantity = quantity;
    }

    public CurrencyUnit WithQuantity(int quantity)
    {
        return new CurrencyUnit(Value, Name, PluralName, quantity);
    }
}
