using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Antd.Discovery {
    public class NetscanSettingModel {
        [JsonIgnore]
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public NetscanSettingModel() {
            var list = Alphabet.Select((t, i) => new Tuple<string, string, string>((i + 1).ToString(), t.ToString(), "")).ToList();
            Values = list;
        }
        public string Subnet { get; set; } = "10.1.";
        public string SubnetLabel { get; set; } = "primary";
        public List<Tuple<string, string, string>> Values { get; set; }
    }
}
