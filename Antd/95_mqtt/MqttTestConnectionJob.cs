using anthilla.scheduler;
using System.Threading.Tasks;
using System;
using anthilla.core;

namespace Antd.Mqtt {
    public class MqttTestConnectionJob : Job {

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

        private int _repetitionIntervalTime = 5000;

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
            DoJobAsync().GetAwaiter().GetResult();
        }

        static async Task DoJobAsync() {
            for(var i = 0; i < Application.CLUSTER_NODES.Length; i++) {
                if(CommonString.AreEquals(Application.CLUSTER_NODES[i].MachineUid.ToLowerInvariant(), Application.MACHINE_ID.MachineUid.ToString().ToLowerInvariant())) {
                    continue;
                }
                if(Application.CLUSTER_MQTT_CLIENTS[i].IsConnected()) {
                    continue;
                }
                ConsoleLogger.Log($"[mqtt] reconnecting to node #{i}: {Application.CLUSTER_NODES[i].Hostname} - {Application.CLUSTER_NODES[i].PublicIp}");
                Application.CLUSTER_MQTT_CLIENTS[i] = new MqttSyncClient(Application.CLUSTER_NODES[i].PublicIp, Application.MQTT_DEFAULT_PORT);
                await Application.CLUSTER_MQTT_CLIENTS[i].Connect();
            }
        }
    }
}
