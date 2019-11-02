using CommandLine;
using System;
using System.Linq;
using System.Threading;

namespace Antd {

    internal partial class Application {

        private static void Main() {
            PrepareConsole();
            while (true) {
                var line = Help.ReadLine();
                if (LineCommand.Options.TryGetValue(line[0], out Action<string[]> functionToExecute)) {
                    functionToExecute?.Invoke(line.Skip(1).ToArray());
                }
            }
        }

        private static void PrepareConsole() {
            Console.ForegroundColor = ConsoleColor.White;

        }
    }
}