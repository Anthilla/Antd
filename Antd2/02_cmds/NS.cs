using Antd;
using anthilla.core;
using System;
using System.IO;
using System.Linq;

namespace Antd2.cmds {
    public class NS {

        private const string nsupdateFileLocation = "/usr/bin/nsupdate";

        public static bool Update(string nsupdateConfigurationFile) {
            if (!File.Exists(nsupdateFileLocation)) {
                return false;
            }
            CommonProcess.Do(nsupdateFileLocation, nsupdateConfigurationFile);
            return true;
        }

        public class Switch {

            //public class NsSwitchElementVerbs {
            //    [JsonProperty(PropertyName = "active")]
            //    [JsonProperty(PropertyName = "compat")]
            //    [JsonProperty(PropertyName = "db")]
            //    [JsonProperty(PropertyName = "dns")]
            //    [JsonProperty(PropertyName = "files")]
            //    [JsonProperty(PropertyName = "hesiod")]
            //    [JsonProperty(PropertyName = "nis")]
            //    [JsonProperty(PropertyName = "nisplus")]
            //    [JsonProperty(PropertyName = "winbind")]
            //    [JsonProperty(PropertyName = "resolve")]
            //    [JsonProperty(PropertyName = "mdns_minimal")]
            //}
            //public enum NsSwitchStatus : sbyte {
            //    none = -1,
            //    success,
            //    notfound,
            //    unavail,
            //    tryagain
            //}
            //public enum NsSwitchAction : sbyte {
            //    none = -1,
            //    @return,
            //    @continue,
            //    merge
            //}

            private const string nsswitchFile = "/etc/nsswitch.conf";
            private const string nsswitchFileBackup = "/etc/nsswitch.conf.bck";

            public static NsSwitch Get() {
                if (!File.Exists(nsswitchFile)) {
                    return new NsSwitch();
                }
                var result = File.ReadAllLines(nsswitchFile).Where(_ => !_.StartsWith("#") && !string.IsNullOrEmpty(_)).ToArray();
                var nsswitch = new NsSwitch();
                for (var i = 0; i < result.Length; i++) {
                    var currentLine = result[i];
                    var currentLineData = currentLine.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    var context = currentLineData[0];
                    var attributes = currentLineData[1];
                    switch (context) {
                        case "passwd":
                            nsswitch.Passwd = attributes;
                            continue;
                        case "group":
                            nsswitch.Group = attributes;
                            continue;
                        case "shadow":
                            nsswitch.Shadow = attributes;
                            continue;
                        case "hosts":
                            nsswitch.Hosts = attributes;
                            continue;
                        case "networks":
                            nsswitch.Hosts = attributes;
                            continue;
                        case "services":
                            nsswitch.Hosts = attributes;
                            continue;
                        case "protocols":
                            nsswitch.Hosts = attributes;
                            continue;
                        case "rpc":
                            nsswitch.Hosts = attributes;
                            continue;
                        case "ethers":
                            nsswitch.Hosts = attributes;
                            continue;
                        case "netmasks":
                            nsswitch.Hosts = attributes;
                            continue;
                        case "netgroup":
                            nsswitch.Hosts = attributes;
                            continue;
                        case "bootparams":
                            nsswitch.Hosts = attributes;
                            continue;
                        case "automount":
                            nsswitch.Hosts = attributes;
                            continue;
                        case "aliases":
                            nsswitch.Hosts = attributes;
                            continue;
                        case "initgroups":
                            nsswitch.Initgroups = attributes;
                            continue;
                        case "publickey":
                            nsswitch.Publickey = attributes;
                            continue;
                        default:
                            continue;
                    }
                }
                return nsswitch;
            }

            //public static bool Set() {
            //    var current = Application.CurrentConfiguration.NsSwitch;
            //    var running = Application.RunningConfiguration.NsSwitch.ToString();
            //    if (CommonString.AreEquals(current.ToString(), running)) {
            //        return true;
            //    }
            //    var lines = new List<string>();

            //    if (!string.IsNullOrEmpty(current.Aliases)) { lines.Add(CommonString.Append("aliases ", current.Aliases)); }
            //    if (!string.IsNullOrEmpty(current.Ethers)) { lines.Add(CommonString.Append("ethers ", current.Ethers)); }
            //    if (!string.IsNullOrEmpty(current.Group)) { lines.Add(CommonString.Append("group ", current.Group)); }
            //    if (!string.IsNullOrEmpty(current.Hosts)) { lines.Add(CommonString.Append("hosts ", current.Hosts)); }
            //    if (!string.IsNullOrEmpty(current.Initgroups)) { lines.Add(CommonString.Append("initgroups ", current.Initgroups)); }
            //    if (!string.IsNullOrEmpty(current.Netgroup)) { lines.Add(CommonString.Append("netgroup ", current.Netgroup)); }
            //    if (!string.IsNullOrEmpty(current.Networks)) { lines.Add(CommonString.Append("networks ", current.Networks)); }
            //    if (!string.IsNullOrEmpty(current.Passwd)) { lines.Add(CommonString.Append("passwd ", current.Passwd)); }
            //    if (!string.IsNullOrEmpty(current.Protocols)) { lines.Add(CommonString.Append("protocols ", current.Protocols)); }
            //    if (!string.IsNullOrEmpty(current.Publickey)) { lines.Add(CommonString.Append("publickey ", current.Publickey)); }
            //    if (!string.IsNullOrEmpty(current.Rpc)) { lines.Add(CommonString.Append("rpc ", current.Rpc)); }
            //    if (!string.IsNullOrEmpty(current.Services)) { lines.Add(CommonString.Append("services ", current.Services)); }
            //    if (!string.IsNullOrEmpty(current.Shadow)) { lines.Add(CommonString.Append("shadow ", current.Services)); }
            //    if (!string.IsNullOrEmpty(current.Netmasks)) { lines.Add(CommonString.Append("netmasks ", current.Netmasks)); }
            //    if (!string.IsNullOrEmpty(current.Bootparams)) { lines.Add(CommonString.Append("bootparams ", current.Bootparams)); }
            //    if (!string.IsNullOrEmpty(current.Automount)) { lines.Add(CommonString.Append("automount ", current.Automount)); }

            //    if (File.Exists(nsswitchFile)) {
            //        File.Copy(nsswitchFile, nsswitchFileBackup, true);
            //    }
            //    File.WriteAllLines(nsswitchFile, lines);
            //    return true;
            //}
        }
    }
}
