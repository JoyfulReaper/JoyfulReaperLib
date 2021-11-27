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

namespace JoyfulReaperLib.JRNumbers
{
    public static class BaseConverter
    {
        public static string DecimalToBinary(int number)
        {
            string result = string.Empty;
            while (number > 0)
            {
                result = number % 2 + result;
                number /= 2;
            }

            return result == "" ? "0" : result;
        }

        public static int BinaryToDecimal(string number)
        {
            if (!int.TryParse(number, out _))
            {
                throw new ArgumentException(nameof(number) + " could not be converted to an integer!");
            }

            int result = 0;
            int place = 0;
            for (int i = number.Length - 1; i >= 0; i--)
            {
                var curr = int.Parse(number[i].ToString());
                if (curr == 1)
                {
                    result = result + (int)Math.Pow(2, place);
                }
                place++;
            }

            return result;
        }
    }
}
