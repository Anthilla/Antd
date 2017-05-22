using antdlib.models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using anthilla.core;

namespace antdlib.config {
    public class DhcpdLeases {
        private const string FilePath = "/var/lib/dhcp/dhcpd.leases";

        public IEnumerable<Tuple<string, string>> GetAll() {
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

        public IEnumerable<DhcpdLeaseModel> List() {
            if(!File.Exists(FilePath)) {
                return new List<DhcpdLeaseModel>();
            }
            var text = File.ReadAllText(FilePath);
            var list = new List<DhcpdLeaseModel>();
            const string pattern = "lease ([0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}) {([\\s\\w\\d/:;\"\\*\\-~=.+^<>\\(\\)'`!?,&]+)}";

            var matches = Regex.Matches(text, pattern);
            foreach(Match match in matches) {
                var name = match.Groups[1].Value;
                var options = match.Groups[2].Value;

                if(!string.IsNullOrEmpty(options)) {
                    var arr = options.Split(';');
                    ConsoleLogger.Log(arr.Length);
                    if(arr.Length > 1) {
                        var startString = arr.First(_ => _.Contains("starts"));
                        ConsoleLogger.Log(startString);
                        const string datePattern = "([0-9]{1,4}/[0-9]{1,2}/[0-9]{1,2} [0-9]{1,2}:[0-9]{1,2}:[0-9]{1,2})";
                        var s = Regex.Match(startString, datePattern).Value;
                        var startDate = DateTime.ParseExact(s, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
                        var endString = arr.First(_ => _.Contains("ends"));
                        ConsoleLogger.Log(endString);
                        var e = Regex.Match(endString, datePattern).Value;
                        var endDate = DateTime.ParseExact(e, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);

                        var hardware = arr.First(_ => _.Contains("hardware")).Replace("hardware", "").Trim();
                        var host = arr.First(_ => _.Contains("client-hostname")).Replace("client-hostname", "").Trim();
                        ConsoleLogger.Log(hardware);
                        ConsoleLogger.Log(host);

                        var model = new DhcpdLeaseModel {
                            Reference = match.Value,
                            Name = name,
                            StartTime = startDate,
                            EndTime = endDate,
                            Hardware = hardware,
                            ClientHostname = host
                        };
                        list.Add(model);
                    }
                }
            }
            return list;
        }
    }
}
