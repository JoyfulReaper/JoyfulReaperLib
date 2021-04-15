using System;
using System.Collections.Generic;
using System.Text;

namespace JoyfulReaperLib.JRText
{
    public static class StringHelper
    {
        public static string AssignNullIfEmpty(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return value;
        }
    }
}
