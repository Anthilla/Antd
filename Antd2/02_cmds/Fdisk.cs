using System;
using System.Collections.Generic;
using System.Linq;

namespace Antd2.cmds {

    /// <summary>
    /// https://linux.die.net/man/8/fdisk
    /// </summary>
    public class Fdisk {

        private const string fdiskCommand = "fdisk";
        private const char eCR = '\n';      //corrisponde a 'Invio' - conferma il comando/opzione
        private const char eN = 'n';        //add a new partition
        private const char eP = 'p';        //print partition table O   primary type se dopo 'n'
        private const char eE = 'e';        //                          exended type se dopo 'n'
        private const char eW = 'w';        //write table to disk and exit
        private const char eD = 'd';        //delete a partition

        public static IEnumerable<string> GetPartitionTable(string device) {
            var lines = Bash.Execute($"{fdiskCommand} -l {device}");
            return lines;
        }

        /// <summary>
        /// "n \n p \n 1 \n \n +300MB \n w \n"
        /// </summary>
        /// <param name="device"></param>
        public static void CreatePrimaryPartition(string device, string partitionSize) {
            var cmd = $"echo -e \\\"{eN}{eCR}{eP}{eCR}{eCR}{eCR}{partitionSize}{eCR}{eW}{eCR}\\\" | {fdiskCommand} {device}";
            Console.WriteLine("  " + cmd);
            var lines = Bash.Execute(cmd);
            foreach(var line in lines) {
                Console.WriteLine("    " + line);
            }
        }

        public static void DeletePartition(string device, string number) {
            var cmd = $"echo -e \\\"{eD}{eCR}{number}{eCR}{eW}{eCR}\\\" | {fdiskCommand} {device}";
            Bash.Do(cmd);
        }

        private static (string Name, string MajMin, string Rm, string Size, string Ro, string Type, string Mountpoint) ParseLsblkLine(string line) {
            var arr = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            var mp = arr.Length < 7 ? string.Empty : arr[6];
            return (arr[0], arr[1], arr[2], arr[3], arr[4], arr[5], mp);
        }
    }
}
