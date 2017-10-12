using anthilla.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Antd.cmds {
    public class Cloud {

        public class RemoteCommand {
            public DateTime Date { get; set; }
            public string CommandCode { get; set; }
            public string Command { get; set; }
            public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
            public bool Executed { get; set; } = false;
        }

        public static void FetchCommand() {
            var cloudaddress = Application.CurrentConfiguration.WebService.Cloud;
            if(string.IsNullOrEmpty(cloudaddress)) {
                return;
            }
            if(!cloudaddress.EndsWith("/")) {
                cloudaddress = cloudaddress + "/";
            }
            if(Parameter.Cloud.Contains("localhost")) {
                return;
            }
            //fetchcommand/{partnum}/{serialnum}/{machineuid}/{appname}
            var machineIds = MachineIds.Get();
            try {
                var cmds = ApiConsumer.Get<List<Cloud.RemoteCommand>>($"{cloudaddress}repo/assetmanagement/fetchcommand/{machineIds.PartNumber}/{machineIds.SerialNumber}/{machineIds.MachineUid}/Antd");
                if(cmds == null)
                    return;
                if(!cmds.Any())
                    return;
                foreach(var cmd in cmds.OrderBy(_ => _.Date)) {
                    anthilla.commands.CommandLauncher.Launch(cmd.Command, cmd.Parameters);
                    var dict = new Dictionary<string, string> {
                        { "AppName", "Antd" },
                        { "PartNumber", machineIds.PartNumber.ToString() },
                        { "SerialNumber", machineIds.SerialNumber.ToString() },
                        { "MachineUid", machineIds.MachineUid.ToString() },
                        { "Command", cmd.CommandCode }
                    };
                    ApiConsumer.Post($"{cloudaddress}repo/assetmanagement/confirmcommand", dict);
                }
            }
            catch(Exception) {
                //
            }
        }

        public static void Update() {
            var pk = Encoding.ASCII.GetString(Application.Keys.PublicKey);
            var externalIp = WhatIsMyIp.Get();
            var ut = Uptime.Get();
            var du = DiskUsage.Get();
            var diskUsage = !du.Any() ? "" : du?.FirstOrDefault(_ => _.MountedOn.Contains("/mnt/cdrom")).UsePercentage;
            var machineIds = MachineIds.Get();
            var dict = new Dictionary<string, string> {
                { "AppName", "Antd" },
                { "PartNumber", machineIds.PartNumber.ToString() },
                { "SerialNumber", machineIds.SerialNumber.ToString() },
                { "Uid", machineIds.MachineUid.ToString() },
                { "KeyValue", pk },
                { "InternalIp", string.Empty },
                { "ExternalIp", externalIp },
                { "Uptime",  ut.Uptime },
                { "DiskUsage", diskUsage },
                { "LoadAverage", ut.LoadAverage },
                { "Hostname", Application.CurrentConfiguration.Host.HostName },
                { "AntdVersion", Application.RunningConfiguration.Info.Versions.Antd.Ver },
                { "SystemVersion", Application.RunningConfiguration.Info.Versions.System.Ver },
                { "KernelVersion", Application.RunningConfiguration.Info.Versions.Kernel.Ver }
            };
            if(Parameter.Cloud.Contains("localhost")) {
                return;
            }
            ApiConsumer.Post($"{Parameter.Cloud}repo/assetmanagement/save/uptime", dict);
            //ConsoleLogger.Log($"[cloud-uptime] info sent to cloud - data gathered in {DateTime.Now - dtnow}");
        }

        private static string GetVersionDateFromFile(string path) {
            var r = new Regex("(-\\d{8})", RegexOptions.IgnoreCase);
            var m = r.Match(path);
            var vers = m.Success ? m.Groups[0].Value.Replace("-", "") : "00000000";
            return vers;
        }
    }
}
