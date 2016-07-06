using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using antdlib.common;

namespace Antd.Storage {
    public class Zfs {
        public class Model {
            public string Guid { get; set; }
            public string Name { get; set; }
            public string Used { get; set; }
            public string Available { get; set; }
            public string Refer { get; set; }
            public string Mountpoint { get; set; }

            public string Pool { get; set; }
            public bool HasSnapshot { get; set; }
            public string SnapshotGuid { get; set; } = "";
            public string Snapshot { get; set; } = "";
        }

        public static List<Model> List() {
            var result = new Terminal().Execute("zfs list");
            var list = new List<Model>();
            if (string.IsNullOrEmpty(result)) {
                return list;
            }

            var pools = Zpool.List();

            var lines = result.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList().Skip(1);
            foreach (var line in lines) {
                var cells = Regex.Split(line, @"\s+");
                var model = new Model {
                    Guid = Guid.NewGuid().ToString(),
                    Name = cells[0],
                    Used = cells[1],
                    Available = cells[2],
                    Refer = cells[3],
                    Mountpoint = cells[4],
                    Pool = ""
                };

                var firstPartOfName = model.Name.SplitToList("/").First();
                var p = pools.Where(_ => _.Name == firstPartOfName).ToList();
                if (p.Any()) {
                    model.Pool = firstPartOfName;
                    var j = p.FirstOrDefault();
                    if (j != null) {
                        model.HasSnapshot = j.HasSnapshot;
                        model.Snapshot = j.Snapshot;
                        model.SnapshotGuid = j.SnapshotGuid;
                    }
                }

                list.Add(model);
            }
            return list;
        }
    }
}
