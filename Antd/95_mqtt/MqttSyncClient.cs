using anthilla.core;
using MQTTnet;
using MQTTnet.Core;
using MQTTnet.Core.Client;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antd.Mqtt {
    public class MqttSyncClient {

        public static IMqttClient MQTT_CLIENT;
        public static string MQTT_SERVER_ADDRESS;
        public static int MQTT_SERVER_PORT;

        public MqttSyncClient(string address, int port = 1883) {
            MQTT_SERVER_ADDRESS = address;
            MQTT_SERVER_PORT = port;
        }

        private static string[] MQTT_TOPICS = new string[] {
            "antdcluster",
            "antdcluster/test",
            //"datasync/save",
            //"datasync/delete",
            //"datasync/remove"
        };

        public async Task Connect() {
            var factory = new MqttFactory();
            MQTT_CLIENT = factory.CreateMqttClient();
            var clientOptions = new MqttClientOptionsBuilder()
                .WithClientId(Application.MACHINE_ID.MachineUid.ToString())
                .WithTcpServer(MQTT_SERVER_ADDRESS, MQTT_SERVER_PORT)
                .WithCleanSession()
                .Build();
            ConsoleLogger.Log($"[mqtt] connecting to {MQTT_SERVER_ADDRESS}:{MQTT_SERVER_PORT}");
            MQTT_CLIENT.Connected += async (s, e) => {
                ConsoleLogger.Log($"[mqtt] connected to {MQTT_SERVER_ADDRESS}:{MQTT_SERVER_PORT}");
                for(var i = 0; i < MQTT_TOPICS.Length; i++) {
                    await MQTT_CLIENT.SubscribeAsync(new TopicFilterBuilder().WithTopic(MQTT_TOPICS[i]).Build());
                }
                ConsoleLogger.Log("[mqtt] subscribed");
            };
            MQTT_CLIENT.Disconnected += (s, e) => {
                ConsoleLogger.Log($"[mqtt] unable to connect to {MQTT_SERVER_ADDRESS}:{MQTT_SERVER_PORT}");
            };
            MQTT_CLIENT.ApplicationMessageReceived += (s, e) => {
                //ConsoleLogger.Log("[mqtt] message received");
                ParsePayloadAsync(e.ApplicationMessage.Topic, e.ApplicationMessage.Payload);
            };
            try {
                await MQTT_CLIENT.ConnectAsync(clientOptions);
                ConsoleLogger.Log("[mqtt] client ready");
            }
            catch(Exception) {

            }
        }

        public void Disconnect() {
            MQTT_CLIENT.DisconnectAsync();
        }

        public bool IsConnected() {
            return MQTT_CLIENT.IsConnected;
        }

        /// <summary>
        /// Qui il client mqtt riceve il messaggio, tendenzialmente in broadcast dal broker
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="payload"></param>
        public static void ParsePayloadAsync(string topic, byte[] payload) {
            if(!MQTT_TOPICS.Contains(topic)) {
                ConsoleLogger.Log("[mqtt] received unauthorized message");
                return;
            }
            if(topic == MQTT_TOPICS[1]) {
                var parsedMessage = Encoding.ASCII.GetString(payload);
                ConsoleLogger.Log($"[mqtt] test message: {parsedMessage}");
            }
        }
    }
}
