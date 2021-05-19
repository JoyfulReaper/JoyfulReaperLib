using System;
using System.Collections.Generic;
using System.Text;

namespace JoyfulReaperLib.JRLists
{
    public static class ListExtensions
    {
        private static readonly Random _random = new Random();

        public static T RandomItem<T>(this List<T> list)
        {
            return list[_random.Next(list.Count)];
        }
    }
}
