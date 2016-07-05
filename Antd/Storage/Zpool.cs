using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using antdlib.common;

namespace Antd.Storage {
    public class Zpool {
        public class Model {
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
        }

        public static List<Model> List() {
            var result = Terminal.Execute("zpool status");
            var list = new List<Model>();
            if (string.IsNullOrEmpty(result)) {
                return list;
            }
            var lines = result.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList().Skip(1);
            foreach (var line in lines) {
                var cells = Regex.Split(line, @"\s+");
                var model = new Model {
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
                    Status = Terminal.Execute($"zpool status {cells[0]}")
                };
                list.Add(model);
            }
            return list;
        }
    }
}
