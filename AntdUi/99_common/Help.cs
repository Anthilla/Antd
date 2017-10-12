using Antd;
using Antd.models;
using anthilla.core;
using Nancy;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AntdUi {
    public class Help {
        public static MachineConfig GetCurrentConfiguration() {
            return ApiConsumer.Get<MachineConfig>($"{Application.ServerUrl}/conf");
        }

        public static HttpStatusCode Authenticate(string id, string claim) {
            return Authenticate(id, new string[] { claim });
        }

        public static HttpStatusCode Authenticate(string id, string[] claims) {
            var data = new AuthenticationDataModel() {
                Id = id,
                Claims = claims
            };
            var json = JsonConvert.SerializeObject(data);
            var dict = new Dictionary<string, string> {
                { "Data", json }
            };
            return ApiConsumer.Post($"{Application.ServerUrl}/user/authenticate", dict);
        }
    }
}
