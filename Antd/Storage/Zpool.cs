using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using antdlib.common;
using Antd.Database;
using Antd.SystemdTimer;

namespace Antd.Storage {
    public class Zpool {
        public class Model {
            public string Guid { get; set; }
            public string Name { get; set; }
            public string Size { get; set; }
            public string Alloc { get; set; }
            public string Free { get; set; }
            public string Expandsz { get; set; }
            public string Frag { get; set; }
            public string Cap { get; set; }
            public string Dedup { get; set; }
            public string Health { get; set; }
            public string Altroot { get; set; }
            public string Status { get; set; }

            public bool HasSnapshot { get; set; }
            public string SnapshotGuid { get; set; } = "";
            public string Snapshot { get; set; } = "";
        }

        private static readonly TimerRepository TimerRepository = new TimerRepository();

        public static List<Model> List() {
            var result = Bash.Execute("zpool list");
            var list = new List<Model>();
            if (string.IsNullOrEmpty(result)) {
                return list;
            }
            var lines = result.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList().Skip(1);
            foreach (var line in lines) {
                var cells = Regex.Split(line, @"\s+");
                var model = new Model {
                    Guid = Guid.NewGuid().ToString(),
                    Name = cells[0],
                    Size = cells[1],
                    Alloc = cells[2],
                    Free = cells[3],
                    Expandsz = cells[4],
                    Frag = cells[5],
                    Cap = cells[6],
                    Dedup = cells[7],
                    Health = cells[8],
                    Altroot = cells[9],
                    Status = Bash.Execute($"zpool status {cells[0]}")
                };

                var jobs = TimerRepository.GetAll().Where(_ => _.Alias == model.Name).ToList();
                if (jobs.Any()) {
                    var j = jobs.FirstOrDefault();
                    if (j != null) {
                        model.HasSnapshot = Timers.IsActive(model.Name);
                        model.Snapshot = j.Time;
                        model.SnapshotGuid = j.Id;
                    }
                }

                list.Add(model);
            }
            return list;
        }

        public static IEnumerable<string> ImportList() {
            var text = Bash.Execute("zpool import | grep 'pool:' | awk {'print $2'}");
            var importPools = text.Split(new [] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            return importPools;
        } 

        public static void Import(string poolName) {
            Bash.Execute($"zpool import -f -o altroot=/Data/{poolName} {poolName}");
        }
    }
}
