using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Antd.Dhcpd {
    public class DhcpdLeases {
        private const string FilePath = "/var/lib/dhcp/dhcpd.leases";

        public static IEnumerable<Tuple<string, IEnumerable<string>>> GetAll() {
            if(!File.Exists(FilePath)) {
                return new List<Tuple<string, IEnumerable<string>>>();
            }
            var text = File.ReadAllText(FilePath);
            var list = new List<Tuple<string, IEnumerable<string>>>();
            const string pattern = "lease (?<LeaseName>[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}) {(?<LeaseParam>[\\s\\w\\d/:;\"\\*\\-~=.+^<>\\(\\)'`!?,&]+)}";

            var matches = Regex.Matches(text, pattern);
            foreach(Match match in matches) {
                var name = match.Groups[1].Value;
                var options = match.Groups[2].Value.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                var tuple = new Tuple<string, IEnumerable<string>>(name, options);
                list.Add(tuple);
            }
            return list;
        }
    }
}
