using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using antdlib.common;
using anthilla.commands;
using anthilla.crypto;
using Antd.Info;
using Antd.Storage;

namespace Antd.Cloud {
    public class UpdateCloudInfo {

        public System.Timers.Timer Timer { get; private set; }

        public void Start(int milliseconds) {
            Timer = new System.Timers.Timer(milliseconds);
            Timer.Elapsed += Action;
            Timer.Enabled = true;
        }

        public void Stop() {
            Timer?.Dispose();
        }

        private static string GetVersionDateFromFile(string path) {
            var r = new Regex("(-\\d{8})", RegexOptions.IgnoreCase);
            var m = r.Match(path);
            var vers = m.Success ? m.Groups[0].Value.Replace("-", "") : "00000000";
            return vers;
        }

        private readonly ApiConsumer _api = new ApiConsumer();
        private readonly Bash _bash = new Bash();
        private readonly CommandLauncher _launcher = new CommandLauncher();
        private readonly AsymmetricKeys _asymmetricKeys = new AsymmetricKeys(Parameter.AntdCfgKeys, Application.KeyName);
        private readonly string _machineId = Machine.MachineId.Get;

        private void Action(object sender, ElapsedEventArgs e) {
            var dtnow = DateTime.Now;
            var pk = Encoding.ASCII.GetString(_asymmetricKeys.PublicKey);
            var internalIp = "";
            var externalIp = WhatIsMyIp.Get();
            var machineInfo = new MachineInfo();
            var ut = machineInfo.GetUptime();
            var uptime = ut.Uptime;
            var loadAverage = ut.LoadAverage;
            var du = new DiskUsage().GetInfo().FirstOrDefault(_ => _.MountedOn == "/mnt/cdrom");
            var diskUsage = du?.UsePercentage;
            var hostnamectl = _launcher.Launch("hostnamectl").ToList();
            var hostname = hostnamectl.First(_ => _.Contains("Static hostname:")).Split(new[] { ":" }, 2, StringSplitOptions.RemoveEmptyEntries)[1];
            var antdVersion = GetVersionDateFromFile(_bash.Execute("file /mnt/cdrom/Apps/Anthilla_Antd/active-version"));
            var systemVersion = GetVersionDateFromFile(_bash.Execute("file /mnt/cdrom/System/active-system"));
            var kernelVersion = GetVersionDateFromFile(_bash.Execute("file /mnt/cdrom/Kernel/active-kernel"));
            var dict = new Dictionary<string, string> {
                { "AppName", "Antd" },
                { "MachineUid", _machineId },
                { "KeyValue", pk },
                { "InternalIp", internalIp },
                { "ExternalIp", externalIp },
                { "Uptime", uptime },
                { "DiskUsage", diskUsage },
                { "LoadAverage", loadAverage },
                { "Hostname", hostname },
                { "AntdVersion", antdVersion },
                { "SystemVersion", systemVersion },
                { "KernelVersion", kernelVersion }
            };
            _api.Post($"{Parameter.Cloud}repo/assetinfo/save", dict);
            ConsoleLogger.Log($"[cloud-uptime] info sent to cloud - data gathered in {DateTime.Now - dtnow}");
        }
    }
}
