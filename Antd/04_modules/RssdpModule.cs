using Antd.models;
using anthilla.core;
using Nancy;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Antd.Modules {
    public class RssdpModule : NancyModule {

        private static List<NodeModel> ScannedDevices = new List<NodeModel>();
        private static bool ScanRunning = false;

        public RssdpModule() : base("/rssdp") {

            Before += ctx => {
                ScanRunning = true;
                return null;
            };

            After += ctx => {
                ScanRunning = false;
            };

            Get["/discover"] = x => {
                if(!ScanRunning) {
                    var foundDevices = cmds.Rssdp.Discover().Result;
                    foreach(var device in foundDevices) {
                        if(ScannedDevices.FirstOrDefault(_ => _.MachineUid == device.MachineUid) != null) {
                            ScannedDevices.Add(device);
                        }
                    }
                }
                else {
                    ConsoleLogger.Log($"[rssdp] Scan already running");
                }
                return JsonConvert.SerializeObject(ScannedDevices);
            };

            Get["/clear"] = x => {
                ScannedDevices = new List<NodeModel>();
                return HttpStatusCode.OK;
            };
        }
    }
}