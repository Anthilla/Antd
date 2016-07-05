using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using antdlib.common;

namespace Antd.Storage {
    public class Zpool {
        public class Model {
            public string Name { get; set; }
            public string Stats { get; set; }
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
                    Stats = cells[1]
                };
                list.Add(model);
            }
            return list;
        }
    }
}
