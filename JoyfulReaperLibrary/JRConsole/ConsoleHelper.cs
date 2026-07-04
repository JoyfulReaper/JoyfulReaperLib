/*
MIT License

Copyright (c) 2020 Kyle Givler

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
using System.Collections.Generic;

namespace JoyfulReaperLib.JRConsole;

public static class ConsoleHelper
{
    public static ConsoleColor DefaultColor { get; set; } = Console.ForegroundColor;

    /// <summary>
    /// Output a message to the console in color.
    /// </summary>
    /// <param name="color">The color to output in.</param>
    /// <param name="message">The message to output.</param>
    public static void ColorWrite(ConsoleColor color, string message)
    {
        ArgumentNullException.ThrowIfNull(message);

        ConsoleColor old = Console.ForegroundColor;

        Console.ForegroundColor = color;
        Console.Write(message);
        Console.ForegroundColor = old;
    }

    /// <summary>
    /// Output a message to the console using the default color.
    /// </summary>
    /// <param name="message">The message to output.</param>
    public static void ColorWrite(string message)
    {
        ArgumentNullException.ThrowIfNull(message);

        ColorWrite(DefaultColor, message);
    }

    /// <summary>
    /// Output a message and newline to the console in color.
    /// </summary>
    /// <param name="color">The color to output.</param>
    /// <param name="message">The message to output.</param>
    public static void ColorWriteLine(ConsoleColor color, string message)
    {
        ColorWrite(color, $"{message}{Environment.NewLine}");
    }

    /// <summary>
    /// Write out a message with each letter in alternating colors.
    /// </summary>
    /// <param name="colors">A list of the colors to use.</param>
    /// <param name="message">The message to use.</param>
    /// <param name="random">True selects a random color from the list, false selects colors in sequence.</param>
    public static void MulticolorWriteLine(List<ConsoleColor> colors, string message, bool random = false)
    {
        MulticolorWrite(colors, $"{message}{Environment.NewLine}", random);
    }

    /// <summary>
    /// Write out a message with each letter in alternating colors.
    /// </summary>
    /// <param name="colors">A list of the colors to use.</param>
    /// <param name="message">The message to use.</param>
    /// <param name="random">True selects a random color from the list, false selects colors in sequence.</param>
    public static void MulticolorWrite(List<ConsoleColor> colors, string message, bool random = false)
    {
        foreach (var colorChoice in GetColorSequence(colors, message, random))
        {
            if (colorChoice.Color.HasValue)
            {
                ColorWrite(colorChoice.Color.Value, colorChoice.Character.ToString());
            }
            else
            {
                ColorWrite(DefaultColor, colorChoice.Character.ToString());
            }
        }
    }

    /// <summary>
    /// Output a message and newline to the console in the default color.
    /// </summary>
    /// <param name="message">The message to output.</param>
    public static void ColorWriteLine(string message)
    {
        ColorWriteLine(DefaultColor, message);
    }

    /// <summary>
    /// Display the prompt in a loop until a valid int is entered at the console.
    /// </summary>
    /// <param name="prompt">The prompt to display on the console.</param>
    /// <param name="min">Minimum int to accept.</param>
    /// <param name="max">Maximum int to accept.</param>
    /// <param name="color">Color to use when displaying the prompt.</param>
    /// <returns>The entered integer.</returns>
    public static int GetValidInt(string prompt, int min, int max, ConsoleColor color)
    {
        int output;

        do
        {
            ColorWrite(color, prompt);
        } while (!int.TryParse(Console.ReadLine(), out output) || output < min || output > max);

        return output;
    }

    /// <summary>
    /// Display the prompt in a loop until a valid int is entered at the console.
    /// </summary>
    /// <param name="prompt">The prompt to display on the console.</param>
    /// <param name="min">Minimum int to accept.</param>
    /// <param name="max">Maximum int to accept.</param>
    /// <returns>The entered integer.</returns>
    public static int GetValidInt(string prompt, int min, int max)
    {
        return GetValidInt(prompt, min, max, Console.ForegroundColor);
    }

    /// <summary>
    /// Display the prompt in a loop until a valid int is entered at the console.
    /// </summary>
    /// <param name="prompt">The prompt to display on the console.</param>
    /// <param name="color">The color to display the prompt in.</param>
    /// <returns>The entered integer.</returns>
    public static int GetValidInt(string prompt, ConsoleColor color)
    {
        return GetValidInt(prompt, int.MinValue, int.MaxValue, color);
    }

    /// <summary>
    /// Display the prompt in a loop until a valid int is entered at the console.
    /// </summary>
    /// <param name="prompt">The prompt to display on the console.</param>
    /// <returns>The entered integer.</returns>
    public static int GetValidInt(string prompt)
    {
        return GetValidInt(prompt, Console.ForegroundColor);
    }

    internal static IReadOnlyList<(char Character, ConsoleColor? Color)> GetColorSequence(
        IReadOnlyList<ConsoleColor> colors,
        string message,
        bool random = false)
    {
        ArgumentNullException.ThrowIfNull(colors);
        ArgumentNullException.ThrowIfNull(message);

        if (colors.Count == 0)
        {
            throw new ArgumentException("At least one color is required.", nameof(colors));
        }

        var result = new (char Character, ConsoleColor? Color)[message.Length];
        int colorIndex = -1;

        for (int i = 0; i < message.Length; i++)
        {
            char character = message[i];

            if (random)
            {
                result[i] = (character, colors[Random.Shared.Next(colors.Count)]);
                continue;
            }

            if (!char.IsWhiteSpace(character))
            {
                colorIndex = (colorIndex + 1) % colors.Count;
                result[i] = (character, colors[colorIndex]);
                continue;
            }

            result[i] = colorIndex >= 0
                ? (character, colors[colorIndex])
                : (character, null);
        }

        return result;
    }
}
