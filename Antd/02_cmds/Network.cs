using anthilla.core;
using System;
using System.IO;
using System.Linq;
using LukeSkywalker.IPNetwork;
using Antd.models;

namespace Antd.cmds {
    public class Network {

        /// <summary>
        /// i bond sono elencati in bonding_masters e contengono una cartella che si chiama bonding
        /// i bridge contengono una cartella che si chiama bridge
        /// i bridge e i bond hanno le interfacce gestite indicate come link che inizia con lower_
        /// le interfacce fisiche contengono un link chiamato device
        /// le interfacce virtuali sono le rimanenti
        /// </summary>

        private const string ipFileLocation = "/bin/ip";
        private const string netFolder = "/sys/class/net";
        private const string bondingMastersFile = "bonding_masters";
        private const string childrenFilePrefix = "lower_";
        private const string promistStatus = "PROMISC";
        private const string inet = "inet";
        private const string inet6 = "inet6";

        private static string operstateFile = CommonString.Append(netFolder, "/operstate");
        private const string bridgeFolder = "bridge";
        private const string deviceLink = "device";

        private const string defaultMtu = "6000";
        private const string defaultTxqueuelen = "10000";

        private const string tuntapMudule = "tun";
        private const string tuntapArg = "tuntap";

        public static NetInterface[] Get() {
            if(!Directory.Exists(netFolder)) {
                return new NetInterface[0];
            }
            var result = Directory.EnumerateDirectories(netFolder, "*", SearchOption.TopDirectoryOnly)
                .Select(_ => Path.GetFileName(_))
                .ToArray();
            var bondNames = File.ReadAllText(CommonString.Append(netFolder, "/", bondingMastersFile)).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var interfaces = new NetInterface[result.Length];
            for(var i = 0; i < result.Length; i++) {
                var interfaceName = result[i];
                var interfaceFolder = GetSysClassNetpath(interfaceName);

                var interfaceDataFolders = Directory.EnumerateDirectories(interfaceFolder, "*", SearchOption.TopDirectoryOnly).Select(_ => Path.GetFileName(_)).ToArray();
                var interfaceDataFiles = Directory.EnumerateFiles(interfaceFolder, "*", SearchOption.TopDirectoryOnly).Select(_ => Path.GetFileNameWithoutExtension(_)).ToArray();

                interfaces[i] = new NetInterface();

                if(File.Exists(operstateFile)) {
                    var operstate = File.ReadAllText(operstateFile);
                    interfaces[i].Active = operstate == "up" ? true : false;
                }

                interfaces[i].Id = interfaceName;

                models.NetworkAdapterType type;
                bool isInterfaceAggregator;
                string[] lowerInterfaces;
                if(bondNames.Contains(interfaceName)) {
                    type = models.NetworkAdapterType.Bond;
                    isInterfaceAggregator = true;
                    lowerInterfaces = GetLowerInterfaces(interfaceDataFiles);
                }
                else if(interfaceDataFolders.Contains(bridgeFolder)) {
                    type = models.NetworkAdapterType.Bridge;
                    isInterfaceAggregator = true;
                    lowerInterfaces = GetLowerInterfaces(interfaceDataFiles);
                }
                else if(interfaceDataFolders.Contains(deviceLink) || interfaceDataFiles.Contains(deviceLink)) {
                    type = models.NetworkAdapterType.Physical;
                    isInterfaceAggregator = false;
                    lowerInterfaces = new string[0];
                }
                else {
                    type = models.NetworkAdapterType.Virtual;
                    isInterfaceAggregator = false;
                    lowerInterfaces = new string[0];
                }
                interfaces[i].Type = type;
                interfaces[i].InterfaceAggregator = isInterfaceAggregator;
                interfaces[i].LowerInterfaces = lowerInterfaces;

                var interfaceIpAddr = CommonProcess.Execute(ipFileLocation, CommonString.Append("addr show ", interfaceName)).ToArray();

                interfaces[i].HardwareConfiguration = new NetworkAdapterInfo.HardwareConfiguration() {
                    MacAddress = Ip.GetNetworkAdapterMacAddresss(interfaceName),
                    Mtu = Ip.GetNetworkAdapterMTU(interfaceName),
                    Txqueuelen = Ip.GetNetworkAdapterTxqueuelen(interfaceName),
                    Promisc = interfaceIpAddr[0].Contains(promistStatus) ? true : false
                };

                var addrData = interfaceIpAddr.Where(_ => _.Contains(inet) && !_.Contains(inet6)).ToArray();
                if(!addrData.Any()) {
                    continue;
                }
                var primaryAddressData = GetIpAddressAndNetworkRange(addrData[0]);

                interfaces[i].PrimaryAddressConfiguration = new NetworkAdapterInfo.AddressConfiguration() {
                    IpAddr = primaryAddressData.Item1,
                    NetworkRange = primaryAddressData.Item2
                };

                IPNetwork ipnetwork = IPNetwork.Parse(GetNetworkRange(addrData[0]));
                interfaces[i].NetworkClass = ipnetwork.Network.ToString();

                var secondaryAddressConfiguration = new NetworkAdapterInfo.AddressConfiguration[addrData.Length - 1];
                for(var s = 1; s < addrData.Length; s++) {
                    var currentData = GetIpAddressAndNetworkRange(addrData[s]);
                    secondaryAddressConfiguration[s - 1] = new NetworkAdapterInfo.AddressConfiguration() {
                        IpAddr = currentData.Item1,
                        NetworkRange = currentData.Item2
                    };
                }
            }
            return interfaces;
        }

