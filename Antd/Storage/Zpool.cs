using Antd.SystemdTimer;
using antdlib.config;
using antdlib.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using anthilla.core;

namespace Antd.Storage {
    public  static class Zpool {

        public  static List<ZpoolModel> List() {
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
                        model.HasSnapshot = Timers.IsActive(model.Name);
                        model.Snapshot = j.Time;
                        model.SnapshotGuid = j.Guid;
                    }
                }

                list.Add(model);
            }
            return list;
        }

        public  static IEnumerable<string> ImportList() {
            var text = Bash.Execute("zpool import").SplitBash().Grep("'pool:'").Print(2, " ");
            return text;
        }

        public  static void Import(string poolName) {
            Bash.Execute($"zpool import -f -o altroot=/Data/{poolName} {poolName}", false);
        }

        public  static List<string> History() {
            var obj = Bash.Execute("zpool history");
            var list = obj.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            return list.ToList();
        }
    }
}
