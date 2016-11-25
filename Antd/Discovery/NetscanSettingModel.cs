using System.Collections.Generic;
using Newtonsoft.Json;

namespace Antd.Discovery {
    public class NetscanSettingModel {
        [JsonIgnore]
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public NetscanSettingModel() {
            var dict = new Dictionary<NetscanLabel, string>();
            for(var i = 0; i < Alphabet.Length; i++) {
                var mo = new NetscanLabel { Letter = Alphabet[i].ToString(), Number = (i + 1).ToString() };
                dict[mo] = "";
            }
            Values = dict;
        }
        public string Subnet { get; set; } = "10.1.";
        public Dictionary<NetscanLabel, string> Values { get; set; }
    }

    public class NetscanLabel {
        public string Letter { get; set; }
        public string Number { get; set; }
    }
}
