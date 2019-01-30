using anthilla.core;
using System.Threading;
using System.Threading.Tasks;

namespace Antd.Mqtt {
    public static class MqttHandler {

        public static async Task MqttServerSetupForCluster() {
            if(Const.IsUnix == false) {
                return;
            }
            if(!Application.CurrentConfiguration.Cluster.Active) {
                return;
            }
            Application.CLUSTER_NODES = Application.CurrentConfiguration.Cluster.Nodes;
            if(Application.CLUSTER_NODES.Length < 1) {
                return;
            }
            if(Application.MQTT == null) {
                Application.MQTT = MqttBroker.StartMqttServer(Application.MQTT_DEFAULT_PORT).GetAwaiter().GetResult();
                Application.CLUSTER_MQTT_CLIENTS = new MqttSyncClient[Application.CLUSTER_NODES.Length];
                Thread.Sleep(5000);
                for(var i = 0; i < Application.CLUSTER_NODES.Length; i++) {
                    if(CommonString.AreEquals(Application.CLUSTER_NODES[i].MachineUid.ToLowerInvariant(), Application.MACHINE_ID.MachineUid.ToString().ToLowerInvariant())) {
                        ConsoleLogger.Log($"[mqtt] node #{i}: {Application.CLUSTER_NODES[i].Hostname} - {Application.CLUSTER_NODES[i].PublicIp} (self)");
                        continue;
                    }
                    ConsoleLogger.Log($"[mqtt] node #{i}: {Application.CLUSTER_NODES[i].Hostname} - {Application.CLUSTER_NODES[i].PublicIp}");
                    Application.CLUSTER_MQTT_CLIENTS[i] = new MqttSyncClient(Application.CLUSTER_NODES[i].PublicIp, Application.MQTT_DEFAULT_PORT);
                    await Application.CLUSTER_MQTT_CLIENTS[i].Connect();
                }
            }
            if(MqttTestConnectionJob.IsRunning == false) {
                Application.Scheduler.ExecuteJob<MqttTestConnectionJob>();
            }
        }
    }
}
