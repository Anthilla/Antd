using CommandLine;
using System;
using System.Linq;
using System.Threading;

namespace Antd2 {

    internal partial class Application {

        /// <summary>
        /// todo 
        /// da togliere poi e sostituire con configurazione corretta
        /// ma almeno non ho errori nei Moduli
        /// </summary>
        public static dynamic CurrentConfiguration = null;
        public static dynamic RunningConfiguration = null;
        public static dynamic Agent = null;

        public static bool IsUnix => Environment.OSVersion.Platform == PlatformID.Unix;

        private static void Main(string[] args) {
            PrepareConsole();
            if (args.Length > 0) {
                if (LineCommand.Options.TryGetValue(args[0], out Action<string[]> functionToExecute)) {
                    functionToExecute?.Invoke(args.Skip(1).ToArray());
                }
            }
            else {
                while (true) {
                    var line = Help.ReadLine();
                    if (LineCommand.Options.TryGetValue(line[0], out Action<string[]> functionToExecute)) {
                        functionToExecute?.Invoke(line.Skip(1).ToArray());
                    }
                }
            }
        }

        private static void PrepareConsole() {
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}