        /// <summary>
        /// Ottiene tutti gli ip configurati sulle varie schede
        /// NB: Non controllo al momento se ci sono ip doppi perché presumo non ce ne siano: dato che ci sarebbero ben altri problemi dovuti a un ip doppio
        /// </summary>
        /// <returns></returns>
        public static string[] GetAllLocalAddress() {
            var current = Application.RunningConfiguration.Network.NetworkInterfaces;
            var addressList = new System.Collections.Generic.List<string>();
            for(var i = 0; i < current.Length; i++) {
                var currentAdapter = current[i];
                if(!string.IsNullOrEmpty(currentAdapter.PrimaryAddressConfiguration.IpAddr)) {
                    addressList.Add(currentAdapter.PrimaryAddressConfiguration.IpAddr);
                }
                for(var a = 0; i < currentAdapter.SecondaryAddressConfigurations.Length; a++) {
                    if(!string.IsNullOrEmpty(currentAdapter.SecondaryAddressConfigurations[a].IpAddr)) {
                        addressList.Add(currentAdapter.SecondaryAddressConfigurations[a].IpAddr);
                    }
                }
            }
            return addressList.ToArray();
        }

        public static string[] GetAllNames() {
            var current = Application.RunningConfiguration.Network.NetworkInterfaces;
            var addressList = new string[current.Length];
            for(var i = 0; i < current.Length; i++) {
                addressList[i] = current[i].Id;
            }
            return addressList.ToArray();
        }

        /// <summary>
        /// Applica i settaggi di default a tutte le interfacce
        /// </summary>
        /// <returns></returns>
        public static bool Prepare() {
            var running = Application.RunningConfiguration.Network.NetworkInterfaces;
            if(running.Length > 0) {
                ConsoleLogger.Log($"[network] start prepare");
                for(var i = 0; i < running.Length; i++) {
                    SetInterfaceHardwareConfiguration(running[i]);
                    Ip.EnableNetworkAdapter(running[i].Id);
                }
                ConsoleLogger.Log($"[network] end prepare");
            }
            return true;
        }

