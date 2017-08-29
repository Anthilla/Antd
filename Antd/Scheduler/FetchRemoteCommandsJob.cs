using antdlib.config.shared;
using antdlib.models;
using anthilla.core;
using anthilla.scheduler;
using System;
using anthilla.commands;
using System.Collections.Generic;
using Parameter = antdlib.common.Parameter;
using System.Linq;

namespace Antd.Scheduler {
    public class FetchRemoteCommandsJob : Job {

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

        private int _repetitionIntervalTime = 1000 * 60 * 2 + 330;

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

        private static readonly MachineIdsModel MachineId = Machine.MachineIds.Get;

        public override void DoJob() {
            //ConsoleLogger.Log("Scheduled action: Watch Cloud Stored Commands");
            var cloudaddress = new AppConfiguration().Get().CloudAddress;
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

            try {
                var cmds = ApiConsumer.Get<List<RemoteCommand>>($"{cloudaddress}repo/assetmanagement/fetchcommand/{MachineId.PartNumber}/{MachineId.SerialNumber}/{MachineId.MachineUid}/Antd");
                if(cmds == null)
                    return;
                if(!cmds.Any())
                    return;
                foreach(var cmd in cmds.OrderBy(_ => _.Date)) {
                    CommandLauncher.Launch(cmd.Command, cmd.Parameters);
                    var dict = new Dictionary<string, string> {
                        { "AppName", "Antd" },
                        { "PartNumber", MachineId.PartNumber },
                        { "SerialNumber", MachineId.SerialNumber },
                        { "MachineUid", MachineId.MachineUid },
                        { "Command", cmd.CommandCode }
                    };
                    ApiConsumer.Post($"{cloudaddress}repo/assetmanagement/confirmcommand", dict);
                }
            }
            catch(Exception) {

            }
        }
    }
}
