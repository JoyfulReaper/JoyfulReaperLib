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
using System.Collections.Generic;
using System.Text;

namespace JoyfulReaperLib.JRText;

/// <summary>
/// A class for line wrapping, using a greedy wrapping algorithm.
/// </summary>
public class GreedyWrap
{
    private int _lineWidth;

    /// <summary>
    /// The line width to wrap to.
    /// </summary>
    public int LineWidth
    {
        get => _lineWidth;
        set => _lineWidth = value > 0
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value), "Line width must be greater than zero.");
    }

    /// <summary>
    /// The width of tabs in spaces.
    /// </summary>
    public ushort TabWidth { get; set; } = 4;

    /// <summary>
    /// If true, split words that are longer than the line width.
    /// </summary>
    public bool WrapWordsLongerThanLineWidth { get; set; }

    /// <summary>
    /// Construct a GreedyWrap instance.
    /// </summary>
    /// <param name="lineWidth">The line width to wrap to.</param>
    /// <param name="wrapWordsLongerThanLineWidth">If true, split words longer than line width.</param>
    public GreedyWrap(int lineWidth = 80, bool wrapWordsLongerThanLineWidth = false)
    {
        LineWidth = lineWidth;
        WrapWordsLongerThanLineWidth = wrapWordsLongerThanLineWidth;
    }

    /// <summary>
    /// Wrap text greedily.
    /// </summary>
    /// <param name="input">The text to wrap.</param>
    /// <returns>The wrapped text.</returns>
    public string LineWrap(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        string normalized = NormalizeWhitespace(input);
        string[] words = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (words.Length == 0)
        {
            return string.Empty;
        }

        StringBuilder output = new StringBuilder();
        int currentLineLength = 0;

        foreach (string word in words)
        {
            foreach (string segment in SplitWord(word))
            {
                if (currentLineLength == 0)
                {
                    output.Append(segment);
                    currentLineLength = segment.Length;
                    continue;
                }

                if (currentLineLength + 1 + segment.Length <= LineWidth)
                {
                    output.Append(' ').Append(segment);
                    currentLineLength += segment.Length + 1;
                    continue;
                }

                output.Append(Environment.NewLine).Append(segment);
                currentLineLength = segment.Length;
            }
        }

        return output.ToString();
    }

    private string NormalizeWhitespace(string input)
    {
        string expandedTabs = input.Replace("\t", new string(' ', TabWidth));
        return expandedTabs
            .Replace("\r\n", " ")
            .Replace('\r', ' ')
            .Replace('\n', ' ');
    }

    private IEnumerable<string> SplitWord(string word)
    {
        if (!WrapWordsLongerThanLineWidth || word.Length <= LineWidth)
        {
            yield return word;
            yield break;
        }

        int chunkLength = Math.Max(1, LineWidth - 1);
        int index = 0;

        while (index < word.Length)
        {
            int remaining = word.Length - index;
            if (remaining > chunkLength)
            {
                yield return $"{word.Substring(index, chunkLength)}-";
                index += chunkLength;
                continue;
            }

            yield return word[index..];
            yield break;
        }
    }
}
