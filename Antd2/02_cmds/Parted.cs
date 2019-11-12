using System;
using System.Collections.Generic;
using System.Linq;

namespace Antd2.cmds {

    /// <summary>
    /// https://linux.die.net/man/8/parted
    /// </summary>
    public class Parted {

        private const string partedCommand = "parted";
        private const char eCR = '\n';      //corrisponde a 'Invio' - conferma il comando/opzione

        public static IEnumerable<string> GetPartitionTable() {
            var lines = Bash.Execute($"echo -e print | {partedCommand}");
            return lines;
        }

        public static IEnumerable<string> GetPartitionTable(string device) {
            var lines = Bash.Execute($"echo -e \\\"select {device}{eCR}print\\\" | {partedCommand}");
            return lines;
        }
    }
}
