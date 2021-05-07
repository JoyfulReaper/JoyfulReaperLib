/*
MIT License

Copyright(c) 2021 Kyle Givler
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
using System.Linq;

namespace JoyfulReaperLib.JREnums
{

    public static class EnumHelper
    {
        private static readonly Random _random = new Random();

        public static T RandomEnumValue<T>()
        {
            // Todo work with negative numbers
            var values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(_random.Next(values.Length));
        }

        /// <summary>
        /// Check is a value is valid for the given enum
        /// </summary>
        /// <typeparam name="T">The type of the Enum</typeparam>
        /// <param name="value">The value to check</param>
        /// <returns>True if valid, otherwise false</returns>
        public static bool EnumValueIsValid<T>(T value)
        {
            return Enum.IsDefined(typeof(T), value);
        }

        /// <summary>
        /// Get a tuple with the min and max enum values
        /// </summary>
        /// <typeparam name="T">Enum to check</typeparam>
        /// <returns>Tuple with min and max enum values</returns>
        public static (int min, int max) GetEnumMinMax<T>()
        {
            return (
                Enum.GetValues(typeof(T)).Cast<int>().Min(), 
                Enum.GetValues(typeof(T)).Cast<int>().Max()
            );
        }
    }
}
