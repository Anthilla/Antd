using antdlib.common;
using antdlib.models;
using anthilla.commands;
using Nancy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using anthilla.core;
using IoDir = System.IO.Directory;
using Parameter = antdlib.common.Parameter;

namespace antdlib.config {
    public class VpnConfiguration {

        private readonly VpnConfigurationModel _serviceModel;

        private readonly string _cfgFile = $"{Parameter.AntdCfgServices}/vpn.conf";
        private const string ServiceName = "sshd.service";

        public VpnConfiguration() {
            IoDir.CreateDirectory(Parameter.AntdCfgServices);
            if(!File.Exists(_cfgFile)) {
                _serviceModel = new VpnConfigurationModel();
            }
            else {
                try {
                    var text = File.ReadAllText(_cfgFile);
                    var obj = JsonConvert.DeserializeObject<VpnConfigurationModel>(text);
                    _serviceModel = obj;
                }
                catch(Exception) {
                    _serviceModel = new VpnConfigurationModel();
                }

            }
        }

        public void Save(VpnConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            FileWithAcl.WriteAllText(_cfgFile, text, "644", "root", "wheel");
            ConsoleLogger.Log("[vpn] configuration saved");
        }

        public void Set() {
            Enable();
            Start();
        }

        public bool IsActive() {
            if(!File.Exists(_cfgFile)) {
                return false;
            }
            return _serviceModel != null && _serviceModel.IsActive;
        }

        public VpnConfigurationModel Get() {
            return _serviceModel;
        }

        public void Enable() {
            _serviceModel.IsActive = true;
            Save(_serviceModel);
            ConsoleLogger.Log("[vpn] enabled");
        }

        public void Disable() {
            _serviceModel.IsActive = false;
            Save(_serviceModel);
            ConsoleLogger.Log("[vpn] disabled");
        }

        public void Stop() {
            Systemctl.Stop(ServiceName);
            ConsoleLogger.Log("[vpn] stop");
        }

        public void Start() {
            if(Systemctl.IsEnabled(ServiceName) == false) {
                Systemctl.Enable(ServiceName);
            }
            if(Systemctl.IsActive(ServiceName) == false) {
                Systemctl.Restart(ServiceName);
            }

            Systemctl.Restart(ServiceName);

            var remoteHost = _serviceModel.RemoteHost;
            var r = Handshake(remoteHost);
            if(!r) {
                ConsoleLogger.Warn($"[vpn] remote host {remoteHost} is unreachable");
                return;
            }

            remoteHost = remoteHost.Split(':').FirstOrDefault();

            var lsmod = Bash.Execute("lsmod").SplitBash().Grep("tun").FirstOrDefault();
            if(lsmod == null) {
                Bash.Execute("modprobe tun");
            }
            var lsmodRemote = Bash.Execute($"ssh root@{remoteHost} \"modprobe tun\"").SplitBash().Grep("tun").FirstOrDefault();
            if(lsmodRemote == null) {
                Bash.Execute($"ssh root@{remoteHost} \"modprobe tun\"");
            }

            Bash.Execute($"ssh root@{remoteHost} \"systemctl restart {ServiceName}\"");

            var unit = SetUnitForTunnel(remoteHost);
            if(unit == false) {
                ConsoleLogger.Warn("[vpn] something went wrong while creating the tunnel");
                return;
            }

            var localTap = Bash.Execute("ip link show").SplitBash().ToList();
            if(!localTap.Any(_ => _.Contains("tap1"))) {
                ConsoleLogger.Warn("[vpn] something went wrong while setting the local tunnel interface");
                return;
            }
            Bash.Execute("ip link set dev tap1 up");
            Bash.Execute("ip addr flush dev tap1");
            Bash.Execute($"ip addr add {_serviceModel.LocalPoint.Address}/{_serviceModel.LocalPoint.Range} dev tap1");
            localTap = Bash.Execute("ip link show tap1").SplitBash().ToList();
            if(localTap.Any(_ => _.ToLower().Contains("up"))) {
                ConsoleLogger.Log("[vpn] local tunnel interface is up");
            }

            var remoteTap = Bash.Execute($"ssh root@{remoteHost} \"ip link show\"").SplitBash().ToList();
            if(!remoteTap.Any(_ => _.Contains("tap1"))) {
                ConsoleLogger.Warn("[vpn] something went wrong while setting the remote tunnel interface");
                return;
            }
            Bash.Execute($"ssh root@{remoteHost} \"ip link set dev tap1 up\"");
            Bash.Execute($"ssh root@{remoteHost} \"ip addr flush dev tap1\"");
            Bash.Execute($"ssh root@{remoteHost} \"ip addr add {_serviceModel.LocalPoint.Address}/{_serviceModel.LocalPoint.Range} dev tap1\"");
            remoteTap = Bash.Execute($"ssh root@{remoteHost} \"ip link show tap1\"").SplitBash().ToList();
            if(remoteTap.Any(_ => _.ToLower().Contains("up"))) {
                ConsoleLogger.Log("[vpn] remote tunnel interface is up");
            }

            ConsoleLogger.Log("[vpn] connection established");
        }

