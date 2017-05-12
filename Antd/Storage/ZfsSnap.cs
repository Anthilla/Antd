using antdlib.common;
using antdlib.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Antd.Storage {
    public class ZfsSnap {
        public  List<ZfsSnapModel> List() {
            var result = Bash.Execute("zfs list -t snap");
            var list = new List<ZfsSnapModel>();
            if(string.IsNullOrEmpty(result)) {
                return list;
            }
            var lines = result.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList().Skip(1);
            foreach(var line in lines) {
                var cells = Regex.Split(line, @"\s+");
                var model = new ZfsSnapModel {
                    Guid = Guid.NewGuid().ToString(),
                    Name = cells[0],
                    Used = cells[1],
                    Available = cells[2],
                    Refer = cells[3],
                    Mountpoint = cells[4]
                };
                list.Add(model);
            }
            return list;
        }
    }
}
