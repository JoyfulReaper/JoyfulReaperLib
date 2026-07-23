using System;
using System.Text;

namespace JoyfulReaperLib.JREncryption;

/// <summary>
/// Classical cipher helpers. These algorithms are educational toy ciphers and are not suitable for real security.
/// </summary>
public static class CipherHelper
{
    /// <summary>
    /// Encrypt using a Caesar cipher.
    /// </summary>
    /// <param name="input">The string to encrypt.</param>
    /// <param name="key">The number of positions to shift.</param>
    /// <param name="stripNonAlphaChars">Uppercase letters and remove non-letter characters when true.</param>
    /// <returns>The transformed string.</returns>
    public static string CaesarEncipher(string input, int key, bool stripNonAlphaChars = true)
    {
        ArgumentNullException.ThrowIfNull(input);

        StringBuilder output = new StringBuilder(input.Length);
        foreach (char c in input)
        {
            char source = stripNonAlphaChars && char.IsAsciiLetter(c)
                ? char.ToUpperInvariant(c)
                : c;
            char result = CaesarCipher(source, key, stripNonAlphaChars);
            if (result != char.MinValue)
            {
                output.Append(result);
            }
        }

        return output.ToString();
    }

    /// <summary>
    /// Rotate a character a given number of positions.
    /// </summary>
    /// <param name="ch">The character to rotate.</param>
    /// <param name="key">The number of positions to rotate. Positive values move forward.</param>
    /// <param name="stripNonAlphaChars">Remove spaces and any other non-alphabetic characters.</param>
    /// <returns>The rotated character, or <see cref="char.MinValue"/> when a non-letter is stripped.</returns>
    public static char CaesarCipher(char ch, int key, bool stripNonAlphaChars)
    {
        if (!char.IsAsciiLetter(ch))
        {
            return stripNonAlphaChars ? char.MinValue : ch;
        }

        int normalizedKey = ((key % 26) + 26) % 26;
        char offset = char.IsUpper(ch) ? 'A' : 'a';
        int position = ch - offset;
        return (char)(offset + ((position + normalizedKey) % 26));
    }

    [Obsolete("Use CaesarCipher instead.")]
    public static char CeasarCipher(char ch, int key, bool stripNonAlphaChars)
    {
        return CaesarCipher(ch, key, stripNonAlphaChars);
    }

    /// <summary>
    /// Decrypt using a Caesar cipher.
    /// </summary>
    /// <param name="input">The string to decrypt.</param>
    /// <param name="key">The number of positions to shift.</param>
    /// <returns>The decrypted string.</returns>
    public static string CaesarDecipher(string input, int key)
    {
        ArgumentNullException.ThrowIfNull(input);

        return CaesarEncipher(input, -key, false);
    }

    [Obsolete("Use CaesarDecipher instead.")]
    public static string CeasarDecipher(string input, int key)
    {
        return CaesarDecipher(input, key);
    }

    public enum Direction
    {
        Encipher = 1,
        Decipher = -1
    }

    /// <summary>
    /// Encipher or decipher text using the Vigenere cipher.
    /// </summary>
    /// <param name="text">The text to transform.</param>
    /// <param name="key">The key to use.</param>
    /// <param name="dir">The direction to move the text.</param>
    /// <param name="stripNonAlphaChars">Remove or preserve non-letter characters.</param>
    /// <returns>The transformed text.</returns>
    public static string VigenereCipher(string text, string key, Direction dir, bool stripNonAlphaChars = true)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        string normalizedKey = key.ToUpperInvariant();
        foreach (char keyChar in normalizedKey)
        {
            if (!char.IsAsciiLetter(keyChar))
            {
                throw new ArgumentException("Key must contain only A-Z letters.", nameof(key));
            }
        }

        int keyIndex = 0;
        int direction = (int)dir;
        StringBuilder output = new StringBuilder(text.Length);

        foreach (char character in text)
        {
            if (!char.IsAsciiLetter(character))
            {
                if (!stripNonAlphaChars)
                {
                    output.Append(character);
                }

                continue;
            }

            int plainTextIndex = char.ToUpperInvariant(character) - 'A';
            int keyOffset = normalizedKey[keyIndex] - 'A';
            int rotatedIndex = plainTextIndex + (direction * keyOffset);
            if (rotatedIndex < 0)
            {
                rotatedIndex += 26;
            }

            output.Append((char)('A' + (rotatedIndex % 26)));

            keyIndex++;
            if (keyIndex == normalizedKey.Length)
            {
                keyIndex = 0;
            }
        }

        return output.ToString();
    }

    [Obsolete("Use VigenereCipher instead.")]
    public static string VigenereChiper(string text, string key, Direction dir, bool stripNonAlphaChars = true)
    {
        return VigenereCipher(text, key, dir, stripNonAlphaChars);
    }
}
