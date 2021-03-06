﻿/*
MIT License

Copyright(c) 2020 Kyle Givler

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

namespace JoyfulReaperLib.JRText
{
    public static class StringHelper
    {
        /// <summary>
        /// Return null if value is null or whitespace, otherwise return value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string AssignNullIfEmpty(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return value;
        }

        /// <summary>
        /// Reverse a string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Reverse(string input)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = input.Length - 1; i >= 0; i--)
            {
                sb.Append(input[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Check to see if a string is a palindrome
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool isPalindrome(string input)
        {
            input = input.Replace(" ", string.Empty).ToUpperInvariant();

            return input == Reverse(input).ToUpperInvariant();
        }
    }
}
