using Antd2.Configuration;
using System;
using System.Linq;

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

        private const string ConfFile = "/cfg/antd/antd.toml";

        private static void Main(string[] args) {
            PrepareConsole();

            var file = GetFileArgument(args);
            ConfigManager.Config.TomlPath = string.IsNullOrEmpty(file) ? ConfFile : file;
            ConfigManager.Config.Load();

            args = RemovedUsedArgument(args);

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

        private static string GetFileArgument(string[] args) {
            var index = Array.IndexOf(args, "-f");
            return index > -1 ? args[index + 1] : string.Empty;
        }

        private static string[] RemovedUsedArgument(string[] args) {
            var index = Array.IndexOf(args, "-f");
            if (index < 0) {
                return args;
            }
            return args.Where((_, i) => i != index && i != index + 1).ToArray();
        }
    }
}