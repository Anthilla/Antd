using Nancy;

namespace Antd2.Modules {
    public class SshModule : NancyModule {

        public SshModule() : base("/ssh") {

            Get("/authorizedkeys", x => ApiGetAuthorizedkeys());

            Get("/publickey", x => ApiGetPublickey());

            Post("/save/authorizedkeys", x => ApiPostSave());

            Post("/apply/authorizedkeys", x => ApiPostApply());

            Post("/publickey/regen", x => ApiPostRegen());
        }

        private dynamic ApiGetAuthorizedkeys() {
            return Response.AsJson((object)Application.CurrentConfiguration.Services.Ssh.AuthorizedKey);
        }

        private dynamic ApiGetPublickey() {
            return Response.AsJson((object)Application.CurrentConfiguration.Services.Ssh.PublicKey);
        }

        private dynamic ApiPostSave() {
            //string data = Request.Form.Data;
            //var objects = JsonConvert.DeserializeObject<AuthorizedKey[]>(data);
            //Application.CurrentConfiguration.Services.Ssh.AuthorizedKey = objects;
            //ConfigRepo.Save();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostApply() {
            //Ssh.SetAuthorizedKey();
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostRegen() {
            //Console.WriteLine("[ssh] regen key");
            //Ssh.RegenRootKeys();
            //if (Application.CurrentConfiguration.Cluster.Active) {
            //    ClusterSetup.HandshakeCheck();
            //}
            return HttpStatusCode.OK;
        }
    }
}