using Antd.cmds;
using anthilla.core;
using Nancy;
using Newtonsoft.Json;

namespace Antd2.Modules {
    public class SshModule : NancyModule {

        public SshModule() : base("/ssh") {

            Get["/authorizedkeys"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.Services.Ssh.AuthorizedKey);
            };

            Get["/publickey"] = x => {
                return Response.AsText(Application.CurrentConfiguration.Services.Ssh.PublicKey);
            };

            Post["/save/authorizedkeys"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<AuthorizedKey[]>(data);
                Application.CurrentConfiguration.Services.Ssh.AuthorizedKey = objects;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/apply/authorizedkeys"] = x => {
                Ssh.SetAuthorizedKey();
                return HttpStatusCode.OK;
            };

            Post["/publickey/regen"] = x => {
                ConsoleLogger.Log("[ssh] regen key");
                Ssh.RegenRootKeys();
                if(Application.CurrentConfiguration.Cluster.Active) {
                    ClusterSetup.HandshakeCheck();
                }
                return HttpStatusCode.OK;
            };
        }
    }
}