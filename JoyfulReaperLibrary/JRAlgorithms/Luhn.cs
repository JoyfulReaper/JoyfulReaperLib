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
// https://en.wikipedia.org/wiki/Luhn_algorithm

namespace JoyfulReaperLib.JRAlgorithms
{
    public static class Luhn
    {
        public enum checkType { Luhn, Visa };

        /// <summary>
        /// Validate the check digit using the Luhn Algorithm
        /// </summary>
        /// <param name="number">The number to validate as a string</param>
        /// <returns>True if check digit is valid, false otherwise</returns>
        public static bool LuhnValidate(string number)
        {
            var checkDigit = ComputeCheckDigit(number.Substring(0, number.Length - 1));
            var test = int.Parse(number.Substring(number.Length - 1));
            return checkDigit == test;
        }

        public static bool LuhnValidate(string number, checkType type)
        {
            if(type == checkType.Luhn)
            {
                return LuhnValidate(number);
            }

            if(type == checkType.Visa)
            {
                return ValidateVisa(number);
            }

            return false;
        }

        private static bool ValidateVisa(string number)
        {
            if(!number.StartsWith("4"))
            {
                return false;
            }

            if(number.Length != 13 && number.Length != 16)
            {
                return false;
            }

            return LuhnValidate(number);
        }

        /// <summary>
        /// Adds the check digit to a number using the Lugn Algorithm
        /// </summary>
        /// <param name="number">The number to calculate the check digit of</param>
        /// <param name="checkDigit">The check digit</param>
        /// <returns></returns>
        public static string LuhnCreate(string number, out int checkDigit)
        {
            checkDigit = ComputeCheckDigit(number);
            return $"{number}{checkDigit}";
        }

        /// <summary>
        /// Compute the check digit for the given number using the Luhn Algorithm
        /// </summary>
        /// <param name="luhn">The number to computer the check digit for as a string</param>
        /// <returns>The check digit</returns>
        public static int ComputeCheckDigit(string number)
        {
            int nDigits = number.Length;
            int sum = 0;
            int parity = nDigits % 2;

            for (int i = 0; i < nDigits; i++)
            {
                int digit = int.Parse(number[i].ToString());

                if (i % 2 != parity)
                {
                    digit *= 2;
                }

                if (digit > 9)
                {
                    digit -= 9;
                }

                sum += digit;
            }

            return (sum * 9) % 10;
        }
    }
}