        /// <summary>
        /// Applica i settaggi delle interfacce di rete così come sono salvati nella configurazione Current
        /// </summary>
        /// <returns></returns>
        public static bool Set() {
            var current = Application.CurrentConfiguration.Network.NetworkInterfaces;
            var running = Application.RunningConfiguration.Network.NetworkInterfaces;
            var bridgeIfs = current.Where(_ => _.Type == models.NetworkAdapterType.Bridge).ToArray();
            for(var i = 0; i < bridgeIfs.Length; i++) {
                var netInterface = bridgeIfs[i];
                var runningConf = running.FirstOrDefault(_ => _.Id == netInterface.Id) ?? new NetInterface();
                SetBridgeInterface(netInterface);
                SetInterface(netInterface, runningConf);
            }
            var bondIfs = current.Where(_ => _.Type == models.NetworkAdapterType.Bond).ToArray();
            for(var i = 0; i < bondIfs.Length; i++) {
                var netInterface = bondIfs[i];
                var runningConf = running.FirstOrDefault(_ => _.Id == netInterface.Id) ?? new NetInterface();
                SetBondInterface(netInterface);
                SetInterface(netInterface, runningConf);
            }
            var physicalIfs = current.Where(_ => _.Type == models.NetworkAdapterType.Physical).ToArray();
            for(var i = 0; i < physicalIfs.Length; i++) {
                var netInterface = physicalIfs[i];
                var runningConf = running.FirstOrDefault(_ => _.Id == netInterface.Id) ?? new NetInterface();
                SetInterface(netInterface, runningConf);
            }
            return true;
        }

        public static bool SetInterface(NetInterface netInterface, NetInterface running) {
            var name = netInterface.Id;
            if(netInterface.Active == false) {
                ConsoleLogger.Log($"[network] disabling '{name}'");
                Ip.DisableNetworkAdapter(name);
                return true;
            }
            else {
                ConsoleLogger.Log($"[network] enabling '{name}'");
                Ip.EnableNetworkAdapter(name);
            }
            if(CommonString.AreEquals(netInterface.ToString(), running.ToString()) == false) {
                if(CommonString.AreEquals(netInterface.HardwareConfiguration.ToString(), running.HardwareConfiguration.ToString()) == false) {
                    Ip.SetNetworkAdapterMTU(name, netInterface.HardwareConfiguration.Mtu.ToString());
                    ConsoleLogger.Log($"[network] set '{name}' mtu to {netInterface.HardwareConfiguration.Mtu}");
                    Ip.SetNetworkAdapterTxqueuelen(name, netInterface.HardwareConfiguration.Txqueuelen.ToString());
                    ConsoleLogger.Log($"[network] set '{name}' txqueuelen to {netInterface.HardwareConfiguration.Txqueuelen}");
                }
                if(CommonString.AreEquals(netInterface.PrimaryAddressConfiguration.ToString(), running.PrimaryAddressConfiguration.ToString()) == false) {
                    if(!string.IsNullOrEmpty(netInterface.PrimaryAddressConfiguration.IpAddr)) {
                        if(netInterface.PrimaryAddressConfiguration.StaticAddress) {
                            Ip.AddAddress(name, netInterface.PrimaryAddressConfiguration.IpAddr, netInterface.PrimaryAddressConfiguration.NetworkRange.ToString());
                            ConsoleLogger.Log($"[network] set '{name}' address to {netInterface.PrimaryAddressConfiguration.IpAddr}/{netInterface.PrimaryAddressConfiguration.NetworkRange}");
                        }
                        else {
                            Dhclient.Start(name);
                            ConsoleLogger.Log($"[network] dinamically set '{name}' address");
                        }
                    }
                }

                var currentSecondaryAddressConfiguration = netInterface.SecondaryAddressConfigurations;
                if(currentSecondaryAddressConfiguration.Length < 1) {
                    return true;
                }
                var runningSecondaryAddressConfiguration = running.SecondaryAddressConfigurations;
                // quelli unici di currentSecondaryAddressConfiguration sono da applicare
                var add = currentSecondaryAddressConfiguration.Except(runningSecondaryAddressConfiguration).ToArray();
                for(var s = 0; s < add.Length; s++) {
                    if(!string.IsNullOrEmpty(add[s].IpAddr)) {
                        Ip.AddAddress(name, add[s].IpAddr, add[s].NetworkRange.ToString());
                        ConsoleLogger.Log($"[network] set '{name}' secondary address to {add[s].IpAddr}/{add[s].NetworkRange}");
                    }
                }
                // quelli unici di runningSecondaryAddressConfiguration sono da togliere
                var del = runningSecondaryAddressConfiguration.Except(currentSecondaryAddressConfiguration).ToArray();
                for(var s = 0; s < del.Length; s++) {
                    if(!string.IsNullOrEmpty(del[s].IpAddr)) {
                        Ip.DeleteAddress(name, del[s].IpAddr, del[s].NetworkRange.ToString());
                    }
                }
            }
            return true;
        }

