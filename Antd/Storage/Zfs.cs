using antdlib.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using anthilla.core;

namespace Antd.Storage {
    public class Zfs {

        public  List<ZfsModel> List() {
            var result = Bash.Execute("zfs list");
            var list = new List<ZfsModel>();
            if (string.IsNullOrEmpty(result)) {
                return list;
            }

            var pools = Zpool.List();

            var lines = result.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList().Skip(1);
            foreach (var line in lines) {
                var cells = Regex.Split(line, @"\s+");
                var model = new ZfsModel {
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
