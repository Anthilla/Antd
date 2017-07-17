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

        private readonly ApiConsumer _api = new ApiConsumer();
        private readonly AsymmetricKeys _asymmetricKeys = new AsymmetricKeys(Parameter.AntdCfgKeys, Application.KeyName);
        private readonly MachineIdsModel _machineId = Machine.MachineIds.Get;

        public override void DoJob() {
            try {
                var pk = Encoding.ASCII.GetString(_asymmetricKeys.PublicKey);
                var internalIp = "";
                var externalIp = WhatIsMyIp.Get();
                var ut = MachineInfo.GetUptime();
                var uptime = ut.Uptime;
                var loadAverage = ut.LoadAverage;
                var du = new DiskUsage().GetInfo().FirstOrDefault(_ => _.MountedOn == "/mnt/cdrom");
                var diskUsage = du?.UsePercentage;
                var hostnamectl = CommandLauncher.Launch("hostnamectl").ToList();
                var hostname = hostnamectl.First(_ => _.Contains("Static hostname:")).Split(new[] { ":" }, 2, StringSplitOptions.RemoveEmptyEntries)[1];
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
                    { "Uptime", uptime },
                    { "DiskUsage", diskUsage },
                    { "LoadAverage", loadAverage },
                    { "Hostname", hostname },
                    { "AntdVersion", antdVersion },
                    { "SystemVersion", systemVersion },
                    { "KernelVersion", kernelVersion }
                };
                if(Parameter.Cloud.Contains("localhost")) {
                    return;
                }
                _api.Post($"{Parameter.Cloud}repo/assetinfo/save", dict);
                //ConsoleLogger.Log($"[cloud-uptime] info sent to cloud - data gathered in {DateTime.Now - dtnow}");
            }
            catch(Exception ex) {
                //ConsoleLogger.Error(ex.Message);
            }
        }
    }
}
