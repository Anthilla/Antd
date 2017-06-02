using System;
using antdlib.config.shared;
using antdlib.models;
using anthilla.commands;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using anthilla.core;
using Parameter = antdlib.common.Parameter;

namespace Antd.Cloud {
    public class FetchRemoteCommand {

        public System.Timers.Timer Timer { get; private set; }

        /// <summary>
        /// Start the timer
        /// </summary>
        /// <param name="milliseconds">Time in milliseconds</param>
        public void Start(int milliseconds) {
            Timer = new System.Timers.Timer(milliseconds);
            Timer.Elapsed += _timer_Elapsed;
            Timer.Enabled = true;
            Timer.Start();
            Do();
        }

        public void Stop() {
            Timer.Dispose();
        }

        private static readonly ApiConsumer Api = new ApiConsumer();
        private static readonly MachineIdsModel MachineId = Machine.MachineIds.Get;

        private static void _timer_Elapsed(object sender, ElapsedEventArgs e) {
            Do();
        }

        private static void Do() {
            try {
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
                var cmds = Api.Get<List<RemoteCommand>>($"{cloudaddress}repo/assetinfo/fetchcommand/{MachineId.PartNumber}/{MachineId.SerialNumber}/{MachineId.MachineUid}/Antd");
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
                    Api.Post($"{cloudaddress}repo/assetinfo/confirmcommand", dict);
                }
            }
            catch(Exception) {
                //ConsoleLogger.Error(ex.Message);
            }
        }
    }
}
