using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingHammerDataLib
{
    public class Log
    {
        private static readonly ConsoleColor COLOR_FATAL = ConsoleColor.Red;
        private static readonly ConsoleColor COLOR_ERROR = ConsoleColor.DarkRed;
        private static readonly ConsoleColor COLOR_WARN  = ConsoleColor.DarkYellow;
        private static readonly ConsoleColor COLOR_DEBUG = ConsoleColor.DarkCyan;

        public static void DEBUG(string message, Exception e = null)
        {
            ClearCurrentConsoleLine();
            ConsoleColor defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = COLOR_DEBUG;
            Console.WriteLine(message);
            Console.ForegroundColor = defaultColor;
        }

        public static void INFO(string message, bool desktopClient = false)
        {
            if(!desktopClient)
                ClearCurrentConsoleLine();

            Console.WriteLine(message);
        }

        public static void WARN(string message)
        {
            ClearCurrentConsoleLine();
            ConsoleColor defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = COLOR_WARN;
            Console.WriteLine(message);
            Console.ForegroundColor = defaultColor;
        }

        public static void ERROR(string message)
        {
            ClearCurrentConsoleLine();
            ConsoleColor defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = COLOR_ERROR;
            Console.WriteLine(message);
            Console.ForegroundColor = defaultColor;
        }

        public static void FATAL(string message)
        {
            ClearCurrentConsoleLine();
            ConsoleColor defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = COLOR_FATAL;
            Console.WriteLine(message);
            Console.ForegroundColor = defaultColor;
        }        

        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
}
