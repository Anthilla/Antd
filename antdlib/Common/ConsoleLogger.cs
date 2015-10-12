///-------------------------------------------------------------------------------------
///     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
///     All rights reserved.
///
///     Redistribution and use in source and binary forms, with or without
///     modification, are permitted provided that the following conditions are met:
///         * Redistributions of source code must retain the above copyright
///           notice, this list of conditions and the following disclaimer.
///         * Redistributions in binary form must reproduce the above copyright
///           notice, this list of conditions and the following disclaimer in the
///           documentation and/or other materials provided with the distribution.
///         * Neither the name of the Anthilla S.r.l. nor the
///           names of its contributors may be used to endorse or promote products
///           derived from this software without specific prior written permission.
///
///     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
///     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
///     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
///     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
///     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
///     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
///     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
///     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
///     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
///     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///
///     20141110
///-------------------------------------------------------------------------------------

using System;
using System.Linq;

namespace antdlib {

    /// <summary>
    /// Simple console Logger
    /// </summary>
    public static class ConsoleLogger {

        public static void Log(dynamic message, params object[] args) {
            if (args.Any())
                message = String.Format(message, args);
            Console.WriteLine("{0}{1}"
                , ConsoleTime.GetTime(DateTime.Now)
                , message);
        }

        public static void Info(dynamic message, params object[] args) {
            if (args.Any())
                message = String.Format(message, args);
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("{0}{1}"
                , ConsoleTime.GetTime(DateTime.Now)
                , message);
            Console.ForegroundColor = currentColor;
        }

        public static void Success(dynamic message, params object[] args) {
            if (args.Any())
                message = String.Format(message, args);
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("{0}{1}"
                , ConsoleTime.GetTime(DateTime.Now)
                , message);
            Console.ForegroundColor = currentColor;
        }

        public static void Warn(dynamic message, params object[] args) {
            if (args.Any())
                message = String.Format(message, args);
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("{0}Warn: {1}"
                , ConsoleTime.GetTime(DateTime.Now)
                , message);
            Console.ForegroundColor = currentColor;
        }

        public static void Error(dynamic message, params object[] args) {
            if (args.Any())
                message = String.Format(message, args);
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("{0}Error: {1}"
                , ConsoleTime.GetTime(DateTime.Now)
                , message);
            Console.ForegroundColor = currentColor;
            //Environment.Exit(-1);
        }

        public static void Point(string where, string message = "") {
            //System.Reflection.MethodBase.GetCurrentMethod().Name
            var currentFG = Console.ForegroundColor;
            var currentBG = Console.BackgroundColor;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"○○○ {where} ○○○ {message}");
            Console.ForegroundColor = currentFG;
            Console.BackgroundColor = currentBG;
        }
    }
}