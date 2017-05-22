using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using anthilla.core;

namespace Antd.Asset {
    public class AvahiBrowse {
        public List<string> Locals { get; } = new List<string>();

        public void DiscoverService(string serviceName) {
            var result = Bash.Execute("avahi-browse -d local _http._tcp --resolve -tp");
            var list = result.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Where(_ => _.ToLower().Contains(serviceName));
            foreach(var el in list) {
                var regex = new Regex(@"[0-9a-zA-Z\.]*\;[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\;[0-9]{1,5}");
                var match = regex.Match(el);
                if(match.Success) {
                    var address = match.Value.Replace(";", ":");
                    Locals.Add(address);
                }
            }
        }
    }
}
