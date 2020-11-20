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

namespace JoyfulReaperLib.JRMath.Fractions
{
    public static class FractionHelper
    {
        /// <summary>
        /// Return the number of places after the decimal point
        /// </summary>
        /// <param name="model">The FractionModel to check</param>
        /// <returns>The number of places after the decimal point</returns>
        public static int NumberOfDecimalPlaces(FractionModel model)
        {
            decimal number = decimal.Divide(model.Numerator, model.Denominator);

            return NumberOfDecimalPlaces(number);
        }

        /// <summary>
        /// Return the number of places after the decimal point
        /// </summary>
        /// <param name="d">The decimal to check</param>
        /// <returns>The number of places after the decimal point</returns>
        public static int NumberOfDecimalPlaces(decimal d)
        {
            string[] digits = d.ToString().Split('.');

            if (digits.Length != 2)
            {
                return 0;
            }

            return digits[1].Length;
        }

        /// <summary>
        /// Finds the greatest common divisor of a fraction
        /// </summary>
        /// <param name="model">The fraction model to find the GCD of</param>
        /// <returns>The greatest common divisor</returns>
        public static int FindGreatestCommonFactor(FractionModel model)
        {
            return FindGreatestCommonFactor(model.Denominator, model.Numerator);
        }

        /// <summary>
        /// Finds the greatest common divisor of two positive numbers
        /// </summary>
        /// <param name="a">First number</param>
        /// <param name="b">Second number</param>
        /// <returns>The greatest Common Divisor</returns>
        public static int FindGreatestCommonFactor(int a, int b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);
            //Euclid's algorithm
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            return a | b;
        }

        public static int FindGreatestCommonFactor(int[] terms)
        {
            int result = terms[0];

            for (int i = 0; i < terms.Length; i++)
            {
                result = FindGreatestCommonFactor(terms[i], result);

                if (result == 1)
                {
                    return 1;
                }
            }

            return result;
        }

        public static int FindLeastCommonMultiple(int[] terms)
        {
            int result = terms[0];

            for (int i = 0; i < terms.Length; i++)
            {
                result = FindLeastCommonMultiple(terms[i], result);
            }

            return result;
        }

        /// <summary>
        /// Find the least common multiple of two intgers
        /// </summary>
        /// <param name="a">An intger</param>
        /// <param name="b">An intger</param>
        /// <returns>The least common multiple</returns>
        public static int FindLeastCommonMultiple(int a, int b)
        {
            return (a / FindGreatestCommonFactor(a, b) * b);
        }

        /// <summary>
        /// Convert a decimal to a fraction
        /// </summary>
        /// <param name="d">The decimal to convert</param>
        /// <returns>The decimal represented as a fraction</returns>
        public static FractionModel ConvertDecimalToFraction(decimal d)
        {
            double decimalPlaces = NumberOfDecimalPlaces(d);
            int multiplier = (int) Math.Pow(10, decimalPlaces);

            FractionModel fraction = new FractionModel((int)(d * multiplier), multiplier);

            fraction.Simplify();

            return fraction;
        }

        /// <summary>
        /// Reduce a fraction
        /// </summary>
        /// <param name="m">The fraction to reduce</param>
        public static void Simplify(this FractionModel m)
        {
            int gcd = FindGreatestCommonFactor(m);
            m.Numerator /= gcd;
            m.Denominator /= gcd;

            //return new FractionModel(m.Numerator / gcd, m.Denominator / gcd);
        }

        /// <summary>
        /// Get the Reciprocal of a fraction
        /// </summary>
        /// <param name="model">The fraction to get the Reciprocal of</param>
        /// <returns>The reciprocal of the fraction</returns>
        public static FractionModel GetReciprocal(FractionModel model)
        {
            return new FractionModel(model.Denominator, model.Numerator);
        }

        /// <summary>
        /// Multiply two fractions
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>The product of a and b</returns>
        public static FractionModel Multiply(FractionModel a, FractionModel b)
        {
            var res = new FractionModel(a.Numerator * b.Numerator, a.Denominator * b.Denominator);
            res.Simplify();
            return res;
        }

        /// <summary>
        /// Divides two fractions
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>The quotient of a and b</returns>
        public static FractionModel Divide(FractionModel a, FractionModel b)
        {
            var res = Multiply(a, GetReciprocal(b));
            res.Simplify();

            if(res.Denominator < 0 && res.Numerator < 0)
            {
                res.Numerator = Math.Abs(res.Numerator);
                res.Denominator = Math.Abs(res.Denominator);
            }

            return res;
        }

        /// <summary>
        /// Adds two fractions
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>The sum of a and b</returns>
        public static FractionModel Add(FractionModel a, FractionModel b)
        {
            int lcm = FindLeastCommonMultiple(a.Denominator, b.Denominator);
            int num = a.Numerator * (lcm / a.Denominator) + b.Numerator * (lcm / b.Denominator);
            FractionModel res = new FractionModel(num, lcm);
            res.Simplify();

            return res;
        }

        /// <summary>
        /// Subtracts two fractions
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>The difference of a and f=b</returns>
        public static FractionModel Subtract(FractionModel a, FractionModel b)
        {
            int lcm = FindLeastCommonMultiple(a.Denominator, b.Denominator);
            int num = a.Numerator * (lcm / a.Denominator) - b.Numerator * (lcm / b.Denominator);
            FractionModel res = new FractionModel(num, lcm);
            res.Simplify();

            return res;
        }

        /// <summary>
        /// Parse to a fraction
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FractionModel ParseFraction(string value)
        {
            int whole = 0;
            string[] tokens;

            if (value.Contains(" "))
            {
                tokens = value.Split(' ');

                whole = int.Parse(tokens[0]);
                value = tokens[1];
            }

            tokens = value.Split('/');
            int num;
            int den;

            if (tokens.Length == 1 && int.TryParse(tokens[0], out num))
            {
                return new FractionModel(num, 1);
            }
            else if (tokens.Length == 2 && int.TryParse(tokens[0], out num) && int.TryParse(tokens[1], out den))
            {
                if(whole > 0)
                {
                    num += whole * den;
                }

                return new FractionModel(num, den);
            }
            throw new ArgumentException("Invalid fraction format");
        }

        /// <summary>
        /// Try to parse a fraction
        /// </summary>
        /// <param name="value">The value to parse</param>
        /// <param name="fraction">The result of parsing value, or 0 on failure</param>
        /// <returns>True on success, false on failure</returns>
        public static bool TryParseFraction(string value, out FractionModel fraction)
        {
            fraction = new FractionModel(0,1);

            try
            {
                fraction = ParseFraction(value);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }
    }
}
