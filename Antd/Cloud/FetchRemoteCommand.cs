using System;
using antdlib.common;
using antdlib.config.shared;
using antdlib.models;
using anthilla.commands;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

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
        }

        public void Stop() {
            Timer.Dispose();
        }

        private static readonly ApiConsumer Api = new ApiConsumer();
        private static readonly CommandLauncher Launcher = new CommandLauncher();
        private static readonly string MachineId = Machine.MachineId.Get;

        private static void _timer_Elapsed(object sender, ElapsedEventArgs e) {
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
                var cmds = Api.Get<List<RemoteCommand>>($"{cloudaddress}repo/assetinfo/fetchcommand/Antd/" + MachineId);
                if(!cmds.Any())
                    return;
                foreach(var cmd in cmds.OrderBy(_ => _.Date)) {
                    Launcher.Launch(cmd.Command, cmd.Parameters);
                    var dict = new Dictionary<string, string> {
                        { "AppName", "Antd" },
                        { "MachineUid", MachineId },
                        { "Command", cmd.CommandCode }
                    };
                    Api.Post($"{Parameter.Cloud}repo/assetinfo/confirmcommand", dict);
                }
            }
            catch(Exception) {

            }
        }
    }
}
