using System;
using System.Text;

namespace JoyfulReaperLib.JREncryption
{
    public static class CipherHelper
    {
        /// <summary>
        /// Encrypt using Caesar cipher
        /// </summary>
        /// <param name="input">The string to encrypt</param>
        /// <param name="key">The number of positions to shift</param>
        /// <param name="stripNonAlphaChars">Capitialze all letters, remove spaces and any other non-alphabetic characters</param>
        /// <returns>The input rotated key positions to the left</returns>
        public static string CaesarEncipher(string input, int key, bool stripNonAlphaChars = true)
        {
            StringBuilder sb = new StringBuilder();

            if (stripNonAlphaChars)
            {
                input = input.ToUpperInvariant();
            }

            foreach (char c in input)
            {
                var currentChar = CeasarCipher(c, key, stripNonAlphaChars);
                if (currentChar != char.MinValue)
                {
                    sb.Append(currentChar);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Rotate a character a given number of positions
        /// </summary>
        /// <param name="ch">The character to rotate</param>
        /// <param name="key">The number of positions to roate (+ for left - for right)</param>
        /// <param name="stripNonAlphaChars">Remove spaces and any other non-alphabetic characters</param>
        /// <returns></returns>
        public static char CeasarCipher(char ch, int key, bool stripNonAlphaChars)
        {
            if (!char.IsLetter(ch))
            {
                if (!stripNonAlphaChars)
                {
                    return ch;
                }
                else
                {
                    return char.MinValue;
                }
            }

            char offset = char.IsUpper(ch) ? 'A' : 'a';
            int position = (ch + key) - offset;
            int replacement = (position % 26);
            return (char)(replacement + offset);
        }

        /// <summary>
        /// Decrypt using Caesar cipher
        /// </summary>
        /// <param name="input">The string to decrypt</param>
        /// <param name="key">The number of positions to shift</param>
        /// <returns>The input rotated key positions to the right</returns>
        public static string CeasarDecipher(string input, int key)
        {
            return CaesarEncipher(input, 26 - key, false);
        }



        public enum Direction
        {
            Encipher = 1,
            Decipher = -1
        }

        //Based on this: https://rosettacode.org/wiki/Vigen%C3%A8re_cipher#C.23
        /// <summary>
        /// Enchiper using Vigenere Chiper
        /// </summary>
        /// <param name="text">The text to en/dechiper</param>
        /// <param name="key">The key to use</param>
        /// <param name="dir">The direction Direction.Enciper or Direction.Deciper</param>
        /// <param name="stripNonAlphaChars">Remove spaces and any other non-alphabetic characters</param>
        /// <returns>The de/enchipered text</returns>
        public static string VigenereChiper(string text, string key, Direction dir, bool stripNonAlphaChars = true)
        {
            int keyIndex = 0, tmp;
            StringBuilder output = new StringBuilder();
            int direction = (int)dir;

            text = text.ToUpperInvariant();
            key = key.ToUpperInvariant();

            foreach (char c in text)
            {
                if (c < 65)
                {
                    if (stripNonAlphaChars)
                    {
                        continue;
                    }
                    else
                    {
                        output.Append(c);
                        continue;
                    }
                }

                tmp = c - 65 + direction * (key[keyIndex] - 65);
                if (tmp < 0)
                {
                    tmp += 26;
                }

                output.Append(Convert.ToChar(65 + (tmp % 26)));

                if (++keyIndex == key.Length)
                {
                    keyIndex = 0;
                }
            }

            return output.ToString();
        }
    }
}