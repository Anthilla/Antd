using MQTTnet.Core;
using MQTTnet.Core.Server;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Antd.Mqtt {
    public class RetainedMessageHandler : IMqttServerStorage {

        private const string Filename = "/cfg/antd/mqtt/retainedmessage.json";

        public RetainedMessageHandler() {
            Directory.CreateDirectory("/cfg/antd/mqtt");
        }

        public Task SaveRetainedMessagesAsync(IList<MqttApplicationMessage> messages) {
            File.WriteAllText(Filename, JsonConvert.SerializeObject(messages));
            return Task.FromResult(0);
        }

        public Task<IList<MqttApplicationMessage>> LoadRetainedMessagesAsync() {
            IList<MqttApplicationMessage> retainedMessages;
            if(File.Exists(Filename)) {
                var json = File.ReadAllText(Filename);
                retainedMessages = JsonConvert.DeserializeObject<List<MqttApplicationMessage>>(json);
            }
            else {
                retainedMessages = new List<MqttApplicationMessage>();
            }

            return Task.FromResult(retainedMessages);
        }
    }
}