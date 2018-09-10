using MQTTnet.Core;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Antd.Mqtt {
    public class SyncController {

        private const string NAMESPACE = "antdcluster";

        public static async Task TestCommand(string command) {
            if(Application.MQTT == null) {
                Console.WriteLine("[mqtt] local server is down");
                return;
            }
            var topic = $"{NAMESPACE}/test";
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(NAMESPACE)
                .WithPayload(command)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();
            await Application.MQTT.PublishAsync(message);
        }
    }
}
