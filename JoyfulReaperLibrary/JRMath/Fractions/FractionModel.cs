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
using System.Text;

namespace JoyfulReaperLib.JRMath.Fractions
{
    public class FractionModel
    {
        /// <summary>
        /// Represents a fraction's Numerator
        /// </summary>
        public int Numerator { get; set; }

        /// <summary>
        /// Represents a fraction's Denominator
        /// </summary>
        public int Denominator { get; set; }

        /// <summary>
        /// Display the Fraction in a friendly format
        /// Just wraps ToString()
        /// </summary>
        public string Display 
        {
            get => ToString();
        }
  
        /// <summary>
        /// Construct a new Fraction Model
        /// </summary>
        /// <param name="numerator">Initial numerator</param>
        /// <param name="denominator">Inital denominator</param>
        public FractionModel(int numerator, int denominator)
        {
            if (denominator == 0)
            {
                throw new ArgumentOutOfRangeException("denominator cannot be zero.");
            }

            if(denominator < 0)
            {
                denominator = Math.Abs(denominator);
                numerator *= -1;
            }

            Numerator = numerator;
            Denominator = denominator;
        }

        public override string ToString()
        {
            if (Numerator == 0)
            {
                return "0";
            }

            if(Numerator == Denominator)
            {
                return "1";
            }

            if(Denominator == 1)
            {
                return Numerator.ToString();
            }

            bool negative = false;
            int num = Numerator;
            int dem = Denominator;
            int whole = 0;

            if(dem < 0 || num < 0)
            {
                negative = true;
                num = Math.Abs(num);
                dem = Math.Abs(dem);
            }

            if(num > dem)
            {
                whole = num / dem;
            }

            StringBuilder sb = new StringBuilder();

            if(negative)
            {
                sb.Append("-");
            }

            if(whole > 0)
            {
                sb.Append($"{ whole } ");
                num -= dem * whole;
            }

            sb.Append($"{num}/{dem}");

            return sb.ToString();
        }

        public static implicit operator FractionModel(string value)
        {
            return FractionHelper.ParseFraction(value);
        }

        public static FractionModel Parse(string value)
        {
            return FractionHelper.ParseFraction(value);
        }

        public static bool TryParse(string value, out FractionModel result)
        {
            return FractionHelper.TryParseFraction(value, out result);
        }

        public static explicit operator FractionModel(decimal value)
        {
            return FractionHelper.ConvertDecimalToFraction(value);
        }

        public static FractionModel operator + (FractionModel left, FractionModel right)
        {
            return FractionHelper.Add(left, right);
        }

        public static FractionModel operator - (FractionModel left, FractionModel right)
        {
            return FractionHelper.Subtract(left, right);
        }

        public static FractionModel operator * (FractionModel left, FractionModel right)
        {
            return FractionHelper.Multiply(left, right);
        }

        public static FractionModel operator / (FractionModel left, FractionModel right)
        {
            return FractionHelper.Divide(left, right);
        }

    }
}