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
        /// <returns>The input rotated key positions to the left</returns>
        public static string CaesarEncipher(string input, int key)
        {
            StringBuilder sb = new StringBuilder();

            foreach(char c in input)
            {
                sb.Append(CeasarCipher(c, key));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Rotate a character a given number of positions
        /// </summary>
        /// <param name="ch">The character to rotate</param>
        /// <param name="key">The number of positions to roate (+ for left - for right)</param>
        /// <returns></returns>
        public static char CeasarCipher(char ch, int key)
        {
            if(!char.IsLetter(ch))
            {
                return ch;
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
            return CaesarEncipher(input, 26 - key);
        }
    }
}
