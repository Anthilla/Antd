using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Antd.Dhcpd {
    public class DhcpdLeases {
        private const string FilePath = "/var/lib/dhcp/dhcpd.leases";

        public static IEnumerable<Tuple<string, string>> GetAll() {
            if(!File.Exists(FilePath)) {
                return new List<Tuple<string, string>>();
            }
            var text = File.ReadAllText(FilePath);
            var list = new List<Tuple<string, string>>();
            const string pattern = "lease ([0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}) {([\\s\\w\\d/:;\"\\*\\-~=.+^<>\\(\\)'`!?,&]+)}";

            var matches = Regex.Matches(text, pattern);
            foreach(Match match in matches) {
                var name = match.Groups[1].Value;
                var options = match.Groups[2].Value;
                var tuple = new Tuple<string, string>(name, options);
                list.Add(tuple);
            }
            return list;
        }
    }
}
