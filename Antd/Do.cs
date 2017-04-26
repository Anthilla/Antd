using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib.common;
using antdlib.config;
using antdlib.models;

namespace Antd {
    public class Do {
        private const string LocalTemplateDirectory = "Templates";
        private readonly Dictionary<string, string> _replacements;

        private readonly Host2Configuration _host2Configuration = new Host2Configuration();
        private readonly Network2Configuration _network2Configuration = new Network2Configuration();

        private readonly Host2Model Host;
        private readonly DnsConfiguration Dns;
        private readonly bool IsDnsPublic;
        private readonly bool IsDnsDynamic;
        private readonly bool IsDnsExternal;

        public Do() {
            Host = _host2Configuration.Host;
            Dns = _network2Configuration.DnsConfigurationList.FirstOrDefault(_ => _.Id == _network2Configuration.Conf.ActiveDnsConfiguration);

            IsDnsPublic = Dns?.Type == DnsType.Public;
            IsDnsDynamic = Dns?.Mode == DnsMode.Dynamic;
            IsDnsExternal = Dns?.Dest == DnsDestination.External;

            _replacements = new Dictionary<string, string> {
                { "$internalIp", Host.InternalHostIpPrimary },
                { "$externalIp", Host.ExternalHostIpPrimary },
                { "$internalNet", Host.InternalNetPrimary },
                { "$externalNet", Host.ExternalNetPrimary },
                { "$internalMask", Host.InternalNetMaskPrimary },
                { "$externalMask", Host.ExternalNetMaskPrimary },
                { "$internalDomain", Host.InternalDomainPrimary },
                { "$externalDomain", Host.ExternalDomainPrimary },
                { "$internalBroadcast", Host.InternalBroadcastPrimary },
                { "$externalBroadcast", Host.ExternalBroadcastPrimary },
                { "$internalNetArpa", Host.InternalArpaPrimary },
                { "$externalNetArpa", Host.ExternalArpaPrimary },
                { "$resolvNameserver", Host.ResolvNameserver },
                { "$resolvDomain", Host.ResolvDomain },

                { "$dnsDomain", Dns?.Domain },
                { "$dnsIp", Dns?.Ip },

                { "$secret", Host.Secret }
            };
        }

        public void oo() {
            SaveNtpFile();
            SaveResolvFile();
            SaveNsswitchFile();
            SaveNetworksFile();
            SaveHostsFile();
            SaveDhcpdFile();
            SaveNamedFile();
        }

        private string EditLine(string input) {
            var output = input;
            foreach(var r in _replacements) {
                output = output.Replace(r.Key, r.Value);
            }
            return output;
        }

        #region [    ntp.conf    ]
        private readonly string _ntpFileInput = $"{LocalTemplateDirectory}/ntp.conf.tmlp.tmlp";
        private const string NtpFileOutput = "/etc/ntp.conf";

