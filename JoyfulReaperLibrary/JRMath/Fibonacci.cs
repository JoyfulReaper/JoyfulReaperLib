using System;
using System.Collections.Generic;
using System.Text;

namespace JoyfulReaperLib.JRMath
{
    public static class Fibonacci
    {
        /// <summary>
        /// Calculate Fibonacci Sequence
        /// </summary>
        /// <param name="first">F(0)</param>
        /// <param name="second">F(1)</param>
        /// <param name="term">Number of terms to calculate</param>
        /// <param name="recursive">True to calculate recursivly, false to use a loop</param>
        /// <returns>Fibonacci Sequence!</returns>
        public static long[] FibonacciSequence(long first, long second, long term, bool recursive = false)
        {
            if(recursive)
            {
                return FibonacciReceursive(first, second, term);
            }

            return FibonacciLoop(first, second, term);
        }

        /// <summary>
        /// Calculate Fibonacci Sequence
        /// </summary>
        /// <param name="first">F(0)</param>
        /// <param name="second">F(1)</param>
        /// <param name="term">Number of terms to calculate</param>
        /// <returns>Fibonacci Sequence!</returns>
        public static long[] FibonacciReceursive(long first, long second, long term)
        {
            long[] result = new long[term];
            FibonacciRecursiveHelper(first, second, 0, term, result);
            return result;
        }

        private static void FibonacciRecursiveHelper(long first, long second, long count, long term, long[] result)
        {
            if( count < term)
            {
                result[count] = first;
                FibonacciRecursiveHelper(second, first + second, ++count, term, result);
            }
        }

        /// <summary>
        /// Calculate Fibonacci Sequence
        /// </summary>
        /// <param name="first">F(0)</param>
        /// <param name="second">F(1)</param>
        /// <param name="term">Number of terms to calculate</param>
        /// <returns>Fibonacci Sequence!</returns>
        public static long[] FibonacciLoop(long first, long second, long term)
        {
            long[] output = new long[term];

            long a = first;
            long b = second;

            for (int i = 0; i < term; i++)
            {
                long fib = a;

                output[i] = fib;

                a = b;
                b = fib + b;
            }

            return output;
        }
    }
}