        public static bool SetInterfaceHardwareConfiguration(NetInterface netInterface) {
            if(netInterface.Active != true) {
                Ip.EnableNetworkAdapter(netInterface.Id);
                ConsoleLogger.Log($"[network] enabling '{netInterface.Id}'");
            }
            if(netInterface.HardwareConfiguration.Promisc != true) {
                Ip.SetNetworkAdapterPromiscOn(netInterface.Id);
                ConsoleLogger.Log($"[network] set '{netInterface.Id}' promisc to on");
            }
            if(CommonString.AreEquals(netInterface.HardwareConfiguration.Mtu.ToString(), defaultMtu) == false) {
                Ip.SetNetworkAdapterMTU(netInterface.Id, defaultMtu);
                ConsoleLogger.Log($"[network] set '{netInterface.Id}' mtu to {defaultMtu}");
            }
            if(CommonString.AreEquals(netInterface.HardwareConfiguration.Txqueuelen.ToString(), defaultTxqueuelen) == false) {
                Ip.SetNetworkAdapterTxqueuelen(netInterface.Id, defaultTxqueuelen);
                ConsoleLogger.Log($"[network] set '{netInterface.Id}' txqueuelen to {defaultTxqueuelen}");
            }
            return true;
        }

        /// <summary>
        /// Per gestire le interfacce virtuali tun/tap mi serve il modulo 'tun'
        /// Qui controllo se il modulo è caricato
        /// </summary>
        /// <returns>Devo caricare il modulo? Quindi se null -> true -> carico</returns>
        private static bool CheckTuntapModule() {
            var running = Application.RunningConfiguration.Boot.Modules;
            return running.FirstOrDefault(_ => _.Module == tuntapMudule) == null;
        }

        public static bool SetTuns() {
            var current = Application.CurrentConfiguration.Network.Tuns;
            for(var i = 0; i < current.Length; i++) {
                SetTunInterface(current[i]);
            }
            return true;
        }

        /// <summary>
        /// ip tuntap add dev tun0 mode tun
        /// </summary>
        /// <param name="netInterface"></param>
        /// <returns></returns>
        public static bool SetTunInterface(NetTun netInterface) {
            if(CheckTuntapModule()) {
                Mod.Add(tuntapMudule);
            }
            var args = CommonString.Append(tuntapArg, " add dev ", netInterface.Id, " mode tun");
            CommonProcess.Do(ipFileLocation, args);
            Ip.EnableNetworkAdapter(netInterface.Id);
            ConsoleLogger.Log($"[network] tun '{netInterface.Id}' created");
            return true;
        }

        public static bool SetTaps() {
            var current = Application.CurrentConfiguration.Network.Taps;
            for(var i = 0; i < current.Length; i++) {
                SetTapInterface(current[i]);
            }
            return true;
        }

        /// <summary>
        /// ip tuntap add dev tap0 mode tap
        /// </summary>
        /// <param name="netInterface"></param>
        /// <returns></returns>
        public static bool SetTapInterface(NetTap netInterface) {
            if(CheckTuntapModule()) {
                Mod.Add(tuntapMudule);
            }
            var args = CommonString.Append(tuntapArg, " add dev ", netInterface.Id, " mode tap");
            CommonProcess.Do(ipFileLocation, args);
            Ip.EnableNetworkAdapter(netInterface.Id);
            ConsoleLogger.Log($"[network] tap '{netInterface.Id}' created");
            return true;
        }

