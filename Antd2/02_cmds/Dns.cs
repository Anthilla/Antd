using anthilla.core;
using System.IO;
using System;
using System.Linq;
using Antd2.Configuration;

namespace Antd2.cmds {
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

        public static DnsClientConfiguration GetResolv() {
            if (!File.Exists(etcResolv)) {
                return new DnsClientConfiguration();
            }
            var result = File.ReadAllLines(etcResolv).Where(_ => !string.IsNullOrEmpty(_)).ToArray();
            var nameservers = result.Where(_ => _.Contains(nameserver))
                .Select(_ => _.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault())
                .ToArray();
            var resolv = new DnsClientConfiguration() {
                Nameserver = nameservers
            };
            if (result.Any(_ => _.Contains(search))) {
                var resultSearch = result.FirstOrDefault(_ => _.Contains(search));
                resolv.Search = resultSearch.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            }
            if (result.Any(_ => _.Contains(domain))) {
                var resultDomain = result.FirstOrDefault(_ => _.Contains(domain));
                resolv.Domain = resultDomain.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            }
            return resolv;
        }

        public static void SetResolv(DnsClientConfiguration conf) {
            if (File.Exists(etcResolv)) {
                File.Move(etcResolv, etcResolvBackup, true);
            }
            var nameserverLines = new string[conf.Nameserver.Length];
            for (var i = 0; i < conf.Nameserver.Length; i++) {
                nameserverLines[i] = CommonString.Append(nameserver, " ", conf.Nameserver[i]);
            }
            File.WriteAllLines(etcResolv, nameserverLines);
            if (!string.IsNullOrEmpty(conf.Search)) {
                var newLines = new string[] { CommonString.Append(search, " ", conf.Search) };
                File.AppendAllLines(etcResolv, newLines);
            }
            if (!string.IsNullOrEmpty(conf.Domain)) {
                var newLines = new string[] { CommonString.Append(domain, " ", conf.Domain) };
                File.AppendAllLines(etcResolv, newLines);
            }
        }

        //public static void Set() {
        //    var currentResolv = Application.CurrentConfiguration.Network.KnownDns;
        //    var runningResolv = Application.RunningConfiguration.Network.KnownDns;
        //    if (CommonString.AreEquals(currentResolv.ToString(), runningResolv.ToString()) == false) {
        //        if (File.Exists(etcResolv)) {
        //            File.Copy(etcResolv, etcResolvBackup, true);
        //        }
        //        var nameserverLines = new string[currentResolv.Nameserver.Length];
        //        for (var i = 0; i < currentResolv.Nameserver.Length; i++) {
        //            nameserverLines[i] = CommonString.Append(nameserver, " ", currentResolv.Nameserver[i]);
        //        }
        //        File.WriteAllLines(etcResolv, nameserverLines);
        //        if (!string.IsNullOrEmpty(currentResolv.Search)) {
        //            var newLines = new string[] { CommonString.Append(search, " ", currentResolv.Search) };
        //            File.AppendAllLines(etcResolv, newLines);
        //        }
        //        if (!string.IsNullOrEmpty(currentResolv.Domain)) {
        //            var newLines = new string[] { CommonString.Append(domain, " ", currentResolv.Domain) };
        //            File.AppendAllLines(etcResolv, newLines);
        //        }
        //    }

        //    var currentHosts = CommonArray.Merge(DefaultHosts, Application.CurrentConfiguration.Network.KnownHosts);
        //    var runningHosts = Application.RunningConfiguration.Network.KnownHosts;
        //    if (currentHosts.Select(_ => _.ToString()).SequenceEqual(runningHosts.Select(_ => _.ToString())) == false) {
        //        if (File.Exists(etcHosts)) {
        //            File.Copy(etcHosts, etcHostsBackup, true);
        //        }
        //        var lines = new string[currentHosts.Length];
        //        for (var i = 0; i < currentHosts.Length; i++) {
        //            lines[i] = CommonString.Append(currentHosts[i].IpAddr, " ", CommonString.Build(currentHosts[i].CommonNames, ' '));
        //        }
        //        File.WriteAllLines(etcHosts, lines);
        //    }

        //    var currentNetworks = CommonArray.Merge(DefaultNetworks, Application.CurrentConfiguration.Network.KnownNetworks);
        //    var runningNetworks = Application.RunningConfiguration.Network.KnownNetworks;
        //    if (currentNetworks.Select(_ => _.ToString()).SequenceEqual(runningNetworks.Select(_ => _.ToString())) == false) {
        //        if (File.Exists(etcNetworks)) {
        //            File.Copy(etcNetworks, etcNetworksBackup, true);
        //        }
        //        var lines = new string[currentNetworks.Length];
        //        for (var i = 0; i < currentNetworks.Length; i++) {
        //            lines[i] = CommonString.Append(currentNetworks[i].Label, " ", currentNetworks[i].NetAddr);
        //        }
        //        File.WriteAllLines(etcNetworks, lines);
        //    }
        //}
    }
}
