using System;
using System.Collections.Generic;
using System.Text;

namespace JoyfulReaperLib.JRNumbers
{
    public static class NumberHelper
    {
        /// <summary>
        /// Get the number of digits for a given number
        /// </summary>
        /// <param name="number"></param>
        /// <returns>The number of digts in the given number</returns>
        public static int NumberOfDigits(int number)
        {
            int numDigits = 1;
            while ((number /= 10) != 0)
            {
                numDigits++;
            }

            return numDigits;
        }
    }
}
