using anthilla.core;
using System.IO;
using System;
using System.Linq;

namespace Antd.cmds {
    public class Dns {

        private const string etcResolv = "/etc/resolv.conf";
        private const string etcResolvBackup = "/etc/resolv.conf.bck";
        private const string etcHosts = "/etc/hosts";
        private const string etcHostsBackup = "/etc/hosts.bck";
        private const string etcNetworks = "/etc/networks";
        private const string etcNetworksBackup = "/etc/networks.bck";

        private const string nameserver = "nameserver";
        private const string search = "search";
        private const string domain = "domain";
        private const string comment = "#";

        public static KnownHost[] DefaultHosts = new KnownHost[] {
            new KnownHost() { IpAddr = "127.0.0.1", CommonNames = new[] { "localhost" } },
        };

        public static KnownNetwork[] DefaultNetworks = new KnownNetwork[] {
            new KnownNetwork() { Label = "loopback", NetAddr = "127.0.0.1" },
            new KnownNetwork() { Label = "link-local", NetAddr = "169.254.0.0" }
        };

        public static DnsClientConfiguration GetResolv() {
            if(!File.Exists(etcResolv)) {
                return new DnsClientConfiguration();
            }
            var result = File.ReadAllLines(etcResolv).Where(_ => !string.IsNullOrEmpty(_)).ToArray();
            var nameservers = result.Where(_ => _.Contains(nameserver))
                .Select(_ => _.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault())
                .ToArray();
            var resolv = new DnsClientConfiguration() {
                Nameserver = nameservers
            };
            if(result.Any(_ => _.Contains(search))) {
                var resultSearch = result.FirstOrDefault(_ => _.Contains(search));
                resolv.Search = resultSearch.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            }
            if(result.Any(_ => _.Contains(domain))) {
                var resultDomain = result.FirstOrDefault(_ => _.Contains(domain));
                resolv.Domain = resultDomain.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            }
            return resolv;
        }

        public static KnownHost[] GetHosts() {
            if(!File.Exists(etcHosts)) {
                return new KnownHost[0];
            }
            var result = File.ReadAllLines(etcHosts).Where(_ => !string.IsNullOrEmpty(_) && !_.Contains(comment)).ToArray();
            var hosts = new KnownHost[result.Length];
            for(var i = 0; i < result.Length; i++) {
                var currentLineData = result[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                hosts[i] = new KnownHost() {
                    IpAddr = currentLineData[0],
                    CommonNames = currentLineData.Skip(1).ToArray()
                };
            }
            return hosts;
        }

        public static KnownNetwork[] GetNetworks() {
            if(!File.Exists(etcNetworks)) {
                return new KnownNetwork[0];
            }
            var result = File.ReadAllLines(etcNetworks).Where(_ => !string.IsNullOrEmpty(_) && !_.Contains(comment)).ToArray();
            var networks = new KnownNetwork[result.Length];
            for(var i = 0; i < result.Length; i++) {
                var currentLineData = result[i].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                networks[i] = new KnownNetwork() {
                    Label = currentLineData[0],
                    NetAddr = currentLineData[1]
                };
            }
            return networks;
        }

        public static void Set() {
            var currentResolv = Application.CurrentConfiguration.Network.KnownDns;
            var runningResolv = Application.RunningConfiguration.Network.KnownDns;
            if(CommonString.AreEquals(currentResolv.ToString(), runningResolv.ToString()) == false) {
                if(File.Exists(etcResolv)) {
                    File.Copy(etcResolv, etcResolvBackup, true);
                }
                var nameserverLines = new string[currentResolv.Nameserver.Length];
                for(var i = 0; i < currentResolv.Nameserver.Length; i++) {
                    nameserverLines[i] = CommonString.Append(nameserver, " ", currentResolv.Nameserver[i]);
                }
                File.WriteAllLines(etcResolv, nameserverLines);
                if(!string.IsNullOrEmpty(currentResolv.Search)) {
                    var newLines = new string[] { CommonString.Append(search, " ", currentResolv.Search) };
                    File.AppendAllLines(etcResolv, newLines);
                }
                if(!string.IsNullOrEmpty(currentResolv.Domain)) {
                    var newLines = new string[] { CommonString.Append(domain, " ", currentResolv.Domain) };
                    File.AppendAllLines(etcResolv, newLines);
                }
            }

            var currentHosts = CommonArray.Merge(DefaultHosts, Application.CurrentConfiguration.Network.KnownHosts);
            var runningHosts = Application.RunningConfiguration.Network.KnownHosts;
            if(currentHosts.Select(_ => _.ToString()).SequenceEqual(runningHosts.Select(_ => _.ToString())) == false) {
                if(File.Exists(etcHosts)) {
                    File.Copy(etcHosts, etcHostsBackup, true);
                }
                var lines = new string[currentHosts.Length];
                for(var i = 0; i < currentHosts.Length; i++) {
                    lines[i] = CommonString.Append(currentHosts[i].IpAddr, " ", CommonString.Build(currentHosts[i].CommonNames, ' '));
                }
                File.WriteAllLines(etcHosts, lines);
            }

            var currentNetworks = CommonArray.Merge(DefaultNetworks, Application.CurrentConfiguration.Network.KnownNetworks);
            var runningNetworks = Application.RunningConfiguration.Network.KnownNetworks;
            if(currentNetworks.Select(_ => _.ToString()).SequenceEqual(runningNetworks.Select(_ => _.ToString())) == false) {
                if(File.Exists(etcNetworks)) {
                    File.Copy(etcNetworks, etcNetworksBackup, true);
                }
                var lines = new string[currentNetworks.Length];
                for(var i = 0; i < currentNetworks.Length; i++) {
                    lines[i] = CommonString.Append(currentNetworks[i].Label, " ", currentNetworks[i].NetAddr);
                }
                File.WriteAllLines(etcNetworks, lines);
            }
        }
    }
}
