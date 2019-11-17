using System;

namespace Antd2.cmds {
    public class Rsync {

        private const string rsyncCommand = "rsync";
        private const string option_aHA = "-aHA ";

        public static void Sync(string source, string destination) {
            var cmd = $"{rsyncCommand} {source} {destination}";
            Console.WriteLine(cmd);
            var result = Bash.Execute(cmd);
            foreach (var l in result)
                Console.WriteLine(l);
        }
        public static void SyncArchive(string source, string destination) {
            var cmd = $"{rsyncCommand} {option_aHA} {source} {destination}";
            Console.WriteLine(cmd);
            var result = Bash.Execute(cmd);
            foreach (var l in result)
                Console.WriteLine(l);
        }

        public static void SyncCustom(string options, string source, string destination, bool print = false) {
            var cmd = $"{rsyncCommand} {options} {source} {destination}";
            Console.WriteLine(cmd);
            if (print) {
                var result = Bash.Execute(cmd);
                foreach (var l in result)
                    Console.WriteLine(l);
            }
            else {
                Bash.Do(cmd);
            }
        }
    }
}
