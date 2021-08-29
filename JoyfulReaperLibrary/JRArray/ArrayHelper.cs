using System;
using System.Collections.Generic;
using System.Text;

namespace JoyfulReaperLib.JRArray
{
    public class ArrayHelper
    {
        /// <summary>
        /// Shift array to the left, does not alter start index
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="startIndex"></param>
        private static void ShiftArrayLeft(int[] arr, int startIndex)
        {
            //TODO make generic
            for (int i = arr.Length - 1; i > startIndex; i--)
            {
                arr[i] = arr[i - 1];
            }

            foreach (var n in arr)
            {
                Console.Write($"{n} ");
            }
        }
    }
}
