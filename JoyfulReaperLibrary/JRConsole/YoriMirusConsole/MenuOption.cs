using System;
using System.Collections.Generic;
using System.Text;

namespace JoyfulReaperLib.JRConsole.YoriMirusConsole
{
    /// <summary>
    /// Class for making selections in the menu.
    /// </summary>
    public struct MenuOption
    {
        /// <summary>
        /// Background color of the option when it is selected.
        /// </summary>
        public ConsoleColor SelectedBG    { get; private set; }
        /// <summary>
        /// Background color of the option when it isn't selected.
        /// </summary>
        public ConsoleColor NotSelectedBG { get; private set; }
        /// <summary>
        /// Foreground color of the option when it is selected.
        /// </summary>
        public ConsoleColor SelectedFG    { get; private set; }
        /// <summary>
        /// Background color of the option when it isn't selected.
        /// </summary>
        public ConsoleColor NotSelectedFG { get; private set; }
        
        /// <summary>
        /// CursorTop index for the option to be selected
        /// </summary>
        public int SelectTopIndex  { get; private set; }
        /// <summary>
        /// CursorLeft index for the option to be selected
        /// </summary>
        public int SelectLeftIndex { get; private set; }

        /// <summary>
        /// Text to display.
        /// </summary>
        public string Text { get; private set; }

        public MenuOption(string text, int selectTopIndex, int selectLeftIndex = 0, ConsoleColor selectedBG = ConsoleColor.Blue, ConsoleColor notSelectedBG = ConsoleColor.Black, ConsoleColor selectedFG = ConsoleColor.Gray , ConsoleColor notSelectedFG = ConsoleColor.Gray)
        {
            SelectedBG = selectedBG;
            SelectedFG = selectedFG;

            NotSelectedBG = notSelectedBG;
            NotSelectedFG = notSelectedFG;

            SelectTopIndex = selectTopIndex;
            SelectLeftIndex = selectLeftIndex;
            Text = text;
        }

        public void WriteInCenter(int cursorLeftIndex, int cursorTopIndex, int cursorTop)
        {
            ConsoleColor prevBG = Console.BackgroundColor;

            if(cursorLeftIndex == SelectLeftIndex && cursorTopIndex == SelectTopIndex)
            {
                Console.BackgroundColor = SelectedBG;
                ConsoleHelper.WriteInCenter(Text, SelectedFG, cursorTop);
                Console.BackgroundColor = prevBG;
            }
            else
            {
                Console.BackgroundColor = NotSelectedBG;
                ConsoleHelper.WriteInCenter(Text, NotSelectedFG, cursorTop);
                Console.BackgroundColor = prevBG;
            }

        }
    }
}
