using antdlib.models;
using anthilla.core;
using anthilla.scheduler;
using System;
using anthilla.commands;
using System.Collections.Generic;
using Parameter = antdlib.common.Parameter;
using System.Linq;
using System.Text.RegularExpressions;
using anthilla.crypto;
using System.Text;
using Antd.Info;
using Antd.Storage;
using Antd.Helpers;

namespace Antd.Scheduler {
    public class UpdateCloudInfoJob : Job {

        #region [    Core Parameter    ]
        private bool _isRepeatable = true;

        public override bool IsRepeatable {
            get {
                return _isRepeatable;
            }
            set {
                value = _isRepeatable;
            }
        }

        private int _repetitionIntervalTime = 1000 * 60 * 5;

        public override int RepetitionIntervalTime {
            get {
                return _repetitionIntervalTime;
            }

            set {
                value = _repetitionIntervalTime;
            }
        }

        public override string Name {
            get {
                return GetType().Name;
            }

            set {
                value = GetType().Name;
            }
        }
        #endregion

        private static string GetVersionDateFromFile(string path) {
            var r = new Regex("(-\\d{8})", RegexOptions.IgnoreCase);
            var m = r.Match(path);
            var vers = m.Success ? m.Groups[0].Value.Replace("-", "") : "00000000";
            return vers;
        }

        private readonly AsymmetricKeys _asymmetricKeys = new AsymmetricKeys(Parameter.AntdCfgKeys, Application.KeyName);
        private readonly MachineIdsModel _machineId = Machine.MachineIds.Get;

        public override void DoJob() {
            var pk = Encoding.ASCII.GetString(_asymmetricKeys.PublicKey);
            var internalIp = "";
            var externalIp = WhatIsMyIp.Get();
            var ut = MachineInfo.GetUptime();
            var du = MachineInfo.GetDiskUsage().Where(_ => _ != null);
            var diskUsage = !du.Any() ? "" : du?.FirstOrDefault(_ => _.MountedOn.Contains("/mnt/cdrom")).UsePercentage;
            var hostnamectl = CommandLauncher.Launch("hostnamectl");
            var hostname = hostnamectl.Length == 0 ? "" : hostnamectl.FirstOrDefault(_ => _.Contains("Static hostname:"))?.Split(new[] { ":" }, 2, StringSplitOptions.RemoveEmptyEntries)[1];
            var antdVersion = GetVersionDateFromFile(Bash.Execute("file /mnt/cdrom/Apps/Anthilla_Antd/active-version"));
            var systemVersion = GetVersionDateFromFile(Bash.Execute("file /mnt/cdrom/System/active-system"));
            var kernelVersion = GetVersionDateFromFile(Bash.Execute("file /mnt/cdrom/Kernel/active-kernel"));
            var dict = new Dictionary<string, string> {
                { "AppName", "Antd" },
                { "PartNumber", _machineId.PartNumber },
                { "SerialNumber", _machineId.SerialNumber },
                { "Uid", _machineId.MachineUid },
                { "KeyValue", pk },
                { "InternalIp", internalIp },
                { "ExternalIp", externalIp },
                { "Uptime",  ut.Uptime },
                { "DiskUsage", diskUsage },
                { "LoadAverage", ut.LoadAverage },
                { "Hostname", hostname },
                { "AntdVersion", antdVersion },
                { "SystemVersion", systemVersion },
                { "KernelVersion", kernelVersion }
            };
            if(Parameter.Cloud.Contains("localhost")) {
                return;
            }
            ApiConsumer.Post($"{Parameter.Cloud}repo/assetmanagement/save/uptime", dict);
            //ConsoleLogger.Log($"[cloud-uptime] info sent to cloud - data gathered in {DateTime.Now - dtnow}");
        }
    }
}
