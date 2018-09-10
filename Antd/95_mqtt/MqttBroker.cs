using anthilla.core;
using MQTTnet;
using MQTTnet.Core.Protocol;
using MQTTnet.Core.Server;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Antd.Mqtt {
    public class MqttBroker {

        public static async Task<IMqttServer> StartMqttServer(int port = 1883) {
            var mqtt = new MqttFactory().CreateMqttServer();
            var options = new MqttServerOptions {
                ConnectionValidator = c => {
                    return MqttConnectReturnCode.ConnectionAccepted;
                },
                SubscriptionInterceptor = context => {
                    Console.WriteLine($"{context.ClientId} subscription accepted");
                },
                ApplicationMessageInterceptor = context => {
                    ParsePayload(context);
                },
                Storage = new RetainedMessageHandler()
            };
            options.DefaultEndpointOptions.Port = port;
            await mqtt.StartAsync(options);
            ConsoleLogger.Log($"[mqtt] server ready and listening on {port}");
            return mqtt;
        }

        private static void ParsePayload(MqttApplicationMessageInterceptorContext context) {
            var parsedMessage = Encoding.ASCII.GetString(context.ApplicationMessage.Payload);
            ConsoleLogger.Log($"[mqtt] {parsedMessage}");
        }
    }
}