        private void SaveNtpFile() {
            try {
                using(var reader = new StreamReader(_ntpFileInput)) {
                    using(TextWriter writer = File.CreateText(NtpFileOutput)) {
                        string line;
                        while((line = reader.ReadLine()) != null) {
                            writer.WriteLine(EditLine(line));
                        }
                    }
                }
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region [    resolv.conf    ]
        private const string ResolvFileOutput = "/etc/resolv.conf";

        private void SaveResolvFile() {
            string[] lines;
            if(IsDnsPublic) {
                lines = new[] {
                    "nameserver 8.8.8.8",
                    "nameserver 8.8.4.4"
                };
            }
            else {
                if(IsDnsExternal) {
                    lines = new[] {
                        $"nameserver {Host.ExternalHostIpPrimary}",
                        $"nameserver {Host.ResolvNameserver}",
                        $"search {Host.ResolvDomain}",
                        $"domain {Host.ResolvDomain}"
                    };
                }
                else {
                    lines = new[] {
                        $"nameserver {Host.ResolvNameserver}",
                        $"search {Host.ResolvDomain}",
                        $"domain {Host.ResolvDomain}"
                    };
                }
            }
            try {
                using(TextWriter writer = File.CreateText(ResolvFileOutput)) {
                    foreach(var line in lines) {
                        writer.WriteLine(line);
                    }
                }
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region [    nsswitch.conf    ]
        private readonly string _nsswitchFileInput = $"{LocalTemplateDirectory}/nsswitch.conf.tmlp";
        private const string NsswitchFileOutput = "/etc/nsswitch.conf";

        private void SaveNsswitchFile() {
            try {
                using(var reader = new StreamReader(_nsswitchFileInput)) {
                    using(TextWriter writer = File.CreateText(NsswitchFileOutput)) {
                        string line;
                        while((line = reader.ReadLine()) != null) {
                            writer.WriteLine(EditLine(line));
                        }
                    }
                }
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region [    networks    ]
        private readonly string _networksFileInput = $"{LocalTemplateDirectory}/networks.tmlp";
        private const string NetworksFileOutput = "/etc/networks";

        private void SaveNetworksFile() {
            try {
                using(var reader = new StreamReader(_networksFileInput)) {
                    using(TextWriter writer = File.CreateText(NetworksFileOutput)) {
                        string line;
                        while((line = reader.ReadLine()) != null) {
                            writer.WriteLine(EditLine(line));
                        }
                    }
                }
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region [    hosts    ]
        private readonly string _hostsFileInput = $"{LocalTemplateDirectory}/hosts.tmlp";
        private const string HostsFileOutput = "/etc/hosts";

        private void SaveHostsFile() {
            try {
                using(var reader = new StreamReader(_hostsFileInput)) {
                    using(TextWriter writer = File.CreateText(HostsFileOutput)) {
                        string line;
                        while((line = reader.ReadLine()) != null) {
                            writer.WriteLine(EditLine(line));
                        }
                    }
                }
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region [    dhcpd.conf    ]
        private readonly string _dhcpdDynamicExternalFileInput = $"{LocalTemplateDirectory}/dhcpd.dynamic.external.conf.tmlp";
        private readonly string _dhcpdDynamicInternalFileInput = $"{LocalTemplateDirectory}/dhcpd.dynamic.internal.conf.tmlp";
        private readonly string _dhcpdStaticExternalFileInput = $"{LocalTemplateDirectory}/dhcpd.static.external.conf.tmlp";
        private readonly string _dhcpdStaticInternalFileInput = $"{LocalTemplateDirectory}/dhcpd.static.internal.conf.tmlp";
        private const string DhcpdFileOutput = "/etc/dhcp/dhcpd.conf";

        private void SaveDhcpdFile() {
            try {
                var dhcpdFileInput = IsDnsDynamic ?
                    (IsDnsExternal ? _dhcpdDynamicExternalFileInput : _dhcpdDynamicInternalFileInput) :
                    (IsDnsExternal ? _dhcpdStaticExternalFileInput : _dhcpdStaticInternalFileInput);
                using(var reader = new StreamReader(dhcpdFileInput)) {
                    using(TextWriter writer = File.CreateText(DhcpdFileOutput)) {
                        string line;
                        while((line = reader.ReadLine()) != null) {
                            writer.WriteLine(EditLine(line));
                        }
                    }
                }
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region [    named.conf    ]
        private readonly string _namedFileInput = $"{LocalTemplateDirectory}/named.conf.tmlp";
        private const string NamedFileOutput = "/etc/bind/named.conf";

        private void SaveNamedFile() {
            try {
                using(var reader = new StreamReader(_namedFileInput)) {
                    using(TextWriter writer = File.CreateText(NamedFileOutput)) {
                        string line;
                        while((line = reader.ReadLine()) != null) {
                            writer.WriteLine(EditLine(line));
                        }
                    }
                }
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

    }
}
