using Antd2.models;
using anthilla.core;
using System;
using System.Linq;

namespace Antd2.cmds {
    public class Zpool {

        private const string zpoolCommand = "zpool";

        private const string zpoolEmptyMessage = "no pools available";
        private const string zpoolHistory = "history";
        private const string zpoolImportArgs = "import";
        private const string zpoolImportPoolArgs = "import -f -o altroot=/Data/";
        private const string zpoolImportFilter = "pool:";

        public static ZpoolModel[] GetPools() {
            var result = Bash.Execute($"{zpoolCommand} list").ToArray();
            if (result.Length < 1) {
                return new ZpoolModel[0];
            }
            if (result[0].Contains(zpoolEmptyMessage)) {
                return new ZpoolModel[0];
            }
            var pools = new ZpoolModel[result.Length];
            for (var i = 0; i < pools.Length; i++) {
                var currentData = result[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                pools[i] = new ZpoolModel {
                    Name = currentData[0],
                    Size = currentData[1],
                    Alloc = currentData[2],
                    Free = currentData[3],
                    Expandsz = currentData[4],
                    Frag = currentData[5],
                    Cap = currentData[6],
                    Dedup = currentData[7],
                    Health = currentData[8],
                    Altroot = currentData[9],
                    Status = Bash.Execute($"zpool status {currentData[0]}").FirstOrDefault()
                };
            }
            return pools;
        }

        public static string[] GetImportPools() {
            var result = Bash.Execute($"{zpoolCommand} {zpoolImportArgs}").Where(_ => _.Contains(zpoolImportFilter)).ToArray();
            var pools = new string[result.Length];
            for (var i = 0; i < result.Length; i++) {
                var currentData = result[i].Split(new[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
                pools[i] = currentData[1].Trim();
            }
            return pools;
        }

        public static bool Import(string poolName) {
            Bash.Do($"{zpoolCommand} {CommonString.Append(zpoolImportPoolArgs, poolName, " ", poolName).Replace("//", "/")}");
            return true;
        }

        public static string[] History() {
            return Bash.Execute($"{zpoolCommand} {zpoolHistory}").ToArray();
        }
    }
}
