using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using antdlib.common;
using antdlib.config;
using antdlib.models;
using Antd.Database;
using Antd.SystemdTimer;

namespace Antd.Storage {
    public class Zpool {
        private static readonly Bash Bash = new Bash();
        private readonly Timers _timers = new Timers();

        public List<ZpoolModel> List() {
            var result = Bash.Execute("zpool list");
            var list = new List<ZpoolModel>();
            if(string.IsNullOrEmpty(result)) {
                return list;
            }
            var lines = result.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList().Skip(1);
            foreach(var line in lines) {
                var cells = Regex.Split(line, @"\s+");
                var model = new ZpoolModel {
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

                var schedulerConfiguration = new TimerConfiguration();

                var jobs = schedulerConfiguration.Get().Timers.Where(_ => _.Alias == model.Name).ToList();
                if(jobs.Any()) {
                    var j = jobs.FirstOrDefault();
                    if(j != null) {
                        model.HasSnapshot = _timers.IsActive(model.Name);
                        model.Snapshot = j.Time;
                        model.SnapshotGuid = j.Guid;
                    }
                }

                list.Add(model);
            }
            return list;
        }

        public IEnumerable<string> ImportList() {
            var text = Bash.Execute("zpool import").SplitBash().Grep("'pool:'").Print(2, " ");
            return text;
        }

        public void Import(string poolName) {
            Bash.Execute($"zpool import -f -o altroot=/Data/{poolName} {poolName}", false);
        }

        public List<string> History() {
            var obj = Bash.Execute("zpool history");
            var list = obj.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            return list.ToList();
        }
    }
}
