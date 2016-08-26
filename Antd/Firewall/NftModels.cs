using System.Collections.Generic;

namespace Antd.Firewall {
    public class NftModel {
        public class Table {
            public string Type { get; set; }
            public string Name { get; set; }

            public List<Set> Sets { get; set; }
            public List<Chain> Chains { get; set; }
        }

        public class Set {
            public string Name { get; set; }
            public string Type { get; set; }
            public string Elements { get; set; }
        }

        public class Chain {
            public string Name { get; set; }
            public string RulesString { get; set; }
            public List<string> Rules { get; set; }
        }
    }
}
