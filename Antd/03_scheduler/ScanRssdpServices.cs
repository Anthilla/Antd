using Antd.models;
using anthilla.core;
using anthilla.scheduler;
using System.Collections.Generic;
using System.Linq;

namespace Antd {
    public class ScanRssdpServices : Job {

        public static List<NodeModel> ScannedDevices = new List<NodeModel>();
        private static bool ScanRunning = false;

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

        public override void DoJob() {
            if(!ScanRunning) {
                ScanRunning = true;
                var foundDevices = cmds.Rssdp.Discover().Result;
                foreach(var device in foundDevices) {
                    if(ScannedDevices.FirstOrDefault(_ => _.MachineUid == device.MachineUid) == null) {
                        ScannedDevices.Add(device);
                    }
                }
                ConsoleLogger.Log($"[rssdp] {ScannedDevices.Count()} devices found");
                foreach (var dev in ScannedDevices) {
                    ConsoleLogger.Log($"[rssdp] Found device: {dev.MachineUid}");
                }

                ScanRunning = false;
            }
            else {
                ConsoleLogger.Log($"[rssdp] Scan already running");
            }
        }
    }
}
