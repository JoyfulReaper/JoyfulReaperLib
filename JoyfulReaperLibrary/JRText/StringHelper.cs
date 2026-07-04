/*
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

namespace JoyfulReaperLib.JRText;

public static class StringHelper
{
    /// <summary>
    /// Return null if value is null or whitespace, otherwise return value.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <returns>Null for null or whitespace input; otherwise the original value.</returns>
    public static string? AssignNullIfEmpty(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    /// <summary>
    /// Reverse a string.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The reversed string.</returns>
    public static string Reverse(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        char[] characters = input.ToCharArray();
        Array.Reverse(characters);
        return new string(characters);
    }

    /// <summary>
    /// Check to see if a string is a palindrome.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>True when the string is a palindrome.</returns>
    public static bool IsPalindrome(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        string normalized = input.Replace(" ", string.Empty).ToUpperInvariant();
        return normalized == Reverse(normalized);
    }

    /// <summary>
    /// Returns the number of vowels in the given string.
    /// </summary>
    /// <param name="input">The string to analyze.</param>
    /// <param name="consonants">The number of consonants.</param>
    /// <param name="whiteSpace">The number of whitespace characters.</param>
    /// <param name="numbers">The number of digits.</param>
    /// <param name="unknown">The number of non-alphanumeric, non-whitespace characters.</param>
    /// <returns>The number of vowels in the input.</returns>
    public static int VowelAnalysis(string input, out int consonants, out int whiteSpace, out int numbers, out int unknown)
    {
        ArgumentNullException.ThrowIfNull(input);

        char[] vowels = ['a', 'e', 'i', 'o', 'u'];

        int numVowels = 0;
        consonants = 0;
        whiteSpace = 0;
        numbers = 0;
        unknown = 0;

        foreach (char c in input)
        {
            if (Array.IndexOf(vowels, char.ToLowerInvariant(c)) >= 0)
            {
                numVowels++;
            }
            else if (char.IsLetterOrDigit(c))
            {
                if (char.IsLetter(c))
                {
                    consonants++;
                }
                else
                {
                    numbers++;
                }
            }
            else if (char.IsWhiteSpace(c))
            {
                whiteSpace++;
            }
            else
            {
                unknown++;
            }
        }

        return numVowels;
    }
}