        /// <summary>
        /// Send root public key to a remote host managed by antd
        /// </summary>
        /// <param name="remoteHost">address:port</param>
        /// <returns></returns>
        private static bool Handshake(string remoteHost) {
            if(string.IsNullOrEmpty(remoteHost) || !remoteHost.Contains(":")) {
                ConsoleLogger.Error("[vpn] remote host not defined");
                return false;
            }
            const string pathToPrivateKey = "/root/.ssh/id_rsa";
            const string pathToPublicKey = "/root/.ssh/id_rsa.pub";
            if(!File.Exists(pathToPublicKey)) {
                Bash.Execute($"ssh-keygen -t rsa -N '' -f {pathToPrivateKey}");
            }
            var key = File.ReadAllText(pathToPublicKey);
            if(string.IsNullOrEmpty(key)) {
                ConsoleLogger.Error("[vpn] missing local host public key");
                return false;
            }
            var dict = new Dictionary<string, string> { { "ApplePie", key } };
            var r = new ApiConsumer().Post($"http://{remoteHost}/asset/handshake", dict);
            var kh = new SshKnownHosts();
            kh.Add(remoteHost.Split(':').FirstOrDefault());
            if(r == HttpStatusCode.OK) {
                ConsoleLogger.Log("[vpn] handshake");
                return true;
            }
            ConsoleLogger.Error("[vpn] something went wrong during handshaking");
            return false;
        }

        public bool TestConnection() {
            var r = CommandLauncher.Launch("ping-c", new Dictionary<string, string> { { "$ip", _serviceModel.RemotePoint.Address } }).Grep("From");
            return !r.All(_ => _.ToLower().Contains("host unreachable"));
        }

        private static bool SetUnitForTunnel(string remoteHost) {
            var lines = new List<string> {
                "[Unit]",
                "Description=ExtUnit, VpnConnection",
                "",
                "[Service]",
                $"ExecStart=/usr/bin/ssh -o Tunnel=ethernet -f -w 1:1 root@{remoteHost} true",
                "SuccessExitStatus=1 2 3 4 5 6 7 8 9 0",
                "RemainAfterExit=yes",
                "Type=oneshot",
                "",
                "[Install]",
                "WantedBy=antd.target"
            };
            var unitName = $"/mnt/cdrom/Units/antd.target.wants/antd-{remoteHost}-vpn.service";
            ConsoleLogger.Log(unitName);
            if(!File.Exists(unitName)) {
                FileWithAcl.WriteAllLines(unitName, lines, "644", "root", "wheel");
                Systemctl.DaemonReload();
            }
            Systemctl.Restart($"antd-{remoteHost}-vpn.service");
            return Systemctl.IsActive($"antd-{remoteHost}-vpn.service");
        }
    }
}
