using System;
using System.Collections.Generic;
using System.Text;

namespace YoriMirusLib
{
    abstract class MenuBase
    {
        public ConsoleCursor MenuCursor { get; protected set; }
        public List<MenuOption> MenuOptions { get; protected set; }

        protected bool redisplayMenu;
        protected bool shutdown;

        public int WindowWidth { get; protected set; }
        public int WindowHeight { get; protected set; }

        /// <summary>
        /// Starts the menu, including waiting displaying the menu and waiting for input.
        /// </summary>
        public abstract void Start();
        public abstract void Display();
        protected abstract void DisplayFrame();
        protected abstract void DisplaySelection();

        /// <summary>
        /// Sets the window size of the console to the menu window size.
        /// </summary>
        protected void SetWindowSize()
        {
            Console.SetWindowSize(WindowWidth , WindowHeight);
        }
    }
}
