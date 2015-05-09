using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antd.Common {
    /// <summary>
    /// Simple console Logger
    /// </summary>
    public static class ConsoleLogger {
        public static void Log(string message, params object[] args) {
            if (args.Count() > 0) message = String.Format(message, args);
            Console.WriteLine("{0}{1}"
                , ConsoleTime.GetTime(DateTime.Now)
                , message);
        }
        public static void Warn(string message, params object[] args) {
            if (args.Count() > 0) message = String.Format(message, args);
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("{0}Warn: {1}"
                , ConsoleTime.GetTime(DateTime.Now)
                , message);
            Console.ForegroundColor = currentColor;
        }
        public static void Error(string message, params object[] args) {
            if (args.Count() > 0) message = String.Format(message, args);
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("{0}Error: {1}"
                , ConsoleTime.GetTime(DateTime.Now)
                , message);
            Console.ForegroundColor = currentColor;
            Environment.Exit(-1);
        }
    }
}
