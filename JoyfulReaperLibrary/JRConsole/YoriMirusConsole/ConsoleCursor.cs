using System;
using System.Collections.Generic;
using System.Text;

using JoyfulReaperLib.JRConsole.YoriMirusConsole.Enums;

namespace JoyfulReaperLib.JRConsole.YoriMirusConsole
{
    public class ConsoleCursor
    {
        /// <summary>
        /// CursorTop index of the cursor, starts from 0.
        /// </summary>
        public int  TopIndex { get; private set; }
        /// <summary>
        /// CursorLeft index of the cursor, starts from 0.
        /// </summary>
        public int  LeftIndex { get; private set; }
        public bool CursorRefresh { get; set; }
        public int MaximumLeft { get; private set; }
        public int MaximumTop { get; private set; }


        /// <summary>
        /// Creates a new cursor without any scrolling.
        /// </summary>
        /// <param name="maximumLeft">The highest LeftIndex value you can have</param>
        /// <param name="maximumTop">The highest TopIndex value you can have</param>
        public ConsoleCursor(int maximumLeft, int maximumTop)
        {
            TopIndex = 0;
            LeftIndex = 0;
            CursorRefresh = true;
            MaximumLeft = maximumLeft;
            MaximumTop = maximumTop;
        }
        /// <summary>
        /// Moves the cursor by one in a specific direction.
        /// </summary>
        /// <param name="direction"></param>
        public void MoveCursor(CursorMovementDirection direction)
        {
            switch (direction)
            {
                case CursorMovementDirection.Up:
                    if (TopIndex > 0)
                        TopIndex--;
                    break;
                case CursorMovementDirection.Down:
                    if (TopIndex < MaximumTop)
                        TopIndex++;
                    break;
                case CursorMovementDirection.Left:
                    if (LeftIndex > 0)
                        LeftIndex--;
                    break;
                case CursorMovementDirection.Right:
                    if (LeftIndex < MaximumLeft)
                        LeftIndex++;
                    break;
                default:
                    throw new ArgumentException("Unexpected movement direction.");
            }
            CursorRefresh = true;
        }
        /// <summary>
        /// Sets cursor to a specific position.
        /// If the index is out of range (index is lower than 0 or higher than maximum allowed), chooses closest accepted value.
        /// </summary>
        /// <param name="leftIndex"></param>
        /// <param name="topIndex"></param>
        public void SetPosition(int leftIndex, int topIndex)
        {
            if (LeftIndex > MaximumLeft)
                LeftIndex = MaximumLeft;
            else if (leftIndex < 0)
                LeftIndex = 0;
            else
                LeftIndex = leftIndex;

            if (topIndex > MaximumTop)
                TopIndex = MaximumTop;
            else if (topIndex < 0)
                TopIndex = 0;
            else
                TopIndex = topIndex;
            CursorRefresh = true;
        }

        public void SetCursorRefresh(bool value) => CursorRefresh = value;
    }
}