        /// <summary>
        /// todo
        ///     se una lower interface appartiene a un altro aggregatore toglierla dall'interfaccia trovata
        ///     se c'è una lower interface che non deve appartenerde all'agregatore corrente toglierlo
        /// </summary>
        /// <param name="netInterface"></param>
        /// <returns></returns>
        public static bool SetBridgeInterface(NetInterface netInterface) {
            Brctl.Create(netInterface.Id);
            Ip.EnableNetworkAdapter(netInterface.Id);
            Brctl.SetStpOff(netInterface.Id);
            for(var s = 0; s < netInterface.LowerInterfaces.Length; s++) {
                Brctl.AddNetworkAdapter(netInterface.Id, netInterface.LowerInterfaces[s]);
                Ip.EnableNetworkAdapter(netInterface.LowerInterfaces[s]);
                ConsoleLogger.Log($"[network] add '{netInterface.LowerInterfaces[s]}' to  bridge '{netInterface.Id}'");
            }
            ConsoleLogger.Log($"[network] create bridge '{netInterface.Id}'");
            return true;
        }

        /// <summary>
        /// todo
        ///     se una lower interface appartiene a un altro aggregatore toglierla dall'interfaccia trovata
        ///     se c'è una lower interface che non deve appartenerde all'agregatore corrente toglierlo
        /// </summary>
        /// <param name="netInterface"></param>
        /// <returns></returns>
        public static bool SetBondInterface(NetInterface netInterface) {
            Bond.Set(netInterface.Id);
            Ip.EnableNetworkAdapter(netInterface.Id);
            for(var s = 0; s < netInterface.LowerInterfaces.Length; s++) {
                Bond.AddNetworkAdapter(netInterface.Id, netInterface.LowerInterfaces[s]);
                Ip.EnableNetworkAdapter(netInterface.LowerInterfaces[s]);
            }
            ConsoleLogger.Log($"[network] create bond '{netInterface.Id}'");
            return true;
        }

        private static string GetNetworkRange(string input) {
            var ipaddressAndRange = Help.CaptureGroup(input, "([0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}\\/[0-9]{1,3})");
            return ipaddressAndRange;
        }

        private static Tuple<string, byte> GetIpAddressAndNetworkRange(string input) {
            var ipaddressAndRange = Help.CaptureGroup(input, "([0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}\\/[0-9]{1,3})");
            var array = ipaddressAndRange.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            return new Tuple<string, byte>(array[0], byte.Parse(array[1]));
        }

        private static string GetSysClassNetpath(string interfaceName) {
            return CommonString.Append(netFolder, "/", interfaceName);
        }

        private static string[] GetLowerInterfaces(string[] folderContent) {
            var lowerInferfacesLinks = folderContent.Where(_ => _.StartsWith(childrenFilePrefix)).ToArray();
            var result = new string[lowerInferfacesLinks.Length];
            for(var i = 0; i < lowerInferfacesLinks.Length; i++) {
                result[i] = folderContent[i].Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            }
            return result;
        }

        public static bool ApplyNetwork(SubNetwork network) {
            if(network == null) {
                return false;
            }
            if(!network.Active) {
                return true;
            }
            if(string.IsNullOrEmpty(network.NetworkAdapter)) {
                return false;
            }
            var running = Application.RunningConfiguration.Network.NetworkInterfaces;
            var currentAdapterConfig = running.FirstOrDefault(_ => _.Id == network.NetworkAdapter);
            if(currentAdapterConfig == null) {
                return false;
            }
            var networkInterface = new NetInterface() {
                Active = true,
                Id = network.NetworkAdapter,
                Name = network.NetworkAdapter,
                Type = currentAdapterConfig.Type,
                Membership = NetworkAdapterMembership.none,
                PrimaryAddressConfiguration = new NetworkAdapterInfo.AddressConfiguration() {
                    StaticAddress = network.StaticAddress,
                    IpAddr = network.IpAddress,
                    NetworkRange = network.NetworkRange
                },
                HardwareConfiguration = new NetworkAdapterInfo.HardwareConfiguration() {
                    Mtu = 6000,
                    Txqueuelen = 10000,
                    Promisc = true
                }
            };
            SetInterface(networkInterface, currentAdapterConfig);
            return true;
        }
    }
}
