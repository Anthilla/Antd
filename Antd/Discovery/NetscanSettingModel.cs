using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Antd.Discovery {
    public class NetscanSettingModel {
        [JsonIgnore]
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public NetscanSettingModel() {
            var lisst = new List<Tuple<string, string, string>>();
            for(var i = 0; i < Alphabet.Length; i++) {
                var mo = new Tuple<string, string, string>((i + 1).ToString(), Alphabet[i].ToString(), "");
                lisst.Add(mo);
            }
            Values = lisst;
        }
        public string Subnet { get; set; } = "10.1.";
        public string SubnetLabel { get; set; } = "primary";
        public List<Tuple<string, string, string>> Values { get; set; }
    }
}
